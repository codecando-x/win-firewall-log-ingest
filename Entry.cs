using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewallLogIngestService
{
    [Table(Name = "entries")]
    internal class Entry
    {
        [Column]
        public DateTime date; //date

        [Column]
        public TimeSpan time; //time 

        [Column]
        public string action; //char

        [Column]
        public string protocol; //char

        [Column]
        public string src_ip; //text

        [Column]
        public string dst_ip; //text

        [Column]
        public int src_port; //int

        [Column]
        public int dst_port; //int

        [Column]
        public int size; //int

        [Column]
        public string tcp_flags; //char

        [Column]
        public int tcp_syn; //int

        [Column]
        public int tcp_ack; //int

        [Column]
        public int tcp_win; //int

        [Column]
        public string icmp_type; //text

        [Column]
        public string icmp_code; //text

        [Column]
        public string info; //text

        [Column]
        public string path; //text

        [Column]
        public int pid; //int

        [Column]
        public string src_ip_geo; //string

        [Column]
        public string dst_ip_geo; //string

        [Column]
        public DateTime last_updated; //date

        [Column]
        public string id; //string

        [Column]
        public string src_city; //string

        [Column]
        public string src_country; //string

        [Column]
        public string dst_city; //string

        [Column]
        public string dst_country; //string

        public void save()
        {
            pfirewallDataSetTableAdapters.entriesTableAdapter eta = new pfirewallDataSetTableAdapters.entriesTableAdapter();

            if (recordExists(id, eta.Connection) == false)
            {
                eta.Insert(date,
                    time,
                    action,
                    protocol,
                    src_ip,
                    dst_ip,
                    src_port,
                    dst_port,
                    size,
                    tcp_flags,
                    tcp_syn,
                    tcp_ack,
                    tcp_win,
                    icmp_type,
                    icmp_code,
                    info,
                    path,
                    pid,
                    src_ip_geo,
                    dst_ip_geo,
                    last_updated,
                    id,
                    src_city,
                    src_country,
                    dst_city,
                    dst_country);
            }
        }

        private bool recordExists(string id, SqlConnection conn)
        {
            conn.Open();

            SqlCommand command = new SqlCommand("SELECT COUNT(id) AS haveIt FROM entries where id=@id", conn);
            command.Parameters.AddWithValue("@id", id);
            // int result = command.ExecuteNonQuery();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    //Console.WriteLine(String.Format("{0}", reader["haveIt"]));

                    int haveIt = int.Parse(reader["haveIt"].ToString());

                    if (haveIt > 0)
                    {
                        conn.Close();
                        return true;
                    }
                }
            }
            conn.Close();

            Console.WriteLine("WILL INSERT");
            return false;
        }

    }
}
