using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace HGV.Buckler.Identity.Services
{
    public class IdentityWithAdditionalClaimsProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<IdentityUser> _claimsFactory;
        private readonly UserManager<IdentityUser> _userManager;
 
        public IdentityWithAdditionalClaimsProfileService(UserManager<IdentityUser> userManager,  IUserClaimsPrincipalFactory<IdentityUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }
 
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);
            var logins = await _userManager.GetLoginsAsync(user);
 
            var claims = principal.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

            claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

            foreach (var l in logins)
            {
                if(l.LoginProvider == "Discord")
                {
                    var discordId = l.ProviderKey;
                    claims.Add(new Claim("discord", discordId));
                }
                else if(l.LoginProvider == "Steam")
                {
                    var steamId = l.ProviderKey.Replace("https://steamcommunity.com/openid/id/", "");
                    claims.Add(new Claim("steam", steamId));
                    claims.Add(new Claim("dota", steamId));
                }
                
            }
 
            context.IssuedClaims = claims;
        }
 
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
