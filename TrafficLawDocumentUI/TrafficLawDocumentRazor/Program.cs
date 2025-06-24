using BussinessObject;
using Microsoft.EntityFrameworkCore;
using TrafficLawDocumentRazor.Services;

namespace TrafficLawDocumentRazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Register HttpContextAccessor
            builder.Services.AddHttpContextAccessor();

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

            // Register NewsApiService
            builder.Services.AddScoped<INewsApiService, NewsApiService>(provider =>
            {
                var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient("API");
                var logger = provider.GetRequiredService<ILogger<NewsApiService>>();
                return new NewsApiService(httpClient, logger);
            });
            builder.Services.AddHttpClient<IUserApiService, UserApiService>();


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

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
