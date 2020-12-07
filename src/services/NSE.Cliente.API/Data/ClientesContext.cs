using NSE.Core.Data;
using NSE.Clientes.API.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NSE.Core.Mediator;
using NSE.Core.DomainObjects;
using System.ComponentModel.DataAnnotations;
using NSE.Core.Messages;

namespace NSE.Clientes.API.Data
{
    public sealed class ClientesContext : DbContext, IUnitOfWork
    {
        private readonly IMediatorHandler _mediatorHandler;

        public ClientesContext(DbContextOptions<ClientesContext> options, IMediatorHandler mediatorHandler) : base(options)
        {
            // melhorar a performance = AsNoTracking()
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
            _mediatorHandler = mediatorHandler;
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.Ignore<Event>();

            // esse recurso mapeia as strings caso não mapeado
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            // configura / desabilita o delete em cascata
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientesContext).Assembly);
        }

        public async Task<bool> Commit()
        {
            var sucesso = await base.SaveChangesAsync() > 0;
            if (sucesso) await _mediatorHandler.PublicarEventos(this); // utilizando o MediatorExtension.PublicarEventos()

            return sucesso;
        }
    }


    public static class MediatorExtension
    {

        /// <summary>
        ///  Método Criado para tratar e lançar todos eventos da entidade
        /// </summary>
        public static async Task PublicarEventos<T>(this IMediatorHandler mediator, T ctx) where T : DbContext
        {
            // verifica se existe algum event (notificacoes) lancadas no ChangeTracker (normalmente lançacada pelos commands)
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

            // Selecionar os eventos lançados pelo filtro acima e COPIA para uma nova lista
            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notificacoes)
                .ToList();

            // limpa a lista de eventos de referencia para não ter problema em dupla execução ou algo parecido
            domainEntities.ToList()
                .ForEach(entity => entity.Entity.LimparEventos());

            // publica os eventos da lista copiada anteriormente um a um
            var tasks = domainEvents
                .Select(async (domainEvent) => {
                    await mediator.PublicarEvento(domainEvent);
                });

            // executar todos eventos/tasks
            await Task.WhenAll(tasks);
        }
    }
}
