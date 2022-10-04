using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Design.Aspnet.JsPlugin
{
    public static class JsPluginHelper
    {
        public static IFileProvider GetProvider()
        {
            var provider = new ManifestEmbeddedFileProvider(
                        assembly: typeof(Setup).Assembly, "Libraries");
            return provider;
        }

        public static string[] GetLibraryPaths()
        {
            return GetLibraryDict(false).Select(x => x.Value).ToArray();
        }

        internal static Dictionary<string, string> GetLibraryDictInternal()
        {
            return GetLibraryDict(false);
        }

        internal static Dictionary<string, string> GetLibraryDict(bool includeRoot)
        {
            var assembly = typeof(Setup).Assembly;
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
                                // Console.WriteLine("D {0} - Start Element {1}", reader.Depth, reader.Name);
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
                                    for (var i = !includeRoot ? 3 : 2; i < reader.Depth; i++)
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
                                // Console.WriteLine("D {0} - Text Node: {1}", reader.Depth, reader.Value);

                                if (readyToReadName)
                                {
                                    nameToPathDict.Add(reader.Value, directorySB.ToString());
                                    directorySB.Clear();
                                    readyToReadName = false;
                                }
                                break;
                                //case XmlNodeType.EndElement:
                                //    Console.WriteLine("D {0} - End Element {1}", reader.Depth, reader.Name);
                                //    break;
                                //default:
                                //    Console.WriteLine("D {0} - Other node {1} with value {2}", reader.Depth, reader.NodeType, reader.Value);
                                //    break;
                        }
                    }
                }
            }

            return nameToPathDict;
        }
    }
}
