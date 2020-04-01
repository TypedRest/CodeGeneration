using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace TypedRest.CodeGeneration.Patterns
{
    public class PathTreeFacts
    {
        [Fact]
        public void BuildsFromList()
        {
            var root = new OpenApiPathItem {Summary = "Root"};
            var health = new OpenApiPathItem {Summary = "Health"};
            var usersA = new OpenApiPathItem {Summary = "User A"};
            var usersB = new OpenApiPathItem {Summary = "User B"};
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

        [Fact]
        public void TrimsPlaceholders()
        {
            var itemA = new OpenApiPathItem {Summary = "Item A"};
            var itemB = new OpenApiPathItem {Summary = "Item B"};
            var paths = new OpenApiPaths
            {
                ["/{name}/a"] = itemA,
                ["/{id}/b"] = itemB,
            };

            var tree = PathTree.From(paths);

            tree.Should().BeEquivalentTo(new PathTree
            {
                Children =
                {
                    ["{}"] = new PathTree
                    {
                        Children =
                        {
                            ["a"] = new PathTree {Item = itemA},
                            ["b"] = new PathTree {Item = itemB}
                        }
                    }
                }
            });
        }
    }
}
