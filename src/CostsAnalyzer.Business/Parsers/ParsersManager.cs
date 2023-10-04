namespace CostsAnalyzer.Business.Parsers
{
    public class ParsersManager
    {
        private readonly IEnumerable<ISourceParser> _sourceParsers;

        public ParsersManager(IEnumerable<ISourceParser> sourceParsers)
        {
            _sourceParsers = sourceParsers;
        }

        public async Task<RawMovement[]> ParseAsync(string[] filePaths)
        {
            var groups =
                filePaths
                    .Select(x => (FilePath: x, ParserType: GetParserType(x)))
                    .GroupBy(x => x.ParserType, x => x.FilePath);

            List<RawMovement> rawMovements = new();

            foreach (var group in groups)
            {
                ISourceParser parser =
                    _sourceParsers.FirstOrDefault(x => x.ParserType == group.Key)
                    ?? throw new NotSupportedException($"No parsers configured for the type {group.Key}");

                foreach(string filePath in group)
                {
                    RawMovement[] movements =
                        await parser
                            .ParseFileAsync(filePath)
                            .ConfigureAwait(false);

                    rawMovements.AddRange(movements);
                }
            }

            return rawMovements.ToArray();
        }

        private static ParserType GetParserType(string filePath) =>
            Path.GetExtension(filePath).Remove(0, 1) switch
            {
                "xlsx" => ParserType.IntesaSanPaolo,
                "pdf" => ParserType.Hype,
                "csv" => ParserType .N26,
                _ => throw new NotSupportedException($"Unable to infer the parser type for the file {filePath}")
            };
    }
}
