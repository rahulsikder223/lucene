using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace LuceneWorker
{
    public class ReadIndex
    {
        const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

        public List<string> Read(string searchQuery)
        {
            string indexDocumentName = "data_index";
            string indexDocumentPath = Path.Combine(Environment.CurrentDirectory, indexDocumentName);

            LuceneDirectory directory = FSDirectory.Open(indexDocumentPath);
            IndexReader reader = DirectoryReader.Open(directory);
            IndexSearcher searcher = new IndexSearcher(reader);
            QueryParser queryParser = new QueryParser(luceneVersion, "content", new StandardAnalyzer(luceneVersion));
            Query query = queryParser.Parse(searchQuery);
            TopDocs topDocs = searcher.Search(query, n: 10);

            int numMatchingDocs = topDocs.TotalHits;
            List<string> output = new List<string> { "Total Matching Results: " + topDocs.TotalHits };

            if (numMatchingDocs > 0)
            {
                foreach (var doc in topDocs.ScoreDocs)
                {
                    Document resultDoc = searcher.Doc(doc.Doc);
                    string content = resultDoc.Get("content");
                    output.Add("Result: " + content);
                }
            }

            return output;
        }
    }
}
