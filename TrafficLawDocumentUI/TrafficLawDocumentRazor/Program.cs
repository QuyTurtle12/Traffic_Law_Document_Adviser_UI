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
            // Register HttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            var connectionString = builder.Configuration.GetConnectionString("MyCnn");

            // Register DbContext with DI
            builder.Services.AddDbContext<TrafficLawDocumentDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register AuthTokenHandler
            builder.Services.AddTransient<AuthTokenHandler>();

            // Apply to HTTP clients
            builder.Services.AddHttpClient("API", client =>
            {
                var apiSettings = builder.Configuration.GetSection("ApiSettings:BaseUrl").Value;
                client.BaseAddress = new Uri(apiSettings);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<AuthTokenHandler>();

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
                options.AddPolicy("RequireStaff", policy => policy.RequireClaim("role", "Staff"));
                options.AddPolicy("RequireExpertOrStaff", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("role", "Staff") ||
                    ctx.User.HasClaim("role", "Expert")));
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
            
            // Register UserApiService
            builder.Services.AddScoped<IUserApiService, UserApiService>(provider =>
            {
                var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient("API");
                var configuration = provider.GetRequiredService<IConfiguration>();
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                return new UserApiService(httpClient, configuration, httpContextAccessor);
            });
            
            builder.Services.AddHttpClient<ILawDocumentsApiService, LawDocumentsApiService>();

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
