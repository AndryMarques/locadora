using System.ComponentModel.DataAnnotations;

namespace locadora.Models
{ 
    public class Fabricante
    {
        [Key]
        public int IdFabricante { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        public ICollection<Veiculo> Veiculos { get; set; }
    }
}
