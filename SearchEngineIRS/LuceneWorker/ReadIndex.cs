using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Lucene.Net.Util;
using LuceneWorker.DTOs;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace LuceneWorker
{
    public class ReadIndex
    {
        const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

        public List<ExportDTO> Read(string searchQuery)
        {
            string indexDocumentPath = Path.Combine(Environment.CurrentDirectory, Constants.IndexDirectory);

            LuceneDirectory directory = FSDirectory.Open(indexDocumentPath);
            IndexReader reader = DirectoryReader.Open(directory);
            IndexSearcher searcher = new IndexSearcher(reader);
            searcher.Similarity = new CustomSimilarity();
            var analyzer = new StandardAnalyzer(luceneVersion);
            QueryParser queryParser = new QueryParser(luceneVersion, Constants.SearchContent, analyzer);
            queryParser.AllowLeadingWildcard = true; // allowing wildcards...
            Query phraseQuery = queryParser.CreatePhraseQuery(Constants.SearchContent, searchQuery);
            Query booleanQuery = queryParser.CreateBooleanQuery(Constants.SearchContent, searchQuery);
            Query minShouldMatchQuery = queryParser.CreateMinShouldMatchQuery(Constants.SearchContent, searchQuery, 0.2f);

            TopDocs topDocsPhrase = searcher.Search(phraseQuery, n: 10);
            TopDocs topDocsBoolean = searcher.Search(booleanQuery, n: 10);
            TopDocs topDocsMinShouldMatch = searcher.Search(minShouldMatchQuery, n: 10);

            SimpleHTMLFormatter formatter = new SimpleHTMLFormatter("<b>", "</b>");

            int matchingDocsPhraseCount = topDocsPhrase.TotalHits;
            int matchingDocsBooleanCount = topDocsBoolean.TotalHits;
            int matchingDocsMinShouldMatchCount = topDocsMinShouldMatch.TotalHits;
            List<ExportDTO> output = new List<ExportDTO>();
            //List<string> output = new List<string> { "Total Matching Results: " + topDocs.TotalHits };

            if (matchingDocsPhraseCount > 0)
            {
                foreach (var doc in topDocsPhrase.ScoreDocs)
                {
                    Document resultDoc = searcher.Doc(doc.Doc);
                    string content = resultDoc.Get(Constants.SearchContent);
                    using var firstLine = new StringReader(content);
                    string title = firstLine.ReadLine() ?? string.Empty;
                    Highlighter highlighter = new Highlighter(formatter, new QueryScorer(phraseQuery));
                    string highlightedContent = highlighter.GetBestFragment(analyzer, Constants.SearchContent, content);

                    ExportDTO exportObject = new ExportDTO
                    {
                        PaperName = title,
                        Description = highlightedContent,
                        Score = doc.Score
                    };

                    output.Add(exportObject);
                }
            }

            if (matchingDocsBooleanCount > 0)
            {
                foreach (var doc in topDocsBoolean.ScoreDocs)
                {
                    Document resultDoc = searcher.Doc(doc.Doc);
                    string content = resultDoc.Get(Constants.SearchContent);
                    using var firstLine = new StringReader(content);
                    string title = firstLine.ReadLine() ?? string.Empty;
                    Highlighter highlighter = new Highlighter(formatter, new QueryScorer(booleanQuery));
                    string highlightedContent = highlighter.GetBestFragment(analyzer, Constants.SearchContent, content);

                    ExportDTO exportObject = new ExportDTO
                    {
                        PaperName = title,
                        Description = highlightedContent,
                        Score = doc.Score
                    };

                    output.Add(exportObject);
                }
            }

            if (matchingDocsMinShouldMatchCount > 0)
            {
                foreach (var doc in topDocsMinShouldMatch.ScoreDocs)
                {
                    Document resultDoc = searcher.Doc(doc.Doc);
                    string content = resultDoc.Get(Constants.SearchContent);
                    using var firstLine = new StringReader(content);
                    string title = firstLine.ReadLine() ?? string.Empty;
                    Highlighter highlighter = new Highlighter(formatter, new QueryScorer(minShouldMatchQuery));
                    string highlightedContent = highlighter.GetBestFragment(analyzer, Constants.SearchContent, content);

                    ExportDTO exportObject = new ExportDTO
                    {
                        PaperName = title,
                        Description = highlightedContent,
                        Score = doc.Score
                    };

                    output.Add(exportObject);
                }
            }

            return output;
        }
    }
}
