using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zentient.DependencyInjection
{
    public interface IServiceCollection : IList<ServiceDescriptor>
    {
        IServiceCollection AddSingleton<TService, TImplementation>() where TImplementation : TService;
        IServiceCollection AddScoped<TService, TImplementation>() where TImplementation : TService;
        IServiceCollection AddTransient<TService, TImplementation>() where TImplementation : TService;
        IServiceProvider Build();
    }
}