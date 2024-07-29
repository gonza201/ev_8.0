using MercDevs_ej2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//Para iniciar sesión y prohibir ingresos
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
//FIN
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// coneccion a la bdd
builder.Services.AddDbContext <MercyDeveloperContext>(options => 
options.UseMySql(builder.Configuration.GetConnectionString("connection"),
Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.25-mariadb")));
//end bdd
//Para el login 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Ingresar";

        options.ExpireTimeSpan = TimeSpan.FromDays(1);
    });
//FinParal ogin
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Ingresar}/{id?}");

app.Run();

WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder(args);

// Configura los servicios
webAppBuilder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.WithOrigins("https://localhost:7063/Datosfichatecnicas/FichaTecnica/1") // Cambia esto a la URL de tu frontend
                             .AllowAnyMethod()
                             .AllowAnyHeader();
        });
});

webAppBuilder.Services.AddControllers();

var webApp = webAppBuilder.Build();

// Configura el middleware
if (webApp.Environment.IsDevelopment())
{
    webApp.UseDeveloperExceptionPage();
}
else
{
    webApp.UseExceptionHandler("/Home/Error");
    webApp.UseHsts();
}

webApp.UseCors("AllowSpecificOrigin");

webApp.UseHttpsRedirection();
webApp.UseStaticFiles();
webApp.UseRouting();
webApp.UseAuthorization();

webApp.MapControllers();

webApp.Run();
