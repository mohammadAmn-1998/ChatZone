using ChatZone.ApplicationCore.Services.Installer;
using ChatZone.Domain.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ChatDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("default"));
});

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;

}).AddCookie(config =>
{
	config.LoginPath = "/auth";
	config.ExpireTimeSpan = TimeSpan.FromDays(3);
	config.LogoutPath = "/auth/logout";
	config.AccessDeniedPath = "/auth";

});

builder.Services.AddAuthorization();

builder.Services.AddAuthorization(options =>
{
});

#region IOC

builder.Services.AddCustomServices();

#endregion

var app = builder.Build();


if (app.Environment.IsDevelopment())
{

	app.UseDeveloperExceptionPage();
	app.UseHsts();

}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{

	endpoints.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}");

});


app.Run();
