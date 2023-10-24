using FileHelpers;

namespace CostsAnalyzer.Business.Parsers.Revolut
{
    public class RevolutParser : ISourceParser
    {
        public ParserType ParserType => ParserType.Revolut;
        public ValueTask<RawMovement[]> ParseFileAsync(string filePath)
        {
            FileHelperEngine<RevolutFileRow> engine = new();
            RevolutFileRow[] fileRows = engine.ReadFile(filePath);

            List<RawMovement> rows = new();
            foreach (RevolutFileRow row in fileRows)
            {
                RawMovementSign sign = row.Amount < 0 ? RawMovementSign.Outcome : RawMovementSign.Income;

                RawMovement mov =
                    new()
                    {
                        Date = row.StartedDate,
                        Recipient = row.Type, //?
                        Description = row.Description,
                        Amount = Math.Abs(row.Amount),
                        Sign = sign,
                        Currency = string.IsNullOrWhiteSpace(row.Currency) ? "EUR" : row.Currency,
                        ParserType = ParserType.Revolut
                    };

                rows.Add(mov);
            }

            return ValueTask.FromResult(rows.ToArray());
        }
    }
}
