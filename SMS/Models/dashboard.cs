using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class dashboard
    {


        public int school_strength { get; set; }

        public decimal fees_received { get; set; }

        public decimal cash_received { get; set; }

        public decimal bank_received { get; set; }

        public int transport_std { get; set; }

        public int newAdmission { get; set; }

        public string[] name { get; set; }

        public decimal[] dues { get; set; }

        public decimal[] recovered { get; set; }

        public decimal total_recovered { get; set; }

        public decimal total_dues { get; set; }

    }
}