namespace locadora.DTOs
{
    public class CreateAluguel
    {
        public DateTime DataIni { get; set; }
        public DateTime DataFimPrev { get; set; }
        public double KmIni { get; set; }
        public decimal ValorDia { get; set; }
        public int IdCliente { get; set; }
        public int IdVeiculo { get; set; }
        public int IdFuncionario { get; set; }
    }
}
