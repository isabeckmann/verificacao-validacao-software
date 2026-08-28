using System;
using System.Diagnostics;
using System.IO;
using Xunit;
using CalculadoraLib;

namespace CalculadoraTests
{
    [Trait("Category", "System")]
    public class CalculadoraSystemTests
    {
        private readonly string _logSistemaPath = "logs/log_sistema.txt";
        private readonly string _logErroPath = "logs/log_erro.txt";

        public CalculadoraSystemTests()
        {
            // Garante que a pasta logs existe
            Directory.CreateDirectory("logs");
        }

        [Fact]
        public void Somar_DeveCalcularLogarEPersistir()
        {
            // Arrange
            if (File.Exists(_logSistemaPath)) File.Delete(_logSistemaPath);

            var logger = new FileLogger(_logSistemaPath);
            var repositorio = new DatabaseRepositorio();
            var calculadora = new Calculadora(logger, repositorio);

            // Act
            int resultado = calculadora.Somar(5, 3);

            // Assert
            Assert.Equal(8, resultado);

            var logContent = File.ReadAllText(_logSistemaPath);
            Assert.Contains("5 + 3 = 8", logContent);

            Assert.True(repositorio.ExisteRegistro("Soma: 8"));

            // Log adicional para debug
            Console.WriteLine($"Log gerado em: {Path.GetFullPath(_logSistemaPath)}");
            Console.WriteLine($"Conteúdo do log: {logContent}");
        }

        [Fact]
        public void Somar_DeveLogarErroSeRepositorioFalhar()
        {
            // Arrange
            if (File.Exists(_logErroPath)) File.Delete(_logErroPath);

            var logger = new FileLogger(_logErroPath);
            var repositorio = new DatabaseRepositorioSimuladoComFalha();
            var calculadora = new Calculadora(logger, repositorio);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => calculadora.Somar(2, 2));
            Assert.Contains("Falha ao salvar", exception.Message);

            var errorLogContent = File.ReadAllText(_logErroPath);
            Assert.Contains("Erro", errorLogContent);

            // Log adicional para debug
            Console.WriteLine($"Log de erro gerado em: {Path.GetFullPath(_logErroPath)}");
            Console.WriteLine($"Conteúdo do erro: {errorLogContent}");
        }

        [Fact]
        public void Somar_DeveExecutarEmMenosDe100ms()
        {
            // Arrange
            var logger = new FileLogger(Path.GetTempFileName());
            var repositorio = new DatabaseRepositorio();
            var calculadora = new Calculadora(logger, repositorio);

            // Act
            var watch = Stopwatch.StartNew();
            calculadora.Somar(5, 3);
            watch.Stop();
            var tempo = watch.ElapsedMilliseconds;

            // Assert
            Assert.True(tempo < 100, $"Operação levou {tempo}ms (limite: 100ms)");

            // Output adicional
            Console.WriteLine($"\nRESULTADO PERFORMANCE:");
            Console.WriteLine($"- Tempo total: {tempo}ms");
            Console.WriteLine($"- Limite máximo: 100ms");
            Console.WriteLine($"- Status: {(tempo < 100 ? "APROVADO" : "REPROVADO")}\n");
        }

        [Fact]
        public void Log_NaoDeveConterInformacoesSensiveis()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var logger = new FileLogger(tempFile);
            var repositorio = new DatabaseRepositorio();
            var calculadora = new Calculadora(logger, repositorio);

            // Act
            calculadora.Somar(999999, 888888);
            var logContent = File.ReadAllText(tempFile);

            // Evidência do teste
            Console.WriteLine("\nTESTE DE SEGURANÇA");
            Console.WriteLine("Regra: valores sensíveis não devem aparecer no log.");
            Console.WriteLine("Esperado: ****** + ****** = 1888887");
            Console.WriteLine($"Obtido: {logContent.Trim()}");

            bool contemDadosSensiveis =
                logContent.Contains("999999") ||
                logContent.Contains("888888");

            Console.WriteLine(
                $"Status: {(contemDadosSensiveis ? "REPROVADO - DADOS SENSÍVEIS EXPOSTOS" : "APROVADO")}");

            // Assert
            Assert.DoesNotContain("999999", logContent);
            Assert.DoesNotContain("888888", logContent);
            Assert.Contains("****** + ****** = 1888887", logContent);
        }
    }
}
