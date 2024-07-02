using LMS.Core.Infrastructure.Caching;
using LMS.Core.Search;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Core.Services
{
    public interface IBetterSearchService
    {
        bool IsValidSearchableColumn(string searchableColumnName);
        Task<IActionResult> SearchAsync(SearchParameters parameters);
        void UpdateCacheSettings(CacheSettings settings);
    }
}