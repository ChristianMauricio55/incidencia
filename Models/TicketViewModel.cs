namespace MiAplicacionMVC.Models
{
    public class TicketViewModel
    {

        // Contiene la lista de productos que se mostrarán en la vista
        public IEnumerable<Ticket> Tickets { get; set; } = new List<Ticket>();
        public string SearchString { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }

    }
}
