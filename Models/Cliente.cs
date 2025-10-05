using System.ComponentModel.DataAnnotations;

namespace locadora.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(11)]
        public string CPF { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        public string Telefone { get; set; }

        public ICollection<Aluguel> Alugueis { get; set; }
    }
}
