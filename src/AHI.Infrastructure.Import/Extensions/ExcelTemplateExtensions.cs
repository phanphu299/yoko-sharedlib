using System;
using NPOI.SS.UserModel;
using System.Linq;

namespace AHI.Infrastructure.Import.Extension
{
    public static class ExcelTemplateExtensions
    {
        private static readonly int _doublePrecision = 15;
        private static readonly double[] _precisions = new double[]
       {
            1d, 0.1d, 0.01d, 0.001d, 0.0001d, 0.00001d, 0.000001d, 0.0000001d, 0.00000001d, 0.000000001d,
            0.0000000001d, 0.00000000001d, 0.000000000001d, 0.0000000000001d, 0.00000000000001d, 0.000000000000001d
       };
        public static bool IsEmpty(this IRow row)
        {
            return row is null || row.Cells.Count < 1 || !row.Cells.Any(cell => cell.CellType != CellType.Blank);
        }
        public static object GetFormatedValue(this ICell cell)
        {
            if (cell is null || cell.CellType == CellType.Blank)
                return null;
            switch (cell.CellType)
            {
                case CellType.String:
                    {
                        return cell.StringCellValue.Trim();
                    }
                case CellType.Boolean:
                    {
                        return cell.BooleanCellValue;
                    }
                case CellType.Numeric:
                    {
                        if (DateUtil.IsCellDateFormatted(cell))
                            return cell.DateCellValue;

                        var value = cell.NumericCellValue;
                        if (IsInteger(value, _doublePrecision))
                        {
                            try
                            {
                                return Convert.ToInt32(value);
                            }
                            catch { }
                        }
                        return Math.Round(value, _doublePrecision);
                    }
                default:
                    return null;
            };
        }
        public static bool IsInteger(double n, int k)
        {
            var r = Math.Round(n % 1, k);
            var d = Math.Abs(Math.Round(r) - r);
            var p = _precisions[k];
            return Math.Round(d, k) < p;
        }
        public static string GetExcelColumnName(this int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }
    }
}