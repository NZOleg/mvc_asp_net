﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Store
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        //public static IWebHost BuildWebHost(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //    .UseContentRoot(Directory.GetCurrentDirectory())
        //    .ConfigureAppConfiguration((hostingContext, config) => {
        //        var env = hostingContext.HostingEnvironment;
        //        config.AddJsonFile("appsettings.json",
        //        optional: true, reloadOnChange: true)
        //        .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
        //        optional: true, reloadOnChange: true);
        //        config.AddEnvironmentVariables();
        //        if (args != null)
        //        {
        //            config.AddCommandLine(args);
        //        }
        //    })
        //    .UseIISIntegration()
        //    .UseDefaultServiceProvider((context, options) => {
        //        options.ValidateScopes =
        //        context.HostingEnvironment.IsDevelopment();
        //    })
        //        .UseStartup<Startup>()
        //        .UseDefaultServiceProvider(options =>
        //            options.ValidateScopes = false)
        //        .Build();

        public static IWebHost BuildWebHost(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseDefaultServiceProvider(options =>
            options.ValidateScopes = false)
        .Build();
    }
}