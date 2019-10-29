﻿using System;
using Egil.AngleSharp.Diffing.Core;

namespace Egil.AngleSharp.Diffing.Strategies.AttributeStrategies
{
    public static class IgnoreDiffAttributesFilter
    {
        private const string DiffAttributePrefix = "diff:";

        public static FilterDecision Filter(in AttributeComparisonSource source, FilterDecision currentDecision)
        {
            if (currentDecision.IsExclude()) return currentDecision;

            if (source.Attribute.Name.StartsWith(DiffAttributePrefix, StringComparison.OrdinalIgnoreCase))
                return FilterDecision.Exclude;

            return currentDecision;
        }
    }
}
