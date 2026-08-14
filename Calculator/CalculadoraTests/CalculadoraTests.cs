using Xunit;
using CalculadoraLib;

namespace CalculadoraTests
{
    public class CalculadoraTests
    {
        private readonly Calculadora _calc;

        public CalculadoraTests()
        {
            _calc = new Calculadora(null, null);
        }

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
        public void Subtrair_DoisNumeros_RetornaDiferencaCorreta(int a, int b, int esperado)
        {
            // Act
            int resultado = _calc.Subtrair(a, b);
            // Assert
            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData(3, 4, 12)]
        [InlineData(5, 0, 0)]
        public void Multiplicar_DoisNumeros_RetornaProdutoCorreto(int a, int b, int esperado)
        {
            // Act
            int resultado = _calc.Multiplicar(a, b);
            // Assert
            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData(10, 2, 5)]
        [InlineData(9, 2, 4.5)]
        public void Dividir_DoisNumeros_RetornaQuocienteCorreto(int a, int b, double esperado)
        {
            // Act
            double resultado = _calc.Dividir(a, b);
            // Assert
            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void Dividir_PorZero_LancaExcecao()
        {
            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => _calc.Dividir(10, 0));
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
