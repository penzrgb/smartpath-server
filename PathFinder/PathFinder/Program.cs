using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace PathFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration Config = new Configuration();
            Config.Initialize();
            using (var conn = new NpgsqlConnection("Host="+ Configuration.HostIP+ "; Username=" + Configuration.HostUser + ";Password=" + Configuration.HostPass + ";Database=" + Configuration.HostDBName))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    // Insert some data
                   //  cmd.CommandText = "INSERT INTO data (some_field) VALUES ('Hello world')";
                    // cmd.ExecuteNonQuery();

                    // Retrieve all rows
                    cmd.CommandText = "select name from australia where place='city'";
                    Console.WriteLine("Connection Success!");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader.GetString(0));
                        }
                    }
                }
            }

            while (true)
            {  }
        }
    }
}
