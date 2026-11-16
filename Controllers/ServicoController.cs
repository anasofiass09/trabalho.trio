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
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Codigo", "Nome", servico.CodigoCategoria);
            ViewData["PrestadorId"] = new SelectList(_context.Prestadores, "Codigo", "Nome", servico.CodigoPrestador);
            return View(servico);
        }


        // EDICAO
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Servicos.FindAsync(id);
            if (item == null) return NotFound();
            PrepararListasEdicao(item);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Servico item)
        {
            if (id != item.Codigo) return NotFound();
            ModelState.Remove(nameof(Servico.Categoria));
            ModelState.Remove(nameof(Servico.Prestador));
            if (!ModelState.IsValid)
            {
                PrepararListasEdicao(item);
                return View(item);
            }
            if (!await _context.Categorias.AnyAsync(x => x.Codigo == item.CodigoCategoria) ||
                !await _context.Prestadores.AnyAsync(x => x.Codigo == item.CodigoPrestador))
            {
                ModelState.AddModelError("", "Categoria ou prestador inexistente.");
                PrepararListasEdicao(item);
                return View(item);
            }
            var atual = await _context.Servicos.FindAsync(id);
            if (atual == null) return NotFound();
            atual.Nome = item.Nome;
            atual.Descricao = item.Descricao;
            atual.Preco = item.Preco;
            atual.DuracaoMinutos = item.DuracaoMinutos;
            atual.Ativo = item.Ativo;
            atual.CodigoCategoria = item.CodigoCategoria;
            atual.CodigoPrestador = item.CodigoPrestador;
            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Registro atualizado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Nao foi possivel salvar. Confira os dados e seus relacionamentos.");
            PrepararListasEdicao(item);
                return View(item);
            }
        }

        // EXCLUSAO
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Servicos.AsNoTracking().FirstOrDefaultAsync(x => x.Codigo == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Servicos.FindAsync(id);
            if (item == null) return NotFound();
            _context.Servicos.Remove(item);
            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Registro excluido com sucesso.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Nao e possivel excluir: o registro esta vinculado a outros dados. Remova os vinculos primeiro.";
            }
            return RedirectToAction(nameof(Index));
        }

        private void PrepararListasEdicao(Servico? item = null)
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.AsNoTracking(), "Codigo", "Nome", item?.CodigoCategoria);
            ViewData["PrestadorId"] = new SelectList(_context.Prestadores.AsNoTracking(), "Codigo", "Nome", item?.CodigoPrestador);
        }

    }
}