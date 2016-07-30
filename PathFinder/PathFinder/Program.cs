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
            using (var conn = new NpgsqlConnection("Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase"))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    // Insert some data
                    cmd.CommandText = "INSERT INTO data (some_field) VALUES ('Hello world')";
                    cmd.ExecuteNonQuery();

                    // Retrieve all rows
                    cmd.CommandText = "SELECT some_field FROM data";
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
