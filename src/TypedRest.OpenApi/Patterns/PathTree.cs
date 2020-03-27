using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi.Patterns
{
    /// <summary>
    /// A tree-like structure organizing <see cref="OpenApiPathItem"/>s based on path prefixes.
    /// </summary>
    public class PathTree
    {
        /// <summary>
        /// The <see cref="OpenApiPathItem"/> at this level of the tree, if any.
        /// </summary>
        public OpenApiPathItem? Item { get; set; }

        /// <summary>
        /// A map of sub-paths to sub-trees.
        /// </summary>
        public IDictionary<string, PathTree> Children { get; } = new Dictionary<string, PathTree>();

        /// <summary>
        /// Adds a <see cref="OpenApiPathItem"/> to the tree.
        /// </summary>
        /// <param name="path">The path of the item.</param>
        /// <param name="item">The item.</param>
        public void Add(string[] path, OpenApiPathItem item)
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
        public static PathTree From(OpenApiPaths paths)
        {
            var tree = new PathTree();
            foreach ((string path, var item) in paths)
            {
                tree.Add(
                    path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries),
                    item);
            }
            return tree;
        }
    }
}
