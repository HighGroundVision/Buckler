﻿using System;
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
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IIdentityServerInteractionService _interaction;

        public LogoutModel(SignInManager<IdentityUser> signInManager, IIdentityServerInteractionService interaction, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _interaction = interaction;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string logoutId = null)
        {
            if(string.IsNullOrWhiteSpace(logoutId))
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");
                return RedirectToPage("./LogoutSuccess");
            }
            else
            {
                var context = await _interaction.GetLogoutContextAsync(logoutId);

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(context);
                this.HttpContext.Response.Headers.Add("X-LogoutContext", json);

                 await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");

                 if(string.IsNullOrWhiteSpace(context.PostLogoutRedirectUri))
                    return RedirectToPage("./LogoutSuccess");
                else
                    return Redirect(context.PostLogoutRedirectUri);
            }
        }

        /*
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            _logger.LogInformation("User logged out.");
            await _signInManager.SignOutAsync();
            
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
        */
    }
}
