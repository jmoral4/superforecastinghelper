using SuperforcastingHelper;
using System;
using System.Collections.Generic;

public class PredictionApp
{
    private readonly PredictionRepository predictionRepository;

    public PredictionApp(PredictionRepository repository)
    {
        predictionRepository = repository;
    }

    public void Run()
    {
        int choice;
        do
        {
            ShowMenu();
            choice = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

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
                    Console.WriteLine("Exiting...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        } while (choice != 4);
    }

    private void ShowMenu()
    {
        double? brierScore = predictionRepository.CalculateBrierScore();
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

    private void AddPrediction()
    {
        Console.Write("Enter the description of the prediction: ");
        string description = Console.ReadLine();

        Console.Write("Enter the probability of the prediction (0 to 1): ");
        double probability = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter the date of the prediction (yyyy-mm-dd): ");
        string date = Console.ReadLine();

        Console.Write("Enter any notes related to the prediction: ");
        string notes = Console.ReadLine();

        Console.Write("Enter the outcome of the prediction (1 if occurred, 0 if not, leave empty if unknown): ");
        string outcomeInput = Console.ReadLine();
        int? outcome = null;
        if (!string.IsNullOrWhiteSpace(outcomeInput))
        {
            outcome = Convert.ToInt32(outcomeInput);
        }

        Prediction prediction = new Prediction
        {
            Description = description,
            Probability = probability,
            Date = date,
            Notes = notes,
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Outcome = outcome
        };

        predictionRepository.AddPrediction(prediction);
        Console.WriteLine("Prediction added successfully.");
    }

    private void UpdatePrediction()
    {
        Console.Write("Enter the ID of the prediction to update: ");
        int id = Convert.ToInt32(Console.ReadLine());

        Prediction existingPrediction = predictionRepository.GetPredictionById(id);

        if (existingPrediction == null)
        {
            Console.WriteLine("Prediction not found.");
            return;
        }

        Console.WriteLine($"Updating prediction with ID {existingPrediction.Id}:");
        Console.WriteLine($"Description: {existingPrediction.Description}");
        Console.WriteLine($"Probability: {existingPrediction.Probability}");
        Console.WriteLine($"Date: {existingPrediction.Date}");
        Console.WriteLine($"Notes: {existingPrediction.Notes}");
        Console.WriteLine($"Outcome: {(existingPrediction.Outcome.HasValue ? existingPrediction.Outcome.ToString() : "unknown")}");

        Console.Write("Update the description of the prediction (leave empty to keep current): ");
        string description = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(description))
        {
            description = existingPrediction.Description;
        }

        Console.Write("Update the probability of the prediction (leave empty to keep current): ");
        string probabilityInput =
                Console.ReadLine();
        double probability;
        if (string.IsNullOrWhiteSpace(probabilityInput))
        {
            probability = existingPrediction.Probability;
        }
        else
        {
            probability = Convert.ToDouble(probabilityInput);
        }

        Console.Write("Update the date of the prediction (leave empty to keep current): ");
        string date = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(date))
        {
            date = existingPrediction.Date;
        }

        Console.Write("Update any notes related to the prediction (leave empty to keep current): ");
        string notes = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(notes))
        {
            notes = existingPrediction.Notes;
        }

        Console.Write("Update the outcome of the prediction (1 if occurred, 0 if not, leave empty if unknown): ");
        string outcomeInput = Console.ReadLine();
        int? outcome = null;
        if (!string.IsNullOrWhiteSpace(outcomeInput))
        {
            outcome = Convert.ToInt32(outcomeInput);
        }
        else
        {
            outcome = existingPrediction.Outcome;
        }

        Prediction updatedPrediction = new Prediction
        {
            Id = existingPrediction.Id,
            Description = description,
            Probability = probability,
            Date = date,
            Notes = notes,
            CreatedAt = existingPrediction.CreatedAt,
            Outcome = outcome
        };

        predictionRepository.UpdatePrediction(updatedPrediction);
        Console.WriteLine("Prediction updated successfully.");
    }

    private void ViewPredictions()
    {
        List<Prediction> predictions = predictionRepository.GetAllPredictions();

        if (predictions.Count == 0)
        {
            Console.WriteLine("No predictions found.");
        }
        else
        {
            Console.WriteLine("Predictions:");
            Console.WriteLine("ID\tDescription\t\tProbability\tDate\t\tNotes\t\tOutcome");
            Console.WriteLine("--------------------------------------------------------------------------------------");

            foreach (Prediction prediction in predictions)
            {
                Console.WriteLine($"{prediction.Id}\t{prediction.Description}\t{prediction.Probability:0.00}\t{prediction.Date}\t{prediction.Notes}\t{(prediction.Outcome.HasValue ? prediction.Outcome.ToString() : "unknown")}");
            }
        }
    }
}