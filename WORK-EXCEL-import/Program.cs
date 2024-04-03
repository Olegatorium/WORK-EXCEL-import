using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// add services into IoC container
builder.Services.AddScoped<IWorkService, WorkService>();

var app = builder.Build();


app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
