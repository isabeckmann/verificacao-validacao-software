namespace CalculadoraLib
{
    public class Calculadora
    {
        public int Somar(int a, int b) => a + b;

        public int Subtrair(int a, int b) => a - b;
        public bool EhPar(int numero) => numero % 2 == 0;
    }
}