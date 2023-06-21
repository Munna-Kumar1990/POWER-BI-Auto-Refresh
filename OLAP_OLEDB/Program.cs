using System;

namespace OLAP_OLEDB
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new BaseFunctions().CheckBroker();
            Console.WriteLine("Satellite ON");
            new OLAP_Controller();
            //OLAP_Controller.RefreshFile("");
            Console.WriteLine("All is Well");
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
            } while (keyInfo.Key != ConsoleKey.Escape);

            //string s= @"Team[L3] ANIL KUMAR ARORA
            //Team[L4] MANOJ THAKUR
            //Team[L5] X PB -JAL - 1 - X VARINDER ARORA";

            //foreach (var item in s)
            //{
            //    Console.Write(item + ":"+ Convert.ToInt32(item));

            //}
        }
    }
}
