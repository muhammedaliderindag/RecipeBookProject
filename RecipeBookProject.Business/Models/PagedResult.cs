using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Models
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int TotalCount { get; }
        public int Page { get; }
        public int PageSize { get; }

        public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
            => (Items, TotalCount, Page, PageSize) = (items, totalCount, page, pageSize);
    }
}
