
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistemaagendamento.Data;
using sistemaagendamento.Models;

namespace sistemaagendamento.Controllers
{
    public class AgendamentoController : Controller
    {
        private readonly AppDbContext _context;

        public AgendamentoController(AppDbContext context)
        {
            _context = context;
        }

        // LISTAR AGENDAMENTOS
        public async Task<IActionResult> Index()
        {
            var agendamentos = await _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Servico)
                    .ThenInclude(s => s!.Prestador)
                .ToListAsync();

            return View(agendamentos);
        }

        // ABRIR TELA DE CADASTRO
        public IActionResult Create()
        {
            CarregarDropdowns();
            return View();
        }

        // CADASTRAR AGENDAMENTO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Agendamento agendamento)
        {
            ModelState.Remove(nameof(Agendamento.Cliente));
            ModelState.Remove(nameof(Agendamento.Servico));

            if (!ModelState.IsValid)
            {
                CarregarDropdowns(agendamento);
                return View(agendamento);
            }

            // Verificar se o cliente existe
            var cliente = await _context.Clientes
                .FindAsync(agendamento.ClienteId);

            if (cliente == null)
            {
                ModelState.AddModelError(
                    nameof(Agendamento.ClienteId),
                    "O cliente selecionado não existe."
                );

                CarregarDropdowns(agendamento);
                return View(agendamento);
            }

            // Verificar se o serviço existe
            var servico = await _context.Servicos
                .Include(s => s.Prestador)
                .FirstOrDefaultAsync(s => s.Codigo == agendamento.ServicoId);

            if (servico == null)
            {
                ModelState.AddModelError(
                    nameof(Agendamento.ServicoId),
                    "O serviço selecionado não existe."
                );

                CarregarDropdowns(agendamento);
                return View(agendamento);
            }

            // Não permitir agendamento em data passada
            if (agendamento.DataHora <= DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(Agendamento.DataHora),
                    "Selecione uma data e horário futuros."
                );

                CarregarDropdowns(agendamento);
                return View(agendamento);
            }

            // Calcular início e fim do novo agendamento
            DateTime inicioNovo = agendamento.DataHora;
            DateTime fimNovo = inicioNovo.AddMinutes(
                servico.DuracaoMinutos
            );

            // Verificar conflito de horário do prestador
            bool existeConflito = await _context.Agendamentos
                .Where(a =>
                    a.Situacao != "Cancelado" &&
                    a.Servico != null &&
                    a.Servico.CodigoPrestador == servico.CodigoPrestador
                )
                .AnyAsync(a =>
                    inicioNovo <
                        a.DataHora.AddMinutes(a.Servico!.DuracaoMinutos)
                    && fimNovo > a.DataHora
                );

            if (existeConflito)
            {
                ModelState.AddModelError(
                    "",
                    "Este prestador já possui um agendamento nesse horário."
                );

                CarregarDropdowns(agendamento);
                return View(agendamento);
            }

            // Salvar agendamento
            agendamento.Situacao = "Confirmado";

            _context.Agendamentos.Add(agendamento);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // CANCELAR AGENDAMENTO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var agendamento = await _context.Agendamentos
                .FindAsync(id);

            if (agendamento == null)
            {
                return NotFound();
            }

            agendamento.Situacao = "Cancelado";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // CARREGAR LISTAS DE CLIENTES E SERVIÇOS

        private void CarregarDropdowns(Agendamento? agendamento = null)
        {
            ViewData["ClienteId"] = new SelectList(
                _context.Clientes.AsNoTracking(),
                "Codigo",
                "Nome",
                agendamento?.ClienteId
            );

            ViewData["ServicoId"] = new SelectList(
                _context.Servicos
                    .AsNoTracking()
                    .Where(s => s.Ativo),
                "Codigo",
                "Nome",
                agendamento?.ServicoId
            );
        }



        // EDICAO
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Agendamentos.FindAsync(id);
            if (item == null) return NotFound();
            PrepararListasEdicao(item);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Agendamento item)
        {
            if (id != item.Codigo) return NotFound();
            ModelState.Remove(nameof(Agendamento.Cliente));
            ModelState.Remove(nameof(Agendamento.Servico));
            if (!ModelState.IsValid)
            {
                PrepararListasEdicao(item);
                return View(item);
            }
            if (item.DataHora <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(item.DataHora), "Selecione um horario futuro.");
                PrepararListasEdicao(item);
                return View(item);
            }
            if (!await _context.Clientes.AnyAsync(x => x.Codigo == item.ClienteId) ||
                !await _context.Servicos.AnyAsync(x => x.Codigo == item.ServicoId))
            {
                ModelState.AddModelError("", "Cliente ou servico inexistente.");
                PrepararListasEdicao(item);
                return View(item);
            }
            var atual = await _context.Agendamentos.FindAsync(id);
            if (atual == null) return NotFound();
            atual.ClienteId = item.ClienteId;
            atual.ServicoId = item.ServicoId;
            atual.DataHora = item.DataHora;
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
            var item = await _context.Agendamentos.AsNoTracking().FirstOrDefaultAsync(x => x.Codigo == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Agendamentos.FindAsync(id);
            if (item == null) return NotFound();
            _context.Agendamentos.Remove(item);
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

        private void PrepararListasEdicao(Agendamento? item = null)
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes.AsNoTracking(), "Codigo", "Nome", item?.ClienteId);
            ViewData["ServicoId"] = new SelectList(_context.Servicos.AsNoTracking(), "Codigo", "Nome", item?.ServicoId);
        }

    }
}
