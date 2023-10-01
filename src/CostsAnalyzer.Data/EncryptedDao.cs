using System.Text.Json;

namespace CostsAnalyzer.Data
{
    public record EncryptedDaoOptions(string FolderPath)
    {
        public EncryptedDaoOptions() : this(string.Empty) { }
    }

    public class EncryptedDao
    {
        private readonly EncryptedDaoOptions _options;

        public EncryptedDao(EncryptedDaoOptions options)
        {
            _options = options;
        }

        public async Task SaveDataAsync<T>(T[] items, string? entityName = null)
            where T : IEncryptedEntity
        {
            if (!items?.Any() ?? false)
            {
                return;
            }

            Type t = typeof(T);
            string json = JsonSerializer.Serialize(items);
            string ecryptedType = CAEncryption.Encode(t.AssemblyQualifiedName ?? string.Empty, 50);
            string ecryptedContent = CAEncryption.Encode(json, 50);
            string fileContent = $"{ecryptedType}{Environment.NewLine}{ecryptedContent}";
            string fileName = $"{entityName ?? t.Name}.txt";
            string filePath = Path.Combine(_options.FolderPath, fileName);
            await File
                .WriteAllTextAsync(filePath, fileContent)
                .ConfigureAwait(false);
        }

        public async Task<T[]> ReadDataAsync<T>(string? entityName = null)
            where T : IEncryptedEntity
        {
            Type t = typeof(T);
            string fileName = $"{entityName ?? t.Name}.txt";
            string filePath = Path.Combine(_options.FolderPath, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"No files found for the current type {t.Name}");
            }

            string fileContent =
                await File
                    .ReadAllTextAsync(filePath)
                    .ConfigureAwait(false);

            string[] contentTokens = fileContent.Split(Environment.NewLine);
            if (contentTokens.Length != 2)
            {
                throw new NotSupportedException($"Unable to deserialize the file for the current type {t.Name}");
            }

            string encryptedType = contentTokens[0];
            string encryptedContent = contentTokens[1];

            string typeName = CAEncryption.Decode(encryptedType);
            Type? type = Type.GetType(typeName);
            if (type != t)
            {
                throw new NotSupportedException($"The file found for the current type {t.Name} has a different serialized type");
            }

            string jsonContent = CAEncryption.Decode(encryptedContent);
            T[]? items = JsonSerializer.Deserialize<T[]>(jsonContent);

            return items is null
                ? throw new NotSupportedException($"Unable to deserialize the content of the file for the current type {t.Name}")
                : items;
        }
    }
}
