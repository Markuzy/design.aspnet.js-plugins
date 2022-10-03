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
app.LoadClientPlugins("/dsg-plugin");
```

## WIP / Feasibility Evaluation
- Selective loading
- Script merges
- Script minification

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[MIT](https://choosealicense.com/licenses/mit/)
