using FileHelpers;
using System.Text;

namespace CostsAnalyzer.Business.Parsers.Revolut
{
    public class RevolutParser : ISourceParser
    {
        public ParserType ParserType => ParserType.Revolut;
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
                return new(s.StartsWith("Type"));
            }
            catch (Exception)
            {
                return new(false);
            }
        }

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
