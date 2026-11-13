using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaagendamento.Models
{
    [Table("Categoria")]
    public class Categoria
    {
        [Key]
        [Column("Codigo")]
        public int Codigo { get; set; }

        [Required]
        [StringLength(80)]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }
    }
}