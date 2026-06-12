using GLMS.Web.Services;
using GLMS.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// MVC SERVICES
// =====================================================
builder.Services.AddControllersWithViews();

// =====================================================
// HTTP CLIENT WRAPPER (IMPORTANT FIX)
// =====================================================
// This registers ApiClient properly so DI can resolve it
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7085/");
});

// =====================================================
// API-BASED SERVICES (WRAPPERS AROUND HTTP CALLS)
// =====================================================
builder.Services.AddScoped<IClientService, ClientApiService>();
builder.Services.AddScoped<IContractService, ContractApiService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestApiService>();
builder.Services.AddScoped<ICurrencyService, CurrencyApiService>();

// =====================================================
var app = builder.Build();

// =====================================================
// PIPELINE CONFIGURATION
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