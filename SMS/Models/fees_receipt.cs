using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("fees_receipt")]
    public class fees_receipt
    {
        [Key]
        [Required]
        [ForeignKey("mst_fin")]
        [Display(Name = "FY")]
        public virtual string fin_id { get; set; }

        [Key]
        [Required]
        [Display(Name = "Session")]
        public string session{ get; set; }

        [Required]
        [Display(Name = "Receipt Number")]
        public virtual int receipt_no { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Receipt Date")]
        public virtual DateTime receipt_date { get; set; }
        [Required]
        public int acc_id { get; set; }

        [Required]
        [Display(Name = "Account Head")]
        public virtual string fees_name { get; set; }
        [Required]
        [ForeignKey("sr_register")]
        [Display(Name = "Sr Number")]
        public virtual int sr_number { get; set; }
        [Required]
        [Display(Name = "Class Name")]
        public virtual int class_id { get; set; }
        [Required]
        [Display(Name = "Section Name")]
        public virtual int section_id { get; set; }
        [Required]
        public virtual int batch_id { get; set; }
        [Required]
        public virtual String nature { get; set; }
        [Required]
        [Display(Name = "Fees Amount")]
        public virtual decimal amount { get; set; }
        [Display(Name = "Class Name")]
        public virtual String class_name { get; set; }
        [Display(Name = "Section Name")]
        public virtual string section_name { get; set; }

        [Display(Name = "Reg No")]
        public virtual int reg_no { get; set; }

        [Display(Name = "Reg Date")]
        public DateTime? reg_date { get; set; }

        [Display(Name = "Fine")]
        public decimal dc_fine{ get; set; }

        [Display(Name = "Discount")]
        public decimal dc_discount { get; set; }

        [Display(Name = "Narration")]
        public string narration { get; set; }

        public int serial { get; set; }
        
        public DateTime dt_date { get; set; }

        public bool clear_flag { get; set; }

        public string bnk_name { get; set; }
        public string chq_no { get; set; }
        public DateTime? chq_date { get; set; }
        public string mode_flag { get; set; }
        public int month_no{ get; set; }
        public string chq_reject { get; set; }
        public int user_id{ get; set; }

    }
}