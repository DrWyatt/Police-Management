using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace policeManagementServer
{
    public sealed class Database
    {
        private readonly string file;
        public Database(string file, bool createFile = true)
        {
            this.file = file;
            if (!createFile) return;
            lock (this)
                File.Open(file, (FileMode)4).Dispose();
        }

        public dynamic Read()
        {
            lock (this)
            {
                using (var compressed = new FileStream(file, FileMode.Open))
                {
                    if (!compressed.CanWrite || !compressed.CanRead)
                        throw new IOException("Invalid permissions to read or write");
                    if(compressed.Length == 0)
                        return null;

                    using (var uncompressed = new MemoryStream())
                    {
                        using (var gzip = new GZipStream(compressed, CompressionMode.Decompress, true))
                            gzip.CopyTo(uncompressed);
                        uncompressed.Position = 0;
                        return new BinaryFormatter().Deserialize(uncompressed);
                    }
                }
            }
        }

        public void Write(dynamic data)
        {
            lock (this)
            {
                if (!((object)data).GetType().IsSerializable)
                    throw new SerializationException("The object trying to be serialized is not marked serializable",
                        new NullReferenceException());
                using (var fileStream = new FileStream(file, FileMode.Open))
                {
                    if (!fileStream.CanWrite || !fileStream.CanRead)
                        throw new IOException("Invalid permissions to read or write");
                    byte[] bytes;
                    using (var ms = new MemoryStream())
                    using (var gzip = new GZipStream(ms, CompressionMode.Compress))
                    {
                        new BinaryFormatter().Serialize(gzip, data);
                        gzip.Close();
                        ms.Close();
                        bytes = ms.ToArray();
                    }
                    fileStream.Write(bytes, 0, bytes.Length);
                }
            }
        }
    }
}