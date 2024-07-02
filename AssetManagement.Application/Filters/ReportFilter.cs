namespace AssetManagement.Application.Filters
{
    public class ReportFilter
    {
        public TypeReportSort sortby { get; set; }
        public bool ascending { get; set; } = true;
    }
    public enum TypeReportSort
    {
        CategoryName = 1,
        Total = 2,
        Assigned = 3,
        Available = 4,
        NotAvailable = 5,
        WaitingForRecycling = 6,
        Recycled = 7
    }
}
