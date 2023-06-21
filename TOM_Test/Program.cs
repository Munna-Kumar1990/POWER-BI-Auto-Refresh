using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TOM_Test
{
    internal class Program
    {
        public enum EmbeddedSSASIcon
        {
            PowerBI,
            Devenv,
            PowerBIReportServer,
            Loading,
            None
        }

        private static void Main(string[] args)
        {
            string[] Files = System.IO.File.ReadAllLines("Auto.txt");
            foreach (string item in Files)
            {
                Console.WriteLine(item);
            }
            foreach (string item in Files)
            {
                try
                {
                    RefreshFile(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            ConsoleKeyInfo k;
            do
            {
                Console.WriteLine("Escape to Exit");
                k = Console.ReadKey();
            } while (k.Key == ConsoleKey.Escape);
        }

        private static void RefreshFile(string FileName)
        {
            Console.WriteLine($"Started Processing {FileName} at {DateTime.Now:dd-MM-yyyy HH:mm:ss}");
            int _port = 0;
            string parentTitle = $"localhost:{_port}";
            //string FileName = "Sale Dashboard - Bread - Mgmt";
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo() { FileName = FileName + ".pbix", WindowStyle = ProcessWindowStyle.Normal }
            };
            p.Start();
            System.Threading.Thread.Sleep(30000);

            Process _wrk = null;

            Process[] msmdsrvProcesses = Process.GetProcessesByName("msmdsrv");
            Dictionary<int, TcpRow> dict = ManagedIpHelper.GetExtendedTcpDictionary();
            foreach (Process proc in msmdsrvProcesses)
            {
                Process parent = proc.GetParent();

                if (parent != null)
                {
                    // exit here if the parent == "services" then this is a SSAS instance
                    if (parent.ProcessName.Equals("services", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // exit here if the parent == "RSHostingService" then this is a SSAS instance
                    if (parent.ProcessName.Equals("RSHostingService", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // if the process was launched from Visual Studio change the icon
                    if (parent.ProcessName.Equals("devenv", StringComparison.OrdinalIgnoreCase))
                    {
                    }

                    // get the window title so that we can parse out the file name
                    parentTitle = parent.MainWindowTitle;

                    if (parentTitle.Length == 0)
                    {
                        // for minimized windows we need to use some Win32 api calls to get the title
                        //parentTitle = WindowTitle.GetWindowTitleTimeout( parent.Id, 300);
                        parentTitle = WindowTitle.GetWindowTitle(parent.Id);
                    }
                    string x = FileName.ToLower().Replace(@"d:\powerbiproj\", "");
                    if (parentTitle.ToLower().StartsWith(x))
                    {
                        try
                        {
                            dict.TryGetValue(proc.Id, out TcpRow tcpRow);
                            if (tcpRow != null)
                            {
                                    _wrk = proc;
                                   _port = tcpRow.LocalEndPoint.Port;

                            }
                            else
                            {
                                Console.WriteLine("No Match Found");
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Match Found");
                    }
                }
            }

            if (_port == 0)
            {
                Console.WriteLine("No Match Found");
                Console.WriteLine($"No Processing {FileName} at {DateTime.Now:dd-MM-yyyy HH:mm:ss}");
                return;
            }

            string ConnectionString = $"DataSource=localhost:{_port}";

            using (Server server = new Server())
            {

                server.Connect(ConnectionString);
                Console.WriteLine("Connection established successfully.");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Server name:\t\t{0}", server.Name);
                Console.WriteLine("Server product name:\t{0}", server.ProductName);
                Console.WriteLine("Server product level:\t{0}", server.ProductLevel);
                Console.WriteLine("Server version:\t\t{0}", server.Version);
                Console.ResetColor();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Refresh is in Progress");

                Database db = server.Databases[0];
                db.Model.RequestRefresh(RefreshType.Full);
                db.Model.SaveChanges(new SaveOptions() { SaveFlags = SaveFlags.Default, MaxParallelism = 0 });
                //db.Submit(true);
                db.Update();
                server.Refresh();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Refresh Done");

                Console.WriteLine("Properties for database {0}:", db.Name);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Database ID:\t\t\t{0}", db.ID);
                Console.WriteLine("Database compatibility level:\t{0}", db.CompatibilityLevel);
                Console.WriteLine("Database collation:\t\t{0}", db.Collation);
                Console.WriteLine("Database created timestamp:\t{0}", db.CreatedTimestamp);
                Console.WriteLine("Database language ID:\t\t{0}", db.Language);
                Console.WriteLine("Database model type:\t\t{0}", db.ModelType);
                Console.WriteLine("Database state:\t\t\t{0}", db.State);
                Console.ResetColor();
                Console.WriteLine();
            }
            System.Threading.Thread.Sleep(5000);
            int r = NativeWin32.SetForegroundWindow(p.MainWindowHandle.ToInt32());
            System.Windows.Forms.SendKeys.SendWait("^(s)");
            System.Threading.Thread.Sleep(50000);
            System.Windows.Forms.SendKeys.SendWait("^(s)");
            System.Threading.Thread.Sleep(5000);
            System.Windows.Forms.SendKeys.SendWait("%F4");
            if (p != null)
            {
                p.CloseMainWindow();
                System.Windows.Forms.SendKeys.SendWait("^(s)");
                System.Threading.Thread.Sleep(5000);
            }
            if (_wrk != null)
            {
                _wrk.CloseMainWindow();
                System.Windows.Forms.SendKeys.SendWait("^(s)");
                System.Threading.Thread.Sleep(5000);
            }

            Console.WriteLine($"Completed Processing {FileName} at {DateTime.Now:dd-MM-yyyy HH:mm:ss}");
            //System.Windows.Forms.SendKeys.SendWait("%F4");
            //System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            //System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            if (p != null)
            {
                p.CloseMainWindow();
                p.Close();
                p.Dispose();
            }
            if (_wrk != null)
            {
                _wrk.CloseMainWindow();
                _wrk.Close();
                _wrk.Dispose();
            }
            //System.Threading.Thread.Sleep(10000);

            //foreach (Process item in Process.GetProcessesByName("PBIDesktop"))
            //{
            //    item.Close();
            //    item.CloseMainWindow();
            //}
        }
    }
}