using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_scholastic_grades
    {
        public string session { get; set; }

        public int from_marks { get; set; }

        public int to_marks { get; set; }

        public string grade { get; set; }
    }
}