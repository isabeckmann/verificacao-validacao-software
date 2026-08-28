using System.IO;
using CalculadoraLib;

namespace CalculadoraTests
{
    public class FileLogger : ILogger
    {
        private readonly string _caminho;

        public FileLogger(string caminho) => _caminho = caminho;

        public void Registrar(string mensagem) => File.AppendAllText(_caminho, $"{mensagem}\n");
    }
}
