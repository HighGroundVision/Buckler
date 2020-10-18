using HGV.Daedalus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HGV.Buckler.Identity.Services
{
    public class SteamKeyProvider : ISteamKeyProvider
    {
        private readonly IConfiguration configuration;

        public SteamKeyProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetKey()
        {
            return this.configuration["Authentication:Steam:ApplicationKey"];
        }
    }
}
