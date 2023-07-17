using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace JV_Imprest_Payment.Models
{
    public class PayRequest
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Transaction Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        public int CategoryId { get; set; }
        public AfeStructure? Category { get; set; }
        public string? CreatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ActionedBy { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string? ProcessStatus { get; set; }
        public string? EvidenceOfPayment { get; set; }
        public string? Status { get; set; }
        public string? Assignedto { get; set; }
        public string? Requester { get; set; }
        public DateTime RequesterDate { get; set; }
        public string? JVA { get; set; }
        public DateTime JVADate { get; set; }
        public string? Accountant { get; set; }
        public DateTime AccountantDate { get; set; }

    }
}
