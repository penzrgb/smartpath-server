using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Drawing;
namespace PathFinder
{
    class Program
    {
        static void SendQuery(NpgsqlConnection con, string query)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;

                // Insert some data
                //  cmd.CommandText = "INSERT INTO data (some_field) VALUES ('Hello world')";
                // cmd.ExecuteNonQuery();

                // Retrieve all rows
                Console.WriteLine("Sending Query '" + query + "'");
                cmd.CommandText = query;
                List<PointF> LightPositions = new List<PointF>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PointF LightPos = new PointF();
                        LightPos.X = (float)(double)reader.GetValue(4);
                        LightPos.Y = (float)(double)reader.GetValue(5);
                        //Console.WriteLine(reader.GetString(0) + " | " + reader.GetString(1) + " | " + LightPos.X  + " | " + LightPos.Y);
                        LightPositions.Add(LightPos);
                    }
                    foreach (PointF pnt in LightPositions)
                    {
                        Console.WriteLine(pnt);
                    }
                }
            }
        }
        

        static void Main(string[] args)
        {
            Configuration Config = new Configuration();
            Config.Initialize();
            Console.WriteLine("~Govhack Smartpath Server~\nConnecting to DBhost: " + Configuration.HostIP + "\nDBUsername: " + Configuration.HostUser + "\nDBPassword: " + Configuration.HostPass + "\nDBDatabase: " + Configuration.HostDBName);
            string[] prefixes = new string[1];
             prefixes[0] = "http://localhost:8082/index/";
             ServerListener.SimpleListenerExample(prefixes);
          
        }
    }
}
