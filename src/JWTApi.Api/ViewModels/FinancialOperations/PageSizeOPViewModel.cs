namespace JWTApi.Api.ViewModels.FinancialOperations
{
    public class PageSizeOPViewModel
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int? Id { get; set; }
        public string? KeyValue { get; set; }
        public int? ProjectId { get; set; }
        public int? BankId { get; set; }
    }
}
