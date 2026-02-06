using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalEquipmentStore
{
   /* public static class StringExtensions
    {
        public static string Repeat(this char c, int count)
        {
            return new string(c, Math.Max(0, count));
        }
        public static string GetDatabaseName(this string connectionString)
        {
            try
            {
                var parameters = connectionString.Split(';');
                foreach (var param in parameters)
                {
                    if (param.Trim().StartsWith("Database=", StringComparison.OrdinalIgnoreCase) ||
                        param.Trim().StartsWith("Initial Catalog=", StringComparison.OrdinalIgnoreCase))
                    {
                        return param.Split('=')[1].Trim();
                    }
                }
                return "Не удалось определить";
            }
            catch
            {
                return "Неизвестно";
            }
        }
    }*/
}
