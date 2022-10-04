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
        public static IApplicationBuilder LoadClientPlugins(this IApplicationBuilder app, Action<JsPluginConfig>? cfg = null)
        {
            var config = new JsPluginConfig();
            if (cfg != null)
                cfg.Invoke(config);

            var assembly = Assembly.GetAssembly(typeof(Setup));
            if (assembly == null)
                return app;

            //app.Map(config.DestinationMappedPath, builder =>
            //{
            //    var provider = new ManifestEmbeddedFileProvider(
            //        assembly: assembly, "Libraries");

            //    builder.UseStaticFiles(new StaticFileOptions
            //    {
            //        FileProvider = provider,
            //    });
            //});

            if (!config.OutputToWwwroot)
            {
                app.Map(config.DestinationMappedPath, builder =>
                {
                    var provider = new ManifestEmbeddedFileProvider(
                        assembly: assembly, "Libraries");

                    builder.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = provider,
                    });
                });
            }
            else
            {
                
            }

            var names = assembly.GetManifestResourceNames().Where(name => name.StartsWith($"{typeof(Setup).Namespace}.Libraries"));
            foreach (var name in names)
            {
                var info = assembly.GetManifestResourceInfo(name);
                using (var stream = assembly.GetManifestResourceStream(name))
                {
                    if (stream == null)
                        continue;

                    if (!config.OutputToWwwroot)
                    {
                        var p = $"{config.DestinationMappedPath}/{name}";
                        app.Map(p, builder =>
                        {
                            var provider = new EmbeddedFileProvider(Assembly.GetAssembly(typeof(Setup)), name);
                            builder.UseStaticFiles(new StaticFileOptions
                            {
                                FileProvider = provider,
                            });
                        });
                    }
                    else
                    {
                        var dest = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", config.DestinationMappedPath.Replace("/", string.Empty).Replace("\\", string.Empty), name);
                        if (dest == null)
                            continue;

                        Directory.CreateDirectory(Path.GetDirectoryName(dest));
                        using (var fileStream = new FileStream(dest, FileMode.Create))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }

            return app;
        }
    }
}
