#nullable disable

namespace CostsAnalyzer.Data
{
    public interface IEncryptedEntity
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class EncryptedEntityAttribte : Attribute
    {
        public string EntityName { get; set; }
    }
}
