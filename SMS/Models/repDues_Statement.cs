using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class repDues_Statement
    {
        [Display(Name = "Section Name")]
        [Required(ErrorMessage = "Please Provide Section Name", AllowEmptyStrings = false)]
        public int section_id { get; set; }

        [Display(Name = "Class Name")]
        [Required(ErrorMessage = "Please Provide Class", AllowEmptyStrings = false)]
        public int class_id { get; set; }

        [Display(Name = "Pickup Point")]
        public int pickup_id { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Please Provide Amount", AllowEmptyStrings = false)]
        public decimal amount { get; set; }

        public string operation { get; set; }

        [Display(Name = "Month")]
        public string month_name { get; set; }

        [Display(Name = "Due Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime payment_by { get; set; }

        [Display(Name = "Message")]
        public string message { get; set; }

        [Display(Name = "Session")]
        public string session { get; set; }

        [Display(Name = "Font Size")]
        public int font_size { get; set; }

    }
}