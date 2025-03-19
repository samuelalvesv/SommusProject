using SommusProject.Data;
using SommusProject.Options;
using SommusProject.Repositories;
using SommusProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AlertDengueOptions>(builder.Configuration.GetSection("AlertDengue"));

builder.Services.AddScoped<IAlertDengueDbContext, AlertDengueDbContext>();
builder.Services.AddScoped<IAlertDengueRepository, AlertDengueRepository>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>();

builder.Services.AddScoped<IAlertDengueService, AlertDengueService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();