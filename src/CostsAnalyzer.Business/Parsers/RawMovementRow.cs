#nullable disable

namespace CostsAnalyzer.Business.Parsers
{
    public enum RawMovementSign { Income, Outcome }

    public class RawMovementRow
    {
        public DateTime Date { get; set; }
        public string Recipient { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public RawMovementSign Sign { get; set; }
    }
}
