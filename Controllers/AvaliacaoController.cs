using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistemaagendamento.Data;
using sistemaagendamento.Models;

namespace sistemaagendamento.Controllers
{
    public class AvaliacaoController : Controller
    {
        private readonly AppDbContext _context;

        public AvaliacaoController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var avaliacoes = _context.Avaliacoes.Include(a => a.Agendamento).ThenInclude(ag => ag.Servico);
            return View(await avaliacoes.ToListAsync());
        }

        // GET: Avaliacao/Create
        public IActionResult Create()
        {
            CarregarDropdowns();
            return View();
        }

        // POST: Avaliacao/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Avaliacao avaliacao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(avaliacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recarrega as listas caso o formulário contenha erros de validação
            CarregarDropdowns(avaliacao);
            return View(avaliacao);
        }

        // Método auxiliar para popular o ViewBag
        private void CarregarDropdowns(Avaliacao? avaliacao = null)
        {
            // A chave primária da model Agendamento é "Codigo"
            ViewBag.AgendamentoId = new SelectList(
                _context.Agendamentos.AsNoTracking(),
                "Codigo",                // Valor submetido pelo formulário
                "Codigo",                // Texto exibido na opção (Nº do Agendamento)
                avaliacao?.AgendamentoId
            );
        }
    }
}