using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi.Patterns
{
    [PublicAPI]
    public class PathTree
    {
        [CanBeNull]
        public OpenApiPathItem Item { get; set; }

        [NotNull]
        public IDictionary<string, PathTree> Children { get; } = new Dictionary<string, PathTree>();

        public void Add([NotNull, ItemNotNull] string[] path, [NotNull] OpenApiPathItem item)
        {
            if (path.Length == 0)
            {
                Item = item;
                return;
            }

            var child = GetChild(path[0]);
            child.Add(path.Skip(1).ToArray(), item);
        }

        private PathTree GetChild(string name)
        {
            if (!Children.TryGetValue(name, out var child))
                Children.Add(name, child = new PathTree());
            return child;
        }

        [NotNull]
        public static PathTree From([NotNull] OpenApiPaths paths)
        {
            var tree = new PathTree();
            foreach (var path in paths)
            {
                tree.Add(
                    path.Key.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries),
                    path.Value);
            }
            return tree;
        }
    }
}
