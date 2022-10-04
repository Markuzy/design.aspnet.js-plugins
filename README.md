# design.aspnet.js-plugins
POC for usage of embedded JS scripts in class library in .NET 6.

## Installation

There is no installation provided, please refer to Design.Aspnet.JsPlugin as a candidate for scaffolding your custom JS scripts.

## Usage
Example/Example.Razer.WebApp - The example web app using the class library embedded javascript.
Src/Design.Aspnet.JsPlugin - The class library providing the embedded javascript and extensions.

LoadClientPlugins is an extension provided for your usage in program.cs


```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// loads the client plugins into provided directory
app.LoadClientPlugins(config =>
{
    config.DestinationMappedPath = "/dsg-plugin";
    config.OutputToWwwroot = false;
});
```

## WIP / Feasibility Evaluation
- Selective loading (Likely not doing as a feature, as we can use Web Optimizer library to selectively choose files)
- Script merges (Done POC with Web Optimizer library bundle feature)
- Script minification (Done POC with Web Optimizer library minification feature that requires bundle feature)

See example usage of POC Web Optimizer
```csharp
// web optimizer as POC of able to minify such files
builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.AddJavaScriptBundle("/dsg-plugin/combined.js", JsPluginHelper.GetLibraryPaths()).UseFileProvider(JsPluginHelper.GetProvider()).MinifyJavaScript();
    pipeline.MinifyJsFiles();
    
});
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[MIT](https://choosealicense.com/licenses/mit/)
