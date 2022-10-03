using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Design.Aspnet.JsPlugin
{
    public static class Setup
    {
        public static IApplicationBuilder LoadClientPlugins(this IApplicationBuilder app, string mappedPath = "/dsg-plugin")
        {
            app.Map(mappedPath, builder =>
            {
                var provider = new ManifestEmbeddedFileProvider(
                    assembly: Assembly.GetAssembly(typeof(Setup)), "Libraries");
                // var provider = new EmbeddedFileProvider(Assembly.GetAssembly(typeof(Setup)), "Design.Aspnet.JsPlugin.Libraries");

                builder.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = provider,
                });
            });

            return app;
        }
    }
}
