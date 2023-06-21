using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Data;
using System.Data.SqlClient;

namespace OLAP_OLEDB
{
    public class OLAP_Controller
    {
        public OLAP_Controller()
        {
            Global_Settings.OLAP_port = System.IO.File.ReadAllText(@"Port.txt");
            Global_Settings.Olap_connectionString = $@"Provider=MSOLAP;Data Source=localhost:{Global_Settings.OLAP_port};";
            DefaultSettings.SetDims();
            DefaultSettings.SetFacts();
        }

        public static void RefreshFile(string FileName)
        {
            using (AdomdConnection connection = new AdomdConnection())
            {
                connection.ConnectionString = Global_Settings.Olap_connectionString;
                connection.Open();
                AdomdCommand cmd = connection.CreateCommand();
                cmd.CommandText = "{\"refresh\": {\"type\": \"full\",\"objects\": [{\"database\": \"6defc297-069b-4b43-ab41-beea39ddd237\"}]}}";
                cmd.ExecuteNonQuery();
            }
        }

        public static string ProcessQuery(string MobileNo, string Message)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            //Validate Message for Accurate Process
            if (string.IsNullOrEmpty(MobileNo) || string.IsNullOrEmpty(Message))
            {
                return "Please Try Again";
            }

            //Convert Query into DAX
            string queryString = Sale.Query(MobileNo, Message);
            if (queryString.Length == 0)
            {
                return "Please Try Again";
            }

            // Log into Database
            BaseFunctions.ExecuteNonQuery("INSERT INTO [dbo].[QueryLog]([Query],[Msg],[Mobile]) VALUES(@Query,@Msg,@Mobile)",
                System.Data.CommandType.Text,
                new SqlParameter("@Query", queryString),
                new SqlParameter("@Msg", Message),
                new SqlParameter("@Mobile", MobileNo)
                 );

            // Send DAX to OLAP/Power Bi File
            try
            {
                using (AdomdConnection connection = new AdomdConnection())
                {
                    connection.ConnectionString = Global_Settings.Olap_connectionString;
                    connection.Open();
                    AdomdCommand cmd = connection.CreateCommand();
                    cmd.CommandText = queryString;
                    string shiftenter = OpenQA.Selenium.Keys.Shift + OpenQA.Selenium.Keys.Enter.ToString() + OpenQA.Selenium.Keys.Shift;

                    using (AdomdDataAdapter ad = new AdomdDataAdapter(cmd))
                    {
                        DataTable dtSchema = new DataTable("Schema");
                        AdomdDataReader rdr = cmd.ExecuteReader();
                        if (rdr != null)
                        {
                            dtSchema = rdr.GetSchemaTable();
                            result.AppendLine($"Result of your query {Message}: ");
                            result.Append(Environment.NewLine);

                            while (rdr.Read())
                            {
                                object[] ColArray = new object[rdr.FieldCount];
                                int x = 0;

                                foreach (DataRow schemarow in dtSchema.Rows)
                                {
                                    string _out = "";
                                    try
                                    {
                                        switch (Type.GetTypeCode(rdr[x].GetType()))
                                        {
                                            case TypeCode.DateTime:
                                                _out = $"{rdr[x]:dd-MM-yyyy}";
                                                break;
                                            case TypeCode.Int16:
                                            case TypeCode.Int32:
                                            case TypeCode.Int64:
                                                if (schemarow.ItemArray[0].ToString().Contains("%"))
                                                {
                                                    _out = $"{rdr[x]:P2}";
                                                }
                                                else
                                                {
                                                    _out = $"{rdr[x]:N0}";
                                                }

                                                break;

                                            case TypeCode.Double:
                                            case TypeCode.Decimal:
                                                if (schemarow.ItemArray[0].ToString().Contains("%"))
                                                {
                                                    _out = $"{rdr[x]:P2}";
                                                }
                                                else
                                                {
                                                    _out = $"{rdr[x]:N2}";
                                                }

                                                break;
                                            case TypeCode.Empty:
                                            case TypeCode.DBNull:
                                                _out = "";
                                                break;
                                            default:
                                                _out = rdr[x].ToString();
                                                break;
                                        }
                                    }catch(Exception e)
                                    {
                                        _out = "";
                                    }
                                    
                                    //result.Append($"{schemarow.ItemArray[0].ToString().Replace("["," ").Replace("]","")} : {_out}{shiftenter}");
                                    string colName = schemarow.ItemArray[0].ToString();
                                    colName=colName.Substring(colName.IndexOf('[') + 1, colName.IndexOf(']') - (colName.IndexOf('[')+1));
                                    //Console.WriteLine(colName);
                                    result.Append($"{colName} : {_out}{Environment.NewLine}");
                                    //result.Append(Environment.NewLine);
                                    x++;
                                }
                                result.Append(Environment.NewLine);
                            }
                            rdr.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = new System.Text.StringBuilder();
                result.Append("Unable to Process your Request.");
            }
            return result.ToString();

        }
    }
}
