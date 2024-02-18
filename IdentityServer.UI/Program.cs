using System.Net;
using IdentityServer.Data;
using IdentityServer.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddValidators();
await builder.Services.AddDataServicesAsync(builder.Configuration, builder.Environment.IsDevelopment());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

await app.EnsureDatabaseOnStartupAsync(app.Environment.IsDevelopment());

app.UseStatusCodePages(context => {
    var response = context.HttpContext.Response;
    if (response.StatusCode == (int) HttpStatusCode.Unauthorized) {
        response.Redirect("auth/login");
    }

    return Task.CompletedTask;
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.Run();