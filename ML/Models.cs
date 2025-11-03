using Microsoft.ML.Data;

namespace Mottu.Api.ML;

public class ManutencaoProblema
{
  [LoadColumn(0)]
  public string Problemas { get; set; } = default!;

  [LoadColumn(1)]
  [ColumnName("Label")]
  public string Status { get; set; } = default!;
}

public class StatusPredicao
{
  [ColumnName("PredictedLabel")]
  public string Status { get; set; } = default!;
}
