using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Naanayam.Tools
{
    public class Serializer<T>
    {
        public static Serializer<T> Current { get { return new Serializer<T>(); } }

        private Serializer() { }

        public string Serialize(T o)
        {
            string result = string.Empty;

            if (o is Exception)
            {
                BinaryFormatter bin = new BinaryFormatter();

                using (MemoryStream stream = new MemoryStream())
                {
                    bin.Serialize(stream, o);

                    result = Convert.ToBase64String(stream.ToArray());
                }
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (MemoryStream stream = new MemoryStream())
                {
                    if (stream.CanWrite)
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            serializer.Serialize(writer.BaseStream, o);

                            if (stream.CanSeek)
                                stream.Position = 0;

                            if (stream.CanRead)
                            {
                                using (StreamReader reader = new StreamReader(stream))
                                    result = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return result;
        }

        public string SerializeToJson(T o)
        {
            string result = string.Empty;

            result = JsonConvert.SerializeObject(o);

            return result;
        }

        public async Task<string> SerializeToJsonAsync(T o)
        {
            string result = string.Empty;

            result = await JsonConvert.SerializeObjectAsync(o);

            return result;
        }

        public void SerializeToFile(T o, FileInfo file)
        {
            SerializeToFile(o, file.FullName);
        }

        public void SerializeToFile(T o, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.AutoFlush = true;
                writer.Write(Serialize(o));
                writer.Flush();
            }
        }

        public T Deserialize(string s, Encoding e)
        {
            T result = default(T);

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (MemoryStream stream = new MemoryStream(e.GetBytes(s)))
                result = (T)serializer.Deserialize(stream);

            return result;
        }

        public T DeserializeFromJson(string s)
        {
            T result = default(T);

            result = JsonConvert.DeserializeObject<T>(s);

            return result;
        }

        public async Task<T> DeserializeFromJsonAsync(string s)
        {
            T result = default(T);

            result = await JsonConvert.DeserializeObjectAsync<T>(s);

            return result;
        }

        public T Deserialize(string s)
        {
            T result = default(T);

            if (typeof(T) == typeof(Exception))
            {
                BinaryFormatter bin = new BinaryFormatter();

                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(s)))
                    result = (T)bin.Deserialize(stream);
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(s)))
                    result = (T)serializer.Deserialize(stream);
            }

            return result;
        }

        public T Deserialize(byte[] data)
        {
            T result = default(T);

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (MemoryStream stream = new MemoryStream(data))
                result = (T)serializer.Deserialize(stream);

            return result;
        }

        public T DeserializeFromFile(FileInfo file)
        {
            return DeserializeFromFile(file.FullName);
        }

        public T DeserializeFromFile(string filename)
        {
            T result = default(T);

            if (System.IO.File.Exists(filename))
            {
                string data = string.Empty;

                using (StreamReader reader = new StreamReader(filename))
                    data = reader.ReadToEnd();

                if (!string.IsNullOrEmpty(data))
                    result = (T)Deserialize(data);
            }

            return result;
        }
    }
}