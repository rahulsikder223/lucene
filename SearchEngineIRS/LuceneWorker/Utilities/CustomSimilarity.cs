using Lucene.Net.Index;
using Lucene.Net.Search.Similarities;
using Lucene.Net.Util;

public class CustomSimilarity : DefaultSimilarity
{
    static float _tf = 0;
    static float _idf = 0;

    public override float Idf(long docFreq, long numDocs)
    {
        //_idf = (float)(Math.Log((numDocs + 1) / docFreq) / Math.Log(2)); // Standard Lucene...
        _idf = (float)Math.Log(1 + (numDocs - docFreq + 0.5) / (docFreq + 0.5));
        // The addition of 0.5 in the numerator and denominator is a smoothing technique to prevent
        // division by zero when the term does not appear in the collection(i.e., when docFreq is zero)

        // The formula ensures that terms that are rare (i.e., they appear in fewer documents)
        // receive a higher IDF score, which means they are considered more important when
        // calculating relevance in information retrieval. Smoothing is used to adjust the
        // IDF to avoid extreme values when dealing with very common or very rare terms.
        return _idf;
    }

    public override float LengthNorm(FieldInvertState state)
    {
        return 1.0f; // Disable length normalization
    }

    public override float Tf(float freq)
    {
        _tf = (float)(1 + (Math.Log(freq) / Math.Log(2)));
        return _tf;
    }

    public override float ScorePayload(int doc, int start, int end, BytesRef payload)
    {
        return _tf * _idf;
    }
}
