using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NgoPhucThinhMystore.Models
{
    public class Metadata
    {
        public class CategoryMetadata
        {
            [HiddenInput]
            public int CategoryID { get; set; }

            [Required]
            [StringLength(50, MinimumLength = 5)]
            public string CategoryName { get; set; }
        }
        public class UserMetadata
        {
            [Required(ErrorMessage = "Username is required!")]
            [StringLength(30, MinimumLength = 5)]
            [RegularExpression("\r\n ^[a-zA-Z0-9]([._-](?![._-])|[a-zA-Z0-9]){3,18}[a-zA-Z0-9]$ \r\n")]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            public string Role { get; set; }
        }
        public class CustomerMetadata
        {

        }
        public class SupplierMetadata
        {

        }
        public class ProductMetadata
        {
            [Display(Name = "Mã sản phẩm")]
            public int ProID { get; set; }

            [StringLength(50)]
            [Required(ErrorMessage = "Phải nhập tên sản phẩm")]
            [Display(Name = "Mã sản phẩm")]
            public string ProName { get; set; }

            [Display(Name = "Chủng loại sản phẩm")]
            public int CatID { get; set; }
            [Display(Name = "Đường dẫn ảnh sản phẩm")]
            [DefaultValue("~/Content/image/default_img.jfjf")]
            public string ProImage { get; set; }

            [Display(Name = "Mô tả sản phẩm")]
            public string NameDescription { get; set; }
            [DefaultValue(true)]
            public System.DateTime CreatedDate { get; set; }
        }
    }
}