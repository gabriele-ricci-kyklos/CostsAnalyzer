using System.Text.Json;

namespace CostsAnalyzer.Data
{
    public interface IEncryptedEntity
    {
    }

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

        public async Task SaveDataAsync<T>(T[] items)
            where T : IEncryptedEntity
        {
            string json = JsonSerializer.Serialize(items);
            string ecryptedType = CAEncryption.Encode(typeof(T).AssemblyQualifiedName ?? string.Empty, 50);
            string ecryptedContent = CAEncryption.Encode(json, 50);
            string fileContent = $"{ecryptedType}{Environment.NewLine}{ecryptedContent}";

            string filePath = Path.Combine(_options.FolderPath, $"{typeof(T).Name}.txt");
            await File
                .WriteAllTextAsync(filePath, fileContent)
                .ConfigureAwait(false);
        }

        public async Task<T[]> ReadDataAsync<T>()
            where T : IEncryptedEntity
        {
            string filePath = Path.Combine(_options.FolderPath, $"{typeof(T).Name}.txt");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"No files found for the current type {typeof(T).Name}");
            }

            string fileContent =
                await File
                    .ReadAllTextAsync(filePath)
                    .ConfigureAwait(false);

            string[] contentTokens = fileContent.Split(Environment.NewLine);
            if (contentTokens.Length != 2)
            {
                throw new NotSupportedException($"Unable to deserialize the file for the current type {typeof(T).Name}");
            }

            string encryptedType = contentTokens[0];
            string encryptedContent = contentTokens[1];

            string typeName = CAEncryption.Decode(encryptedType);
            Type? type = Type.GetType(typeName);
            if (type != typeof(T))
            {
                throw new NotSupportedException($"The file found for the current type {typeof(T).Name} has a different serialized type");
            }

            string jsonContent = CAEncryption.Decode(encryptedContent);
            T[]? items = JsonSerializer.Deserialize<T[]>(jsonContent);

            return items is null
                ? throw new NotSupportedException($"Unable to deserialize the content of the file for the current type {typeof(T).Name}")
                : items;
        }
    }
}
