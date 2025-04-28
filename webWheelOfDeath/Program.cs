var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add the session service so Context.Session is available
builder.Services.AddSession();

var app = builder.Build();

// Use the session middleware
app.UseSession();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// add your own types
//public void ConfigureServices(IServiceCollection services)
//{
//    services.AddTransient()
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// route that only maps to IDs that are strings
//app.MapControllerRoute(
//    name: "echoString",
//    pattern: "{controller=Home}/{action=Post}/{id:string?}"); // constraint: ensure id is a string

// route that only maps to ids that are integers
//app.MapControllerRoute(
//    name: "echoInt",
//    pattern: "{controller=Home}/{action=Post}/{id:int?}"); // constraint: ensure id is an int

app.UseFileServer();

app.Run();
