using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class phoneSMS
    {
        public string toNumber { get; set; }

        [Display(Name = "SMS Message")]
        public string toText { get; set; }

        public IEnumerable<class_list> class_l { get; set; }

        public IEnumerable<pickup_list> class_p { get; set; }

        public int[] selected_list { get; set; }

    }

    public class class_list
    {
        public string class_name { get; set; }

        public int class_id { get; set; }
    }

    public class pickup_list
    {
        public string pickup_point { get; set; }

        public int pickup_id { get; set; }
    }
}