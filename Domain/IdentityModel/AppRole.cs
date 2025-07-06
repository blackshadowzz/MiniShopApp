using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IdentityModel
{
    public enum AppRole
    {
        [Display(Description = "Super Administrator")]
        SuperAdmin,
        [Display(Description = "Administrator")]
        Admin,
        [Display(Description = "Cashier")]
        Cashier,
        [Display(Description = "Order")]
        Order,
        [Display(Description = "Cooking Chef")]
        Chef,
        [Display(Description = "Normal Service")]
        Service,
    }
}
