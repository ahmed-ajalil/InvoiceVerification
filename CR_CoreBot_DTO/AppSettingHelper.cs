using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CR_CoreBot_DTO
{
    public static class AppSettingHelper
    {
        public static IConfiguration config;

        public static void Initialize(IConfiguration Configuration)

        {
            config = Configuration;
        }
    }
}
