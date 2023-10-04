using CostsAnalyzer.Business.Categories;
using CostsAnalyzer.Business.Parsers;
using CostsAnalyzer.Data;

namespace CostsAnalyzer.Business.Movements
{
    public record MovementsManagerOptions(string[] TransferRecipients)
    {
        public MovementsManagerOptions() : this(Array.Empty<string>()) { }
    }

    public class MovementsManager
    {
        private readonly EncryptedDao _dao;
        private readonly ParsersManager _parserManager;
        private readonly CategoryManager _categoryManager;
        private readonly MovementsManagerOptions _options;

        public MovementsManager(EncryptedDao dao, ParsersManager parserManager, CategoryManager categoryManager, MovementsManagerOptions options)
        {
            _dao = dao;
            _parserManager = parserManager;
            _categoryManager = categoryManager;
            _options = options;
        }

        public async Task<Movement[]> ImportMovementsAsync(string[] filePathList)
        {
            RawMovement[] rawMovementList =
                await _parserManager
                    .ParseAsync(filePathList)
                    .ConfigureAwait(false);

            string[] recipientList =
                rawMovementList
                    .Select(x => x.Recipient)
                    .Distinct()
                    .ToArray();

            Dictionary<string, CategoryMatch[]> categoryMatchesDict =
                await _categoryManager
                    .MatchCategoryAsync(recipientList)
                    .ConfigureAwait(false);

            List<Movement> movements = new();
            foreach (RawMovement mov in rawMovementList)
            {
                CategoryMatch[] categoryMatches =
                    categoryMatchesDict
                        .GetValueOrDefault(mov.Recipient)
                    ?? throw new ArgumentNullException(nameof(categoryMatches));

                bool isTransfer =
                    _options
                        .TransferRecipients
                        .Any(x => mov.Recipient.Contains(x, StringComparison.OrdinalIgnoreCase));

                Movement movement =
                    new()
                    {
                        Amount = mov.Amount,
                        Category = (isTransfer ? Category.Giroconto : categoryMatches.First().Category).ToString(),
                        Currency = mov.Currency,
                        Date = mov.Date,
                        Description = mov.Description,
                        Recipient = mov.Recipient,
                        Sign = isTransfer ? MovementSign.Transfer : mov.Sign == RawMovementSign.Income ? MovementSign.Income : MovementSign.Outcome
                    };

                movements.Add(movement);
            }

            await SaveMovementsAsync(movements).ConfigureAwait(false);

            return movements.ToArray();
        }

        private async Task SaveMovementsAsync(List<Movement> movements)
        {
            Movement[] existingMovements =
                await _dao
                    .ReadDataAsync<Movement>()
                    .ConfigureAwait(false);

            var allMovements =
                existingMovements
                    .Union(movements)
                    .ToArray();

            await _dao
                .SaveDataAsync(allMovements)
                .ConfigureAwait(false);
        }
    }
}
