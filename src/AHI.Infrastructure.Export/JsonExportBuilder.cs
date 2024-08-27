using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using JsonConstant = AHI.Infrastructure.SharedKernel.Extension.Constant;

namespace AHI.Infrastructure.Export.Builder
{
    public class JsonExportBuilder<T>
    {
        private Func<T, string> _nameBuilder;

        public JsonExportBuilder()
        {
            _nameBuilder = x => $"{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.json";
        }

        public void SetZipEntryNameBuilder(Func<T, string> nameBuilder)
        {
            _nameBuilder = nameBuilder;
        }

        public byte[] BuildContent(IEnumerable<T> data)
        {
            if (data is null || !data.Any())
                return Array.Empty<byte>();

            if (data.Count() == 1)
                return CreateJsonFileContent(data.First());
            else
                return CreateZipFileContent(data);
        }

        private byte[] CreateJsonFileContent(T data)
        {
            using (var contentStream = new MemoryStream())
            {
                SerializeData(data, contentStream);
                return contentStream.ToArray();
            }
        }

        private byte[] CreateZipFileContent(IEnumerable<T> data)
        {
            using (var contentStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(contentStream, ZipArchiveMode.Create, false))
                {
                    foreach (var value in data)
                    {
                        var entryName = _nameBuilder.Invoke(value);
                        var newEntry = zipArchive.CreateEntry(entryName);

                        using (var entryStream = newEntry.Open())
                            SerializeData(value, entryStream);
                    }
                }
                return contentStream.ToArray();
            }
        }

        private void SerializeData(object value, Stream s)
        {
            using (StreamWriter writer = new StreamWriter(s))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;
                JsonConstant.JsonSerializer.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }
    }
}