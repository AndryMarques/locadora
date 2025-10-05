using locadora.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace locadora.Models
{
    public class Aluguel
    {
        [Key]
        public int IdAluguel { get; set; }

        public DateTime DataIni { get; set; }

        public DateTime DataFimPrev { get; set; }

        public DateTime? DataFimReal { get; set; }

        public double KmIni { get; set; }

        public double? KmFim { get; set; }

        public decimal ValorDia { get; set; }

        public decimal? ValorFim { get; set; }

        public int IdCliente { get; set; }
        public int IdVeiculo { get; set; }
        public int IdFuncionario { get; set; }

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }

        [ForeignKey("IdVeiculo")]
        public Veiculo Veiculo { get; set; }

        [ForeignKey("IdFuncionario")]
        public Funcionario Funcionario { get; set; }
    }
}
