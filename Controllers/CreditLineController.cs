﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using CoreApi.Entidades;
using CoreApi.Models;
using CoreApi.Services.Contratos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CoreApi.Controllers
{
    [ApiController]
    [Route("core-api")]
    public class CreditLineController : ControllerBase
    {
        private readonly IConfiguration _config;
        ICreditLineServices _creditLineServices;
        private readonly ILogger<CreditLineController> _logger;
        string _urlApiSeguridad;
        string _rutaApiSeguridad;
        string _aplicationId = "core-api";

        public CreditLineController(
            IConfiguration config,
            ICreditLineServices creditLineServices,
            ILogger<CreditLineController> logger)
        {
            _config = config;
            _creditLineServices = creditLineServices;
            _logger = logger;

            _urlApiSeguridad = _config["URL_API_SEGURIDAD"];
            _rutaApiSeguridad = _config["RUTA_API_SEGURIDAD"];
        }


        #region "Metodos o procedimientos"
        [Route("procesar")]
        [HttpPost]
        public IActionResult procesarLineaCredito([FromBody] CreditLineBE creditLineBE)
        {

            var authorization = Request.Headers[HeaderNames.Authorization];

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                //we have a valid AuthenticationHeaderValue that has the following details:

                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;

                //scheme will be "Bearer"
                //parmameter will be the token itself.

                ValidateRequest validateRequest = new ValidateRequest() { token = parameter, aplicationId = _aplicationId, endPoint = "/valid-creditLine/procesarLineaCredito", httpMethod = "post" };

                RetornoBE retornoBE = ValidateSecurity(validateRequest);

                if (retornoBE != null && retornoBE.code == 200000 && retornoBE.content.isAllowed)
                {
                    RespuestaBE respuestaBE = _creditLineServices.ProcesarLineaCredito(creditLineBE);

                    return new JsonResult(new { code = respuestaBE.code, message = String.Concat(respuestaBE.message, ": ", respuestaBE.messageDetail), status = respuestaBE.status, }) { StatusCode = respuestaBE.status };
                }
                else
                {
                    return Ok(retornoBE);
                }

            }

            return new JsonResult(new { code = 401000, message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

        }


        #endregion

        private RetornoBE ValidateSecurity(ValidateRequest validate)
        {

            RetornoBE retornoBE = new RetornoBE() { code=401000 , message = "Unauthorized" };

            try
            {
                string datosJSON = JsonConvert.SerializeObject(validate);
                var contentData = new StringContent(datosJSON, System.Text.Encoding.UTF8, "application/json");
                
                if (validate.token != null && validate.aplicationId == "core-api")
                {
                    retornoBE.code = 200000;
                    ContentBE contentBE = new ContentBE() { isAllowed = true, subject = "" };
                    retornoBE.content = contentBE;
                }
                else 
                {
                    retornoBE.code = 500403;
                    retornoBE.message = "permission denied";
                    ContentBE contentBE = new ContentBE() { isAllowed = false, subject = "" };
                    retornoBE.content = contentBE;
                }
                
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error consultar " + _urlApiSeguridad + _rutaApiSeguridad);
                retornoBE.message = "Error consultar " + _urlApiSeguridad + _rutaApiSeguridad;
            }

            return retornoBE;

        }

    }
}
