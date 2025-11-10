using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc; 

namespace NgoPhucThinhMystore.Models.ViewModel
{
    public class ProductSearchVM
    {
        public string SearchTerm { get; set; }

        //các tiêu chí để search theo giá
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
        public string SortOrder { get; set; }
        //danh sách sản phẩm thỏa điều kiện tìm kiếm
        //public List<Product> Products { get; set; }

        public PagedList.IPagedList<Product> Products { get; set; }
    }
}