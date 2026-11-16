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


        // EDICAO
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Avaliacoes.FindAsync(id);
            if (item == null) return NotFound();
            PrepararListasEdicao(item);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Avaliacao item)
        {
            if (id != item.Codigo) return NotFound();
            ModelState.Remove(nameof(Avaliacao.Agendamento));
            if (!ModelState.IsValid)
            {
                PrepararListasEdicao(item);
                return View(item);
            }
            var atual = await _context.Avaliacoes.FindAsync(id);
            if (atual == null) return NotFound();
            atual.AgendamentoId = item.AgendamentoId;
            atual.Nota = item.Nota;
            atual.Comentario = item.Comentario;
            atual.DataAvaliacao = item.DataAvaliacao;
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
            var item = await _context.Avaliacoes.AsNoTracking().FirstOrDefaultAsync(x => x.Codigo == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Avaliacoes.FindAsync(id);
            if (item == null) return NotFound();
            _context.Avaliacoes.Remove(item);
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

        private void PrepararListasEdicao(Avaliacao? item = null)
        {
            ViewBag.AgendamentoId = new SelectList(_context.Agendamentos.AsNoTracking(), "Codigo", "Codigo", item?.AgendamentoId);
        }

    }
}