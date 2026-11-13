using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaagendamento.Models
{
    public class Disponibilidade
    {
        [Key]
        [Column("Codigo")] // Associa a propriedade Codigo à coluna Codigo do MySQL
        public int Codigo { get; set; }

        [Required]
        public string DiaSemana { get; set; } = string.Empty; // Ex: Segunda-feira, Terça-feira

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFim { get; set; }

        // Chave Estrangeira - Prestador
        public int PrestadorId { get; set; }
        public Prestador? Prestador { get; set; }
    }
}