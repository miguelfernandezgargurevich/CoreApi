using CoreApi.Entidades;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CoreApi.Repository.Contratos;
using CoreApi.Services.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace CoreApi.Services.Implementaciones
{
    public class CreditLineServices : ICreditLineServices
    {

        private readonly IConfiguration _config;
        private readonly ICreditLineRepository _creditLineRepository;
        private readonly ILogger<CreditLineServices> _logger;
        IEmailServices _emailServices;

        private string claveAcceso;

        public CreditLineServices(IConfiguration config, ICreditLineRepository creditLineRepository,
            ILogger<CreditLineServices> logger, IEmailServices emailServices)
        {
            _config = config;
            _creditLineRepository = creditLineRepository;
            _logger = logger;
            _emailServices = emailServices;
            claveAcceso = _config.GetValue<string>("Claves:CLAVE_GENERICA");
        }

        #region "Metodos o procedimientos"

        public RespuestaBE ProcesarLineaCredito(CreditLineBE creditLineBE)
        {
            RespuestaBE rptaBE = new RespuestaBE();

            try
            {
                rptaBE = _creditLineRepository.ProcesarLineaCredito(creditLineBE);
            }
            catch (Exception ex)
            {
                rptaBE.code = "-1";
                rptaBE.message = "Error: " + ex.Message.ToString();

                CapturarError(ex, "CreditLineService", "ProcesarLineaCredito");
            }

            return rptaBE;
        }

        public void CapturarError(Exception error, string controlador = "", string accion = "")
        {
            var msg = error.Message;
            if (error.InnerException != null)
            {
                msg = msg + "/;/" + error.InnerException.Message;
                if (error.InnerException.InnerException != null)
                {
                    msg = msg + "/;/" + error.InnerException.InnerException.Message;
                    if (error.InnerException.InnerException.InnerException != null)
                        msg = msg + "/;/" + error.InnerException.InnerException.InnerException.Message;
                }
            }

            var fechahora = DateTime.Now.ToString();
            var comentario = $@"***ERROR: [{fechahora}] [{controlador}/{accion}] - MensajeError: {msg}";
            string errorFormat = String.Format("{0} | {1}", comentario, error.StackTrace);
            _logger.LogError(errorFormat);

        }

        #endregion

    }
}
