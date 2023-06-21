using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLAP_OLEDB
{
    internal static class Global_Settings
    {
        internal static string ConnectionString = @"Data Source=10.0.0.3\Express;Initial Catalog=iChat;User ID=sa;Password=TigerFish1!";
        internal static string DBName = "iChat";

        public static string Olap_connectionString { get; internal set; }
        public static string OLAP_port { get; internal set; }
    }
}
