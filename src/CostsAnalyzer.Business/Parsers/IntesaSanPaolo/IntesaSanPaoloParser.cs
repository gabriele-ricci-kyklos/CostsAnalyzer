using OfficeOpenXml;

namespace CostsAnalyzer.Business.Parsers.IntesaSanPaolo
{
    public class IntesaSanPaoloParser : ISourceParser
    {
        public ParserType ParserType => ParserType.IntesaSanPaolo;

        public ValueTask<RawMovementRow[]> ParseFileAsync(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage excelPackage = new(@"C:\Users\gabriele.ricci\Downloads\lista_completa_23092023.xlsx");
            if (excelPackage.Workbook.Worksheets.Count < 1)
            {
                //error
            }

            using ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.First();
            int row = 20;
            int col = 1;
            int colCount = 8;
            List<RawMovementRow> results = new();

            do
            {
                RawMovementRow item = new();

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
                            //error
                            break;
                    }
                }

                results.Add(item);
            }
            while (!string.IsNullOrWhiteSpace(worksheet.Cells[++row, col].Value?.ToString()));

            return ValueTask.FromResult(results.ToArray());
        }
    }
}
