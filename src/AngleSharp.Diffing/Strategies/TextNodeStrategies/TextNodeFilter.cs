using System;

using AngleSharp.Diffing.Core;
using AngleSharp.Diffing.Extensions;
using AngleSharp.Dom;

namespace AngleSharp.Diffing.Strategies.TextNodeStrategies
{
    /// <summary>
    /// Represents the text node filter strategy.
    /// </summary>
    public class TextNodeFilter
    {
        private const string PRE_ELEMENTNAME = "PRE";
        private const string SCRIPT_ELEMENTNAME = "SCRIPT";
        private const string STYLE_ELEMENTNAME = "STYLE";
        private const string WHITESPACE_ATTR_NAME = "diff:whitespace";

        /// <summary>
        /// Gets the whitespace option of the filter instance.
        /// </summary>
        public WhitespaceOption Whitespace { get; }

        /// <summary>
        /// Creates a text node filter with the provided option.
        /// </summary>
        /// <param name="option"></param>
        public TextNodeFilter(WhitespaceOption option)
        {
            Whitespace = option;
        }

        /// <summary>
        /// The text node filter strategy.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="currentDecision"></param>
        /// <returns></returns>
        public FilterDecision Filter(in ComparisonSource source, FilterDecision currentDecision)
        {
            if (currentDecision.IsExclude())
                return currentDecision;
            return source.Node is IText textNode ? Filter(textNode) : currentDecision;
        }

        private FilterDecision Filter(IText textNode)
        {
            var option = GetWhitespaceOption(textNode);
            return option != WhitespaceOption.Preserve && string.IsNullOrWhiteSpace(textNode.Data)
                ? FilterDecision.Exclude
                : FilterDecision.Keep;
        }

        private WhitespaceOption GetWhitespaceOption(IText textNode)
        {
            var parent = textNode.ParentElement ?? throw new UnexpectedDOMTreeStructureException();

            if (parent.NodeName.Equals(PRE_ELEMENTNAME, StringComparison.Ordinal) ||
                parent.NodeName.Equals(SCRIPT_ELEMENTNAME, StringComparison.Ordinal) ||
                parent.NodeName.Equals(STYLE_ELEMENTNAME, StringComparison.Ordinal))
            {
                return parent.TryGetAttrValue(WHITESPACE_ATTR_NAME, out WhitespaceOption option)
                    ? option
                    : WhitespaceOption.Preserve;
            }

            return parent.GetInlineOptionOrDefault(WHITESPACE_ATTR_NAME, Whitespace);
        }
    }
}
