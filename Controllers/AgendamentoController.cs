
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

    }
}
