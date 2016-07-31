using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft;
using Npgsql;
using System.Drawing;
using System.IO;
using System.Diagnostics;

using System.Xml.Linq;

namespace PathFinder
{
    class ServerListener
    {
        static PointF Position = new PointF(-38.17181325521839f, 144.3160629272461f);
        static PointF Destination = new PointF(-38.1642554683019f, 144.338035583496f);
        static string ToSend = "";
        // This example requires the System and System.Net namespaces.
        public static void SimpleListenerExample(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();
            Console.WriteLine();
            Console.WriteLine("Listening on " + prefixes[0]);

            while (true)
            {
                ListenForRequests(listener);
            }
            listener.Stop();
        }

    
        static void ListenForRequests(HttpListener listener)
        {
            double range = 0.0002 * 50;
            // Note: The GetContext method blocks while waiting for a request. 
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            Uri uri = request.Url;
           string URL = request.RawUrl.ToString();

            string[] slat = URL.Split('/');
            int size = slat.Length;
            double lat1 = 0;
            double lon1 = 0;

            double lat2 = 0;
            double lon2 = 0;

            double.TryParse(slat[size - 2], out lat1);
            double.TryParse(slat[size - 1], out lon1);
            double.TryParse(slat[size - 4], out lat2);
            double.TryParse(slat[size - 3], out lon2);

            

            using (var conn = new NpgsqlConnection("Host=" + Configuration.HostIP + "; Username=" + Configuration.HostUser + ";Password=" + Configuration.HostPass + ";Database=" + Configuration.HostDBName))
            {
                conn.Open();
                Console.WriteLine("Connection Success!\n");
                SendQuery(conn, "Select * FROM Lights AS light WHERE light.latitude > " + lat1.ToString() + " AND light.latitude < " + (lat1 + range).ToString() + " AND light.longitude > " + lon1.ToString() + " AND light.longitude < " + (lon1 + range).ToString(),lat1,lon1,lat2,lon2);
            }
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString = ToSend;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ToSend);
            ToSend = "";
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }
        
        static public void RequestPath(double mlat1, double mlon1, double mlat2, double mlon2, List<PointF> lightdat)
        {
            string RequestString = "http://openls.geog.uni-heidelberg.de/route?start=" + mlon2.ToString() + "," + mlat2.ToString() + "&end=" + mlon1.ToString() + "," + mlat1.ToString() +"&via=";
            int i = 0;
            foreach (PointF ldat in lightdat)
            {
                i++;
                RequestString += ldat.Y.ToString() + "," + ldat.X.ToString();
                if(i <= lightdat.Count - 1)
                {
                    RequestString += " ";
                }
            }
            RequestString += "&lang=en&distunit=KM&routepref=Pedestrian&weighting=Shortest&avoidAreas=&useTMC=false&noMotorways=false&noTollways=false&noUnpavedroads=false&noSteps=false&noFerries=false&instructions=false";

            //string RequestString = "http://openls.geog.uni-heidelberg.de/route?start=9.256506,49.240011&end=9.156506,49.230011&via=&lang=de&distunit=KM&routepref=Pedestrian&weighting=Shortest&avoidAreas=&useTMC=false&noMotorways=false&noTollways=false&noUnpavedroads=false&noSteps=false&noFerries=false&instructions=false";
            Console.WriteLine(RequestString+"\n");
            XDocument doc = XDocument.Load(RequestString);
            ToSend = doc.ToString();
       
        }
        static void SendQuery(NpgsqlConnection con, string query, double mlat1, double mlon1, double mlat2, double mlon2)
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
                    List<PointF> ModifiedLightPositions = new List<PointF>();
                    int interval = (int)Math.Floor(LightPositions.Count() / 20.0);
                    for (int i = 0; i < LightPositions.Count; i++)
                    {
                        if (i % interval == 0) {
                            ModifiedLightPositions.Add(LightPositions[i]);
                        }
                    }
                    RequestPath(mlat1, mlon1, mlat2, mlon2, ModifiedLightPositions);
                    //ToSend = Newtonsoft.Json.JsonConvert.SerializeObject(LightPositions);
                    //Console.WriteLine(ToSend);

                }
            }
        }
    }
}