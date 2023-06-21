using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLAP_OLEDB
{
    public static class DefaultSettings
    {
        public static void SetFacts()
        {
            Sale.FactKeys.Add("sale", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"ASP %\", 'Sale'[ASP %],{Environment.NewLine}\"Repl %\", 'Sale'[Replacement %],{Environment.NewLine}\"Net Rate\", 'Sale'[Net Rate]");
            Sale.FactKeys.Add("yoysale", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PYrQuantity\", 'Sale'[Average PY Quantity Sold],{Environment.NewLine}\"YOY %\", 'Sale'[Quantity YOY%],{Environment.NewLine}\"ASP %\", 'Sale'[ASP %],{Environment.NewLine}\"PY ASP %\", 'Sale'[PY ASP %],{Environment.NewLine}\"Repl %\", 'Sale'[Replacement %],{Environment.NewLine}\"PY Repl %\", 'Sale'[Replacement % PY],{Environment.NewLine}\"Net Rate\", 'Sale'[Net Rate],{Environment.NewLine}\"Net Rate PY\", 'Sale'[Net Rate PY]");
            Sale.FactKeys.Add("momsale", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PMnthQuantity\", 'Sale'[Average PM Quantity Sold],{Environment.NewLine}\"MOM %\", 'Sale'[Quantity MOM%],{Environment.NewLine}\"ASP %\", 'Sale'[ASP %],{Environment.NewLine}\"PM ASP %\", 'Sale'[PM ASP %],{Environment.NewLine}\"Repl %\", 'Sale'[Replacement %],{Environment.NewLine}\"PM Repl %\", 'Sale'[Replacement % PM],{Environment.NewLine}\"Net Rate\", 'Sale'[Net Rate],{Environment.NewLine}\"Net Rate PM\", 'Sale'[Net Rate PM]");

            Sale.FactKeys.Add("quantity", "\"Quantity\", 'Sale'[Average Quantity Sold]");
            Sale.FactKeys.Add("quanty", "\"Quantity\", 'Sale'[Average Quantity Sold]");
            Sale.FactKeys.Add("quanti", "\"Quantity\", 'Sale'[Average Quantity Sold]");
            Sale.FactKeys.Add("qunti", "\"Quantity\", 'Sale'[Average Quantity Sold]");
            Sale.FactKeys.Add("qantity", "\"Quantity\", 'Sale'[Average Quantity Sold]");

            Sale.FactKeys.Add("yoyquantity", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PYrQuantity\", 'Sale'[Average PY Quantity Sold],{Environment.NewLine}\"YOY %\", 'Sale'[Quantity YOY%]");
            Sale.FactKeys.Add("yoyquanty", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PYrQuantity\", 'Sale'[Average PY Quantity Sold],{Environment.NewLine}\"YOY %\", 'Sale'[Quantity YOY%]");
            Sale.FactKeys.Add("yoyquanti", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PYrQuantity\", 'Sale'[Average PY Quantity Sold],{Environment.NewLine}\"YOY %\", 'Sale'[Quantity YOY%]");
            Sale.FactKeys.Add("yoyqunti", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PYrQuantity\", 'Sale'[Average PY Quantity Sold],{Environment.NewLine}\"YOY %\", 'Sale'[Quantity YOY%]");
            Sale.FactKeys.Add("yoyqantity", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PYrQuantity\", 'Sale'[Average PY Quantity Sold],{Environment.NewLine}\"YOY %\", 'Sale'[Quantity YOY%]");

            Sale.FactKeys.Add("yoyqty", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PYrQuantity\", 'Sale'[Average PY Quantity Sold],{Environment.NewLine}\"YOY %\", 'Sale'[Quantity YOY%]");
            Sale.FactKeys.Add("yoyqty400", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PYrQuantity\", 'Sale'[Average PY Quantity Sold],{Environment.NewLine}\"YOY %\", 'Sale'[Quantity YOY%]");
            Sale.FactKeys.Add("yoyq400", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PYrQuantity\", 'Sale'[Average PY Quantity Sold],{Environment.NewLine}\"YOY %\", 'Sale'[Quantity YOY%]");
            Sale.FactKeys.Add("yoyqty 400", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PYrQuantity\", 'Sale'[Average PY Quantity Sold],{Environment.NewLine}\"YOY %\", 'Sale'[Quantity YOY%]");

            Sale.FactKeys.Add("momquantity", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PMnthQuantity\", 'Sale'[Average PM Quantity Sold],{Environment.NewLine}\"MOM %\", 'Sale'[Quantity MOM%]");
            Sale.FactKeys.Add("momquanty", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PMnthQuantity\", 'Sale'[Average PM Quantity Sold],{Environment.NewLine}\"MOM %\", 'Sale'[Quantity MOM%]");
            Sale.FactKeys.Add("momquanti", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PMnthQuantity\", 'Sale'[Average PM Quantity Sold],{Environment.NewLine}\"MOM %\", 'Sale'[Quantity MOM%]");
            Sale.FactKeys.Add("momqunti", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PMnthQuantity\", 'Sale'[Average PM Quantity Sold],{Environment.NewLine}\"MOM %\", 'Sale'[Quantity MOM%]");
            Sale.FactKeys.Add("momqantity", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PMnthQuantity\", 'Sale'[Average PM Quantity Sold],{Environment.NewLine}\"MOM %\", 'Sale'[Quantity MOM%]");

            Sale.FactKeys.Add("momqty", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PMnthQuantity\", 'Sale'[Average PM Quantity Sold],{Environment.NewLine}\"MOM %\", 'Sale'[Quantity MOM%]");
            Sale.FactKeys.Add("momqty400", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PMnthQuantity\", 'Sale'[Average PM Quantity Sold],{Environment.NewLine}\"MOM %\", 'Sale'[Quantity MOM%]");
            Sale.FactKeys.Add("momq400", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PMnthQuantity\", 'Sale'[Average PM Quantity Sold],{Environment.NewLine}\"MOM %\", 'Sale'[Quantity MOM%]");
            Sale.FactKeys.Add("momqty 400", $"\"Quantity\", 'Sale'[Average Quantity Sold],{Environment.NewLine}\"PMnthQuantity\", 'Sale'[Average PM Quantity Sold],{Environment.NewLine}\"MOM %\", 'Sale'[Quantity MOM%]");

            Sale.FactKeys.Add("qty", "\"Quantity\", 'Sale'[Average Quantity Sold]");
            Sale.FactKeys.Add("qty400", "\"Quantity\", 'Sale'[Average Quantity Sold]");
            Sale.FactKeys.Add("q400", "\"Quantity\", 'Sale'[Average Quantity Sold]");
            Sale.FactKeys.Add("qty 400", "\"Quantity\", 'Sale'[Average Quantity Sold]");
            Sale.FactKeys.Add("q 400", "\"Quantity\", 'Sale'[Average Quantity Sold]");
            Sale.FactKeys.Add("400", "\"Quantity\", 'Sale'[Average Quantity Sold]");

            Sale.FactKeys.Add("asp", "\"ASP % \", 'Sale'[ASP %]");
            Sale.FactKeys.Add("a&sp", "\"ASP % \", 'Sale'[ASP %]");
            Sale.FactKeys.Add("asp%", "\"ASP % \", 'Sale'[ASP %]");
            Sale.FactKeys.Add("asp %", "\"ASP % \", 'Sale'[ASP %]");
            Sale.FactKeys.Add("a&sp%", "\"ASP % \", 'Sale'[ASP %]");

            Sale.FactKeys.Add("rpl", "\"Replacement %\", 'Sale'[Replacement %]");
            Sale.FactKeys.Add("replacement", "\"Replacement %\", 'Sale'[Replacement %]");
            Sale.FactKeys.Add("repl", "\"Replacement %\", 'Sale'[Replacement %]");
            Sale.FactKeys.Add("rpl%", "\"Replacement %\", 'Sale'[Replacement %]");
            Sale.FactKeys.Add("replacement%", "\"Replacement %\", 'Sale'[Replacement %]");
            Sale.FactKeys.Add("repl%", "\"Replacement %\", 'Sale'[Replacement %]");
            Sale.FactKeys.Add("rpl %", "\"Replacement %\", 'Sale'[Replacement %]");
            Sale.FactKeys.Add("replacement %", "\"Replacement %\", 'Sale'[Replacement %]");
            Sale.FactKeys.Add("repl %", "\"Replacement %\", 'Sale'[Replacement %]");

            Sale.FactKeys.Add("net", "\"Net Rate\", 'Sale'[Net Rate]");
            Sale.FactKeys.Add("net rate", "\"Net Rate\", 'Sale'[Net Rate]");
            Sale.FactKeys.Add("rate", "\"Net Rate\", 'Sale'[Net Rate]");
            Sale.FactKeys.Add("net rate400", "\"Net Rate\", 'Sale'[Net Rate]");
            Sale.FactKeys.Add("rate400", "\"Net Rate\", 'Sale'[Net Rate]");
            Sale.FactKeys.Add("net rate 400", "\"Net Rate\", 'Sale'[Net Rate]");
            Sale.FactKeys.Add("rate 400", "\"Net Rate\", 'Sale'[Net Rate]");
        }

        internal static string FormatMe(string v)
        {
            switch (v)
            {
                case "Calendar[Date]":
                    return "dd-MM-yyyy";
                case "[Quantity]":
                    return "N0";
                case "[ASP %]":
                    return "P2";
                case "[Repl %]":
                    return "P2";
            }
            return "";
        }

        public static void SetDims()
        {
            Sale.DimKeys.Add("team", string.Format("'Team'[L3],{0}'Team'[L4],{0}'Team'[L5]{0}",Environment.NewLine));

            Sale.DimKeys.Add("today", "'Calendar'[Date]");
            Sale.DimKeys.Add("date", "'Calendar'[Date]");
            Sale.DimKeys.Add("daily", "'Calendar'[Date]");

            Sale.DimKeys.Add("month", "'Calendar'[MonthYear]");
            Sale.DimKeys.Add("monthly", "'Calendar'[MonthYear]");
            Sale.DimKeys.Add("mnthly", "'Calendar'[MonthYear]");
            Sale.DimKeys.Add("mnth", "'Calendar'[MonthYear]");
            Sale.DimKeys.Add("yoysale", "'Calendar'[MonthYear]");
            Sale.DimKeys.Add("momsale", "'Calendar'[MonthYear]");

            Sale.DimKeys.Add("l2", "'Team'[Region]");
            Sale.DimKeys.Add("region", "'Team'[Region]");

            Sale.DimKeys.Add("l3", "'Team'[L3]");
            Sale.DimKeys.Add("rsm", "'Team'[L3]");
            Sale.DimKeys.Add("l3 team", "'Team'[L3]");

            Sale.DimKeys.Add("l4", "'Team'[L4]");
            Sale.DimKeys.Add("asm", "'Team'[L4]");
            Sale.DimKeys.Add("l4 team", "'Team'[L4]");

            Sale.DimKeys.Add("l5", "'Team'[L5]");
            Sale.DimKeys.Add("so", "'Team'[L5]");
            Sale.DimKeys.Add("l5 team", "'Team'[L5]");

            Sale.DimKeys.Add("category", "'Category'[Category]");
            Sale.DimKeys.Add("cat", "'Category'[Category]");
            Sale.DimKeys.Add("categ", "'Category'[Category]");
            Sale.DimKeys.Add("catg", "'Category'[Category]");

            Sale.DimKeys.Add("city", "'City'[Cityname]");
            Sale.DimKeys.Add("state", "'State'[StateName]");


        }

        internal static string FetchFirstFact(string result)
        {
            if (result.Contains("Quantity"))
                return "'Sale'[Average Quantity Sold]";
            else if (result.Contains("ASP"))
                return "'Sale'[ASP %]";
            else if (result.Contains("Repl"))
                return "'Sale'[Replacement %]";
            else if (result.Contains("Net Rate"))
                return "'Sale'[Net Rate]";
            else if (result.Contains("YOY %"))
                return "'Sale'[Quantity YOY%]";
            else if (result.Contains("MOM %"))
                return "'Sale'[Quantity MOM%]";
            else
                return "";
        }

        internal static string FactToAlias(string result)
        {
            if (result.Contains("'Sale'[Average Quantity Sold]"))
                return "Quantity";
            else if (result.Contains("'Sale'[ASP %]"))
                return "ASP";
            else if (result.Contains("'Sale'[Replacement %]"))
                return "Replacement %";
            else if (result.Contains("'Sale'[Net Rate]"))
                return "Net Rate";
            else if (result.Contains("'Sale'[Quantity YOY%]"))
                return "YOY %";
            else if (result.Contains("'Sale'[Quantity MOM%]"))
                return "MOM %";
            else
                return "";

        }
    }
}
