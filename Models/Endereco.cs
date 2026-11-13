using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaagendamento.Models
{
    public class Endereco
    {
        [Key]
        [Column("Codigo")] // Associa a propriedade Codigo à coluna Codigo do MySQL
        public int Codigo { get; set; }

        [Required]
        public string Logradouro { get; set; } = string.Empty;

        public string Numero { get; set; } = string.Empty;

        public string Bairro { get; set; } = string.Empty;

        public string Cidade { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string CEP { get; set; } = string.Empty;

        // Pode ser associado ao Prestador ou Cliente
        public int? PrestadorId { get; set; }
        public Prestador? Prestador { get; set; }
    }
}