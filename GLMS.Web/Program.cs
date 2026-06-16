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
var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];

builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});


// =====================================================
// API-BASED SERVICES (WRAPPERS AROUND HTTP CALLS)
// =====================================================
builder.Services.AddScoped<IClientService, ClientApiService>();
builder.Services.AddScoped<IContractService, ContractApiService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestApiService>();
builder.Services.AddScoped<ICurrencyService, CurrencyApiService>();

// =====================================================
// AUTH SERVICE (FOR LOGIN/LOGOUT)
// =====================================================
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AuthApiService>();

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

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();