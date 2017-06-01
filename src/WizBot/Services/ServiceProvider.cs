using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizBot.Services
{
    public interface INServiceProvider : IServiceProvider
    {
        T GetService<T>();
    }

    public class NServiceProvider : INServiceProvider
    {
        public class ServiceProviderBuilder
        {
            private ConcurrentDictionary<Type, object> _dict = new ConcurrentDictionary<Type, object>();

            public ServiceProviderBuilder Add<T>(T obj)
            {
                _dict.TryAdd(typeof(T), obj);
                return this;
            }

            public NServiceProvider Build()
            {
                return new NServiceProvider(_dict);
            }
        }

        private readonly ImmutableDictionary<Type, object> _services;

        private NServiceProvider() { }
        public NServiceProvider(IDictionary<Type, object> services)
        {
            this._services = services.ToImmutableDictionary();
        }

        public T GetService<T>()
        {
            return (T)((IServiceProvider)(this)).GetService(typeof(T));
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            _services.TryGetValue(serviceType, out var toReturn);
            return toReturn;
        }
    }
}