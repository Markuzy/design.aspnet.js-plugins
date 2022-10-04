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

            //app.Map(config.DestinationMappedPath, builder =>
            //{
            //    var provider = new ManifestEmbeddedFileProvider(
            //        assembly: assembly, "Libraries");

            //    builder.UseStaticFiles(new StaticFileOptions
            //    {
            //        FileProvider = provider,
            //    });
            //});


            var names = assembly.GetManifestResourceNames().Where(name => name.StartsWith($"{typeof(Setup).Namespace}.Libraries"));

            if (config.OutputToWwwroot)
            {
                var nameToPathDict = new Dictionary<string, string>();
                var manifest = assembly.GetManifestResourceNames().Where(name => name.EndsWith("Manifest.xml")).Single();

                using (var s = assembly.GetManifestResourceStream(manifest))
                {
                    using (var reader = XmlReader.Create(s))
                    {
                        var directorySB = new StringBuilder();
                        var indexToDirectoryDict = new Dictionary<int, string>();
                        var readyToReadName = false;
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    Console.WriteLine("D {0} - Start Element {1}", reader.Depth, reader.Name);

                                    if (reader.Name == "Directory")
                                    {
                                        var n = reader.GetAttribute("Name");
                                        if (!indexToDirectoryDict.TryAdd(reader.Depth, n))
                                            indexToDirectoryDict[reader.Depth] = n;
                                    }
                                    else if (reader.Name == "File")
                                    {
                                        // depth 3 is the start of directories under the root directory
                                        // this ignores the root directory of "Libraries" as part of building path
                                        for (var i = 3; i < reader.Depth; i++)
                                        {
                                            directorySB.Append(Path.DirectorySeparatorChar).Append(indexToDirectoryDict[i]);
                                        }
                                        directorySB.Append(Path.DirectorySeparatorChar).Append(reader.GetAttribute("Name"));
                                    }
                                    else if (reader.Name == "ResourcePath")
                                    {
                                        readyToReadName = true;
                                    }
                                    break;
                                case XmlNodeType.Text:
                                    Console.WriteLine("D {0} - Text Node: {1}", reader.Depth,
                                             reader.Value);

                                    if (readyToReadName)
                                    {
                                        nameToPathDict.Add(reader.Value, directorySB.ToString());
                                        directorySB.Clear();
                                        readyToReadName = false;
                                    }
                                    break;
                                case XmlNodeType.EndElement:
                                    Console.WriteLine("D {0} - End Element {1}", reader.Depth, reader.Name);
                                    break;
                                default:
                                    Console.WriteLine("D {0} - Other node {1} with value {2}", reader.Depth,
                                                    reader.NodeType, reader.Value);
                                    break;
                            }
                        }
                    }
                }

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
                    var provider = new ManifestEmbeddedFileProvider(
                        assembly: assembly, "Libraries");

                    builder.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = provider,
                    });
                });
            }


                

            return app;
        }
    }
}
