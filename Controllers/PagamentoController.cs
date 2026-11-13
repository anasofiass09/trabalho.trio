using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistemaagendamento.Data;
using sistemaagendamento.Models;

namespace sistemaagendamento.Controllers
{
    public class PagamentoController : Controller
    {
        private readonly AppDbContext _context;

        public PagamentoController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pagamentos = _context.Pagamentos.Include(p => p.Agendamento).ThenInclude(a => a.Cliente);
            return View(await pagamentos.ToListAsync());
        }

        // GET: Pagamento/Create
        public IActionResult Create()
        {
            CarregarDropdowns();
            return View();
        }

        // POST: Pagamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pagamento pagamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pagamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se o formulário tiver erros, recarrega a lista antes de devolver a View
            CarregarDropdowns(pagamento);
            return View(pagamento);
        }

        // Método auxiliar para preencher o ViewBag
        private void CarregarDropdowns(Pagamento? pagamento = null)
        {
            // A chave primária da model Agendamento é "Codigo"
            ViewBag.AgendamentoId = new SelectList(
                _context.Agendamentos.AsNoTracking(),
                "Codigo",                // Valor enviado no formulário
                "Codigo",                // Texto exibido na opção (ex: Nº do Agendamento)
                pagamento?.AgendamentoId
            );
        }
    }
}