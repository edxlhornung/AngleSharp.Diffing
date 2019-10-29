﻿using Egil.AngleSharp.Diffing.Strategies.AttributeStrategies;
using Egil.AngleSharp.Diffing.Strategies.TextNodeStrategies;

namespace Egil.AngleSharp.Diffing
{
    public static partial class DiffBuilderExtensions
    {
        /// <summary>
        /// Sets up the diffing process using the default options.
        /// </summary>
        public static DiffBuilder WithDefaultOptions(this DiffBuilder builder)
        {
            return builder
                .IgnoreDiffAttributes()
                .IgnoreComments()
                .WithSearchingNodeMatcher()
                .WithCssSelectorMatcher()
                .WithAttributeNameMatcher()
                .WithNodeNameComparer()
                .WithIgnoreElementSupport()
                .WithStyleSheetComparer()
                .WithTextComparer(WhitespaceOption.Normalize, ignoreCase: false)
                .WithAttributeComparer()
                .WithClassAttributeComparer()
                .WithBooleanAttributeComparer(BooleanAttributeComparision.Strict)
                .WithStyleAttributeComparer();
            ;
        }
    }
}
