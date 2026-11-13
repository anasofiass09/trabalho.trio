using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaagendamento.Models
{
    public class Agendamento
    {
        [Key]
        [Column("Codigo")] // Associa a propriedade Codigo à coluna Codigo do MySQL
        public int Codigo { get; set; }

        [Required]
        public DateTime DataHora { get; set; }

        public string Situacao { get; set; } = "Pendente"; // Pendente, Confirmado, Cancelado, Concluído

        // Chave Estrangeira - Cliente
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        // Chave Estrangeira - Servico
        public int ServicoId { get; set; }
        public Servico? Servico { get; set; }
    }
}