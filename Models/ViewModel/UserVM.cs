using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NgoPhucThinhMystore.Models.ViewModel
{
    public class UserVM
    {
        
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
            public UserVM()
            {
                this.Customers = new HashSet<CustomerVM>();
            }

            public string Username { get; set; }
            public string Password { get; set; }
            public string UserRole { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<CustomerVM> Customers { get; set; }
        
    }
}
