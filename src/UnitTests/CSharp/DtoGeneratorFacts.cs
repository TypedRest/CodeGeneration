﻿using System.Collections.Generic;
 using FluentAssertions;
 using Microsoft.OpenApi.Models;
 using NanoByte.CodeGeneration;
using Xunit;

namespace TypedRest.CodeGeneration.CSharp
{
    public class DtoGeneratorFacts
    {
        [Fact]
        public void GeneratesCorrectDom()
        {
            var generator = new DtoGenerator(new NamingStrategy("MyService", "MyNamespace", "MyNamespace"));
            var generated = generator.Generate(new Dictionary<string, OpenApiSchema>
            {
                ["contact"] = Sample.ContactSchema,
                ["note"] = Sample.NoteSchema
            });

            generated.Should().BeEquivalentTo(
                Dto("Contact", "A contact in an address book.",
                    Property("Id", "id", "The ID of the contact.", key: true),
                    Property("FirstName", "firstName", "The first name of the contact.", required: true),
                    Property("LastName", "lastName", "The last name of the contact.", required: true)),
                Dto("Note", "A note about a specific contact.",
                    Property("Content", "content", "The content of the note.", required: true)));
        }

        private static CSharpClass Dto(string name, string description, params CSharpProperty[] properties)
        {
            var type = new CSharpClass(new CSharpIdentifier("MyNamespace", name))
            {
                Summary = description,
                Attributes = {Attributes.GeneratedCode}
            };
            type.Properties.AddRange(properties);
            return type;
        }

        private static CSharpProperty Property(string name, string jsonName, string description, bool required = false, bool key = false)
        {
            var property = new CSharpProperty(CSharpIdentifier.String, name)
            {
                Summary = description,
                Attributes = {Attributes.JsonProperty(jsonName)},
                HasSetter = true
            };
            if (required) property.Attributes.Add(Attributes.Required);
            if (key) property.Attributes.Add(Attributes.Key);
            return property;
        }
    }
}
