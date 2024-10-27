
using FirewallLogIngestService.pfirewallDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FirewallLogIngestService
{
    


    internal class FirewallLogFile
    {
        private string filePath;

        public FirewallLogFile(string filePath)
        {
            this.filePath = filePath;
        }

        public void setFilePath(string filePath)
        {
            this.filePath = filePath;
        }



        public void processFile()
        {
            using (var fs = new FileStream(this.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {

                int currentIndex = 0;

                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    while(!sr.EndOfStream)
                    {
                        currentIndex++;

                        var line = sr.ReadLine();

                        if (currentIndex >= Properties.Settings.Default.lastRecIndex)
                        {
                            FirewallLogLine logLine = new FirewallLogLine(line);
                            logLine.process();
                        }

                        
                    }
                }
            }
        }

    }
}
