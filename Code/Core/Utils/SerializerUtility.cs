using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Utils
{
    public class SerializerUtility
    {
        private static SerializerUtility _utility;
        public static SerializerUtility Instance()
        {
            if (_utility == null)
                _utility = new SerializerUtility();

            return _utility;
        }

        public T BinDeserialize<T>(byte[] b)
        {
            if (b == null)
            {
                throw new ArgumentNullException("b");
            }
            T local = default(T);
            using (MemoryStream stream = new MemoryStream(b))
            {
                stream.Position = 0L;
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
        }

        public T BinDeserializeFromFile<T>(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            return BinDeserialize<T>(File.ReadAllBytes(path));
        }

        public byte[] BinSerialize(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, obj);
                stream.Position = 0L;
                return stream.ToArray();
            }
        }

        public void BinSerializeToFile(object o, string path)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            byte[] bytes = BinSerialize(o);
            File.WriteAllBytes(path, bytes);
        }
    }
}
