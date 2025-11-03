using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Mottu.Api.ML;
using Swashbuckle.AspNetCore.Annotations;

namespace Mottu.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ml")]
public class MLController : ControllerBase
{
  private readonly PredictionEngine<ManutencaoProblema, StatusPredicao> _predictionEngine;

  public MLController(PredictionEngine<ManutencaoProblema, StatusPredicao> predictionEngine)
  {
    _predictionEngine = predictionEngine;
  }

  [HttpPost("predict-status")]
  [SwaggerOperation(Summary = "Prever Status da Manutenção", Description = "Usa um modelo de ML para prever se uma manutenção será 'Aberta' ou 'Concluida' com base na descrição do problema.")]
  [ProducesResponseType(typeof(StatusPredicao), StatusCodes.Status200OK)]
  public IActionResult PredictStatus([FromBody] ManutencaoProblema input)
  {
    var prediction = _predictionEngine.Predict(input);
    return Ok(prediction);
  }
}
