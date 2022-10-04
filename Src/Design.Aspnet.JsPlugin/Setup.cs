using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

            var names = assembly.GetManifestResourceNames().Where(name => name.StartsWith($"{typeof(Setup).Namespace}.Libraries"));

            if (config.OutputToWwwroot)
            {
                var nameToPathDict = JsPluginHelper.GetLibraryDictInternal();

                var sanitizedDestinationSubfolder = config.DestinationMappedPath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                foreach (var name in names)
                {
                    var info = assembly.GetManifestResourceInfo(name);
                    using (var stream = assembly.GetManifestResourceStream(name))
                    {
                        if (stream == null)
                            continue;

                        var dest = Path.Join(Directory.GetCurrentDirectory(), "wwwroot", sanitizedDestinationSubfolder, nameToPathDict[name]);
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
            else
            {
                app.Map(config.DestinationMappedPath, builder =>
                {
                    builder.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = JsPluginHelper.GetProvider(),
                    });
                });
            }

            return app;
        }
    }
}
