using System;
using System.Text.Json.Serialization;
using HospitalAPI.Models;
using HospitalAPI.Service;

var serviceHospital = new ServiceHospital(new DbgeralContext());
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddControllers().AddNewtonsoftJson(); // Certifique-se de que não há conflitos com System.Text.Json

var app = builder.Build();

// Middleware para Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Consultas

// Busca todas as consultas
app.MapGet("/consultas", async () =>
{
    var consultas = await serviceHospital.BuscarTodasConsultas();
    return Results.Ok(consultas);
}).WithName("consultas").WithOpenApi();

// Busca consulta por ID
app.MapGet("/consultas/id", async (int IdConsulta) =>
{
    var consulta = await serviceHospital.BuscarConsultaId(IdConsulta);
    return Results.Ok(consulta);
}).WithName("consultas/id").WithOpenApi();

// Cria nova consulta
app.MapPost("/consultas", async (string nomePaciente, string nomeMedico, DateTime dataConsulta, string metodoPagamento) =>
{
    var retorno = await serviceHospital.CriarNovaConsulta(nomePaciente, nomeMedico, dataConsulta, metodoPagamento);
    return Results.Ok(retorno);
}).WithName("consultas/add").WithOpenApi();

// Atualiza a data da consulta
app.MapPut("/consultas", async (int id, DateTime dataConsulta) =>
{
    var retorno = await serviceHospital.AtualizaData(id, dataConsulta);
    return Results.Ok(retorno);
}).WithName("consultas/update").WithOpenApi();

// Deleta uma consulta
app.MapDelete("/consultas", async (int id) =>
{
    var retorno = await serviceHospital.DeletaConsulta(id);
    return Results.Ok(retorno);
}).WithName("consultas/delete").WithOpenApi();


#endregion


app.Run();