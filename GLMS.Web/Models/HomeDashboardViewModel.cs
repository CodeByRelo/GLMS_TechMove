namespace GLMS.Web.Models
{
    public class HomeDashboardViewModel
    {
        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public int ExpiredContracts { get; set; }
        public int PendingRequests { get; set; }
        public int CompletedRequests { get; set; }
    }

}
