#nullable disable

using CostsAnalyzer.Data;

namespace CostsAnalyzer.Business.Movements
{
    public enum MovementSign { Income, Outcome, Transfer }

    public class Movement : IEncryptedEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Recipient { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Category { get; set; }
        public MovementSign Sign { get; set; }
    }
}
