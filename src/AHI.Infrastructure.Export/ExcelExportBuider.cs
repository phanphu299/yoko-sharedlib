using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json;

namespace AHI.Infrastructure.Export.Builder
{
    public class ExcelExportBuider
    {
        private IWorkbook _workbook;
        private ISheet _sheet;
        private ICollection<string> _properties;

        public ExcelExportBuider()
        {
            _workbook = new XSSFWorkbook();
            _properties = new List<string>();
        }

        /// <summary>
        /// Set a excel template for the builder so we can fill data latter
        /// </summary>
        /// <param name="templateName">Full path to the template</param>
        public void SetTemplate(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
            {
                throw new ArgumentException($"{nameof(templateName)} is required");
            }

            DataTable dtTable = new DataTable();
            List<string> rowList = new List<string>();
            using (var stream = new FileStream(templateName, FileMode.Open))
            {
                stream.Position = 0;
                _workbook = new XSSFWorkbook(stream);

                ISheet sheet = _workbook.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(0);
                IRow propertyRow = sheet.GetRow(1);

                int headerCount = headerRow.LastCellNum;
                for (int i = 0; i < headerCount; i++)
                {
                    ICell cell = propertyRow.GetCell(i);
                    if (cell != null)
                    {
                        var property = cell.ToString().Replace("<", "").Replace(">", "");
                        _properties.Add(property);
                    }
                }
            }
        }

        /// <summary>
        /// Set rows of data within the given sheet (row data will be mapped with properties <property_name>)
        /// </summary>
        /// <param name="sheetName">Sheetname</param>
        /// <param name="data">Rows of data</param>
        /// <typeparam name="T"></typeparam>
        public void SetData<T>(string sheetName, List<T> data)
        {
            DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data), (typeof(DataTable)));
            ISheet templateSheet = _workbook.GetSheetAt(0);
            _sheet = templateSheet.CopySheet(sheetName, copyStyle: true);
            IList<string> columns = new List<string>();

            // Fill rows with data
            IRow row = _sheet.CreateRow(1);
            int rowIndex = 1;
            foreach (DataRow dsRow in table.Rows)
            {
                row = _sheet.CreateRow(rowIndex);
                int cellIndex = 0;
                foreach (string property in _properties)
                {
                    string value = dsRow.Table.Columns.Contains(property) ? dsRow[property].ToString() : string.Empty;
                    row.CreateCell(cellIndex).SetCellValue(value);
                    cellIndex++;
                }
                rowIndex++;
            }
        }

        /// <summary>
        /// Set rows of data vertically base on key/value pairs
        /// </summary>
        /// <param name="dic">Key/value pairs</param>
        public void SetVerticalData(IDictionary<string, string> dic)
        {
            int rowIndex = 0;
            IRow row = _sheet.CreateRow(rowIndex);
            foreach (var key in dic.Keys)
            {
                row.CreateCell(0).SetCellValue(key);
                row.CreateCell(1).SetCellValue(dic[key]);
            }
        }

        /// <summary>
        /// Shilf whole block of data move down from top (hearders & rows of data)
        /// </summary>
        /// <param name="rows">Block rows to shift from top to bottom</param>
        /// <param name="shift">Number of rows to move the block</param>
        public void ShiftRowsFromTop(int rows, int shift)
        {
            _sheet.ShiftRows(0, rows, shift);
        }

        /// <summary>
        /// Build stream of the current workbook
        /// </summary>
        /// <returns></returns>
        public byte[] BuildExcelStream()
        {
            using (var ms = new MemoryStream())
            {
                // Remove sheet template
                _workbook.RemoveSheetAt(0);
                _workbook.Write(ms);
                return ms.ToArray();
            }
        }
    }
}