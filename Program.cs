using System;
using System.Text.Json.Serialization;
using HospitalAPI.Models;
using HospitalAPI.Service;

var serviceHospital = new ServiceConsulta(new DbgeralContext());
var servicePaciente = new ServicePaciente(new DbgeralContext()); // Instância do serviço de pacientes
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
app.MapPost("/consultas", async (int idPaciente, int idMedico, DateTime dataConsulta, string metodoPagamento) =>
{
    var retorno = await serviceHospital.CriarNovaConsulta(idPaciente, idMedico, dataConsulta, metodoPagamento);
    return Results.Ok(retorno);
}).WithName("consultas/add").WithOpenApi();

// Atualiza a consulta
app.MapPut("/consultas", async (int id, DateTime dataConsulta, bool cancelar) =>
{
    var retorno = await serviceHospital.AtualizaData(id, dataConsulta, cancelar);
    return Results.Ok(retorno);
}).WithName("consultas/update").WithOpenApi();

// Deleta uma consulta
app.MapDelete("/consultas", async (int id) =>
{
    var retorno = await serviceHospital.DeletaConsulta(id);
    return Results.Ok(retorno);
}).WithName("consultas/delete").WithOpenApi();


#endregion
#region Pacientes

// Pacientes: Busca todos os pacientes
app.MapGet("/pacientes", async () =>
{
    var pacientes = await servicePaciente.BuscarTodosPacientes();
    return Results.Ok(pacientes);
}).Produces<IEnumerable<Paciente>>().WithName("pacientes").WithOpenApi();

// Pacientes: Busca paciente por ID
app.MapGet("/pacientes/{id}", async (int id) =>
{
    var paciente = await servicePaciente.BuscarPacientePorId(id);
    if (paciente == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(paciente);
}).Produces<Paciente>().WithName("pacientes/id").WithOpenApi();

// Endpoint para criar um novo paciente
app.MapPost("/pacientes", async (string nome, DateTime data_Nasc, decimal peso, decimal altura, int? telefone, int? endereco) =>
{
    var novoPaciente = await servicePaciente.CriarNovoPaciente(nome, data_Nasc, peso, altura, telefone, endereco);

    // Aqui você pode retornar o paciente criado ou uma resposta de sucesso, conforme necessário
    return Results.Created($"/pacientes/{novoPaciente.Id}", novoPaciente);
}).Produces<Paciente>().WithName("pacientes/add").WithOpenApi();



// Endpoint para atualizar um paciente
app.MapPut("/pacientes/{id}", async (int id, string nome, DateTime data_Nasc, decimal peso, decimal altura, int? telefone, int? endereco) =>
{
    var pacienteAtualizado = await servicePaciente.AtualizarPaciente(id, nome, data_Nasc, peso, altura, telefone, endereco);

    // Verifica se o paciente foi encontrado e atualizado com sucesso
    if (pacienteAtualizado != null)
    {
        return Results.Ok(pacienteAtualizado); // Retorna o paciente atualizado
    }
    else
    {
        return Results.NotFound($"Paciente com ID {id} não encontrado."); // Retorna erro 404 se o paciente não foi encontrado
    }
}).Produces<Paciente>().WithName("pacientes/update").WithOpenApi();


// Pacientes: Deleta um paciente
app.MapDelete("/pacientes/{id}", async (int id) =>
{
    var pacienteDeletado = await servicePaciente.DeletarPaciente(id);
    if (pacienteDeletado == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(pacienteDeletado);
}).Produces<Paciente>().WithName("pacientes/delete").WithOpenApi();

#endregion

app.Run();