using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static NgoPhucThinhMystore.Models.Metadata;

namespace NgoPhucThinhMystore.Models
{
    public class PartialClasses
    {

        [MetadataType(typeof(UserMetadata))]
        public partial class user
        {
            [NotMapped]
            [Compare("Password")]
            public string ComfirmedPassword { get; set; }
        }

    }
}