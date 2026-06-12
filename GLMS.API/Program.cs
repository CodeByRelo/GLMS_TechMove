using GLMS.Core.Interfaces;
using GLMS.Infrastructure.Data;
using GLMS.Infrastructure.Repositories;
using GLMS.API.Services;
using GLMS.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// CONTROLLERS
// =====================================================
builder.Services.AddControllers();

// =====================================================
// SWAGGER
// =====================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================================================
// DATABASE
// =====================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("GLMS.Infrastructure")
    )
);

// =====================================================
// REPOSITORIES (NOW API OWNS THIS)
// =====================================================
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

// =====================================================
// SERVICES (NOW API OWNS BUSINESS LOGIC)
// =====================================================
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IClientService, ClientService>();

// =====================================================
// EXTERNAL SERVICES
// =====================================================
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

// =====================================================
// CORS (MVC WILL CALL API)
// =====================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// =====================================================
// PIPELINE
// =====================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowMVC");

app.UseAuthorization();

app.MapControllers();

app.Run();