using Design.Aspnet.JsPlugin;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// web optimizer as POC of able to minify such files
builder.Services.AddWebOptimizer(pipeline =>
{
    var provider = JsPluginHelper.GetProvider();
    var libPaths = JsPluginHelper.GetLibraryPaths();
    pipeline.AddJavaScriptBundle("/dsg-plugin/combined.js", libPaths).UseFileProvider(provider).MinifyJavaScript();

    // this does not work
    //foreach (var asset in pipeline.AddFiles("text/javascript", "/dsg-plugin/CategoryA/ca.js").)
    //{
    //    asset.UseFileProvider(provider).MinifyJavaScript();
    //}

    // control example of minifying site.js
    pipeline.AddFiles("text/javascript", "/js/site.js").Single().MinifyJavaScript();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// web optimizer usage
app.UseWebOptimizer();
app.UseStaticFiles();

// loads the client plugins into provided directory
app.LoadClientPlugins(config =>
{
    config.DestinationMappedPath = "/dsg-plugin";
    config.OutputToWwwroot = false;
});

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
