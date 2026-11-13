using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaagendamento.Models
{
    public class Avaliacao
    {
        [Key]
      
        [Column("Codigo")] // Associa a propriedade Codigo à coluna Codigo do MySQL
        public int Codigo { get; set; }

        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5.")]
        public int Nota { get; set; }

        public string Comentario { get; set; } = string.Empty;

        public DateTime DataAvaliacao { get; set; } = DateTime.Now;

        // Chave Estrangeira - Agendamento
        public int AgendamentoId { get; set; }
        public Agendamento? Agendamento { get; set; }
    }
}