using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistemaagendamento.Data;
using sistemaagendamento.Models;

namespace sistemaagendamento.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Categorias.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }


        // EDICAO
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Categorias.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria item)
        {
            if (id != item.Codigo) return NotFound();
            if (!ModelState.IsValid) return View(item);
            var atual = await _context.Categorias.FindAsync(id);
            if (atual == null) return NotFound();
            atual.Nome = item.Nome;
            atual.Descricao = item.Descricao;
            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Registro atualizado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Nao foi possivel salvar. Confira os dados e seus relacionamentos.");
                return View(item);
            }
        }

        // EXCLUSAO
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(x => x.Codigo == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Categorias.FindAsync(id);
            if (item == null) return NotFound();
            _context.Categorias.Remove(item);
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

    }
}