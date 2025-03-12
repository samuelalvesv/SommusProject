using SommusProject.Data;
using SommusProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AlertDengueDbContext>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient<AlertDengueService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();