using System;
using CalculadoraLib;

namespace CalculadoraTests
{
    public class DatabaseRepositorioSimuladoComFalha : IRepositorio
    {
        public void Salvar(string dados)
        {
            throw new Exception("Falha ao salvar no repositório (simulado para teste)");
        }

        public bool ExisteRegistro(string dados)
        {
            return false;
        }
    }
}
