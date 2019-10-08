using System;
using System.Collections.Generic;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace Egil.AngleSharp.Diffing.Core
{
    public abstract class DiffingTestBase
    {
        private readonly IBrowsingContext _context = BrowsingContext.New();
        private readonly IHtmlParser _htmlParser;
        private readonly IDocument _document;

        protected INodeList EmptyNodeList => ToNodeList("");

        protected DiffingTestBase()
        {
            _htmlParser = _context.GetService<IHtmlParser>();
            _document = _context.OpenNewAsync().Result;
        }

        protected INodeList ToNodeList(string? htmlsnippet)
        {
            var fragment = _htmlParser.ParseFragment(htmlsnippet, _document.Body);
            return fragment;
        }

        protected IEnumerable<ComparisonSource> ToComparisonSourceList(string html)
        {
            return ToNodeList(html).ToComparisonSourceList(ComparisonSourceType.Control);
        }

        protected INode ToNode(string htmlsnippet)
        {
            var fragment = _htmlParser.ParseFragment(htmlsnippet, _document.Body);
            return fragment[0];
        }

        protected ComparisonSource ToComparisonSource(string html, ComparisonSourceType sourceType = ComparisonSourceType.Control)
        {
            return ToNode(html).ToComparisonSource(0, sourceType);
        }

        protected static HtmlDifferenceEngine CreateHtmlDiffEngine(
                Func<DiffContext, SourceCollection, SourceCollection, IEnumerable<Comparison>>? nodeMatcher = null,
                Func<DiffContext, SourceMap, SourceMap, IEnumerable<AttributeComparison>>? attrMatcher = null,
                Func<ComparisonSource, FilterDecision>? nodeFilter = null,
                Func<AttributeComparisonSource, FilterDecision>? attrFilter = null,
                Func<Comparison, CompareResult>? nodeComparer = null,
                Func<AttributeComparison, CompareResult>? attrComparer = null
            )
        {
            return new HtmlDifferenceEngine(
                new MockFilterStrategy(nodeFilter, attrFilter),
                new MockMatcherStrategy(nodeMatcher, attrMatcher),
                new MockCompareStrategy(nodeComparer, attrComparer)
            );
        }

        class MockMatcherStrategy : IMatcherStrategy
        {
            private readonly Func<DiffContext, SourceCollection, SourceCollection, IEnumerable<Comparison>>? _nodeMatcher;
            private readonly Func<DiffContext, SourceMap, SourceMap, IEnumerable<AttributeComparison>>? _attrMatcher;

            public MockMatcherStrategy(
                Func<DiffContext, SourceCollection, SourceCollection, IEnumerable<Comparison>>? nodeMatcher = null,
                Func<DiffContext, SourceMap, SourceMap, IEnumerable<AttributeComparison>>? attrMatcher = null)
            {
                _nodeMatcher = nodeMatcher;
                _attrMatcher = attrMatcher;
            }

            public IEnumerable<Comparison> Match(
                DiffContext context,
                SourceCollection controlNodes,
                SourceCollection testNodes) => _nodeMatcher!(context, controlNodes, testNodes);

            public IEnumerable<AttributeComparison> Match(
                DiffContext context,
                SourceMap controlAttributes,
                SourceMap testAttributes) => _attrMatcher!(context, controlAttributes, testAttributes);
        }

        class MockFilterStrategy : IFilterStrategy
        {
            private readonly Func<ComparisonSource, FilterDecision>? _nodeFilter;
            private readonly Func<AttributeComparisonSource, FilterDecision>? _attrFilter;

            public MockFilterStrategy(Func<ComparisonSource, FilterDecision>? nodeFilter = null, Func<AttributeComparisonSource, FilterDecision>? attrFilter = null)
            {
                _nodeFilter = nodeFilter;
                _attrFilter = attrFilter;
            }

            public FilterDecision Filter(in AttributeComparisonSource attributeComparisonSource)
                => _attrFilter!(attributeComparisonSource);

            public FilterDecision Filter(in ComparisonSource comparisonSource)
                => _nodeFilter!(comparisonSource);
        }

        class MockCompareStrategy : ICompareStrategy
        {
            private readonly Func<Comparison, CompareResult>? _nodeCompare;
            private readonly Func<AttributeComparison, CompareResult>? _attrCompare;

            public MockCompareStrategy(Func<Comparison, CompareResult>? nodeCompare = null, Func<AttributeComparison, CompareResult>? attrCompare = null)
            {
                _nodeCompare = nodeCompare;
                _attrCompare = attrCompare;
            }

            public CompareResult Compare(in Comparison comparison)
                => _nodeCompare!(comparison);

            public CompareResult Compare(in AttributeComparison comparison)
                => _attrCompare!(comparison);
        }
    }
}