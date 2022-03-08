using Microsoft.Extensions.DependencyInjection;
using CoreApi.Repository.Contratos;
using CoreApi.Repository.Implementaciones;
using CoreApi.Services.Contratos;
using CoreApi.Services.Implementaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.UnitOfwork
{
    public static class UnitOfwork
    {
        public static void RegisterComponents(IServiceCollection services)
        {
            #region "Services"
            services.AddTransient<ICreditLineServices, CreditLineServices>();
            services.AddTransient<IEmailServices, EmailServices>();
            #endregion

            #region "Repositorio"
            services.AddTransient<ICreditLineRepository, CreditLineRepository>();
            #endregion
        }
    }
}
