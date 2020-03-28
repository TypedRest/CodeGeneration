﻿using System.Collections.Generic;
 using FluentAssertions;
 using Microsoft.OpenApi.Models;
 using TypedRest.OpenApi.CSharp.Dom;
using Xunit;

namespace TypedRest.OpenApi.CSharp
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
                    Property("Id", "The ID of the contact.", CSharpIdentifier.String),
                    Property("FirstName", "The first name of the contact.", CSharpIdentifier.String),
                    Property("LastName", "The last name of the contact.", CSharpIdentifier.String)),
                Dto("Note", "A note about a specific contact.",
                    Property("Content", "The content of the note.", CSharpIdentifier.String)));
        }

        private static CSharpClass Dto(string name, string description, params CSharpProperty[] properties)
        {
            var type = new CSharpClass(new CSharpIdentifier("MyNamespace", name))
            {
                Description = description
            };
            type.Properties.AddRange(properties);
            return type;
        }

        private static CSharpProperty Property(string name, string description, CSharpIdentifier type)
            => new CSharpProperty(type, name)
            {
                Description = description,
                HasSetter = true
            };
    }
}
