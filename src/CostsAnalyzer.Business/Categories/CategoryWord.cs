#nullable disable

using CostsAnalyzer.Data;

namespace CostsAnalyzer.Business.Categories
{
    public class CategoryWord : IEncryptedEntity
    {
        public string Word { get; set; }
        public Category Category { get; set; }
    }
}
