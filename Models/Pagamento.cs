using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaagendamento.Models
{
    public class Pagamento
    {
        [Key]
        [Column("Codigo")] // Associa a propriedade Codigo à coluna Codigo do MySQL
        public int Codigo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        public string FormaPagamento { get; set; } = string.Empty; // Pix, Cartão, Dinheiro

        public DateTime DataPagamento { get; set; } = DateTime.Now;

        public string Situacao { get; set; } = "Pendente"; // Pago, Cancelado, Pendente

        // Chave Estrangeira - Agendamento
        public int AgendamentoId { get; set; }
        public Agendamento? Agendamento { get; set; }
    }
}