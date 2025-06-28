using BussinessObject;
using Microsoft.EntityFrameworkCore;
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

            // Map SignalR Hub
            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}
