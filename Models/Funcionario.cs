using System.ComponentModel.DataAnnotations;

namespace locadora.Models
{
    public class Funcionario
    {
        [Key]
        public int IdFuncionario { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        public string Cargo { get; set; }

        public ICollection<Aluguel> Alugueis { get; set; }
    }
}
