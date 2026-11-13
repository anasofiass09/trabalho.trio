using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaagendamento.Models
{
    [Table("Servico")]
    public class Servico
    {
        [Key]
     
        public int Codigo { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        public int DuracaoMinutos { get; set; }

        public bool Ativo { get; set; } = true;

        // Chave Estrangeira Categoria
        [Column("CodigoCategoria")]
        public int CodigoCategoria { get; set; }

        [ForeignKey("CodigoCategoria")]
        public Categoria? Categoria { get; set; }

        // Chave Estrangeira Prestador
        [Column("CodigoPrestador")]
        public int CodigoPrestador { get; set; }

        [ForeignKey("CodigoPrestador")]
        public Prestador? Prestador { get; set; }
    }
}