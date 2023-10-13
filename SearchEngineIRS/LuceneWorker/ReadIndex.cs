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
            string indexDocumentName = "data_index";
            string indexDocumentPath = Path.Combine(Environment.CurrentDirectory, indexDocumentName);

            LuceneDirectory directory = FSDirectory.Open(indexDocumentPath);
            IndexReader reader = DirectoryReader.Open(directory);
            IndexSearcher searcher = new IndexSearcher(reader);
            var analyzer = new StandardAnalyzer(luceneVersion);
            QueryParser queryParser = new QueryParser(luceneVersion, "content", analyzer);
            Query query = queryParser.Parse(searchQuery);
            TopDocs topDocs = searcher.Search(query, n: 10);

            SimpleHTMLFormatter formatter = new SimpleHTMLFormatter("<b>", "</b>");
            Highlighter highlighter = new Highlighter(formatter, new QueryScorer(query));

            int numMatchingDocs = topDocs.TotalHits;
            List<ExportDTO> output = new List<ExportDTO>();
            //List<string> output = new List<string> { "Total Matching Results: " + topDocs.TotalHits };

            if (numMatchingDocs > 0)
            {
                foreach (var doc in topDocs.ScoreDocs)
                {
                    Document resultDoc = searcher.Doc(doc.Doc);
                    string content = resultDoc.Get("content");
                    using var firstLine = new StringReader(content);
                    string title = firstLine.ReadLine() ?? string.Empty;

                    string highlightedContent = highlighter.GetBestFragment(analyzer, "content", content);

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
