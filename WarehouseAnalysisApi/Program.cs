using WarehouseAnalysisApi.Config;
using WarehouseAnalysisApi.Repository;
using WarehouseAnalysisApi.Service;
using WarehouseAnalysisApi.Service.ServiceImpl;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Đăng ký DbConnectionConfig
builder.Services.AddSingleton<AdomdConnectionFactory>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<Cube1Service, Cube1ServiceImpl>();
builder.Services.AddScoped<Cube1Repository>();

builder.Services.AddScoped<Cube2Service, Cube2ServiceImpl>();
builder.Services.AddScoped<Cube2Repository>();

builder.Services.AddScoped<Cube3Service, Cube3ServiceImpl>();
builder.Services.AddScoped<Cube3Repository>();

builder.Services.AddScoped<Cube4Service, Cube4ServiceImpl>();
builder.Services.AddScoped<Cube4Repository>();

builder.Services.AddScoped<Cube5Service, Cube5ServiceImpl>();
builder.Services.AddScoped<Cube5Repository>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();