namespace CalculadoraLib
{
    public class Calculadora
    {
        private readonly ILogger? _logger;
        private readonly IRepositorio? _repositorio;

        // Injeção de dependências (integração com outros componentes)
        public Calculadora(ILogger? logger, IRepositorio? repositorio)
        {
            _logger = logger;
            _repositorio = repositorio;
        }

        public int Somar(int a, int b)
        {
            int resultado = a + b;
            _logger?.Registrar($"Soma: {a} + {b} = {resultado}");
            _repositorio?.Salvar($"Soma: {resultado}");
            return resultado;
        }

        public int Subtrair(int a, int b)
        {
            int resultado = a - b;
            _logger?.Registrar($"Subtracao: {a} - {b} = {resultado}");
            _repositorio?.Salvar($"Subtracao: {resultado}");
            return resultado;
        }

        public int Multiplicar(int a, int b)
        {
            int resultado = a * b;
            _logger?.Registrar($"Multiplicacao: {a} * {b} = {resultado}");
            _repositorio?.Salvar($"Multiplicacao: {resultado}");
            return resultado;
        }

        public double Dividir(int a, int b)
        {
            if (b == 0)
            {
                throw new DivideByZeroException("Não é possível dividir por zero.");
            }

            double resultado = (double)a / b;
            _logger?.Registrar($"Divisao: {a} / {b} = {resultado}");
            _repositorio?.Salvar($"Divisao: {resultado}");
            return resultado;
        }

        public bool EhPar(int numero)
        {
            return numero % 2 == 0;
        }
    }
}
