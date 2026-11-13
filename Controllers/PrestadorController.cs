using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistemaagendamento.Data;
using sistemaagendamento.Models;

namespace sistemaagendamento.Controllers
{
    public class PrestadorController : Controller
    {
        private readonly AppDbContext _context;

        public PrestadorController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Prestadores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Prestadores.ToListAsync());
        }

        // GET: Prestadores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Prestadores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prestador prestador)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prestador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prestador);
        }

        // GET: Prestadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var prestador = await _context.Prestadores.FindAsync(id);
            if (prestador == null) return NotFound();

            return View(prestador);
        }

        // POST: Prestadores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Prestador prestador)
        {
            if (id != prestador.Codigo) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(prestador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prestador);
        }

        // GET: Prestadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var prestador = await _context.Prestadores.FirstOrDefaultAsync(m => m.Codigo == id);
            if (prestador == null) return NotFound();

            return View(prestador);
        }

        // POST: Prestadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prestador = await _context.Prestadores.FindAsync(id);
            if (prestador != null)
            {
                _context.Prestadores.Remove(prestador);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}