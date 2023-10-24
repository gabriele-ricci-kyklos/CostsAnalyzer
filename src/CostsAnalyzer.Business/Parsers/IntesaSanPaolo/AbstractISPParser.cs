using OfficeOpenXml;

namespace CostsAnalyzer.Business.Parsers.IntesaSanPaolo
{
    public abstract class AbstractISPParser : ISourceParser
    {
        public abstract ParserType ParserType { get; }
        protected abstract int StartingRow { get; }

        public ValueTask<RawMovement[]> ParseFileAsync(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage excelPackage = new(filePath);
            if (excelPackage.Workbook.Worksheets.Count < 1)
            {
                throw new NotSupportedException("No excel sheets have been found");
            }

            using ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.First();
            int row = StartingRow;
            int col = 1;
            int colCount = 8;
            List<RawMovement> results = new();

            do
            {
                RawMovement item = new() { ParserType = ParserType.IntesaSanPaolo };

                for (col = 1; col < colCount; ++col)
                {
                    object cellValue = worksheet.Cells[row, col].Value;

                    switch (col)
                    {
                        case 1:
                            item.Date = (DateTime)cellValue;
                            break;
                        case 2:
                            item.Recipient = cellValue.ToString();
                            break;
                        case 3:
                            item.Description = cellValue.ToString();
                            break;
                        case 4:
                            break;
                        case 5:
                            break;
                        case 6:
                            item.Category = cellValue.ToString();
                            break;
                        case 7:
                            item.Currency = cellValue.ToString();
                            break;
                        case 8:
                            decimal value = Convert.ToDecimal(cellValue);
                            RawMovementSign sign = value < 0 ? RawMovementSign.Outcome : RawMovementSign.Income;
                            item.Amount = Math.Abs(value);
                            break;
                        default:
                            throw new NotSupportedException($"Unknown column with index {col}");
                    }
                }

                results.Add(item);
            }
            while (!string.IsNullOrWhiteSpace(worksheet.Cells[++row, col].Value?.ToString()));

            return ValueTask.FromResult(results.ToArray());
        }
    }
}
