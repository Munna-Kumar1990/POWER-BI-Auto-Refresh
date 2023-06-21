using System;
using System.Data;
using System.Data.SqlClient;

namespace OLAP_OLEDB
{
    internal class BaseFunctions
    {

        public BaseFunctions()
        {
            GetDataSet("if Not exists (SELECT Is_broker_enabled,* FROM sys.databases WHERE database_id=db_id() and Is_broker_enabled=1) Alter Database " + Global_Settings.DBName + " SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE; ");
            SqlDependency.Stop(Global_Settings.ConnectionString);
            SqlDependency.Start(Global_Settings.ConnectionString);
        }

        public void CheckBroker()
        {
            try
            {
                SearchUserLog();
            }
            catch (Exception exception1)
            {
                Console.WriteLine(exception1.Message);
            }
        }

        public static DataSet GetDataSet(string Query)
        {
            DataSet set2;
            using (DataSet set = new DataSet())
            {
                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(Query, new SqlConnection(Global_Settings.ConnectionString)))
                    {
                        adapter.SelectCommand.CommandTimeout = 0xe10;
                        adapter.Fill(set);
                    }
                    set2 = set;
                }
                catch (Exception exception1)
                {
                    Console.WriteLine(exception1.Message);
                    set2 = null;
                }
            }
            return set2;
        }

        private void GetMessages()
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(Global_Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "dbo.GetMessagesX";
                    new SqlDependency(command).OnChange += new OnChangeEventHandler(sqlDependency_OnChange);
                    table.Rows.Clear();
                    table.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
                }
            }
        }

        public void SearchUserLog()
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(Global_Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT  user_log_id, UserMsg, UserMob, CreatedOnApp, LogDateTime, AppSrc, action_date, SMSIsHandled FROM dbo.SMSApp WHERE [SMSIsHandled] = 0";
                    new SqlDependency(command).OnChange += new OnChangeEventHandler(sqlDependency_OnChange);
                    table.Rows.Clear();
                    table.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
                    foreach (DataRow row in table.Rows)
                    {
                        try
                        {
                            Console.WriteLine($"{row["UserMsg"]}");
                            string Result = OLAP_Controller.ProcessQuery(row["UserMob"].ToString(), row["UserMsg"].ToString());
                            SendWhatsApp(Result, row["UserMob"].ToString(), "");
                            GetDataSet($"Update dbo.SMSApp Set SMSIsHandled=1 Where user_log_id='{row["user_log_id"]}'");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        internal static void SendWhatsApp(string msg, string mobileNo, string filename)
        {
            using (SqlConnection conn = new SqlConnection(Global_Settings.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "[USP_InsertWhatsAppMsg]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Msg", SqlDbType.NVarChar, 4000).Value = string.IsNullOrEmpty(msg) ? "Info" : msg;
                cmd.Parameters.Add("@MobileNo", SqlDbType.VarChar, 10).Value = mobileNo;
                cmd.Parameters.Add("@RefId", SqlDbType.VarChar, 200).Value = filename;
                cmd.Parameters.Add("@IsSent", SqlDbType.Bit).Value = 0;
                cmd.Parameters.Add("@OnlyMsg", SqlDbType.Bit).Value = (filename.Trim().Length > 0) ? 0 : 1;

                cmd.ExecuteNonQuery();
            }
        }

        private void sqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            Console.WriteLine($"OnChange Event fired. SqlNotificationEventArgs: Info={e.Info}, Source={e.Source}, Type={e.Type}");
            if ((e.Info != SqlNotificationInfo.Invalid) && (e.Type != SqlNotificationType.Subscribe))
            {
                SearchUserLog();
            }
        }

        // Set the connection, command, and then execute the command with non query.  
        public static int ExecuteNonQuery(string commandText,
            CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(Global_Settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    // There're three command types: StoredProcedure, Text, TableDirect. The TableDirect   
                    // type is only for OLE DB.    
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

