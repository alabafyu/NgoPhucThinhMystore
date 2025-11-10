using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NgoPhucThinhMystore.Models.ViewModel
{
    public class HomeProductVM
    {
        public string SearchTerm { get; set; }

        

        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
        public string SortOrder { get; set; }
        //danh sách sản phẩm thỏa điều kiện tìm kiếm
        public List<Product> Products { get; set; }
        public List<Product> FeaturedProducts { get; set; }
        public PagedList.IPagedList<Product> NewProducts { get; set; }
    }
}