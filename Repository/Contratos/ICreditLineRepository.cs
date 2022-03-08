using CoreApi.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Repository.Contratos
{
    public interface ICreditLineRepository
    {

        #region "Metodos o procedimientos"

        RespuestaBE ProcesarLineaCredito(CreditLineBE creditLineBE);

        #endregion

    }

}
