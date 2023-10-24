using CostsAnalyzer.Business.Categories;
using CostsAnalyzer.Business.Movements;
using CostsAnalyzer.Business.Parsers;
using CostsAnalyzer.Business.Parsers.Hype;
using CostsAnalyzer.Business.Parsers.IntesaSanPaolo;
using CostsAnalyzer.Business.Parsers.Isybank;
using CostsAnalyzer.Business.Parsers.N26;
using CostsAnalyzer.Business.Parsers.Revolut;
using CostsAnalyzer.Data;

EncryptedDao dao = new(new(@"C:\Temp\CostsAnalyzer\data"));
CategoryManager categoryManager = new(dao, new(30));
ISourceParser n26Parser = new N26Parser();
ISourceParser ispParser = new IntesaSanPaoloParser();
ISourceParser hypeParser = new HypeParser();
ISourceParser revolutParser = new RevolutParser();
ISourceParser isybankParser = new IsybankParser();
ParsersManager parsersManager = new(new[] { n26Parser, ispParser, hypeParser, revolutParser });
MovementsManager movementsManager = new(dao, parsersManager, categoryManager, new(new[] { "hype", "n26", "intesa", "revolut", "isybank", "arrotondamento" }));

var result = await movementsManager.ImportMovementsAsync(Directory.GetFiles(@"C:\Temp\CostsAnalyzer\movements_data"));

Console.ReadLine();

//StopWord[] stopWords =
//    File
//        .ReadAllLines(@"C:\Users\gabriele.ricci\Downloads\italian.txt")
//        .Select(x => new StopWord() { Word = x })
//        .ToArray();

//await dao.SaveDataAsync(stopWords);

//List<CategoryWord> data = new();
//foreach(string file in Directory.GetFiles(@"C:\Temp\CostsAnalyzer\categories_data"))
//{
//    Category category = Enum.Parse<Category>(Path.GetFileNameWithoutExtension(file));
//    string[] lines = File.ReadAllLines(file);
//    foreach(string line in lines)
//    {
//        string[] words = line.Split(' ');
//        data.AddRange(words.Select(x => new CategoryWord() { Word = x, Category = category }));
//    }
//}

//await dao.SaveDataAsync(data.ToArray());

//Console.ReadLine();


//using OfficeOpenXml;

//ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//using ExcelPackage excelPackage = new(@"C:\Users\gabriele.ricci\Downloads\lista_completa_24092023.xlsx");
//if (excelPackage.Workbook.Worksheets.Count < 1)
//{
//    //error
//}

//using ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.First();
//int row = 20;
//int col = 1;
//int colCount = 8;
//Dictionary<string, List<string>> results = new();

//do
//{
//    string name = worksheet.Cells[row, 2].Value.ToString() ?? string.Empty;
//    string category = worksheet.Cells[row, 6].Value.ToString() ?? string.Empty;

//    if (results.ContainsKey(category))
//    {
//        results[category].Add(name);
//    }
//    else
//    {
//        results.Add(category, new() { name });
//    }
//}
//while (!string.IsNullOrWhiteSpace(worksheet.Cells[++row, col].Value?.ToString()));

//foreach(string key in results.Keys)
//{
//    var data = results[key].Distinct().ToList();
//    File.WriteAllLines(@$"C:\temp\costsanalyzer\{key}.txt", data);
//}

//bool x = true;