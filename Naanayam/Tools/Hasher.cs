using Naanayam.Enum;
using System.Security.Cryptography;
using System.Text;

namespace Naanayam.Tools
{
    public class Hasher
    {
        public static Hasher Default { get { return new Hasher(); } }

        private Hasher() { }

        public string Execute(byte[] data, HashType type)
        {
            string result = string.Empty;

            byte[] hash = null;

            switch (type)
            {
                case HashType.MD5:
                    hash = new MD5CryptoServiceProvider().ComputeHash(data);
                    break;

                case HashType.SHA1:
                    hash = new SHA1Managed().ComputeHash(data);
                    break;

                case HashType.SHA256:
                    hash = new SHA256Managed().ComputeHash(data);
                    break;

                case HashType.SHA384:
                    hash = new SHA384Managed().ComputeHash(data);
                    break;

                case HashType.SHA512:
                    hash = new SHA512Managed().ComputeHash(data);
                    break;
            }

            if (hash != null)
                result = ByteArrayToString(hash);

            return result;
        }

        public string Execute(string data, HashType type)
        {
            return Execute(Encoding.ASCII.GetBytes(data), type);
        }

        private string ByteArrayToString(byte[] data)
        {
            int i;

            StringBuilder result = new StringBuilder(data.Length);

            for (i = 0; i < data.Length; i++)
                result.Append(data[i].ToString("X2"));

            return result.ToString();
        }
    }
}