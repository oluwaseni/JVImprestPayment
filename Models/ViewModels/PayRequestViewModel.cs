using Microsoft.AspNetCore.Identity;

namespace JV_Imprest_Payment.Models.ViewModels
{
    public class PayRequestViewModel
    {

        public IList<AfeStructure>? GetAfeStructures { get; set; }
        public IList<PayRequest>? PayRequests { get; set; }
        public PayRequest? PayRequestModel { get; set; }
        public int AssignedToUserCount { get; set; }
        public IList<PayRequest>? MyRequestsCount { get; set; }
        public IList<PayRequest>? InProgressCount { get; set; }
        public IList<PayRequest>? ApprovedCount { get; set; }
        public IList<PayRequest>? RejectedCount { get; set; }
        public int TotalRequestCount { get; set; }
        public IList<PayRequest>? PendingRequestCount { get; set; }
        public IList<PayRequest>? ActionedRequestCount { get; set; }
        public double AccountantPerformance { get; set; }
        public IList<PayRequest>? AFEContracts { get; set; }
        public IList<PayRequest>? AFEApproved { get; set; }
        public IList<PayRequest>? AFERejected { get; set; }
        public double AFEPerformance { get; set; }
        public IList<PayRequest>? OverAllPendingAccountant { get; set; }
        public IList<PayRequest>? OverAllPendingAFE { get; set; }
        public IList<PayRequest>? OverAllRejected { get; set; }
        public List<IdentityUser>? Users { get; set; }
        public IList<PayRequest>? OverAllApproved { get; set; }
        public double OverAllPerformance { get; set; }
    }
}
