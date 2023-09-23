#nullable disable

namespace CostsAnalyzer.Core.Parsers
{
    public class RawMovementRow
    {
        public DateTime Date { get; set; }
        public string Recipient { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
