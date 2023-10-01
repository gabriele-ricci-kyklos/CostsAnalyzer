using FileHelpers;

namespace CostsAnalyzer.Business.Parsers.N26
{
    public class N26Parser : ISourceParser
    {
        public ParserType ParserType => ParserType.N26;
        public ValueTask<RawMovement[]> ParseFileAsync(string filePath)
        {
            FileHelperEngine<N26FileRow> engine = new();
            N26FileRow[] fileRows = engine.ReadFile(filePath);

            List<RawMovement> rows = new();
            foreach (N26FileRow row in fileRows)
            {
                decimal amount = row.CurrencyAmount == default ? row.EuroAmount : row.CurrencyAmount;
                RawMovementSign sign = amount < 0 ? RawMovementSign.Outcome : RawMovementSign.Income;

                RawMovement mov =
                    new()
                    {
                        Date = row.Date,
                        Recipient = row.Recipient,
                        Description = row.Description,
                        Amount = Math.Abs(amount),
                        Sign = sign,
                        Currency = string.IsNullOrWhiteSpace(row.Currency) ? "EUR" : row.Currency,
                        ParserType = ParserType.N26
                    };

                rows.Add(mov);
            }

            return ValueTask.FromResult(rows.ToArray());
        }
    }
}
