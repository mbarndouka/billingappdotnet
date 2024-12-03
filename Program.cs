using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Resend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/billing/login";
        options.AccessDeniedPath = "/billing/AccesDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AccountantOrAdmin", policy => policy.RequireRole("Admin", "Accountant"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});

// Configure Resend
builder.Services.AddResend(options =>
{
    options.ApiKey = builder.Configuration["Resend:ApiKey"];
});

// Add other necessary services
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<PdfService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
