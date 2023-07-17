namespace JV_Imprest_Payment.Models
{
    public class AfeStructure
    {
        public int Id { get; set; }
        public string? AfeCode { get; set; }
        public string? Classification { get; set; }
        public string? AfeOwner { get; set; }
        public string? OwnerEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
