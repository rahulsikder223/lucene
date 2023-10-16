using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.Text;
using static Lucene.Net.Documents.Field;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace LuceneWorker
{
    public class WriteIndex
    {
        const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

        public void Write()
        {
            string indexDirectoryPath = Path.Combine(Environment.CurrentDirectory, Constants.IndexDirectory);
            string documentFilePath = Path.Combine(Environment.CurrentDirectory, Constants.DatasetDirectory);

            using (LuceneDirectory indexDir = FSDirectory.Open(indexDirectoryPath)) {
                Analyzer standardAnalyzer = new StandardAnalyzer(luceneVersion);
                IndexWriterConfig indexConfig = new IndexWriterConfig(luceneVersion, standardAnalyzer);
                indexConfig.OpenMode = OpenMode.CREATE;
                IndexWriter writer = new IndexWriter(indexDir, indexConfig);
                WriteIndexRecursive(writer, documentFilePath);
                writer.Commit();
            }
        }

        void WriteIndexRecursive(IndexWriter writer, string startDirectory)
        {
            try
            {
                foreach (string file in System.IO.Directory.GetFiles(startDirectory))
                {
                    Document document = new Document();
                    document.Add(new StringField(Constants.SearchContentPath, file, Store.YES));
                    document.Add(new TextField(Constants.SearchContentPath, File.ReadAllText(file, Encoding.UTF8), Store.YES));
                    writer.AddDocument(document);
                }

                foreach (string directory in System.IO.Directory.GetDirectories(startDirectory))
                {
                    WriteIndexRecursive(writer, directory);
                }
            }
            catch (Exception) { }
        }
    }
}
