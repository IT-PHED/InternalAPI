using System.Text;
using System.Reflection;
using System.Linq;

namespace INTERNAL.Utilities
{
    public static class CsvGenerator
    {
        public static byte[] Generate<T>(List<T> records)
        {
            var sb = new StringBuilder();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Header
            sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            foreach (var r in records)
            {
                var values = properties.Select(p => p.GetValue(r)?.ToString() ?? "");
                sb.AppendLine(string.Join(",", values));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
