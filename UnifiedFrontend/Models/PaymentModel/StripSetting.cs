using System.Collections.Generic;


namespace UnifiedFrontend.Models.PaymentModel
{


    public class PaginatedResult<T>
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Items { get; set; }
    }

}
