namespace LMS.Core.Search
{
    public class PaginationParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = string.Empty;
        public bool SortDescending { get; set; } = false;
        public List<FilterCriterion> Filters { get; set; } = new List<FilterCriterion>();

        public string SearchQuery { get; set; } = string.Empty;

    }

}
