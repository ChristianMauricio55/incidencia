using Microsoft.AspNetCore.Identity;
using System;

namespace MiAplicacionMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
