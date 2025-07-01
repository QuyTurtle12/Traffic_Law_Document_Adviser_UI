using BussinessObject;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TrafficLawDocumentRazor.Services;
using TrafficLawDocumentRazor.Hubs;

namespace TrafficLawDocumentRazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Add SignalR
            builder.Services.AddSignalR();

            var connectionString = builder.Configuration.GetConnectionString("MyCnn");

            // Register DbContext with DI
            builder.Services.AddDbContext<TrafficLawDocumentDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register HttpClient for making API calls
            builder.Services.AddHttpClient("API", client =>
            {
                var apiSettings = builder.Configuration.GetSection("ApiSettings:BaseUrl").Value;
                client.BaseAddress = new Uri(apiSettings);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy => policy.RequireClaim("role", "Admin"));
                options.AddPolicy("RequireExpertOrAdmin", policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.HasClaim("role", "Admin") ||
                        ctx.User.HasClaim("role", "Expert")));
            });


            // Register NewsApiService
            builder.Services.AddScoped<INewsApiService, NewsApiService>(provider =>
            {
                var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient("API");
                var logger = provider.GetRequiredService<ILogger<NewsApiService>>();
                return new NewsApiService(httpClient, logger);
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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            // Map SignalR Hub
            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}
