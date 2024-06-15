using HospitalAPI.Models;
using HospitalAPI.Service;

var builder = WebApplication.CreateBuilder(args);

var serviceHospital = new ServiceHospital(new DbgeralContext());
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/consultas", async () =>
    {
        var consultas = await serviceHospital.BuscarTodasConsultas();
        return Results.Ok(consultas);;
    })
    .WithName("consultas")
    .WithOpenApi();

app.Run();
