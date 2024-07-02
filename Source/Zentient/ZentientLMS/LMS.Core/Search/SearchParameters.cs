namespace LMS.Core.Search
{
    public class SearchParameters
    {
        public string Query { get; set; } // For keyword searches
        public int PageNumber { get; set; } = 1; // Default to first page
        public int PageSize { get; set; } = 10; // Default page size
                                                // Add properties for sorting
        public string SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
        // Add any specific filter criteria your application needs
        public string FilterBy { get; set; }
        public string FilterValue { get; set; }
        public string SearchableColumnName { get; internal set; }

        // Extend with additional fields as needed for your application's search functionality
    }

}