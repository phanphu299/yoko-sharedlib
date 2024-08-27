using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using AHI.Infrastructure.Import.Abstraction;

namespace AHI.Infrastructure.Import.Handler
{
    public abstract class JsonFileHandler<T> : IFileHandler<T>
    {
        public virtual IEnumerable<T> Handle(Stream stream)
        {
            stream.Position = 0;
            IList<T> result = new List<T>();
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                foreach (var data in Parse(jsonTextReader))
                    result.Add(data);
            }
            return result;
        }

        protected abstract IEnumerable<T> Parse(JsonTextReader reader);
    }
}