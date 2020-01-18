using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;

namespace DatabaseManager
{
    struct EarthQuake
    {
        public float latitude;
        public float longitude;
        public float depth;
        public float magnitude;
        public EarthQuake(float la, float lo, float d, float mag)
        {
            latitude = la;
            longitude = lo;
            depth = d;
            magnitude = mag;
        }
    }

    class Program
    {
        static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Aryan\\Documents\\Visual Studio 2017\\Projects\\EarthquakeMagnitudePredictor\\DatabaseManager\\EarthquakeDatabase.mdf\";Integrated Security=True;Connect Timeout=30";

        static List<EarthQuake> earthQuakes = new List<EarthQuake>();

        static void Main(string[] args)
        {
            string datasetAdd = @"C:\Users\Aryan\Documents\Datasets\earthquake-database\database.csv";

            foreach (string line in File.ReadLines(datasetAdd))
            {
                if(line[0] != 'L')
                    ExtractData(line);
            }

            //Updating the database
            UpdateDatabase();
        }

        static void ExtractData(string line)
        {
            EarthQuake eq = new EarthQuake(0, 0, 0, 0);

            for (int a = 0, commaCtr = 1, lastComma = 0; a < line.Length; ++a)
            {
                if (line[a] == ',')
                {
                    if (commaCtr == 1) //Extracting the latitude
                    {
                        eq.latitude = float.Parse(line.Substring(0, a));
                        ++commaCtr;
                    }
                    else if (commaCtr == 2) //Extracting the longitude
                    {
                        eq.longitude = float.Parse(line.Substring(lastComma + 1, a - lastComma - 1));
                        ++commaCtr;
                    }
                    else if (commaCtr == 3) //Extracting the depth
                    {
                        eq.depth = float.Parse(line.Substring(lastComma + 1, a - lastComma - 1));
                        //Extracting the magnitude
                        eq.magnitude = float.Parse(line.Substring(a + 1, line.Length - a - 1));
                        break;
                    }
                    lastComma = a;
                }
                
            }
            earthQuakes.Add(eq);
        }

        static void UpdateDatabase()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter("", connection);
            SqlCommand command = new SqlCommand("", connection);

            for(int a = 0; a < earthQuakes.Count; ++a)
            {
                EarthQuake eq = earthQuakes[a];

                command.CommandText = "INSERT INTO EarthquakesDatabase(Latitude, Longitude, Depth, Magnitude)" +
                "VALUES (" + eq.latitude + "," + eq.longitude + "," + eq.depth + "," + eq.magnitude + ")";

                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
            }

            command.Dispose();
            adapter.Dispose();
            connection.Close();
        }
    }
}
