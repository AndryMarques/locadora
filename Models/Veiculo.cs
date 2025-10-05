using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace locadora.Models
{
    public class Veiculo
    {
        [Key]
        public int IdVeiculo { get; set; }

        [Required]
        [StringLength(100)]
        public string Modelo { get; set; }

        public int Ano { get; set; }

        public double KM { get; set; }

        // Chave estrangeira
        public int IdFabricante { get; set; }

        // Propriedade de navegação
        [ForeignKey("IdFabricante")]
        public Fabricante Fabricante { get; set; }

        public ICollection<Aluguel> Alugueis { get; set; }
    }
}