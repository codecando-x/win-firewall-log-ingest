using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FirewallLogIngestService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            /*ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);*/

            StringCollection ipAddresses = new StringCollection();

            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            for (int i = 0; i < localIPs.Length; i++)
            {
                //Console.WriteLine("IP Address {0}: {1} ", i, localIPs[i].ToString());
                ipAddresses.Add(localIPs[i].ToString());
            }

            Properties.Settings.Default.LocalIpAddress = ipAddresses;

            Properties.Settings.Default.lastRecIndex = getProcessedCount();

            long oldSize = Properties.Settings.Default.FirewallLogFileSize;
            oldSize = 4 * 1024 * 1024;

            Properties.Settings.Default.FirewallLogFileSize = getFileSize(Properties.Settings.Default.FirewallLogFilePath);

            if(Properties.Settings.Default.FirewallLogFileSize < oldSize)
            {
                Properties.Settings.Default.lastRecIndex = 0;
            }

            Console.WriteLine(Properties.Settings.Default.lastRecIndex);
            Console.WriteLine(Properties.Settings.Default.FirewallLogFileSize);

            Properties.Settings.Default.lastRecIndex = 0;

            Console.WriteLine(Properties.Settings.Default.lastRecIndex);
            Console.WriteLine(Properties.Settings.Default.FirewallLogFileSize);

            FirewallLogFile logFile = new FirewallLogFile(Properties.Settings.Default.FirewallLogFilePath);
            logFile.processFile();

        }

        static long getFileSize(string path)
        {
            long fileSize = 0;
            try
            {
                fileSize = new System.IO.FileInfo(path).Length;
                
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                fileSize = -1;
            }

            return fileSize;
            
        }

        static int getProcessedCount()
        {

            int haveIt = 0;

            pfirewallDataSetTableAdapters.entriesTableAdapter eta = new pfirewallDataSetTableAdapters.entriesTableAdapter();

            eta.Connection.Open();

            SqlCommand command = new SqlCommand("SELECT COUNT(*) AS howMany FROM [pfirewall].[dbo].[entries];", eta.Connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    Console.WriteLine(String.Format("UPDATED INDEX TO: {0}", reader["howMany"]));

                    haveIt = int.Parse(reader["howMany"].ToString());
                    
                }
            }
            eta.Connection.Close();

            return haveIt;
        }
    }
}
