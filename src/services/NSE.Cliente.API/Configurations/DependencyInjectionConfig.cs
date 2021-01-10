using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSE.Clientes.API.Application.Commands;
using NSE.Clientes.API.Application.Events;
//using NSE.Clientes.API.Application.Events;
using NSE.Clientes.API.Data;
using NSE.Clientes.API.Data.Repository;
//using NSE.Clientes.API.Data.Repository;
using NSE.Clientes.API.Models;
using NSE.Clientes.API.Services;
using NSE.Core.Mediator;

namespace NSE.Clientes.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            // COMMANDS
            services.AddScoped<IRequestHandler<RegistrarClienteCommand, ValidationResult>, ClienteCommandHandler>();


            // EVENTS
            services.AddScoped<INotificationHandler<ClienteRegistradoEvent>, ClienteEventHandler>();

            // REPOSITORIES
            services.AddScoped<IClienteRepository, ClienteRepository>();

            // CONTEXTO
            services.AddScoped<ClientesContext>();

            // HOSTED INTEGRATION HANDLER
            services.AddHostedService<RegistroClienteIntegrationHandler>(); //singleton
        }
    }
}