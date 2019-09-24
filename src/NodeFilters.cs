﻿using System;
using AngleSharp.Dom;

namespace Egil.AngleSharp.Diffing
{
    public static class Filters
    {
        public static bool None<T>(T node) => true;

        public static bool ElementIgnoreAttributeFilter(INode node)
        {
            if (node is IElement element)
            {
                var ignoreAttr = element.Attributes["diff:ignore"];
                return ignoreAttr == null || !ignoreAttr.IsEmptyOr("TRUE");
            }
            return true;
        }
    }

    public static class AttrExtensions
    {
        public static bool IsEmptyOr(this IAttr attr, string testValue, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            attr = attr ?? throw new ArgumentNullException(nameof(attr));
            var value = attr.Value;
            return string.IsNullOrWhiteSpace(value) || value.Equals(testValue, comparison);
        }
    }

    // public class DifferenceEngine
    // {
    //     private NodeFilter _nodeFilter = _ => true;

    //     public NodeFilter NodeFilter { get => _nodeFilter; set => _nodeFilter = value ?? throw new ArgumentNullException(nameof(NodeFilter)); }

    //     public IReadOnlyCollection<Difference> Compare(INodeList controlNodes, INodeList testNodes)
    //     {
    //         if (controlNodes is null) throw new ArgumentNullException(nameof(controlNodes));
    //         if (testNodes is null) throw new ArgumentNullException(nameof(testNodes));

    //         if (controlNodes.Length == 0 && testNodes.Length == 0)
    //             return Array.Empty<Difference>();

    //         var selectedControlNodes = controlNodes
    //             .WalkNodeTree()
    //             .Where(x => NodeFilter(x))
    //             .ToList();

    //         var selectedTestNodes = testNodes
    //             .WalkNodeTree()
    //             .Where(x => NodeFilter(x))
    //             .ToList();            

    //         return null;
    //     }
    // }
}
