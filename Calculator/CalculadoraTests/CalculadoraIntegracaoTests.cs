using Xunit;
using Xunit.Abstractions;
using Moq;
using CalculadoraLib;

namespace CalculadoraTests
{
    public class CalculadoraIntegracaoTests
    {
        // Mocks das dependências
        private readonly Mock<ILogger> _mockLogger = new();
        private readonly Mock<IRepositorio> _mockRepositorio = new();
        private readonly Calculadora _calculadora;
        private readonly ITestOutputHelper _output;

        // Arrange: Configuração inicial (executada antes de cada teste)
        public CalculadoraIntegracaoTests(ITestOutputHelper output)
        {
            _output = output;
            _calculadora = new Calculadora(_mockLogger.Object, _mockRepositorio.Object);
        }

        // Teste de integração para o método Somar
        [Fact]
        public void Somar_DeveChamarLoggerERepositorio()
        {
            // Act
            int resultado = _calculadora.Somar(2, 3);

            // Assert (verifica o resultado e as interações)
            Assert.Equal(5, resultado);

            _mockLogger.Verify(
                l => l.Registrar("Soma: 2 + 3 = 5"),
                Times.Once
            );
            _output.WriteLine("LOG: chamada ao Logger verificada com sucesso.");

            _mockRepositorio.Verify(
                r => r.Salvar("Soma: 5"),
                Times.Once
            );
            _output.WriteLine("REPOSITÓRIO: gravação verificada com sucesso.");
        }

        // Teste de integração para o método Subtrair
        [Fact]
        public void Subtrair_DeveChamarLoggerERepositorio()
        {
            // Act
            int resultado = _calculadora.Subtrair(5, 3);

            // Assert
            Assert.Equal(2, resultado);

            _mockLogger.Verify(
                l => l.Registrar("Subtracao: 5 - 3 = 2"),
                Times.Once
            );
            _output.WriteLine("LOG: chamada ao Logger verificada com sucesso.");

            _mockRepositorio.Verify(
                r => r.Salvar("Subtracao: 2"),
                Times.Once
            );
            _output.WriteLine("REPOSITÓRIO: gravação verificada com sucesso.");
        }

        // Teste de integração para o método Multiplicar
        [Fact]
        public void Multiplicar_DeveChamarLoggerERepositorio()
        {
            // Act
            int resultado = _calculadora.Multiplicar(4, 3);

            // Assert
            Assert.Equal(12, resultado);

            _mockLogger.Verify(
                l => l.Registrar("Multiplicacao: 4 * 3 = 12"),
                Times.Once
            );
            _output.WriteLine("LOG: chamada ao Logger verificada com sucesso.");

            _mockRepositorio.Verify(
                r => r.Salvar("Multiplicacao: 12"),
                Times.Once
            );
            _output.WriteLine("REPOSITÓRIO: gravação verificada com sucesso.");
        }

        // Teste de integração para o método Dividir
        [Fact]
        public void Dividir_DeveChamarLoggerERepositorio()
        {
            // Act
            double resultado = _calculadora.Dividir(10, 2);

            // Assert
            Assert.Equal(5, resultado);

            _mockLogger.Verify(
                l => l.Registrar("Divisao: 10 / 2 = 5"),
                Times.Once
            );
            _output.WriteLine("LOG: chamada ao Logger verificada com sucesso.");

            _mockRepositorio.Verify(
                r => r.Salvar("Divisao: 5"),
                Times.Once
            );
            _output.WriteLine("REPOSITÓRIO: gravação verificada com sucesso.");
        }
    }
}
