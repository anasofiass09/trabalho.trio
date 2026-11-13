using Microsoft.EntityFrameworkCore;
using sistemaagendamento.Models;

namespace sistemaagendamento.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets (Mapeamento das Tabelas)
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Prestador> Prestadores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Disponibilidade> Disponibilidades { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>().ToTable("Cliente").HasKey(c => c.Codigo);
            modelBuilder.Entity<Prestador>().ToTable("Prestador").HasKey(p => p.Codigo);
            modelBuilder.Entity<Categoria>().ToTable("Categoria").HasKey(c => c.Codigo);
            modelBuilder.Entity<Endereco>().ToTable("Endereco").HasKey(e => e.Codigo);
            modelBuilder.Entity<Disponibilidade>().ToTable("Disponibilidade").HasKey(d => d.Codigo);
            modelBuilder.Entity<Agendamento>().ToTable("Agendamento").HasKey(a => a.Codigo);
            modelBuilder.Entity<Pagamento>().ToTable("Pagamento").HasKey(p => p.Codigo);
            modelBuilder.Entity<Avaliacao>().ToTable("Avaliacao").HasKey(a => a.Codigo);


            modelBuilder.Entity<Pagamento>(entity =>
            {
                entity.ToTable("Pagamento");

                entity.HasKey(p => p.Codigo);

                entity.Property(p => p.AgendamentoId)
                    .HasColumnName("CodigoAgendamento");

                entity.Property(p => p.Valor)
                    .HasColumnType("decimal(10,2)");

                entity.Property(p => p.FormaPagamento)
                    .HasColumnName("Metodo");

                entity.Property(p => p.Situacao)
                    .HasColumnName("Status");

                entity.Property(p => p.DataPagamento)
                    .HasColumnName("DataPagamento");

                entity.HasOne(p => p.Agendamento)
                    .WithOne()
                    .HasForeignKey<Pagamento>(p => p.AgendamentoId);
            });

            modelBuilder.Entity<Avaliacao>(entity =>
            {
                entity.ToTable("Avaliacao");

                entity.HasKey(a => a.Codigo);

                entity.Property(a => a.AgendamentoId)
                    .HasColumnName("CodigoAgendamento");

                entity.Property(a => a.DataAvaliacao)
                    .HasColumnName("DataAvaliacao");

                entity.HasOne(a => a.Agendamento)
                    .WithOne()
                    .HasForeignKey<Avaliacao>(a => a.AgendamentoId);
            });



            modelBuilder.Entity<Agendamento>(entity =>
            {
                entity.ToTable("Agendamento");

                entity.HasKey(a => a.Codigo);

                entity.Property(a => a.ClienteId)
                    .HasColumnName("CodigoCliente");

                entity.Property(a => a.ServicoId)
                    .HasColumnName("CodigoServico");

                entity.Property(a => a.Situacao)
                    .HasColumnName("Status");

                entity.HasOne(a => a.Cliente)
                    .WithMany()
                    .HasForeignKey(a => a.ClienteId);

                entity.HasOne(a => a.Servico)
                    .WithMany()
                    .HasForeignKey(a => a.ServicoId);
            });

        }
    }
}