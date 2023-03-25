using SuperforcastingHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

public class PredictionRepository
{
    private readonly SQLiteConnection sqlite_conn;

    public PredictionRepository(string connectionString)
    {
        sqlite_conn = new SQLiteConnection(connectionString);
        sqlite_conn.Open();
        CreateTable();
    }

    private void CreateTable()
    {
        SQLiteCommand sqlite_cmd;
        string createTableQuery = "CREATE TABLE IF NOT EXISTS predictions (id INTEGER PRIMARY KEY AUTOINCREMENT, description TEXT NOT NULL, probability REAL NOT NULL, date TEXT NOT NULL, notes TEXT, created_at TEXT NOT NULL, outcome INTEGER);";
        sqlite_cmd = sqlite_conn.CreateCommand();
        sqlite_cmd.CommandText = createTableQuery;
        sqlite_cmd.ExecuteNonQuery();
    }

    public void AddPrediction(Prediction prediction)
    {
        SQLiteCommand sqlite_cmd;
        sqlite_cmd = sqlite_conn.CreateCommand();
        sqlite_cmd.CommandText = "INSERT INTO predictions (description, probability, date, notes, created_at, outcome) VALUES (@description, @probability, @date, @notes, @created_at, @outcome);";
        sqlite_cmd.Parameters.AddWithValue("@description", prediction.Description);
        sqlite_cmd.Parameters.AddWithValue("@probability", prediction.Probability);
        sqlite_cmd.Parameters.AddWithValue("@date", prediction.Date);
        sqlite_cmd.Parameters.AddWithValue("@notes", prediction.Notes);
        sqlite_cmd.Parameters.AddWithValue("@created_at", prediction.CreatedAt);
        sqlite_cmd.Parameters.AddWithValue("@outcome", prediction.Outcome.HasValue ? (object)prediction.Outcome.Value : DBNull.Value);
        sqlite_cmd.ExecuteNonQuery();
    }

    public void UpdatePrediction(Prediction prediction)
    {
        SQLiteCommand sqlite_cmd;
        sqlite_cmd = sqlite_conn.CreateCommand();
        sqlite_cmd.CommandText = "UPDATE predictions SET description = @description, probability = @probability, date = @date, notes = @notes, outcome = @outcome WHERE id = @id;";
        sqlite_cmd.Parameters.AddWithValue("@id", prediction.Id.Value);
        sqlite_cmd.Parameters.AddWithValue("@description", prediction.Description);
        sqlite_cmd.Parameters.AddWithValue("@probability", prediction.Probability);
        sqlite_cmd.Parameters.AddWithValue("@date", prediction.Date);
        sqlite_cmd.Parameters.AddWithValue("@notes", prediction.Notes);
        sqlite_cmd.Parameters.AddWithValue("@outcome", prediction.Outcome.HasValue ? (object)prediction.Outcome.Value : DBNull.Value);
        sqlite_cmd.ExecuteNonQuery();
    }

    public Prediction GetPredictionById(int id)
    {
        SQLiteCommand sqlite_cmd;
        sqlite_cmd = sqlite_conn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT * FROM predictions WHERE id = @id;";
        sqlite_cmd.Parameters.AddWithValue("@id", id);

        SQLiteDataReader sqlite_datareader;
        sqlite_datareader = sqlite_cmd.ExecuteReader();

        if (sqlite_datareader.Read())
        {
            return new Prediction
            {
                Id = sqlite_datareader.GetInt32(0),
                Description = sqlite_datareader.GetString(1),
                Probability = sqlite_datareader.GetDouble(2),
                Date = sqlite_datareader.GetString(3),
                Notes = sqlite_datareader.GetString(4),
                CreatedAt = sqlite_datareader.GetString(5),
                Outcome = sqlite_datareader.IsDBNull(6) ? (int?)null : sqlite_datareader.GetInt32(6)
            };
        }
        else
        {
            return null;
        }
    }

    public List<Prediction> GetAllPredictions()
    {
        SQLiteCommand sqlite_cmd;
        sqlite_cmd = sqlite_conn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT * FROM predictions;";

        SQLiteDataReader sqlite_datareader;
        sqlite_datareader = sqlite_cmd.ExecuteReader();

        List<Prediction> predictions = new List<Prediction>();

        while (sqlite_datareader.Read())
        {
            predictions.Add(new Prediction
            {
                Id = sqlite_datareader.GetInt32(0),
                Description = sqlite_datareader.GetString(1),
                Probability = sqlite_datareader.GetDouble(2),
                Date = sqlite_datareader.GetString(3),
                Notes = sqlite_datareader.GetString(4),
                CreatedAt = sqlite_datareader.GetString(5),
                Outcome = sqlite_datareader.IsDBNull(6) ? (int?)null : sqlite_datareader.GetInt32(6)
            });
        }

        return predictions;
    }

    public double? CalculateBrierScore()
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