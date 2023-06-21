using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace OLAP_OLEDB
{
    internal class Sale
    {
        internal static Dictionary<string, string> DimKeys = new Dictionary<string, string>();
        internal static Dictionary<string, string> FactKeys = new Dictionary<string, string>();
        internal static Dictionary<string, Key> Keys = new Dictionary<string, Key>();
        private static string FirstFact;
        private static bool IsFirstFact = true;

        public static string CategoryFilter(string[] Categories = null)
        {
            if (Categories == null || Categories.Length == 0)
            {
                return @"VAR CategoryFilter =
                    FILTER(
                    KEEPFILTERS(VALUES('Category'[Category])),
                    NOT('Category'[Category] IN { \""Biscuit\"",\""Cake\"",\""Rusk\"",\""Other\"" } ))";
            }
            else
            {
                return @"VAR CategoryFilter =
                    FILTER(
                    KEEPFILTERS(VALUES('Category'[Category])),
                    NOT('Category'[Category] IN { \""Biscuit\"",\""Cake\"",\""Rusk\"",\""Other\"" } ))
                    && ('Category'[Category] IN { \""" + string.Join("\",\"", Categories) + "\"} ))";
            }
        }

        public static string RegionFilter(string[] Regions = null)
        {
            if (Regions == null || Regions.Length == 0)
            {
                return @"VAR RegionFilter =
                    FILTER(
                    KEEPFILTERS(VALUES('Team'[Region])),
                    NOT('Team'[Region] IN { \""Other\"" } ))";
            }
            else
            {
                return @"VAR RegionFilter =
                    FILTER(
                    KEEPFILTERS(VALUES('Team'[Region])),
                    NOT('Team'[Region] IN { \""Other\"" } )
                    && ('Team'[Region] IN { \""" + string.Join("\",\"", Regions) + "\"} ))";

            }
        }

        public static string DateFilter(List<mDate> mDates)
        {
            List<string> datefilterlist = new List<string>();
            foreach (mDate item in mDates)
            {
                datefilterlist.Add($@"AND (
                                'Calendar'[Date] >= DATE ( {item.From.Year}, {item.From.Month}, {item.From.Day} ),
                                'Calendar'[Date] <= DATE ( {item.To.Year}, {item.To.Month}, {item.To.Day} )
                                )
                        ");
            }

            return $@"VAR DateFilter =
                            FILTER (
                                KEEPFILTERS ( VALUES ( 'Calendar'[Date] ) ),
                                {string.Join(" || ", datefilterlist)}    
                                )
                            ";
        }

        public static string DimStr(HashSet<string> Dims)
        {
            string result = "";
            HashSet<string> _DimStr = new HashSet<string>();

            foreach (string item in Dims)
            {
                if (DimKeys.TryGetValue(item.ToLower(), out result))
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        _DimStr.Add(result);
                    }
                }

            }
            return string.Join($" ,{Environment.NewLine}", _DimStr);
        }

        public static string FactStr(HashSet<string> Facts)
        {
            string result = "";
            HashSet<string> _Facts = new HashSet<string>();

            foreach (string item in Facts)
            {
                if (FactKeys.TryGetValue(item.ToLower(), out result))
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        if (IsFirstFact)
                        {
                            //Console.WriteLine(result);
                            FirstFact = DefaultSettings.FetchFirstFact(result);
                            //Console.Write(FirstFact);
                            IsFirstFact = false;
                        }
                        _Facts.Add(result);
                    }
                }
            }
            return string.Join($" ,{Environment.NewLine}", _Facts);
        }

        public static string Query(string MobileNumber, string message)
        {
            IsFirstFact = true;
            List<string> _FilterKeys = new List<string>();
            List<string> _FilterValues = new List<string>();
            HashSet<string> _Facts = new HashSet<string>();
            HashSet<string> _Dims = new HashSet<string>();
            List<Key> _FilterWithHash = new List<Key>();
            List<mDate> mDates = new List<mDate>();
            bool IsTop = false;
            bool IsBottom = false;

            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(MobileNumber))
            {
                return "";
            }


            // Check User
            using (SqlConnection connection = new SqlConnection(Global_Settings.ConnectionString))
            {
                connection.Open();
                SqlCommand sqlCmd = connection.CreateCommand();
                sqlCmd.CommandText = "USP_GetUser";
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCmd.Parameters.Clear();
                sqlCmd.Parameters.AddWithValue("@MobileNumber", MobileNumber.ToLower());
                SqlDataReader reader = sqlCmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    return "";
                }

                while (reader.Read())
                {
                    string _FKeys = reader["FilterKey"].ToString().Trim();
                    string _FValues = reader["FilterValue"].ToString().Trim();

                    if (!(string.IsNullOrEmpty(_FKeys) || string.IsNullOrEmpty(_FValues)))
                    {
                        _FilterKeys.AddRange(_FKeys.Split('#'));
                        _FilterValues.AddRange(_FValues.Split('#'));
                    }
                }
            }
            using (SqlConnection connection = new SqlConnection(Global_Settings.ConnectionString))
            {
                connection.Open();
                SqlCommand sqlCmd = connection.CreateCommand();
                message = message.Replace(',', ' ');
                message = message.Replace("  ", " ");
                string[] words = message.Split(' ');
                

                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCmd.Parameters.Clear();
                sqlCmd.CommandText = "USP_GetKey";
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                // Rules to parse words
                foreach (string item in words)
                {
                    switch (item.ToLower())
                    {
                        case "top":
                            IsTop = true;
                            continue;
                        case "bottom":
                            IsBottom = true;
                            continue;

                        case "yoysale":
                        case "month":
                        case "monthly":
                        case "mnthly":
                        case "mnth":
                        case var a when item.ToLower().StartsWith("mom"):
                            _Dims.Add("month");
                            mDates.Add(new mDate() { From = DateTime.ParseExact(DateTime.Now.Date.AddDays(-1).ToString("01-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture), To = DateTime.Now.Date.AddDays(-1) });
                            break;

                        case var a when item.ToLower().StartsWith("yearly"):
                            using (DataSet xset = BaseFunctions.GetDataSet($"USp_GetFinYearRange '{DateTime.Now.Date.AddDays(-1):yyyy-MM-dd}'"))
                            {
                                mDates.Add(new mDate() { From = Convert.ToDateTime(xset.Tables[0].Rows[0][0]), To = DateTime.Now.Date.AddDays(-1) });
                            }
                            break;

                    }

                    bool proc = false;
                    string _hashFilter = string.Empty;
                    string _item = string.Empty;

                    if (item.ToLower().Contains("#"))
                    {
                        
                        _hashFilter = item.Substring(item.IndexOf('#') + 1, item.Length - (item.IndexOf('#') + 1));
                        _item = item.Substring(0, item.IndexOf('#'));
                        Console.WriteLine(_hashFilter);
                    }
                    else
                        _item = item;

                    if (Keys.TryGetValue(_item.ToLower(), out Key _result))
                    {
                        if (_result.IsFact)
                        {
                            _Facts.Add(_result.KeyValue);
                        }
                        else
                        {
                            _result.KeyFilter = _hashFilter;
                            Console.WriteLine(_hashFilter);
                            _Dims.Add(_result.KeyValue);
                            if (!string.IsNullOrEmpty(_hashFilter))
                            {
                                Console.WriteLine("Added Hash filter" + _hashFilter);
                                _FilterWithHash.Add(_result);
                            }
                        }

                        proc = true;
                    }
                    else
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.AddWithValue("@string", _item.ToLower());
                        SqlDataReader _reader = sqlCmd.ExecuteReader();
                        while (_reader.Read())
                        {
                            Console.WriteLine(_hashFilter);
                            Key _key = new Key() { IsFact = Convert.ToBoolean(_reader["IsFact"]), KeyValue = _reader["MainKey"].ToString(), KeyFilter=_hashFilter };
                            Keys.Add(_item.ToLower(), _key);

                            if (_key.IsFact)
                            {
                                _Facts.Add(_key.KeyValue);
                            }
                            else
                            {
                                _Dims.Add(_key.KeyValue);
                                if (!string.IsNullOrEmpty(_hashFilter))
                                {
                                    Console.WriteLine("Added Hash filter" + _hashFilter);
                                    _FilterWithHash.Add(_key);
                                }
                            }

                            proc = true;
                        }
                        _reader.Close();
                    }

                    if (!proc)
                    {
                        //No. of Days
                        if (_item.Trim().Length <= 2 && _item.Trim().Length > 0)
                        {
                            if (int.TryParse(_item, out int days))
                            {
                                if (days > 15)
                                {
                                    days = 15;
                                }
                                _Dims.Add("DAILY");
                                if (mDates.Count == 0)
                                {
                                    mDates.Add(new mDate() { From = DateTime.Now.Date.AddDays(-days), To = DateTime.Now.Date });
                                }
                            }
                            else
                            {
                                _Dims.Add(_item.ToLower().Trim());
                                if (!string.IsNullOrEmpty(_hashFilter))
                                {
                                    Console.WriteLine("Added Hash filter" + _hashFilter);
                                    _FilterWithHash.Add(new Key() { IsFact = false, KeyFilter = _hashFilter, KeyValue = _item.ToLower() });
                                }
                            }
                        }
                        else
                        {
                            _Dims.Add(_item.ToLower().Trim());
                            if (!string.IsNullOrEmpty(_hashFilter))
                            {
                                Console.WriteLine("Added Hash filter" + _hashFilter);
                                _FilterWithHash.Add(new Key() {IsFact=false,KeyFilter=_hashFilter,KeyValue=_item.ToLower() });
                            }
                        }
                    }
                }
            }
            // Prepare Query
            StringBuilder builder = new StringBuilder();
            string xlookup = "";
            builder.Append("DEFINE");
            builder.Append(Environment.NewLine);
            foreach (string _PrimaryFilter in _FilterValues)
            {
                builder.Append(_PrimaryFilter);
                builder.Append(Environment.NewLine);
            }
            builder.Append(CategoryFilter());
            builder.Append(Environment.NewLine);
            builder.Append(RegionFilter());
            builder.Append(Environment.NewLine);
            if (mDates.Count == 0)
            {
                mDates.Add(new mDate() { From = DateTime.Now.AddDays(-1).Date, To = DateTime.Now.AddDays(-1).Date });
            }
            builder.Append(Environment.NewLine);
            builder.Append(DateFilter(mDates));
            builder.Append(Environment.NewLine);
            builder.Append("VAR Result = ");
            builder.Append(Environment.NewLine);
            if (IsTop || IsBottom)
            {
                builder.Append("TopN(15, ");
                builder.Append(Environment.NewLine);
            }

            builder.Append("SUMMARIZECOLUMNS(");
            builder.Append(Environment.NewLine);

            if (_Dims.Count > 0)
            {
                xlookup = DimStr(_Dims);
                builder.Append(xlookup);
                if (xlookup.Trim().Length > 0)
                {
                    builder.Append(",");
                    builder.Append(Environment.NewLine);
                }
            }
           

            foreach (string _pKeys in _FilterKeys)
            {
                builder.Append(_pKeys);
                builder.Append(",");
                builder.Append(Environment.NewLine);
            }
            builder.Append("CategoryFilter,");
            builder.Append(Environment.NewLine);
            builder.Append("RegionFilter,");
            builder.Append(Environment.NewLine);
            builder.Append("DateFilter");

            if (_FilterWithHash.Count > 0)
            {
                Console.WriteLine("#Str");
                string FilterWithHash = FilterStr(_FilterWithHash);
                if (xlookup.Trim().Length > 0)
                {
                    builder.Append(",");
                    builder.Append(Environment.NewLine);
                    builder.Append(FilterWithHash);
                    builder.Append(Environment.NewLine);
                }
            }

            if (_Facts.Count > 0)
            {
                builder.Append(",");
                builder.Append(Environment.NewLine);
                builder.Append(FactStr(_Facts));
            }

            builder.Append(Environment.NewLine);
            builder.AppendLine(")");
            if (IsTop || IsBottom)
            {
                if (FirstFact.Length > 2)
                {
                    if (IsTop)
                    {
                        builder.Append($", {FirstFact},0)");
                    }
                    else
                    {
                        builder.Append($", {FirstFact},1)");
                    }

                    builder.Append(Environment.NewLine);
                }
                else
                {
                    builder.Append(") ");
                    builder.Append(Environment.NewLine);
                }
            }

            builder.AppendLine("EVALUATE");
            builder.AppendLine("Result");

            if (xlookup.Contains("[Date]"))
            {
                builder.Append("Order By 'Calendar'[Date]");
            }
            else if (DefaultSettings.FactToAlias(FirstFact).Length > 2)
            {
                builder.Append($"Order By  [{DefaultSettings.FactToAlias(FirstFact)}] {(IsTop ? " Desc" : " Asc")}");
                builder.Append(Environment.NewLine);
            }

            string result = builder.ToString();
            return result.Replace(@"\", string.Empty);
        }

        private static string FilterStr(List<Key> filterWithHash)
        {
            string result = "";
            HashSet<string> _hashStr = new HashSet<string>();

            foreach (Key item in filterWithHash)
            {
                if (DimKeys.TryGetValue(item.KeyValue.ToLower(), out result))
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        _hashStr.Add($"FILTER({result.Substring(0,result.IndexOf('['))}, CONTAINSSTRING({result}, \"{item.KeyFilter}\"))");
                    }
                }

            }
            return string.Join($" ,{Environment.NewLine}", _hashStr);
        }
    }
}
