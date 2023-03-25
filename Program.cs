using System;
using System.Data;
using System.Data.SQLite;
namespace SuperforcastingHelper
{
    class Program
    {
        private static SQLiteConnection sqlite_conn;
        private static void ShowMenu()
        {
            double? brierScore = CalculateBrierScore();
            Console.WriteLine("\nSuperforecaster Console App");
            if (brierScore.HasValue)
            {
                Console.WriteLine($"Current Brier Score: {brierScore.Value:0.000}");
            }
            else
            {
                Console.WriteLine("Current Brier Score: N/A (no predictions with outcomes)");
            }
            Console.WriteLine("1. Add a prediction");
            Console.WriteLine("2. Update a prediction");
            Console.WriteLine("3. View predictions");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");
        }

        static void Main(string[] args)
        {
            sqlite_conn = CreateConnection();
            CreateTable();

            while (true)
            {
                ShowMenu();
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        AddPrediction();
                        break;
                    case 2:
                        UpdatePrediction();
                        break;
                    case 3:
                        ViewPredictions();
                        break;
                    case 4:
                        sqlite_conn.Close();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = new SQLiteConnection("Data Source=predictions.db; Version=3; New=True; Compress=True;");
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return sqlite_conn;
        }

        private static void CreateTable()
        {
            SQLiteCommand sqlite_cmd;
            string createTableQuery = "CREATE TABLE IF NOT EXISTS predictions (id INTEGER PRIMARY KEY AUTOINCREMENT, description TEXT NOT NULL, probability REAL NOT NULL, date TEXT NOT NULL, notes TEXT, created_at TEXT NOT NULL, outcome INTEGER);";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = createTableQuery;
            sqlite_cmd.ExecuteNonQuery();
        }



        private static void AddPrediction()
        {
            Console.Write("Enter prediction description: ");
            string description = Console.ReadLine();

            Console.Write("Enter probability (0-1): ");
            double probability = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter prediction date (yyyy-mm-dd): ");
            string date = Console.ReadLine();

            Console.Write("Enter any additional notes: ");
            string notes = Console.ReadLine();

            Console.Write("Enter the outcome of the prediction (1 if occurred, 0 if not, leave empty if unknown): ");
            string outcomeInput = Console.ReadLine();
            int? outcome = null;
            if (!string.IsNullOrWhiteSpace(outcomeInput))
            {
                outcome = Convert.ToInt32(outcomeInput);
            }

            DateTime createdAt = DateTime.Now;
            string createdAtString = createdAt.ToString("yyyy-MM-dd HH:mm:ss");

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO predictions (description, probability, date, notes, created_at) VALUES (@description, @probability, @date, @notes, @created_at);";
            sqlite_cmd.Parameters.AddWithValue("@description", description);
            sqlite_cmd.Parameters.AddWithValue("@probability", probability);
            sqlite_cmd.Parameters.AddWithValue("@date", date);
            sqlite_cmd.Parameters.AddWithValue("@notes", notes);
            sqlite_cmd.Parameters.AddWithValue("@outcome", outcome.HasValue ? (object)outcome.Value : DBNull.Value);
            sqlite_cmd.Parameters.AddWithValue("@created_at", createdAtString);
            sqlite_cmd.ExecuteNonQuery();

            Console.WriteLine("\nPrediction added successfully!");
        }


        private static void UpdatePrediction()
        {
            Console.Write("Enter the ID of the prediction you want to update: ");
            int id = Convert.ToInt32(Console.ReadLine());

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM predictions WHERE id = @id;";
            sqlite_cmd.Parameters.AddWithValue("@id", id);

            SQLiteDataReader sqlite_datareader;
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            if (sqlite_datareader.Read())
            {
                string existingDescription = sqlite_datareader.GetString(1);
                double existingProbability = sqlite_datareader.GetDouble(2);
                string existingDate = sqlite_datareader.GetString(3);
                string existingNotes = sqlite_datareader.GetString(4);
                int? existingOutcome = sqlite_datareader.IsDBNull(6) ? (int?)null : sqlite_datareader.GetInt32(6);


                Console.Write($"Update prediction description (current: {existingDescription}): ");
                string description = Console.ReadLine();
                description = string.IsNullOrWhiteSpace(description) ? existingDescription : description;

                Console.Write($"Update probability (0-1) (current: {existingProbability}): ");
                string probabilityInput = Console.ReadLine();
                double probability = string.IsNullOrWhiteSpace(probabilityInput) ? existingProbability : Convert.ToDouble(probabilityInput);

                Console.Write($"Update prediction date (yyyy-mm-dd) (current: {existingDate}): ");
                string date = Console.ReadLine();
                date = string.IsNullOrWhiteSpace(date) ? existingDate : date;

                Console.Write($"Update any additional notes (current: {existingNotes}): ");
                string notes = Console.ReadLine();
                notes = string.IsNullOrWhiteSpace(notes) ? existingNotes : notes;

                Console.Write($"Update the outcome of the prediction (current: {(existingOutcome.HasValue ? existingOutcome.ToString() : "unknown")}, 1 if occurred, 0 if not, leave empty if unknown): ");
                string outcomeInput = Console.ReadLine();
                int? outcome = null;
                if (!string.IsNullOrWhiteSpace(outcomeInput))
                {
                    outcome = Convert.ToInt32(outcomeInput);
                }
                else
                {
                    outcome = existingOutcome;
                }

                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = "UPDATE predictions SET description = @description, probability = @probability, date = @date, notes = @notes WHERE id = @id;";
                sqlite_cmd.Parameters.AddWithValue("@id", id);
                sqlite_cmd.Parameters.AddWithValue("@description", description);
                sqlite_cmd.Parameters.AddWithValue("@probability", probability);
                sqlite_cmd.Parameters.AddWithValue("@date", date);
                sqlite_cmd.Parameters.AddWithValue("@outcome", outcome.HasValue ? (object)outcome.Value : DBNull.Value);
                sqlite_cmd.Parameters.AddWithValue("@notes", notes);
                int rowsAffected = sqlite_cmd.ExecuteNonQuery();

                if (rowsAffected == 1)
                {
                    Console.WriteLine("\nPrediction updated successfully!");
                }
                else
                {
                    Console.WriteLine("\nError: Prediction not found.");
                }
            }
            else
            {
                Console.WriteLine("\nError: Prediction not found.");
            }
        }


        private static void ViewPredictions()
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM predictions;";

            SQLiteDataReader sqlite_datareader;
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            Console.WriteLine("\nID | Description | Probability | Prediction Date | Notes | Prediction Made At");
            Console.WriteLine("---------------------------------------------------------------------------");

            while (sqlite_datareader.Read())
            {
                Console.WriteLine($"{sqlite_datareader.GetInt32(0)} | {sqlite_datareader.GetString(1)} | {sqlite_datareader.GetDouble(2):0.00} | {sqlite_datareader.GetString(3)} | {sqlite_datareader.GetString(4)} | {sqlite_datareader.GetString(5)}");
            }
        }

        private static double? CalculateBrierScore()
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT probability, outcome FROM predictions WHERE outcome IS NOT NULL;";

            SQLiteDataReader sqlite_datareader;
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            int predictionCount = 0;
            double brierScoreSum = 0;

            while (sqlite_datareader.Read())
            {
                double probability = sqlite_datareader.GetDouble(0);

                int outcome = sqlite_datareader.GetInt32(1);
                double squaredError = Math.Pow(probability - outcome, 2);
                brierScoreSum += squaredError;
                predictionCount++;
            }
            if (predictionCount > 0)
            {
                return brierScoreSum / predictionCount;
            }
            else
            {
                return null;
            }
        }
        }
    }