using System.IO;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using AHI.Infrastructure.Import.Abstraction;

namespace AHI.Infrastructure.Import.Handler
{
    public abstract class ExcelFileHandler<T> : IFileHandler<T>
    {
        public virtual IEnumerable<T> Handle(Stream stream)
        {
            stream.Position = 0;
            var workbook = new XSSFWorkbook(stream);
            IList<T> result = new List<T>();
            for (int sheetNumber = 0; sheetNumber < workbook.Count; sheetNumber++)
            {
                var sheet = workbook[sheetNumber];
                foreach (var data in Parse(sheet)) result.Add(data);
            }
            return result;
        }

        protected abstract IEnumerable<T> Parse(ISheet reader);
    }
}