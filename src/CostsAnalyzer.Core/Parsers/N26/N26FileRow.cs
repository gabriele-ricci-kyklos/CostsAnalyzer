#nullable disable

using FileHelpers;

namespace CostsAnalyzer.Core.Parsers.N26
{
    [DelimitedRecord(",")]
    [IgnoreEmptyLines]
    [IgnoreFirst]
    internal class N26FileRow
    {
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Date { get; set; }
        public string Recipient { get; set; }
        public string Typology { get; set; }
        public string Description { get; set; }
        public decimal EuroAmount { get; set; }
        public decimal CurrencyAmount { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
