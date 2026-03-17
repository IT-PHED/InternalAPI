using ClosedXML.Excel;
using System.IO;
using System.Reflection;

namespace INTERNAL.Utilities
{
    public static class ExcelGenerator
    {
        public static byte[] Generate<T>(List<T> records)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Report");
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Generate Headers
            for (int i = 0; i < properties.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = properties[i].Name;
            }

            // Generate Rows
            for (int i = 0; i < records.Count; i++)
            {
                for (int j = 0; j < properties.Length; j++)
                {
                    var value = properties[j].GetValue(records[i]);
                    sheet.Cell(i + 2, j + 1).Value = value?.ToString() ?? string.Empty;
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
