using FileHelpers;

namespace CostsAnalyzer.Core.Parsers.N26
{
    public class N26Parser : ISourceParser
    {
        public ParserType ParserType => ParserType.N26;
        public ValueTask<RawMovementRow[]> ParseFileAsync(string filePath)
        {
            FileHelperEngine<N26FileRow> engine = new();
            N26FileRow[] fileRows = engine.ReadFile(filePath);

            List<RawMovementRow> rows = new();
            foreach (N26FileRow row in fileRows)
            {
                RawMovementRow mov =
                    new()
                    {
                        Date = row.Date,
                        Recipient = row.Recipient,
                        Description = row.Description,
                        Amount = row.CurrencyAmount == default ? row.EuroAmount : row.CurrencyAmount,
                        Currency = string.IsNullOrWhiteSpace(row.Currency) ? "EUR" : row.Currency,
                    };

                rows.Add(mov);
            }

            return ValueTask.FromResult(rows.ToArray());
        }
    }
}
