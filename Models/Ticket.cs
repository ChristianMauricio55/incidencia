using System;
using System.ComponentModel.DataAnnotations;

namespace MiAplicacionMVC.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El pedido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string Order { get; set; } = string.Empty;

        [Required(ErrorMessage = "El situation es obligatorio.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres.")]
        public string Situation { get; set; }= string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres.")]
        public string Solution { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
