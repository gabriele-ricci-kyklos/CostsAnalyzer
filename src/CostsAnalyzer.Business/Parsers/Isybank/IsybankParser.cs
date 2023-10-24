using CostsAnalyzer.Business.Parsers.IntesaSanPaolo;

namespace CostsAnalyzer.Business.Parsers.Isybank
{
    public class IsybankParser : AbstractISPParser
    {
        public override ParserType ParserType => ParserType.Isybank;
        protected override int StartingRow => 19;
        protected override string RGBHeaderCellColor => "FF0373C9";
    }
}
