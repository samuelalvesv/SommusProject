using SommusProject.Data;
using SommusProject.Repositories;
using SommusProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AlertDengueDbContext>();
builder.Services.AddScoped<AlertDengueRepository>();
builder.Services.AddHttpClient<AlertDengueService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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