using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NanoByte.CodeGeneration;
using Xunit;

namespace TypedRest.CodeGeneration.CSharp.Dtos
{
    public class DtoGeneratorFacts
    {
        private readonly DtoGenerator _generator = new(
            new NamingStrategy("MyService", "MyNamespace", "MyNamespace"));

        [Fact]
        public void GeneratesClasses()
        {
            _generator.Generate(new Dictionary<string, OpenApiSchema>
            {
                ["contact"] = Sample.ContactSchema,
                ["note"] = Sample.NoteSchema
            }).Should().BeEquivalentTo(
                DtoClass("Contact", "A contact in an address book.",
                    Property("Id", "id", "The ID of the contact.", key: true),
                    Property("FirstName", "firstName", "The first name of the contact.", required: true),
                    Property("LastName", "lastName", "The last name of the contact.", required: true)),
                DtoClass("Note", "A note about a specific contact.",
                    Property("Content", "content", "The content of the note.", required: true)));
        }

        private static readonly OpenApiSchema _enumSchema = new()
        {
            Description = "My enum",
            Type = "string",
            Enum = new List<IOpenApiAny>
            {
                new OpenApiString("value1"),
                new OpenApiString("value2")
            }
        };

        private static readonly CSharpEnum _dtoEnum = DtoEnum("MyEnum", "My enum",
            DtoEnumValue("Value1", "value1"),
            DtoEnumValue("Value2", "value2"));

        [Fact]
        public void GeneratesEnums()
        {
            _generator.Generate(new Dictionary<string, OpenApiSchema>
            {
                ["myEnum"] = _enumSchema
            }).Should().BeEquivalentTo(_dtoEnum);
        }

        [Fact]
        public void GeneratesInlineEnums()
        {
            _generator.Generate(new Dictionary<string, OpenApiSchema>
            {
                ["myType"] = new()
                {
                    Description = "My type",
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["myEnum"] = _enumSchema
                    }
                }
            }).Should().BeEquivalentTo(
                DtoClass("MyType", "My type",
                    Property("MyEnum", "myEnum", "My enum", type: _dtoEnum.Identifier)),
                _dtoEnum);
        }

        private static CSharpClass DtoClass(string name, string description, params CSharpProperty[] properties)
        {
            var type = new CSharpClass(new CSharpIdentifier("MyNamespace", name))
            {
                Summary = description,
                Attributes = {Attributes.GeneratedCode}
            };
            type.Properties.AddRange(properties);
            return type;
        }

        private static CSharpProperty Property(string name, string jsonName, string description, bool required = false, bool key = false, CSharpIdentifier? type = null)
        {
            var property = new CSharpProperty(type ?? CSharpIdentifier.String, name)
            {
                Summary = description,
                Attributes = {Attributes.JsonProperty(jsonName)},
                HasSetter = true
            };
            if (required) property.Attributes.Add(Attributes.Required);
            if (key) property.Attributes.Add(Attributes.Key);
            return property;
        }

        private static CSharpEnum DtoEnum(string name, string description, params CSharpEnumValue[] values)
        {
            var type = new CSharpEnum(new CSharpIdentifier("MyNamespace", name))
            {
                Summary = description,
                Attributes = {Attributes.GeneratedCode}
            };
            type.Values.AddRange(values);
            return type;
        }

        private static CSharpEnumValue DtoEnumValue(string name, string jsonName)
            => new(name) {Attributes = { Attributes.EnumMember(jsonName)}};
    }
}
