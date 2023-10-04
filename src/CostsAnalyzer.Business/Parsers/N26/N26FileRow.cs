#nullable disable

using FileHelpers;

namespace CostsAnalyzer.Business.Parsers.N26
{
    [DelimitedRecord(",")]
    [IgnoreEmptyLines]
    [IgnoreFirst]
    internal class N26FileRow
    {
        [FieldQuoted]
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Date { get; set; }
        [FieldQuoted]
        public string Recipient { get; set; }
        [FieldQuoted]
        public string AccountNumber { get; set; }
        [FieldQuoted]
        public string Typology { get; set; }
        [FieldQuoted]
        public string Description { get; set; }
        [FieldQuoted]
        public decimal EuroAmount { get; set; }
        [FieldQuoted]
        [FieldNullValue(typeof(decimal), "0")]
        public decimal CurrencyAmount { get; set; }
        [FieldQuoted]
        public string Currency { get; set; }
        [FieldQuoted]
        [FieldNullValue(typeof(decimal), "0")]
        public decimal ExchangeRate { get; set; }
    }
}
