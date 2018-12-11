using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace TypedRest.OpenApi.Patterns
{
    public class PathTreeFacts
    {
        [Fact]
        public void BuildsFromList()
        {
            var root = new OpenApiPathItem {Summary = "Root"};
            var health = new OpenApiPathItem {Summary = "Health"};
            var usersA = new OpenApiPathItem {Summary = "Users A"};
            var usersB = new OpenApiPathItem {Summary = "Users B"};
            var paths = new OpenApiPaths
            {
                ["/"] = root,
                ["/health"] = health,
                ["/users/a"] = usersA,
                ["/users/b"] = usersB,
            };

            var tree = PathTree.From(paths);

            tree.Should().BeEquivalentTo(new PathTree
            {
                Item = root,
                Children =
                {
                    ["health"] = new PathTree {Item = health},
                    ["users"] = new PathTree
                    {
                        Children =
                        {
                            ["a"] = new PathTree {Item = usersA},
                            ["b"] = new PathTree {Item = usersB}
                        }
                    }
                }
            });
        }
    }
}
