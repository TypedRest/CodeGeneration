using System;
using Microsoft.OpenApi.Models;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoClassBuilder : DtoBuilder
{
    public DtoClassBuilder(string key, OpenApiSchema schema, INamingStrategy naming)
        : base(key, schema, naming)
    {}

    protected override ICSharpType BuildTypeInner()
    {
        var type = new CSharpClass(Identifier);

        foreach ((string propKey, var propSchema) in Schema.Properties)
        {
            var property = BuildProperty(propKey, propSchema);

            if (Schema.Required.Contains(propKey))
                property.Attributes.Add(Attributes.Required);

            if (propKey.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                property.Attributes.Add(Attributes.Key);

            type.Properties.Add(property);
        }

        return type;
    }

    protected virtual CSharpProperty BuildProperty(string key, OpenApiSchema? schema)
    {
        string propertyName = Naming.Property(key);
        if (propertyName == Identifier.Name)
            propertyName += "Value";

        return new CSharpProperty(GetPropertyType(propertyName, schema), propertyName)
        {
            Summary = schema?.Description,
            Attributes = {Attributes.JsonProperty(key)},
            HasSetter = true
        };
    }

    private CSharpIdentifier GetPropertyType(string propertyName, OpenApiSchema? schema)
    {
        if (schema?.Type == "string" && schema.Enum.Count != 0)
        {
            var dtoEnum = new DtoEnumBuilder(propertyName, schema, Naming).BuildType();
            ChildTypes.Add(dtoEnum);
            return dtoEnum.Identifier;
        }

        return Naming.TypeFor(schema);
    }
}
