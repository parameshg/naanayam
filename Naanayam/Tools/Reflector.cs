using System;
using System.Reflection;

namespace Naanayam.Tools
{
    public class Reflector<T>
    {
        public static Reflector<T> Default { get { return new Reflector<T>(); } }

        private Reflector() { }

        public T CreateFromExecutingAssembly(string typeName)
        {
            T result = default(T);

            Assembly a = Assembly.GetExecutingAssembly();

            if (a != null)
            {
                Type t = a.GetType(typeName);

                if (t != null)
                    result = (T)Activator.CreateInstance(t);
            }

            return result;
        }

        public T Create(string fullyQualifiedTypeName)
        {
            T result = default(T);

            string assembly = string.Empty;
            string type = string.Empty;
            string version = string.Empty;
            string culture = string.Empty;
            string publickey = string.Empty;

            string[] args = fullyQualifiedTypeName.Split(',');

            if (args.Length >= 2)
            {
                assembly = args[1].Trim();
                type = args[0].Trim();

                if (args.Length.Equals(5))
                {
                    version = args[2].Trim();
                    culture = args[3].Trim();
                    publickey = args[4].Trim();
                }
            }

            if (!string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(type))
            {
                Assembly a = null;

                if (args.Length.Equals(5))
                    a = Assembly.Load(string.Format("{0}, {1}, {2}, {3}", assembly, version, culture, publickey));

                if (args.Length.Equals(2))
                    a = Assembly.Load(assembly);

                if (a != null)
                {
                    Type t = a.GetType(type);

                    if (t != null)
                        result = (T)Activator.CreateInstance(t);
                }
            }

            return result;
        }
    }
}