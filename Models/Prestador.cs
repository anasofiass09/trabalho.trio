using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaagendamento.Models
{
    public class Prestador
    {
        [Key]
       
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Relacionamentos
        public ICollection<Servico>? Servicos { get; set; }
        public ICollection<Disponibilidade>? Disponibilidades { get; set; }
    }
}