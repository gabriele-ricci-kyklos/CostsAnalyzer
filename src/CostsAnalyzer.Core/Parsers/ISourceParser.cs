namespace CostsAnalyzer.Core.Parsers
{
    public enum ParserType { N26, IntesaSanPaolo, Hype }
    public interface ISourceParser
    {
        ParserType ParserType { get; }
        ValueTask<RawMovementRow[]> ParseFileAsync(string filePath);
    }
}
