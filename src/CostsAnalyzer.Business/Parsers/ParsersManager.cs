using CostsAnalyzer.Business.Categories;

namespace CostsAnalyzer.Business.Parsers
{
    public class ParsersManager
    {
        private readonly IEnumerable<ISourceParser> _sourceParsers;

        public ParsersManager(IEnumerable<ISourceParser> sourceParsers)
        {
            _sourceParsers = sourceParsers;
        }

        public async Task<RawMovement[]> ParseAsync(string[] filePaths, ParserType parserType)
        {
            ISourceParser parser =
                _sourceParsers.FirstOrDefault(x => x.ParserType == parserType)
                ?? throw new NotSupportedException($"No parsers configured for the type {parserType}");

            List<RawMovement> rawMovements = new();

            foreach (string filePath in filePaths)
            {
                RawMovement[] movements =
                    await parser
                        .ParseFileAsync(filePath)
                        .ConfigureAwait(false);

                rawMovements.AddRange(movements);
            }

            return rawMovements.ToArray();
        }
    }
}
