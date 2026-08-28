using System.Collections.Generic;
using CalculadoraLib;

namespace CalculadoraTests
{
    public class DatabaseRepositorio : IRepositorio
    {
        private readonly List<string> _registros = new List<string>();

        public void Salvar(string dados) => _registros.Add(dados);

        public bool ExisteRegistro(string dados) => _registros.Contains(dados);
    }
}
