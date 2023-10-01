namespace CostsAnalyzer.Business.Parsers
{
    public enum ParserType { N26, IntesaSanPaolo, Hype }
    public interface ISourceParser
    {
        ParserType ParserType { get; }
        ValueTask<RawMovement[]> ParseFileAsync(string filePath);
    }
}
