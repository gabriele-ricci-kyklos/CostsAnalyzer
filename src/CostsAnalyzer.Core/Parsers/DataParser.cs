using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostsAnalyzer.Core.Parsers
{
    public class DataParser
    {
        private readonly IEnumerable<ISourceParser> _sourceParsers;

        public DataParser(IEnumerable<ISourceParser> sourceParsers)
        {
            _sourceParsers = sourceParsers;
        }

        public async Task ParseAsync(string[] filePaths)
        {

        }
    }
}
