using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace EarthquakeMagnitudePredictor
{
    struct Earthquake
    {
        public float latitude;
        public float longitude;
        public float depth;
        public float magnitude;
        public Earthquake(float la, float lo, float d, float mag)
        {
            latitude = la;
            longitude = lo;
            depth = d;
            magnitude = mag;
        }
    }

    class Program
    {
        [DllImport(@"C:\Users\Aryan\Documents\Visual Studio 2017\Projects\EarthquakeMagnitudePredictor\Debug\LinearNeuron.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void InitializeBot(short numOfInputs);
        [DllImport(@"C:\Users\Aryan\Documents\Visual Studio 2017\Projects\EarthquakeMagnitudePredictor\Debug\LinearNeuron.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void BatchTraining(float[] input, int batchSize, float[] expected, short epoch);
        [DllImport(@"C:\Users\Aryan\Documents\Visual Studio 2017\Projects\EarthquakeMagnitudePredictor\Debug\LinearNeuron.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern float ExampleTraining(float[] input, float expected);
        [DllImport(@"C:\Users\Aryan\Documents\Visual Studio 2017\Projects\EarthquakeMagnitudePredictor\Debug\LinearNeuron.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern float GetOutput(float[] input);

        static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Aryan\\Documents\\Visual Studio 2017\\Projects\\EarthquakeMagnitudePredictor\\DatabaseManager\\EarthquakeDatabase.mdf\";Integrated Security=True";

        static List<Earthquake> earthquakes = new List<Earthquake>();
        static float maxLatitude;
        static float maxLongitude;
        static float maxDepth;
        static float maxMagnitude;

        static void Main(string[] args)
        {
            //Loading data
            LoadData();

            //Initializing the bot
            InitializeBot(3);

            //Training the bot
            Train();

            //Testing the bot
            Test();

            Console.ReadKey();
        }

        private static void LoadData()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand("SELECT * FROM EarthquakesDatabase", connection);
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Earthquake eq = new Earthquake();
                eq.latitude = (float)Convert.ToDouble(reader["Latitude"]);
                eq.longitude = (float)Convert.ToDouble(reader["Longitude"]);
                eq.depth = (float)Convert.ToDouble(reader["Depth"]);
                eq.magnitude = (float)Convert.ToDouble(reader["Magnitude"]);

                earthquakes.Add(eq);
            }
            reader.Close();

            command.CommandText = "SELECT MAX (Latitude) FROM EarthquakesDatabase";
            maxLatitude = (float)Convert.ToDouble(command.ExecuteScalar());
            command.CommandText = "SELECT MAX (Longitude) FROM EarthquakesDatabase";
            maxLongitude = (float)Convert.ToDouble(command.ExecuteScalar());
            command.CommandText = "SELECT MAX (Depth) FROM EarthquakesDatabase";
            maxDepth = (float)Convert.ToDouble(command.ExecuteScalar());
            command.CommandText = "SELECT MAX (Magnitude) FROM EarthquakesDatabase";
            maxMagnitude = (float)Convert.ToDouble(command.ExecuteScalar());

            command.Dispose();
            connection.Close();
        }

        private static void Train()
        {
            const int BATCH_SIZE = 100;
            const short INPUT_CTR = 3;

            float[] inputs = new float[BATCH_SIZE * INPUT_CTR];
            float[] expectedOutputs = new float[BATCH_SIZE];
            
            //Batch learning
            for(int a = 0, b = 0; a < BATCH_SIZE; ++a, ++b)
            {
                Earthquake eq = earthquakes[a];
                inputs[b] = eq.latitude / maxLatitude;
                inputs[++b] = eq.longitude / maxLongitude;
                inputs[++b] = eq.depth / maxDepth;

                expectedOutputs[a] = eq.magnitude / maxMagnitude;
            }

            BatchTraining(inputs, BATCH_SIZE, expectedOutputs, 20);
        }


        private static void Test()
        {
            Random rand = new Random();
            for (int a = 0; a < 100; ++a)
            {
                Earthquake eq = earthquakes[rand.Next(earthquakes.Count + 1)];
                float[] inputs = { eq.latitude / maxLatitude, eq.longitude / maxLongitude, eq.depth / maxDepth };

                float guess = GetOutput(inputs) * maxMagnitude;
                ExampleTraining(inputs, eq.magnitude / maxMagnitude);

                string msg = "Latidue: " + eq.latitude + ", Longitude: " +
                eq.longitude + ", Depth: " + eq.depth + ", Actual Magnitude : " + eq.magnitude + ", Guess: " + guess;
                Console.WriteLine(msg);
            }
        }
    }
}

