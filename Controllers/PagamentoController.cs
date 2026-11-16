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


        // EDICAO
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Pagamentos.FindAsync(id);
            if (item == null) return NotFound();
            PrepararListasEdicao(item);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pagamento item)
        {
            if (id != item.Codigo) return NotFound();
            ModelState.Remove(nameof(Pagamento.Agendamento));
            if (!ModelState.IsValid)
            {
                PrepararListasEdicao(item);
                return View(item);
            }
            var atual = await _context.Pagamentos.FindAsync(id);
            if (atual == null) return NotFound();
            atual.AgendamentoId = item.AgendamentoId;
            atual.Valor = item.Valor;
            atual.FormaPagamento = item.FormaPagamento;
            atual.DataPagamento = item.DataPagamento;
            atual.Situacao = item.Situacao;
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
            var item = await _context.Pagamentos.AsNoTracking().FirstOrDefaultAsync(x => x.Codigo == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Pagamentos.FindAsync(id);
            if (item == null) return NotFound();
            _context.Pagamentos.Remove(item);
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

        private void PrepararListasEdicao(Pagamento? item = null)
        {
            ViewBag.AgendamentoId = new SelectList(_context.Agendamentos.AsNoTracking(), "Codigo", "Codigo", item?.AgendamentoId);
        }

    }
}