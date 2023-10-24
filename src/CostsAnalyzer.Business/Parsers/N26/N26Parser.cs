using FileHelpers;
using System;
using System.Text;

namespace CostsAnalyzer.Business.Parsers.N26
{
    public class N26Parser : ISourceParser
    {
        public ParserType ParserType => ParserType.N26;
        public string[] SupportedFileExtensions => new[] { "csv" };

        public ValueTask<bool> IsFileOfParserType(string filePath)
        {
            if (Path.GetExtension(filePath).Remove(0, 1) != "csv")
            {
                return new(false);
            }

            byte[] buffer = new byte[512];
            using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            int bytes_read = fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            if (bytes_read != buffer.Length)
            {
                throw new FileLoadException($"Unable to read first {buffer.Length} bytes of the file", filePath);
            }

            try
            {
                string s = Encoding.UTF8.GetString(buffer);
                return new(s.StartsWith("\"Data\""));
            }
            catch (Exception)
            {
                return new(false);
            }
        }

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
