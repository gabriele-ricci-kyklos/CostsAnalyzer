namespace CostsAnalyzer.Business.Parsers
{
    public enum ParserType { N26, IntesaSanPaolo, Hype, Revolut, Isybank }
    public interface ISourceParser
    {
        ParserType ParserType { get; }
        string[] SupportedFileExtensions { get; }
        ValueTask<bool> IsFileOfParserType(string filePath);
        ValueTask<RawMovement[]> ParseFileAsync(string filePath);
    }
}
