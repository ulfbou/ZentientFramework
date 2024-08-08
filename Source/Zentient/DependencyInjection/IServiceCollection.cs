using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zentient.DependencyInjection
{
    public interface IServiceCollection : IList<ServiceDescriptor>
    {
        void AddSingleton<TService, TImplementation>() where TImplementation : TService;
        void AddScoped<TService, TImplementation>() where TImplementation : TService;
        void AddTransient<TService, TImplementation>() where TImplementation : TService;
        IServiceProvider Build();
    }
}