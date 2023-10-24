#nullable disable
// Type,Product,Started Date,Completed Date,Description,Amount,Fee,Currency,State,Balance

using FileHelpers;

namespace CostsAnalyzer.Business.Parsers.Revolut
{
    [DelimitedRecord(",")]
    [IgnoreEmptyLines]
    [IgnoreFirst]
    internal class RevolutFileRow
    {
        public string Type { get; set; }
        public string Product { get; set; }
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime StartedDate { get; set; }
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime CompletedDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        [FieldNullValue(typeof(decimal), "0")]
        public decimal Fee { get; set; }
        public string Currency { get; set; }
        public string State { get; set; }
        [FieldNullValue(typeof(decimal), "0")]
        public decimal Balance { get; set; }
    }
}
