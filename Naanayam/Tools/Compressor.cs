using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Naanayam.Tools
{
    public class Compressor
    {
        public enum Type
        {
            Deflate,
            GZip
        }

        public enum Mode
        {
            Zip,
            Unzip
        }

        public static Compressor Default { get { return new Compressor(); } }

        private Compressor() { }

        public byte[] Execute(byte[] data, Type type, Mode action)
        {
            byte[] result = null;

            if (type == Type.GZip)
            {
                if (action == Mode.Zip)
                    result = GZipCompression(data);

                if (action == Mode.Unzip)
                    result = GZipDecompression(data);
            }

            if (type == Type.Deflate)
            {
                if (action == Mode.Zip)
                    result = DeflateCompression(data);

                if (action == Mode.Unzip)
                    result = DeflateDecompression(data);
            }

            return result;
        }

        private byte[] GZipCompression(byte[] data)
        {
            byte[] result = null;

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (MemoryStream ouput = new MemoryStream())
                {
                    using (GZipStream compressor = new GZipStream(ouput, CompressionMode.Compress))
                    {
                        stream.CopyTo(compressor);
                        compressor.Close();

                        result = ouput.ToArray();
                    }
                }
            }

            return result;
        }

        private byte[] GZipDecompression(byte[] data)
        {
            byte[] result = null;

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (MemoryStream ouput = new MemoryStream())
                {
                    using (GZipStream compressor = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        compressor.CopyTo(ouput);
                        compressor.Close();

                        result = ouput.ToArray();
                    }
                }
            }

            return result;
        }

        private byte[] DeflateCompression(byte[] data)
        {
            byte[] result = null;

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (MemoryStream ouput = new MemoryStream())
                {
                    using (DeflateStream compressor = new DeflateStream(ouput, CompressionMode.Compress))
                    {
                        stream.CopyTo(compressor);
                        compressor.Close();

                        result = ouput.ToArray();
                    }
                }
            }

            return result;
        }

        private byte[] DeflateDecompression(byte[] data)
        {
            byte[] result = null;

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (MemoryStream ouput = new MemoryStream())
                {
                    using (DeflateStream compressor = new DeflateStream(stream, CompressionMode.Decompress))
                    {
                        compressor.CopyTo(ouput);
                        compressor.Close();

                        result = ouput.ToArray();
                    }
                }
            }

            return result;
        }
    }
}
