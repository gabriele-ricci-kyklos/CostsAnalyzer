using CostsAnalyzer.Data;
using System.Text.RegularExpressions;

namespace CostsAnalyzer.Business.Categories
{
    public record CategoryMatch(Category PredictedCategory, int Score);
    public record CategoryManagerOptions(int MatchingPerc)
    {
        public CategoryManagerOptions() : this(100) { }
    }

    public partial class CategoryManager
    {
        private readonly EncryptedDao _dao;
        private readonly CategoryManagerOptions _options;

        public CategoryManager(EncryptedDao dao, CategoryManagerOptions options)
        {
            _dao = dao;
            _options = options;
        }

        public async Task<CategoryMatch[]> MatchCategoryAsync(string recipient) =>
            (await MatchCategoryAsync(new[] { recipient }).ConfigureAwait(false)).FirstOrDefault().Value;

        public async Task<Dictionary<string, CategoryMatch[]>> MatchCategoryAsync(string[] recipientList)
        {
            (Category Category, HashSet<string> Words)[] categoryTable =
                await GetCategoryTableAsync().ConfigureAwait(false);

            HashSet<string> stopWordsSet = await GetStopWordsSetAsync().ConfigureAwait(false);

            Dictionary<string, CategoryMatch[]> results = new();

            foreach (string recipient in recipientList)
            {
                string[] recipientWords = GetSanitizedWords(recipient, stopWordsSet);

                Dictionary<Category, int> scoresDict = new();
                foreach (string word in recipientWords)
                {
                    foreach ((Category category, HashSet<string> words) in categoryTable)
                    {
                        if (!words.Contains(word))
                        {
                            continue;
                        }

                        if (scoresDict.ContainsKey(category))
                        {
                            scoresDict[category]++;
                        }
                        else
                        {
                            scoresDict.Add(category, 1);
                        }
                    }
                }

                CategoryMatch[] matches =
                    scoresDict
                        .Select(x => new CategoryMatch(x.Key, x.Value))
                        .Where(x =>
                        {
                            float perc = x.Score / (float)recipientWords.Length;
                            if (perc >= _options.MatchingPerc)
                            {
                                return true;
                            }

                            return false;
                        })
                        .OrderByDescending(x => x.Score)
                        .ToArray();

                if (!matches.Any())
                {
                    matches = new[] { new CategoryMatch(Category.Sconosciuta, 99) };
                }

                results.Add(recipient, matches);
            }

            return results;
        }

        private async Task<HashSet<string>> GetStopWordsSetAsync()
        {
            StopWord[] stopWords =
                await _dao
                    .ReadDataAsync<StopWord>()
                    .ConfigureAwait(false);

            HashSet<string> stopWordsSet =
                stopWords
                    .Select(x => x.Word)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return stopWordsSet;
        }

        private async Task<(Category Category, HashSet<string> Words)[]> GetCategoryTableAsync()
        {
            CategoryWord[] items =
                await _dao
                    .ReadDataAsync<CategoryWord>()
                    .ConfigureAwait(false);

            return
                items
                    .GroupBy(x => x.Category)
                    .Select(x => (Category: x.Key, Words: x.Select(x => x.Word).ToHashSet(StringComparer.OrdinalIgnoreCase)))
                    .ToArray();
        }

        private static string[] GetSanitizedWords(string str, HashSet<string> stopWordsSet)
        {
            string lineParsed = StripCharsRegex().Replace(str, " ");
            string[] words =
                lineParsed
                    .Split(' ')
                    .Where(x => x.Length > 1 && !stopWordsSet.Contains(x))
                    .ToArray();

            return words;
        }

        [GeneratedRegex("[^0-9a-zA-Z\\s]")]
        private static partial Regex StripCharsRegex();
    }
}
