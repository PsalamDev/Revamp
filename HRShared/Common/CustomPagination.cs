using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRShared.Common
{
    public class CustomPagination<T>
    {
        public int pageSize { get; set; }
        public int pageNumber { get; set; }

        public int TotalCount { get; set; }

        public T modelresult { get; set; }
    }

    public class CustomPaginationReq<T>
    {
        public int pageSize { get; set; }
        public int pageNumber { get; set; }

        public T model { get; set; }
    }

    public class PaginationRequest
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

    }
}
