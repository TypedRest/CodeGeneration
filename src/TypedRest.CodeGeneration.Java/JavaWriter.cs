using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm;
using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Java;

/// <summary>
/// Renders the shared JVM type model as Java source code.
/// </summary>
/// <param name="serializer">Supplies the annotations carrying wire names on generated DTOs.</param>
/// <param name="entryConstructor">Controls whether the entry endpoint gets a generated constructor.</param>
/// <param name="nullableAnnotations">Controls whether properties are annotated with JSpecify nullability.</param>
public sealed class JavaWriter(JvmSerializer serializer, bool entryConstructor = true, bool nullableAnnotations = true)
{
    /// <summary>
    /// The file extension of Java source files.
    /// </summary>
    public const string FileExtension = ".java";

    /// <summary>
    /// The line length past which a class declaration is wrapped before its <c>extends</c> clause.
    /// </summary>
    private const int MaxLineLength = 120;

    private static readonly JvmPackage _jspecify = JvmPackage.External("org.jspecify.annotations");

    /// <summary>
    /// Marks a value as possibly <c>null</c>.
    /// </summary>
    private static JvmAnnotation Nullable
        => new(new JvmIdentifier(_jspecify, "Nullable"));

    /// <summary>
    /// Writes a file declaring <paramref name="type"/>.
    /// </summary>
    public void WriteFile(TextWriter textWriter, IJvmType type)
    {
        var writer = new JvmWriter(textWriter);

        var package = type.Identifier.Package;
        if (package is {Name.Length: > 0})
        {
            writer.WriteLine($"package {package.Name};");
            writer.WriteLine();
        }

        var imports = Imports(type).ToList();
        if (imports.Count != 0)
        {
            foreach (string import in imports)
                writer.WriteLine($"import {import};");
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

        foreach (var annotation in AnnotationsFor(type))
        {
            foreach (var import in annotation.GetImports()) yield return import;
        }

        if (type is JvmEndpointClass endpoint)
        {
            // Java has no inherited constructors, so one is always synthesized.
            // The entry endpoint's takes only the base URI; every other endpoint's takes a referrer as well.
            if (Equals(endpoint.BaseType, Packages.EntryEndpoint))
            {
                if (entryConstructor)
                {
                    yield return JvmIdentifier.Uri;
                    if (serializer.RuntimeSerializer is {} runtimeSerializer)
                    {
                        yield return runtimeSerializer;
                        yield return Packages.HttpCredentials;
                    }
                }
            }
            else
            {
                yield return Packages.Endpoint;
                yield return JvmIdentifier.Uri;
            }
        }
    }

    private IEnumerable<JvmAnnotation> AnnotationsFor(IJvmType type)
    {
        switch (type)
        {
            case JvmDto dto:
                foreach (var annotation in serializer.TypeAnnotations()) yield return annotation;
                foreach (var property in dto.Properties)
                {
                    if (serializer.PropertyName(property.WireName) is {} annotation) yield return annotation;
                    if (nullableAnnotations && property.Type.Nullable) yield return Nullable;
                }
                break;

            case JvmEnum @enum:
                foreach (var annotation in serializer.EnumAnnotations()) yield return annotation;
                foreach (var value in @enum.Values)
                {
                    if (serializer.EnumMemberName(value.WireName) is {} annotation) yield return annotation;
                }
                break;
        }
    }

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
                throw new ArgumentException($"Cannot write a {type.GetType().Name} as Java.", nameof(type));
        }
    }

    private void WriteEndpoint(JvmWriter writer, JvmEndpointClass type)
    {
        writer.WriteDocComment(type.Summary, type.Deprecated);
        if (type.Deprecated) writer.WriteLine("@Deprecated");

        // Every child endpoint takes its parent as the referrer, so the field initializers hand out `this` while the class is still initializing.
        // That is safe here because an endpoint only reads the referrer's URI and HTTP client, both set by the super constructor.
        // Suppressed on the generated class so the warning does not land in every consumer's build.
        if (type.Children.Count != 0) writer.WriteLine("@SuppressWarnings(\"this-escape\")");

        string declaration = $"public class {type.Identifier.Name}";
        string extends = $" extends {TypeExpression(type.BaseType)}";

        if (declaration.Length + extends.Length > MaxLineLength)
        {
            writer.WriteLine(declaration);
            using (writer.Indent())
                writer.WriteLine(extends.TrimStart() + " {");
        }
        else
            writer.WriteLine(declaration + extends + " {");

        using (writer.Indent())
        {
            WriteConstructor(writer, type);

            foreach (var child in type.Children)
            {
                writer.WriteLine();
                WriteChild(writer, child);
            }
        }

        writer.WriteLine("}");
    }

    /// <summary>
    /// Writes the constructor of an endpoint class.
    /// </summary>
    private void WriteConstructor(JvmWriter writer, JvmEndpointClass type)
    {
        string name = type.Identifier.Name;

        if (Equals(type.BaseType, Packages.EntryEndpoint))
        {
            if (!entryConstructor) return;

            // Kotlin passes the serializer as a named argument and lets the credentials parameter before it default.
            // Java has neither, so the credentials have to be passed positionally, and cast because @JvmOverloads generates a (URI, OkHttpClient, Serializer) overload a bare null could also match.
            string arguments = serializer.RuntimeSerializer is {} runtimeSerializer
                ? $"uri, ({Packages.HttpCredentials.Name}) null, new {runtimeSerializer.Name}()"
                : "uri";

            writer.WriteDocComment($"Creates a new {name}.");
            writer.WriteLine($"public {name}(URI uri) {{");
            using (writer.Indent())
                writer.WriteLine($"super({arguments});");
            writer.WriteLine("}");
            return;
        }

        var constructor = type.Constructor;
        var parameters = constructor?.Parameters ?? [];
        var baseArguments = constructor?.BaseArguments ?? [];

        string parameterList = string.Join(", ", parameters.Select(x => $"{TypeExpression(x.Type)} {x.Name}"));
        string argumentList = string.Join(", ", baseArguments.Select(Expression));

        writer.WriteDocComment($"Creates a new {name}.");
        writer.WriteLine($"public {name}({parameterList}) {{");
        using (writer.Indent())
            writer.WriteLine($"super({argumentList});");
        writer.WriteLine("}");
    }

    /// <summary>
    /// Writes a child endpoint as a <c>public final</c> field.
    /// </summary>
    private void WriteChild(JvmWriter writer, JvmChildEndpoint child)
    {
        writer.WriteDocComment(child.Summary, child.Deprecated);
        if (child.Deprecated) writer.WriteLine("@Deprecated");

        writer.WriteLine($"public final {TypeExpression(child.Type)} {child.Name} =");
        using (writer.Indent())
            writer.WriteLine(Expression(child.Value) + ";");
    }

    /// <summary>
    /// Writes a DTO as a class with public final fields and a constructor.
    /// </summary>
    private void WriteDto(JvmWriter writer, JvmDto type)
    {
        writer.WriteDocComment(type.Summary, type.Deprecated);
        if (type.Deprecated) writer.WriteLine("@Deprecated");

        foreach (var annotation in serializer.TypeAnnotations())
            writer.WriteLine(annotation.Write());

        writer.WriteLine($"public class {type.Identifier.Name} {{");

        using (writer.Indent())
        {
            foreach (var property in type.Properties)
            {
                writer.WriteDocComment(property.Summary, property.Deprecated);
                if (property.Deprecated) writer.WriteLine("@Deprecated");
                if (serializer.PropertyName(property.WireName) is {} annotation)
                    writer.WriteLine(annotation.Write());
                if (nullableAnnotations && property.Type.Nullable)
                    writer.WriteLine(Nullable.Write());

                writer.WriteLine($"public {TypeExpression(property.Type)} {property.Name};");
                writer.WriteLine();
            }

            // The serializers construct the instance and then populate the fields, so they need a no-argument constructor.
            // Writing it explicitly keeps it once the full constructor below removes the default one.
            writer.WriteDocComment($"Creates an empty {type.Identifier.Name}.");
            writer.WriteLine($"public {type.Identifier.Name}() {{}}");

            if (type.Properties.Count != 0)
            {
                writer.WriteLine();
                WriteDtoConstructor(writer, type);
            }
        }

        writer.WriteLine("}");
    }

    private void WriteDtoConstructor(JvmWriter writer, JvmDto type)
    {
        string parameters = string.Join(", ", type.Properties.Select(x => $"{TypeExpression(x.Type)} {x.Name}"));

        writer.WriteDocComment($"Creates a {type.Identifier.Name} with all fields set.");
        writer.WriteLine($"public {type.Identifier.Name}({parameters}) {{");
        using (writer.Indent())
        {
            foreach (var property in type.Properties)
                writer.WriteLine($"this.{property.Name} = {property.Name};");
        }
        writer.WriteLine("}");
    }

    private void WriteEnum(JvmWriter writer, JvmEnum type)
    {
        writer.WriteDocComment(type.Summary, type.Deprecated);
        if (type.Deprecated) writer.WriteLine("@Deprecated");

        foreach (var annotation in serializer.EnumAnnotations())
            writer.WriteLine(annotation.Write());

        writer.WriteLine($"public enum {type.Identifier.Name} {{");

        using (writer.Indent())
        {
            for (int i = 0; i < type.Values.Count; i++)
            {
                var value = type.Values[i];
                string separator = i == type.Values.Count - 1 ? ";" : ",";

                writer.WriteDocComment(value.Summary);
                if (serializer.EnumMemberName(value.WireName) is {} annotation)
                    writer.WriteLine(annotation.Write());
                writer.WriteLine(value.Name + separator);
            }
        }

        writer.WriteLine("}");
    }

    /// <summary>
    /// Writes a type reference, e.g. <c>List&lt;Contact&gt;</c>.
    /// </summary>
    public string TypeExpression(JvmIdentifier identifier)
    {
        string name = identifier.Kind switch
        {
            JvmTypeKind.Int => "Integer",
            JvmTypeKind.Long => "Long",
            JvmTypeKind.Double => "Double",
            JvmTypeKind.Boolean => "Boolean",
            JvmTypeKind.Object => "Object",
            _ => identifier.Name
        };

        return identifier.TypeArguments.Count == 0
            ? name
            : $"{name}<{string.Join(", ", identifier.TypeArguments.Select(TypeExpression))}>";
    }

    /// <summary>
    /// Writes the type of an object creation, using the diamond operator where there are type arguments to infer.
    /// </summary>
    private string CreationType(JvmIdentifier identifier)
        => identifier.TypeArguments.Count == 0
            ? TypeExpression(identifier)
            : TypeExpression(identifier).Split('<')[0] + "<>";

    /// <summary>
    /// Writes an expression.
    /// </summary>
    public string Expression(JvmExpression expression)
        => expression switch
        {
            JvmThis => "this",
            JvmName name => name.Name,
            JvmLiteral literal => JvmSyntax.Quote(literal.Value),

            // Not `new URI(...)`, whose checked URISyntaxException a field initializer cannot handle
            JvmUriLiteral uri => $"URI.create({JvmSyntax.Quote(uri.Value)})",

            // Java's class literal is already a java.lang.Class, with no type arguments allowed on it
            JvmClassLiteral classLiteral => $"{TypeExpression(classLiteral.Type.ToNonNullable()).Split('<')[0]}.class",

            // The runtime declares the factory as a Kotlin function type, which Java sees as a Function2 whose single abstract invoke method a lambda can implement
            JvmElementFactory factory =>
                $"({JvmElementFactory.ReferrerParameter}, {JvmElementFactory.RelativeUriParameter}) -> {Expression(factory.Body)}",

            // Every generated member declares its type explicitly, so the diamond operator infers the arguments
            JvmNew creation =>
                $"new {CreationType(creation.Type)}({string.Join(", ", creation.Arguments.Select(Expression))})",

            _ => throw new ArgumentException($"Cannot write a {expression.GetType().Name} as Java.", nameof(expression))
        };
}
