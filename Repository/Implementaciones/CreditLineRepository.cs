
using CoreApi.Entidades;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CoreApi.Repository.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace CoreApi.Repository.Implementaciones
{

    public class CreditLineRepository : ICreditLineRepository
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<CreditLineRepository> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreditLineRepository(IConfiguration configuration, ILogger<CreditLineRepository> logger, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _logger = logger;
        }

        #region "Metodos y Funciones"

        public RespuestaBE ProcesarLineaCredito(CreditLineBE creditLineBE)
        {
            RespuestaBE respuestaBE = new RespuestaBE();

            try
            { 
                //obtiene lastRequest
                bool lastRequestAccepted = true;
                int lastRequestNumbers = 1;
                TimeSpan lastRequestDate = DateTime.Now.TimeOfDay; ;
                TimeSpan nowRequestDate = Convert.ToDateTime(creditLineBE.requestedDate).TimeOfDay;

                CreditLineBE lastTrama = LeerTramas();
                if (lastTrama.requestNumbers != null)
                {
                    lastRequestNumbers = (int)lastTrama.requestNumbers + 1;
                    lastRequestDate = Convert.ToDateTime(lastTrama.requestedDate).TimeOfDay;
                    lastRequestAccepted = Convert.ToBoolean(lastTrama.requestAccepted);
                }
                
                if (ValidPreviousRequest(lastRequestAccepted, nowRequestDate, lastRequestDate, respuestaBE) == false)
                    return respuestaBE;

                //calcula Recommended CreditLine
                double recommendedCreditLine = GetRecommendedCreditLine(creditLineBE);

                //valida accepted o rejected
                bool accepted = recommendedCreditLine > creditLineBE.requestedCreditLine;
                if (accepted)
                {
                    var timeRequest = (nowRequestDate - lastRequestDate);

                    if (ValidAcceptedRequest(lastRequestNumbers, timeRequest, respuestaBE) == false)
                        return respuestaBE;
                    else if (lastRequestNumbers > 3 && timeRequest.TotalMinutes > 2)
                        lastRequestNumbers = 1;

                    creditLineBE.recommendedCreditLine = Math.Round(recommendedCreditLine, 2);
                    creditLineBE.requestAccepted = accepted.ToString();
                    creditLineBE.requestNumbers = lastRequestNumbers;

                    //guardar Request
                    GuardarTrama(creditLineBE);

                    respuestaBE.code = "1";
                    respuestaBE.status = StatusCodes.Status200OK;
                    respuestaBE.retornoBool = true;
                    respuestaBE.message = "Acepted";
                    respuestaBE.messageDetail = "Credit line authorized: " + creditLineBE.recommendedCreditLine.ToString();

                }
                else
                {
                    if (lastRequestAccepted == true)
                        lastRequestNumbers = 1;

                    creditLineBE.recommendedCreditLine = Math.Round(recommendedCreditLine, 2);
                    creditLineBE.requestAccepted = accepted.ToString();
                    creditLineBE.requestNumbers = lastRequestNumbers;

                    //guardar Request
                    GuardarTrama(creditLineBE);

                    ValidRejectedRequest(lastRequestAccepted, lastRequestNumbers, respuestaBE);

                }

                return respuestaBE;

            }
            catch (Exception ex)
            {
                CapturarError(ex, "Repository", "ProcesarLineaCredito");

                respuestaBE.code = "-1";
                respuestaBE.status = StatusCodes.Status500InternalServerError;
                respuestaBE.retornoBool = false;
                respuestaBE.message = "Error";
                respuestaBE.messageDetail = ex.Message;

                return respuestaBE;
            }
        }

        private void ValidRejectedRequest(bool lastRequestAccepted, int lastRequestNumbers, RespuestaBE respuestaBE)
        {
            //After failing 3 times, return the message "A sales agent will contact you".
            if (lastRequestAccepted == false && lastRequestNumbers >= 3)
            {
                respuestaBE.code = "-101";
                respuestaBE.status = StatusCodes.Status200OK;
                respuestaBE.retornoBool = false;
                respuestaBE.message = "Rejected";
                respuestaBE.messageDetail = "A sales agent will contact you";
            }
            else
            {
                respuestaBE.code = "-100";
                respuestaBE.status = StatusCodes.Status200OK;
                respuestaBE.retornoBool = false;
                respuestaBE.message = "Rejected";
                respuestaBE.messageDetail = "The recommended credit line is lower than requested";
            }

        }
        
        private bool ValidAcceptedRequest(int lastRequestNumbers, TimeSpan timeRequest, RespuestaBE respuestaBE)
        {
            //If the system receives 3 or more requests within two minutes, return the http code 429.
            if (lastRequestNumbers > 3 && timeRequest.TotalMinutes <= 2)
            {
                respuestaBE.code = "-102";
                respuestaBE.status = StatusCodes.Status429TooManyRequests;
                respuestaBE.retornoBool = false;
                respuestaBE.message = "Error";
                respuestaBE.messageDetail = "The system receives 3 or more requests within two minutes";

                return false;
            }
            return true;

        }
        
        private bool ValidPreviousRequest(bool lastRequestAccepted, TimeSpan nowRequestDate, TimeSpan lastRequestDate, RespuestaBE respuestaBE)
        {
            //Don't allow a new application requests within 30 seconds next to the previous one, if so,
            //return HTTP code 429.
            if (lastRequestAccepted == false)
            {
                var timeRequest = (nowRequestDate - lastRequestDate);
                var timeLimit = Convert.ToDateTime("01/01/2020 00:00:30").TimeOfDay;
                if (timeRequest <= timeLimit)
                {
                    respuestaBE.code = "-102";
                    respuestaBE.status = StatusCodes.Status429TooManyRequests;
                    respuestaBE.retornoBool = false;
                    respuestaBE.message = "Error";
                    respuestaBE.messageDetail = "Don't allow a new application requests within 30 seconds next to the previous one";

                    return false;
                }
            }
            return true;

        }
        
        private double GetRecommendedCreditLine(CreditLineBE creditLineBE) {
            //calcula recommendedCreditLine
            var recommendedCreditLine = 0.0;
            if (creditLineBE.foundingType == "SME")
            {
                recommendedCreditLine = creditLineBE.monthlyRevenue / 5; //One fifth of the monthly revenue(5:1 ratio)
            }
            else if (creditLineBE.foundingType == "Startup")
            {
                var mR = creditLineBE.monthlyRevenue / 5;
                var cB = creditLineBE.cashBalance / 3; //One third of the cash balance(3:1 ratio)
                var maxValue = Math.Max(mR, cB);
                recommendedCreditLine = maxValue;
            }
            return recommendedCreditLine;
        }

        private void GuardarTrama(CreditLineBE creditLineBE) 
        {

            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string hoy = DateTime.Today.ToString("yyyyMMdd");
            string fileUnicoName = String.Concat(hoy, ".txt");
            string fileUnicoPath = Path.Combine(contentRootPath, "Tramas", fileUnicoName);

            string trama = JsonConvert.SerializeObject(creditLineBE);
           
            File.AppendAllLines(fileUnicoPath, new string[] { trama });

        }

        private CreditLineBE LeerTramas() {
            CreditLineBE creditLineBE = new CreditLineBE();
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string hoy = DateTime.Today.ToString("yyyyMMdd");
            string fileUnicoName = String.Concat(hoy, ".txt");
            string fileUnicoPath = Path.Combine(contentRootPath, "Tramas", fileUnicoName);

           
            bool result = File.Exists(fileUnicoPath);
            if (result == false)
            {
                File.CreateText(fileUnicoPath).Close(); ;
            }
            else
            {
                var line = File.ReadLines(fileUnicoPath);

                if (line.Count() > 0)
                {
                    var lastLine = line.Last();
                    if (!String.IsNullOrEmpty(lastLine))
                        creditLineBE = JsonConvert.DeserializeObject<CreditLineBE>(lastLine);
                }
            }

            return creditLineBE;
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
