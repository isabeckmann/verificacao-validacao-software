using Xunit;
using CalculadoraLib;
namespace CalculadoraTests
{
    public class CalculadoraTests
    {
        private readonly Calculadora _calc = new();
        [Fact]
        public void Somar_DoisNumeros_RetornaSomaCorreta()
            {
            // Act
            int resultado = _calc.Somar(2, 3);
            // Assert
            Assert.Equal(5, resultado);
            }
            [Theory]
            [InlineData(1, 1, 0)]
            [InlineData(5, 3, 2)]
            public void Subtrair_DoisNumeros_RetornaDiferencaCorreta(int a, int b,
            int esperado)
            {
            // Act
            int resultado = _calc.Subtrair(a, b);
            // Assert
            Assert.Equal(esperado, resultado);
            }
            [Theory]
            [InlineData(2, true)]
            [InlineData(3, false)]
            public void EhPar_Numero_RetornaSeEhPar(int numero, bool esperado)
            {
            // Act & Assert
            Assert.Equal(esperado, _calc.EhPar(numero));
        }
    }
}