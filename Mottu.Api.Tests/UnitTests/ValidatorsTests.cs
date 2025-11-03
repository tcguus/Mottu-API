using Mottu.Api.Utils;
using Xunit;
using Assert = Xunit.Assert;

namespace Mottu.Api.Tests.UnitTests;

public class ValidatorsTests
{
  [Theory]
  [InlineData("ABC-1234", true)]
  [InlineData("XYZ-9876", true)]
  [InlineData("abc-1234", false)]
  [InlineData("ABC1234", false)]
  [InlineData("AB-12345", false)]
  [InlineData(null, false)]
  public void PlacaValida_DeveRetornarResultadoCorreto(string placa, bool expected)
  {
    var result = Validators.PlacaValida(placa);
    Assert.Equal(expected, result);
  }

  [Theory]
  [InlineData(2020)]
  [InlineData(2025)]
  public void AnoValido_ComAnoDentroDoIntervalo_DeveRetornarTrue(int ano)
  {
    var result = Validators.AnoValido(ano);
    Assert.True(result);
  }

  [Theory]
  [InlineData(2019)]
  [InlineData(2026)]
  public void AnoValido_ComAnoForaDoIntervalo_DeveRetornarFalse(int ano)
  {
    var result = Validators.AnoValido(ano);
    Assert.False(result);
  }
}
