using GLMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using GLMS.Core.Interfaces;
using GLMS.Infrastructure.Repositories;
using GLMS.Web.Services;
using GLMS.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC SERVICES
// =====================================================
builder.Services.AddControllersWithViews();


// 2. DATABASE
// =====================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("GLMS.Infrastructure")
    )
);


// 3. REPOSITORIES (Data Layer)
// =====================================================
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();


// 4. SERVICES (Business Layer)
// =====================================================
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientService, ClientService>();


// 5. EXTERNAL SERVICES (API / HTTP CLIENTS)
// =====================================================
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();


// =====================================================
var app = builder.Build();


// 6. PIPELINE CONFIGURATION
// =====================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();