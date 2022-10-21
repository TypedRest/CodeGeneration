using System.Net;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints.Raw;
using Xunit;

namespace TypedRest.CodeGeneration.Patterns.Raw;

public class UploadPatternFacts : PatternFactsBase<UploadPattern>
{
    [Fact]
    public void GetsEndpoint()
    {
        var tree = new PathTree
        {
            Item = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Post] = Sample.Operation(statusCode: HttpStatusCode.Accepted, mimeType: "application/octet-stream", request: new OpenApiSchema(), description: "Upload a file.")
                }
            }
        };

        TryGetEndpoint(tree).Should().BeEquivalentTo(new UploadEndpoint
        {
            Description = "Upload a file."
        }, options => options.IncludingAllRuntimeProperties());
    }

    [Fact]
    public void GetsEndpointForFormUpload()
    {
        var formSchema = new OpenApiSchema
        {
            Properties =
            {
                ["my-file"] = new OpenApiSchema {Type = "binary"}
            }
        };

        var tree = new PathTree
        {
            Item = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Post] = Sample.Operation(statusCode: HttpStatusCode.Accepted, mimeType: "multipart/form-data", request: formSchema, description: "Upload a file.")
                }
            }
        };

        TryGetEndpoint(tree).Should().BeEquivalentTo(new UploadEndpoint
        {
            FormField = "my-file",
            Description = "Upload a file."
        }, options => options.IncludingAllRuntimeProperties());
    }
}
