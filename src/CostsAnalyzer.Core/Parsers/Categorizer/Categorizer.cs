namespace CostsAnalyzer.Core.Parsers.Categorizer
{
    public record CategorizerOptions(string DefaultCategory, string FilePath);
    internal record CategoryItem(string Token, string Category);

    public interface ICategorizer
    {
        Task<string> TryGetCategoryAsync(string description);
    }

    public class Categorizer : ICategorizer
    {
        private readonly CategorizerOptions _options;

        public Categorizer(CategorizerOptions options)
        {
            _options = options;
        }

        public async Task<string> TryGetCategoryAsync(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return _options.DefaultCategory;
            }

            CategoryItem[] categoryItems = await GetCategoryItemsAsync(_options.FilePath).ConfigureAwait(false);

            foreach (CategoryItem categoryItem in categoryItems)
            {
                if (description.Contains(categoryItem.Token, StringComparison.OrdinalIgnoreCase))
                {
                    return categoryItem.Category;
                }
            }

            return _options.DefaultCategory;
        }

        private static async Task<CategoryItem[]> GetCategoryItemsAsync(string file)
        {
            string[] lines = await File.ReadAllLinesAsync(file).ConfigureAwait(false);

            return
                lines
                .Select(x => x.Split(',', ';'))
                .Where(x => x.Length == 2)
                .Select(x => new CategoryItem(x[0], x[1]))
                .ToArray();
        }
    }
}
