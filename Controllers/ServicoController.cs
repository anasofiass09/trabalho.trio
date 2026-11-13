using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistemaagendamento.Data;
using sistemaagendamento.Models;

namespace sistemaagendamento.Controllers
{
    public class ServicoController : Controller
    {
        private readonly AppDbContext _context;

        public ServicoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Servicos
        public async Task<IActionResult> Index()
        {
            var servicos = _context.Servicos.Include(s => s.Categoria).Include(s => s.Prestador);
            return View(await servicos.ToListAsync());
        }

        // GET: Servicos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(
                _context.Categorias, "Codigo", "Nome");

            ViewData["PrestadorId"] = new SelectList(
                _context.Prestadores, "Codigo", "Nome");

            return View();
        }

        // POST: Servicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Servico servico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome", servico.CodigoCategoria);
            ViewData["PrestadorId"] = new SelectList(_context.Prestadores, "Id", "Nome", servico.CodigoPrestador);
            return View(servico);
        }
    }
}