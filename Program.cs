using System;
using System.Data;
using System.Data.SQLite;
namespace SuperforcastingHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=predictions.db;Version=3;New=True;Compress=True;";
            PredictionRepository predictionRepository = new PredictionRepository(connectionString);
            PredictionApp predictionApp = new PredictionApp(predictionRepository);
            predictionApp.Run();
        }

      
        }
    }