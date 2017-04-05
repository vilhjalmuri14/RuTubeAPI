using System.Collections.Generic;

namespace  RuTubeAPI.Services.Utilities
{
    public class PageInfo
    {
        /// <summary>
        /// Should store the number of pages.
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// The number of items in each page (10 in our case)
        /// </summary>
        public int PageSize { get; set;}

        /// <summary>
        /// A 1-based index of the current page being returned
        /// </summary>
        public int? PageNumber { get; set;}

        /// <summary>
        /// The total number of items in the collection
        /// </summary>
        public int TotalNumberOfItems { get; set; }
    }


    public class PageResult<T>
    {
        /// <summary>
        /// The list of courses
        /// </summary>
        public List<T> Items { get; set; }

        public PageInfo Paging { get; set; }
    }
}