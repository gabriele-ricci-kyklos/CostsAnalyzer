using System.Text;

namespace CostsAnalyzer.Data
{
    public class BinaryString
    {
        private readonly Encoding _encoding;
        private readonly string _binaryString;

        public byte[] Bytes { get; set; }
        public string OriginalString => _encoding.GetString(Bytes);

        public static BinaryString FromString(string str, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            byte[] bytes = encoding.GetBytes(str);
            return new(bytes, encoding);
        }

        public static BinaryString FromByteArray(byte[] byteArray, Encoding? encoding = null) =>
            new(byteArray, encoding);

        public static BinaryString FromBinaryString(string binaryString, Encoding? encoding = null)
        {
            if (binaryString.Any(x => x != '0' && x != '1'))
            {
                throw new ArgumentException($"The provided string '{binaryString}' is not a binary string");
            }

            byte[] byteArray = GetByteArrayFromBinaryString(binaryString);

            return new BinaryString(byteArray, encoding);
        }

        private BinaryString(byte[] byteArray, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            Bytes = byteArray;

            _encoding = encoding;
            _binaryString =
                string
                .Join
                (
                    string.Empty,
                    byteArray
                        .Select(x => Convert.ToString(x, 2).PadLeft(8, '0'))
                );
        }

        public BinaryString InvertBinaries()
        {
            StringBuilder buffer = new();

            foreach (char c in _binaryString)
            {
                buffer.Append((c == '1') ? '0' : '1');
            }

            string invertedBinaryStr = buffer.ToString();
            byte[] bytes = GetByteArrayFromBinaryString(invertedBinaryStr);

            return new BinaryString(bytes);
        }

        public BinaryString ReverseBinaries()
        {
            string[] strArray =
                _binaryString
                    .ToCharArray()
                    .Select(x => x.ToString())
                    .ToArray();

            Array.Reverse(strArray);
            byte[] bytes = GetByteArrayFromBinaryString(string.Join(string.Empty, strArray));

            return new BinaryString(bytes);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not BinaryString binStr)
            {
                return false;
            }

            return OriginalString.Equals(binStr.OriginalString);
        }

        public override int GetHashCode() => OriginalString.GetHashCode();

        public override string ToString() => _binaryString;

        private static byte[] GetByteArrayFromBinaryString(string binaryStr)
        {
            int numOfBytes = binaryStr.Length / 8;
            byte[] bytes = new byte[numOfBytes];

            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(binaryStr.Substring(8 * i, 8), 2);
            }

            return bytes;
        }
    }
}
