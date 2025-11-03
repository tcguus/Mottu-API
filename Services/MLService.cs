using Microsoft.ML;
using Mottu.Api.ML;

namespace Mottu.Api.Services;

public static class MLService
{
    public static PredictionEngine<ManutencaoProblema, StatusPredicao>? CreatePredictionEngine()
    {
        try
        {
            var mlContext = new MLContext(seed: 0);

            var trainingData = new[]
            {
                new ManutencaoProblema { Problemas = "troca de oleo", Status = "Concluida" },
                new ManutencaoProblema { Problemas = "revisao geral", Status = "Concluida" },
                new ManutencaoProblema { Problemas = "pneu furado", Status = "Concluida" },
                new ManutencaoProblema { Problemas = "motor falhando", Status = "Aberta" },
                new ManutencaoProblema { Problemas = "problema eletrico", Status = "Aberta" },
                new ManutencaoProblema { Problemas = "barulho estranho no freio", Status = "Aberta" },
                new ManutencaoProblema { Problemas = "ajuste de corrente", Status = "Concluida" },
                new ManutencaoProblema { Problemas = "sistema de injecao com defeito", Status = "Aberta" }
            };
            
            var trainingDataView = mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Status", outputColumnName: "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Problemas", outputColumnName: "ProblemasFeaturized"))
                .Append(mlContext.Transforms.Concatenate("Features", "ProblemasFeaturized"))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(trainingDataView);

            var predictionEngine = mlContext.Model.CreatePredictionEngine<ManutencaoProblema, StatusPredicao>(model);

            return predictionEngine;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MLService] Aviso: Não foi possível criar PredictionEngine: {ex.Message}");
            return null;
        }
    }
}
