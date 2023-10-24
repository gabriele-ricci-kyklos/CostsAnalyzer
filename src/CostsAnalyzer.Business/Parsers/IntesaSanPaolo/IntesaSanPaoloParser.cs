namespace CostsAnalyzer.Business.Parsers.IntesaSanPaolo
{
    public class IntesaSanPaoloParser : AbstractISPParser
    {
        public override ParserType ParserType => ParserType.IntesaSanPaolo;
        protected override int StartingRow => 20;
    }
}
