using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace HGV.Buckler.Identity.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutSuccessModel : PageModel
    {

        public LogoutSuccessModel()
        {
        }
    }
}
