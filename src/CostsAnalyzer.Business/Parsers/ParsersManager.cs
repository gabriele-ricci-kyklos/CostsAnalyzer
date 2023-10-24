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
            List<(string FilePath, ParserType ParserType)> filePathData = new();
            foreach (string filePath in filePaths)
            {
                ParserType parserType = await GetParserType(filePath).ConfigureAwait(false);
                filePathData.Add((filePath, parserType));
            }

            var groups =
                filePathData
                    .GroupBy(x => x.ParserType, x => x.FilePath);

            List<RawMovement> rawMovements = new();

            foreach (var group in groups)
            {
                ISourceParser parser =
                    _sourceParsers.FirstOrDefault(x => x.ParserType == group.Key)
                    ?? throw new NotSupportedException($"No parsers configured for the type {group.Key}");

                foreach (string filePath in group)
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

        private async ValueTask<ParserType> GetParserType(string filePath)
        {
            string ext = Path.GetExtension(filePath).Remove(0, 1);

            foreach (ISourceParser sourceParser in _sourceParsers)
            {
                if (!sourceParser.SupportedFileExtensions.Contains(ext))
                {
                    continue;
                }

                bool isFileOfParserType = await sourceParser.IsFileOfParserType(filePath).ConfigureAwait(false);

                if (!isFileOfParserType)
                {
                    continue;
                }

                return sourceParser.ParserType;
            }

            throw new NotSupportedException($"Unable to infer the parser type for the file {filePath}");
        }
    }
}
