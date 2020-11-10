using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Services;
using NSE.WebApp.MVC.Services.Handlers;

namespace NSE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // adiciona o HttpCliente q atualiza o login conforme as claims do usuário (HttpClientFactory)
            // e adiciona essa msma referencia na service q vai utilizada com o método 'AddHttpMessageHandler'
            // por exemplo no add de 'ICatalogoService'a abaixo
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>(); 

            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>();

            // HttpClientFactory  usando a classe concreta 'CatalogoService'
            //services.AddHttpClient<ICatalogoService, CatalogoService>()
            //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

            // HttpClientFactory usando o RFIT (não precisa da classe concreta)
            services.AddHttpClient("Refit", options =>
                {
                    options.BaseAddress = new Uri(configuration.GetSection("CatalogoUrl").Value);
                })
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddTypedClient(Refit.RestService.For<ICatalogoServiceRefit>);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // tem q add para trabalhar com httpContext

            services.AddScoped<IUser, AspNetUser>();
        }
    }
}
