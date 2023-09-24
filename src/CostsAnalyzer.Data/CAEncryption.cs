using System.Text;

namespace CostsAnalyzer.Data
{
    public class CAEncryption
    {
        private readonly Random _random;
        private readonly char[] _charsForZero;
        private readonly char[] _charsForOne;
        private readonly char[] _charsInTheMiddle;

        public string String { get; set; }
        public int InMiddleCharsStrength { get; set; }

        private CAEncryption(string str, int inMiddleCharsStrength)
        {
            _random = new Random();
            _charsForZero = "/[]{~)@#_,;:".ToCharArray();
            _charsForOne = "+-%?}<>(!".ToCharArray();
            _charsInTheMiddle = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789&|*^=".ToCharArray();

            String = str;
            InMiddleCharsStrength = inMiddleCharsStrength;
        }

        public static string Encode(string str, int inMiddleCharsStrength = 0)
        {
            CAEncryption obj = new(str, inMiddleCharsStrength);
            string encodedStr = obj.Encode();
            return encodedStr;
        }

        public static string Decode(string str)
        {
            CAEncryption obj = new(str, 0);
            string decodedStr = obj.Decode();
            return decodedStr;
        }

        private string Encode()
        {
            string workedString =
                BinaryString
                    .FromString(String, Encoding.UTF8)
                    .InvertBinaries()
                    .ReverseBinaries()
                    .ToString();

            StringBuilder buffer = new(workedString.Length * 2);
            int index;

            foreach (char c in workedString)
            {
                if (c == '0')
                {
                    index = _random.Next(_charsForZero.Length);
                    buffer.Append(_charsForZero[index]);
                }
                else
                {
                    index = _random.Next(_charsForOne.Length);
                    buffer.Append(_charsForOne[index]);
                }

                if (_random.Next(100) > 100 - InMiddleCharsStrength)
                {
                    index = _random.Next(_charsInTheMiddle.Length);
                    buffer.Append(_charsInTheMiddle[index]);
                }
            }

            return buffer.ToString();
        }

        private string Decode()
        {
            HashSet<char> charsInTheMiddle = _charsInTheMiddle.ToHashSet();
            HashSet<char> charsForZero = _charsForZero.ToHashSet();
            HashSet<char> charsForOne = _charsForOne.ToHashSet();

            StringBuilder buffer = new(String.Length);

            foreach (char c in String.Where(x => !charsInTheMiddle.Contains(x)))
            {
                if (charsForZero.Contains(c))
                {
                    buffer.Append('0');
                }
                else if (charsForOne.Contains(c))
                {
                    buffer.Append('1');
                }
                else
                {
                    throw new ArgumentException($"Invalid character '{c}'");
                }
            }

            return
                BinaryString
                    .FromBinaryString(buffer.ToString())
                    .ReverseBinaries()
                    .InvertBinaries()
                    .OriginalString;
        }
    }
}