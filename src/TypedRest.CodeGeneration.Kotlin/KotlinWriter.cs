using TypedRest.CodeGeneration.Jvm;
using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Kotlin;

/// <summary>
/// Renders the shared JVM type model as Kotlin source code.
/// </summary>
/// <param name="serializer">Supplies the annotations carrying wire names on generated DTOs.</param>
/// <param name="entryConstructor">Controls whether the entry endpoint gets a generated constructor.</param>
public sealed class KotlinWriter(JvmSerializer serializer, bool entryConstructor = true)
{
    /// <summary>
    /// The file extension of Kotlin source files.
    /// </summary>
    public const string FileExtension = ".kt";

    /// <summary>
    /// Writes a file declaring <paramref name="type"/>.
    /// </summary>
    public void WriteFile(TextWriter textWriter, IJvmType type)
    {
        var writer = new JvmWriter(textWriter);

        var package = type.Identifier.Package;
        if (package is {Name.Length: > 0})
        {
            writer.WriteLine($"package {package.Name}");
            writer.WriteLine();
        }

        var imports = Imports(type).ToList();
        if (imports.Count != 0)
        {
            foreach (string import in imports)
                writer.WriteLine($"import {import}");
            writer.WriteLine();
        }

        Write(writer, type);
    }

    /// <summary>
    /// Returns the sorted, deduplicated imports a file declaring <paramref name="type"/> needs.
    /// </summary>
    private IEnumerable<string> Imports(IJvmType type)
        => AllImports(type)
          .Where(x => x.Package is {} package
                   && package.Name.Length != 0
                   && !Equals(package, type.Identifier.Package)
                   && !Equals(package, Packages.JavaLang))
          .Select(x => x.QualifiedName)
          .Distinct(StringComparer.Ordinal)
          .OrderBy(x => x, StringComparer.Ordinal);

    private IEnumerable<JvmIdentifier> AllImports(IJvmType type)
    {
        foreach (var import in type.GetImports()) yield return import;

        // Annotations are attached by this writer rather than by the model, so they contribute imports of their own
        foreach (var annotation in AnnotationsFor(type))
        {
            foreach (var import in annotation.GetImports()) yield return import;
        }

        if (type is JvmEndpointClass {BaseType: var baseType} && Equals(baseType, Packages.EntryEndpoint) && entryConstructor)
        {
            yield return JvmIdentifier.Uri;
            if (serializer.RuntimeSerializer is {} runtimeSerializer) yield return runtimeSerializer;
        }
    }

    /// <summary>
    /// Returns every annotation on a type, including the ones its members carry.
    /// </summary>
    private IEnumerable<JvmAnnotation> AnnotationsFor(IJvmType type)
        => type switch
        {
            JvmDto dto => serializer.TypeAnnotations()
                                    .Concat(dto.Properties.Select(x => serializer.PropertyName(x.WireName)).OfType<JvmAnnotation>()),
            JvmEnum @enum => serializer.EnumAnnotations()
                                       .Concat(@enum.Values.Select(x => serializer.EnumMemberName(x.WireName)).OfType<JvmAnnotation>()),
            _ => []
        };

    private void Write(JvmWriter writer, IJvmType type)
    {
        switch (type)
        {
            case JvmEndpointClass endpoint:
                WriteEndpoint(writer, endpoint);
                break;
            case JvmDto dto:
                WriteDto(writer, dto);
                break;
            case JvmEnum @enum:
                WriteEnum(writer, @enum);
                break;
            default:
                throw new ArgumentException($"Cannot write a {type.GetType().Name} as Kotlin.", nameof(type));
        }
    }

    private void WriteEndpoint(JvmWriter writer, JvmEndpointClass type)
    {
        writer.WriteDocComment(type.Summary, type.Deprecated);
        if (type.Deprecated) writer.WriteLine("@Deprecated(\"\")");

        // Generated endpoints are open so that consumers can derive from them to add their own members
        string declaration = $"open class {type.Identifier.Name}";
        var (parameters, baseCall) = Header(type);

        // The base call carries the entity class and, for a collection, a factory lambda, which together run well past a readable line length.
        if (declaration.Length + parameters.Length + baseCall.Length > MaxLineLength)
        {
            writer.WriteLine(declaration + parameters);
            using (writer.Indent())
                writer.WriteLine(baseCall.TrimStart() + " {");
        }
        else
            writer.WriteLine(declaration + parameters + baseCall + " {");

        using (writer.Indent())
        {
            bool first = true;
            foreach (var child in type.Children)
            {
                if (!first) writer.WriteLine();
                WriteChild(writer, child);
                first = false;
            }
        }

        writer.WriteLine("}");
    }

    /// <summary>
    /// The line length past which a class declaration is wrapped before its supertype call.
    /// </summary>
    private const int MaxLineLength = 120;

    /// <summary>
    /// Writes the constructor parameters and supertype call of an endpoint, e.g.
    /// <c>(referrer: Endpoint)</c> and <c> : FooEndpointImpl(referrer, "foo")</c>.
    /// </summary>
    private (string parameters, string baseCall) Header(JvmEndpointClass type)
    {
        if (Equals(type.BaseType, Packages.EntryEndpoint))
        {
            // Without a generated constructor the class only names its base type, leaving the constructors to the
            // consumer. Kotlin needs the base type spelled without parentheses for that to compile.
            if (!entryConstructor) return ("", $" : {TypeExpression(type.BaseType)}");

            string arguments = serializer.RuntimeSerializer is {} runtimeSerializer
                ? $"uri, serializer = {runtimeSerializer.Name}()"
                : "uri";
            return ("(uri: URI)", $" : {TypeExpression(type.BaseType)}({arguments})");
        }

        var constructor = type.Constructor;
        if (constructor is null or {Parameters.Count: 0}) return ("", $" : {TypeExpression(type.BaseType)}()");

        string parameters = string.Join(", ", constructor.Parameters.Select(x => $"{x.Name}: {TypeExpression(x.Type)}"));
        string baseArguments = string.Join(", ", constructor.BaseArguments.Select(Expression));

        return ($"({parameters})", $" : {TypeExpression(type.BaseType)}({baseArguments})");
    }

    private void WriteChild(JvmWriter writer, JvmChildEndpoint child)
    {
        writer.WriteDocComment(child.Summary, child.Deprecated);
        if (child.Deprecated) writer.WriteLine("@Deprecated(\"\")");

        writer.WriteLine($"val {child.Name}: {TypeExpression(child.Type)} =");
        using (writer.Indent())
            writer.WriteLine(Expression(child.Value));
    }

    /// <summary>
    /// Writes a DTO as a <c>data class</c>, which gives it equality, <c>toString</c> and <c>copy</c> for free.
    /// </summary>
    private void WriteDto(JvmWriter writer, JvmDto type)
    {
        writer.WriteDocComment(type.Summary, type.Deprecated);
        if (type.Deprecated) writer.WriteLine("@Deprecated(\"\")");

        foreach (var annotation in serializer.TypeAnnotations())
            writer.WriteLine(annotation.Write());

        // A data class needs at least one property; anything else has to be a plain class
        if (type.Properties.Count == 0)
        {
            writer.WriteLine($"class {type.Identifier.Name}");
            return;
        }

        writer.WriteLine($"data class {type.Identifier.Name}(");

        using (writer.Indent())
        {
            for (int i = 0; i < type.Properties.Count; i++)
            {
                var property = type.Properties[i];
                string separator = i == type.Properties.Count - 1 ? "" : ",";

                writer.WriteDocComment(property.Summary, property.Deprecated);
                if (property.Deprecated) writer.WriteLine("@Deprecated(\"\")");
                if (serializer.PropertyName(property.WireName) is {} annotation)
                    writer.WriteLine(annotation.Write());

                // An optional property defaults to null so that a DTO can be built without naming every field.
                // A required one deliberately gets no default, making a missing value a compile error.
                string @default = property.Required ? "" : " = null";
                writer.WriteLine($"val {property.Name}: {TypeExpression(property.Type)}{@default}{separator}");
            }
        }

        writer.WriteLine(")");
    }

    private void WriteEnum(JvmWriter writer, JvmEnum type)
    {
        writer.WriteDocComment(type.Summary, type.Deprecated);
        if (type.Deprecated) writer.WriteLine("@Deprecated(\"\")");

        foreach (var annotation in serializer.EnumAnnotations())
            writer.WriteLine(annotation.Write());

        writer.WriteLine($"enum class {type.Identifier.Name} {{");

        using (writer.Indent())
        {
            for (int i = 0; i < type.Values.Count; i++)
            {
                var value = type.Values[i];
                string separator = i == type.Values.Count - 1 ? "" : ",";

                writer.WriteDocComment(value.Summary);
                if (serializer.EnumMemberName(value.WireName) is {} annotation)
                    writer.WriteLine(annotation.Write());
                writer.WriteLine(value.Name + separator);
            }
        }

        writer.WriteLine("}");
    }

    /// <summary>
    /// Writes a type reference, e.g. <c>List&lt;Contact&gt;?</c>.
    /// </summary>
    public string TypeExpression(JvmIdentifier identifier)
    {
        string name = identifier.Kind switch
        {
            // Kotlin has its own names for these, mapped back to the JVM types by the compiler
            JvmTypeKind.Object => "Any",
            _ => identifier.Name
        };

        string core = identifier.TypeArguments.Count == 0
            ? name
            : $"{name}<{string.Join(", ", identifier.TypeArguments.Select(TypeExpression))}>";

        return identifier.Nullable ? core + "?" : core;
    }

    /// <summary>
    /// Writes an expression.
    /// </summary>
    public string Expression(JvmExpression expression)
        => expression switch
        {
            JvmThis => "this",
            JvmName name => name.Name,
            JvmLiteral literal => JvmSyntax.Quote(literal.Value, escapeDollar: true),
            JvmUriLiteral uri => $"URI({JvmSyntax.Quote(uri.Value, escapeDollar: true)})",

            // Kotlin's class literal is a KClass, which the endpoints need as a java.lang.Class
            JvmClassLiteral classLiteral => $"{TypeExpression(classLiteral.Type.ToNonNullable())}::class.java",

            // Kotlin infers the lambda's parameter types from the function type the constructor declares
            JvmElementFactory factory =>
                $"{{ {JvmElementFactory.ReferrerParameter}, {JvmElementFactory.RelativeUriParameter} -> {Expression(factory.Body)} }}",

            JvmNew creation => WriteCreation(creation),

            _ => throw new ArgumentException($"Cannot write a {expression.GetType().Name} as Kotlin.", nameof(expression))
        };

    /// <summary>
    /// Writes an object creation, moving a trailing lambda outside the parentheses as Kotlin style prefers.
    /// </summary>
    private string WriteCreation(JvmNew creation)
    {
        string type = creation.Type.Name;

        var arguments = creation.Arguments;
        if (arguments.Count != 0 && arguments[arguments.Count - 1] is JvmElementFactory trailing)
        {
            string leading = string.Join(", ", arguments.Take(arguments.Count - 1).Select(Expression));
            return $"{type}({leading}) {Expression(trailing)}";
        }

        return $"{type}({string.Join(", ", arguments.Select(Expression))})";
    }
}
