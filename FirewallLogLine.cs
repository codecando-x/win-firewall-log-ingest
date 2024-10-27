using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewallLogIngestService
{
    internal class FirewallLogLine
    {
        private string line;
        private string QuickHashSha256(string input)
        {
            return BitConverter.ToString(new System.Security.Cryptography.SHA256Managed().ComputeHash(System.Text.Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty);
        }
        public FirewallLogLine(string line) {

            this.line = line;
            
        }

        public void process()
        {
            if (line.StartsWith("#")) return;

            line = line.Replace("\0", "");
            line = line.Replace(" -", " 0");
            var pieces = line.Split(' ');

            Console.WriteLine(line);

            if (pieces.Length != 18 || !pieces[0].Contains("-")) return;

            Entry entry = new Entry();
            entry.date = DateTime.Parse(pieces[0]);
            entry.time = TimeSpan.Parse(pieces[1]);
            entry.action = pieces[2];
            entry.protocol = pieces[3];
            entry.src_ip = pieces[4];
            entry.dst_ip = pieces[5];
            entry.src_port = int.Parse(pieces[6]);
            entry.dst_port = int.Parse(pieces[7]);
            entry.size = int.Parse(pieces[8]);
            entry.tcp_flags = pieces[9];
            entry.tcp_syn = int.Parse(pieces[10]);
            entry.tcp_ack = int.Parse(pieces[11]);
            entry.tcp_win = int.Parse(pieces[12]);
            entry.icmp_type = pieces[13];
            entry.icmp_code = pieces[14];
            entry.info = pieces[15];
            entry.path = pieces[16];
            entry.pid = int.Parse(pieces[17]);
            entry.src_ip_geo = null;
            entry.dst_ip_geo = null;
            entry.last_updated = DateTime.Now;
            entry.id = QuickHashSha256(line);
            entry.src_city = null;
            entry.src_country = null;
            entry.dst_city = null;
            entry.dst_country = null;

            if (!Properties.Settings.Default.LocalIpAddress.Contains(entry.src_ip))
            {
                MaxmindGeoWrapper mmGeoSrc = new MaxmindGeoWrapper(entry.src_ip);
                mmGeoSrc.load();

                if (mmGeoSrc.isLoaded())
                {
                    entry.src_city = mmGeoSrc.getCity();
                    entry.src_country = mmGeoSrc.getCountryCode();
                    entry.src_ip_geo = mmGeoSrc.getLatLng();
                }
            }

            if (!Properties.Settings.Default.LocalIpAddress.Contains(entry.dst_ip))
            {
                MaxmindGeoWrapper mmGeoDst = new MaxmindGeoWrapper(entry.dst_ip);
                mmGeoDst.load();

                if (mmGeoDst.isLoaded())
                {
                    entry.dst_city = mmGeoDst.getCity();
                    entry.dst_country = mmGeoDst.getCountryCode();
                    entry.dst_ip_geo = mmGeoDst.getLatLng();
                }
            }

            entry.save();

        }
    }
}
