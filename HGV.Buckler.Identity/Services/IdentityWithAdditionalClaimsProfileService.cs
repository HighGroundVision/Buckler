using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using HGV.Daedalus;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace HGV.Buckler.Identity.Services
{
    public class IdentityWithAdditionalClaimsProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<IdentityUser> _claimsFactory;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IDotaApiClient _client;
        private readonly IConfiguration _configuration;
 
        public IdentityWithAdditionalClaimsProfileService(IConfiguration configuration, IDotaApiClient client, UserManager<IdentityUser> userManager,  IUserClaimsPrincipalFactory<IdentityUser> claimsFactory)
        {
            _configuration = configuration;
            _client = client;
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

            claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

            foreach (var l in logins)
            {
                if(l.LoginProvider == "Discord")
                    AddDiscordClaims(claims, l);
                else if(l.LoginProvider == "Steam")
                    await AddSteamClaims(claims, l);
            }

            context.IssuedClaims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
        }

        private async Task AddSteamClaims(List<Claim> claims, UserLoginInfo l)
        {
            var steamId = l.ProviderKey.Replace("https://steamcommunity.com/openid/id/", "");
            claims.Add(new Claim("steam_id", steamId));

            bool.TryParse(_configuration["Authentication:Steam:IncludeProfile"], out bool includeProfile);

            if(includeProfile)
            {
                var id = ulong.Parse(steamId);
                var profile = await _client.GetPlayerSummary(id);
                if(profile != null)
                {
                    claims.Add(new Claim("steam_persona", profile.Persona));
                    claims.Add(new Claim("steam_avatar", profile.AvatarLarge));
                }
            }
        }

        private void AddDiscordClaims(List<Claim> claims, UserLoginInfo l)
        {
            var discordId = l.ProviderKey;
            claims.Add(new Claim("discord_id", discordId));
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
