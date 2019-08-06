using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi.Patterns
{
    /// <summary>
    /// A tree-like structure organizing <see cref="OpenApiPathItem"/>s based on path prefixes.
    /// </summary>
    [PublicAPI]
    public class PathTree
    {
        /// <summary>
        /// The <see cref="OpenApiPathItem"/> at this level of tree, if any.
        /// </summary>
        [CanBeNull]
        public OpenApiPathItem Item { get; set; }

        /// <summary>
        /// A map of sub-paths to sub-trees.
        /// </summary>
        [NotNull]
        public IDictionary<string, PathTree> Children { get; } = new Dictionary<string, PathTree>();

        /// <summary>
        /// Adds a <see cref="OpenApiPathItem"/> to the tree.
        /// </summary>
        /// <param name="path">The path of the item.</param>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Builds a <see cref="PathTree"/> from an <see cref="OpenApiPaths"/> collection.
        /// </summary>
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
