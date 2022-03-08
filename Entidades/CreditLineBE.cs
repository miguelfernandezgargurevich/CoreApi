using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Entidades
{
    public class CreditLineBE : BaseBE
    {
        public string foundingType { get; set; }
        public double cashBalance { get; set; }
        public double monthlyRevenue { get; set; }
        public double requestedCreditLine { get; set; }
        public string requestedDate { get; set; }
        public double? recommendedCreditLine { get; set; }
        public string requestAccepted { get; set; }
        public Int32? requestNumbers { get; set; }


    }
}
