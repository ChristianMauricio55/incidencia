using MiAplicacionMVC.Models;
using Microsoft.AspNetCore.Mvc;
using MiAplicacionMVC.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace MiAplicacionMVC.Controllers
{
    public class TicketController : Controller
    {

        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Filtro para buscar productos por nombre y paginación
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 5)
        {
            var viewModel = new TicketViewModel
            {
                SearchString = searchString,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var query = _context.Tickets.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => EF.Functions.Like(p.Order.ToLower(), $"%{searchString.ToLower()}%"));
            }

            var totalRecords = await query.CountAsync();
            viewModel.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            viewModel.Tickets = await query
                .OrderBy(p => p.Order)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: Producto - obtener la lista de productos almacenados en la base de datos
        //public IActionResult Index()
        //{
          //  var tickets = _context.Tickets.ToList();
            //return View(tickets);
        //}

        // GET: Producto/Create - mostrar el formulario para crear un nuevo producto
        public IActionResult Create()
        {
            return View();
        }

        // POST: Producto/Create - crear un nuevo producto con los datos del formulario y almacenar en la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Tickets.Add(ticket);
                _context.SaveChanges(); // Ensure changes are saved to the database
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Producto/Edit/5 - mostrar los datos del producto seleccionando en el formulario para editar un producto existente
        public IActionResult Edit(int id)
        {
            var ticket = _context.Tickets.Find(id); // Use Find to retrieve the entity by its primary key
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Producto/Edit/5 - actualizar los datos del producto con los datos del formulario y almacenar en la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var existingTicket = _context.Tickets.Find(id); // Retrieve the existing entity
                if (existingTicket == null)
                {
                    return NotFound();
                }

                // Update the properties of the existing entity
                existingTicket.Order = ticket.Order;
                existingTicket.Situation = ticket.Situation;
                existingTicket.Solution = ticket.Solution;
                existingTicket.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges(); // Save changes to the database
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Producto/Delete/5 - mostrar el formulario para eliminar un producto existente
        public IActionResult Delete(int id)
        {
            var ticket = _context.Tickets.Find(id); // Use Find to retrieve the entity by its primary key
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Producto/Delete/5 - eliminar el producto seleccionado de la base de datos
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var ticket = _context.Tickets.Find(id); // Use Find to retrieve the entity by its primary key
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket); // Remove the entity from the DbSet
                _context.SaveChanges(); // Save changes to the database
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ExportToPdf()
        {
            var tickets = _context.Tickets.ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Agregar título
                document.Add(new Paragraph("Lista de Tickets")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20));

                // Crear tabla
                Table table = new Table(4).UseAllAvailableWidth();

                // Agregar encabezados
                table.AddCell("Order");
                table.AddCell("Situation");
                table.AddCell("Solution");
                table.AddCell("Date");

                // Agregar datos
                foreach (var ticket in tickets)
                {
                    table.AddCell(ticket.Order);
                    table.AddCell(ticket.Situation.ToString());
                    table.AddCell(ticket.Solution.ToString());
                    table.AddCell(ticket.CreatedAt.ToString("dd/MM/yyyy"));
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(), "application/pdf", "Tickets.pdf");
            }
        }

        public IActionResult ExportToExcel()
        {
            var tickets = _context.Tickets.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Tictets");

                // Agregar encabezados
                worksheet.Cell(1, 1).Value = "Order";
                worksheet.Cell(1, 2).Value = "Situation";
                worksheet.Cell(1, 3).Value = "Solution";
                worksheet.Cell(1, 4).Value = "Date";

                // Dar formato al encabezado
                var rango = worksheet.Range(1, 1, 1, 4);
                rango.Style.Font.Bold = true;
                rango.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Agregar datos
                int row = 2;
                foreach (var ticket in tickets)
                {
                    worksheet.Cell(row, 1).Value = ticket.Order;
                    worksheet.Cell(row, 2).Value = ticket.Situation;
                    worksheet.Cell(row, 3).Value = ticket.Solution;
                    worksheet.Cell(row, 4).Value = ticket.CreatedAt;

                    // Formato para las columnas de precio
                    //worksheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                    //worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";

                    row++;
                }

                // Autoajustar columnas
                worksheet.Columns().AdjustToContents();

                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Tickets.xlsx");
                }
            }
        }

    }
}
