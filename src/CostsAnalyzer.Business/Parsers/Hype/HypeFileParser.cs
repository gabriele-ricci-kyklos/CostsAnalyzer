using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Globalization;

namespace CostsAnalyzer.Business.Parsers.Hype
{
    public class HypeFileParser : ISourceParser
    {
        public ParserType ParserType => ParserType.Hype;

        public ValueTask<RawMovement[]> ParseFileAsync(string filePath)
        {
            List<RawMovement> results = new();
            using PdfReader reader = new(filePath);
            using PdfDocument doc = new(reader);
            for (int i = 1; i <= doc.GetNumberOfPages(); ++i)
            {
                var page = doc.GetPage(i);
                string text = PdfTextExtractor.GetTextFromPage(page, new SimpleTextExtractionStrategy());

                string[] lines = text.Split('\n');
                List<string> buffer = new();
                for (int j = 0; j < lines.Length; ++j)
                {
                    string line = lines[j];
                    bool startsWithDate =
                        line.Length >= 10
                        && DateTime.TryParseExact(line[..10], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

                    bool endsWithEuroSign = line.EndsWith('€');

                    if (startsWithDate && endsWithEuroSign)
                    {
                        RawMovement row = ParseMovementRow(line);
                        results.Add(row);
                    }
                    else
                    {
                        if (buffer.Any() || startsWithDate)
                        {
                            buffer.Add(line);
                        }

                        if (buffer.Any() && endsWithEuroSign)
                        {
                            string fullLine = string.Join(" ", buffer);
                            RawMovement row = ParseMovementRow(fullLine);
                            results.Add(row);
                            buffer.Clear();
                        }
                    }
                }
            }
            doc.Close();
            reader.Close();

            return ValueTask.FromResult(results.ToArray());
        }

        private static RawMovement ParseMovementRow(string line)
        {
            string strDate = line[..10];
            string strSecondDate = line.Contains("---") ? " --- " : line.Substring(11, 10);
            (string amountString, RawMovementSign sign, decimal amount) = ExtractAmount(line);

            line =
                line
                    .Replace("pagamento", string.Empty, true, CultureInfo.InvariantCulture)
                    .Replace("risparmi", string.Empty, true, CultureInfo.InvariantCulture)
                    .Replace(strDate, string.Empty)
                    .Replace(strSecondDate, string.Empty)
                    .Replace(amountString, string.Empty)
                    .Trim()
                    ;

            (string recipient, string description) = ExtractRecipientAndDescription(line);

            RawMovement item =
                new()
                {
                    Date = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                    Amount = amount,
                    Currency = "EUR",
                    Recipient = recipient,
                    Description = description,
                    Sign = sign,
                    ParserType = ParserType.Hype
                };

            return item;
        }

        private static (string AmountString, RawMovementSign Sign, decimal Amount) ExtractAmount(string line)
        {
            string strRawAmount = string.Empty;

            for (int i = line.Length - 1; i >= 0; --i)
            {
                char c = line[i];
                strRawAmount += c;
                if (c == '-' || c == '+')
                {
                    break;
                }
            }

            strRawAmount = ReverseString(strRawAmount);

            RawMovementSign sign =
                strRawAmount.Contains('+')
                    ? RawMovementSign.Income
                    : RawMovementSign.Outcome;

            string strAmount =
                strRawAmount
                    .Replace(" ", string.Empty)
                    .Replace("-", string.Empty)
                    .Replace("+", string.Empty)
                    .Replace("€", string.Empty)
                    .Replace(',', '.');

            return (strRawAmount, sign, Convert.ToDecimal(strAmount));
        }

        private static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static (string Recipient, string Description) ExtractRecipientAndDescription(string line)
        {
            string[] tokens = line.Split(' ');
            List<string> oneOccurrenceTokens = new();
            List<string> multipleOccurrencesTokens = new();

            foreach (string token in tokens)
            {
                if (oneOccurrenceTokens.Contains(token, StringComparer.OrdinalIgnoreCase))
                {
                    multipleOccurrencesTokens.Add(token);
                }
                else
                {
                    oneOccurrenceTokens.Add(token);
                }
            }

            string oneOccurrenceString = string.Join(' ', oneOccurrenceTokens);
            string multipleOccurrencesString = multipleOccurrencesTokens.Any() ? string.Join(' ', multipleOccurrencesTokens) : oneOccurrenceString;

            return (multipleOccurrencesString, oneOccurrenceString);
        }
    }
}
