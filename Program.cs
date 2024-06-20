using System.Text.Json.Serialization;
using HospitalAPI.DTOs;
using HospitalAPI.Models;
using HospitalAPI.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbgeralContext>();
builder.Services.AddScoped<ServicePaciente>();
builder.Services.AddScoped<ServiceMedico>();
builder.Services.AddScoped<ServiceConsulta>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddControllers().AddNewtonsoftJson(); // Certifique-se de que não há conflitos com System.Text.Json

builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod())
);
var app = builder.Build();

// Middleware para Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

#region Consultas

// Busca todas as consultas
app.MapGet("/consultas", async (ServiceConsulta serviceConsulta) =>
{
    var consultas = await serviceConsulta.BuscarTodasConsultas();

    var consultasDto = consultas.Select(c => new ConsultaResponse(c.Id, c.IdPacienteNavigation!.Nome,
        c.IdPaciente!.Value, c.IdMedicoNavigation!.Nome, c.IdMedico!.Value, c.Data, c.Valor, c.GetMetodoPagamento(),
        c.Cancelada));

    return Results.Ok(consultasDto);
}).WithName("consultas").WithOpenApi();

// Busca consulta por ID
app.MapGet("/consultas/{id}", async (ServiceConsulta serviceConsulta, int id) =>
{
    var consulta = await serviceConsulta.BuscarConsultaId(id);
    return Results.Ok(consulta);
}).WithName("consultas/id").WithOpenApi();

// Cria nova consulta
app.MapPost("/consultas", async (ServiceConsulta serviceConsulta, ConsultaRequest consultaRequest) =>
{
    var idPaciente = consultaRequest.paciente_id;
    var idMedico = consultaRequest.medico_id;
    var dataConsulta = consultaRequest.data_marcada;
    var metodoPagamento = consultaRequest.metodo_pagamento;

    var retorno = await serviceConsulta.CriarNovaConsulta(idPaciente, idMedico, dataConsulta, metodoPagamento);
    return Results.Created($"/consultas/{retorno.Id}", retorno);
}).WithName("consultas/add").WithOpenApi();

// Atualiza a consulta
app.MapPut("/consultas/{id}", async (ServiceConsulta serviceConsulta, int id, ConsultaRequest ConsultaRequest) =>
{
    await serviceConsulta.AtualizaConsulta(new Consulta()
    {
        Id = id,
        IdPaciente = ConsultaRequest.paciente_id,
        IdMedico = ConsultaRequest.medico_id,
        Data = ConsultaRequest.data_marcada,
        Valor = ConsultaRequest.valor,
        MetodoPagamento = (int)Enum.Parse<MetodoPagamentoEnum>(ConsultaRequest.metodo_pagamento),
        Cancelada = ConsultaRequest.cancelada
    });
    return Results.NoContent();
}).WithName("consultas/update").WithOpenApi();

// Deleta uma consulta
app.MapDelete("/consultas/{id}", async (ServiceConsulta serviceConsulta, int id) =>
{
    var retorno = await serviceConsulta.DeletaConsulta(id);
    return Results.NoContent();
}).WithName("consultas/delete").WithOpenApi();

#endregion

#region Pacientes

// Pacientes: Busca todos os pacientes
app.MapGet("/pacientes", async (ServicePaciente servicePaciente) =>
{
    var pacientes = await servicePaciente.BuscarTodosPacientes();

    var pacientesDto = pacientes.Select(p => new PacienteResponse(p.Id, p.Nome, p.DataNasc, p.Peso, p.Altura));

    return Results.Ok(pacientesDto);
}).Produces<IEnumerable<Paciente>>().WithName("pacientes").WithOpenApi();

// Pacientes: Busca paciente por ID
app.MapGet("/pacientes/{id}", async (ServicePaciente servicePaciente, int id) =>
{
    var paciente = await servicePaciente.BuscarPacientePorId(id);
    if (paciente == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new PacienteResponse(paciente.Id, paciente.Nome, paciente.DataNasc, paciente.Peso,
        paciente.Altura));
}).Produces<Paciente>().WithName("pacientes/id").WithOpenApi();

// Endpoint para criar um novo paciente
app.MapPost("/pacientes", async (ServicePaciente servicePaciente, PacienteRequest pacienteRequest) =>
{
    var novoPaciente = await servicePaciente.CriarNovoPaciente(pacienteRequest.nome, pacienteRequest.data_nascimento,
        pacienteRequest.peso, pacienteRequest.altura, pacienteRequest.telefone, pacienteRequest.endereco);

    var pacienteDto = new PacienteResponse(novoPaciente.Id, novoPaciente.Nome, novoPaciente.DataNasc, novoPaciente.Peso,
        novoPaciente.Altura);

    // Aqui você pode retornar o paciente criado ou uma resposta de sucesso, conforme necessário
    return Results.Created($"/pacientes/{novoPaciente.Id}", pacienteDto);
}).Produces<Paciente>().WithName("pacientes/add").WithOpenApi();


// Endpoint para atualizar um paciente
app.MapPut("/pacientes/{id}",
    async (ServicePaciente servicePaciente, PacienteRequest pacienteRequest, int id) =>
    {
        var nome = pacienteRequest.nome;
        var data_Nasc = pacienteRequest.data_nascimento;
        var peso = pacienteRequest.peso;
        var altura = pacienteRequest.altura;
        var telefone = pacienteRequest.telefone;
        var endereco = pacienteRequest.endereco;

        var pacienteAtualizado =
            await servicePaciente.AtualizarPaciente(id, nome, data_Nasc, peso, altura, telefone, endereco);

        // Verifica se o paciente foi encontrado e atualizado com sucesso
        if (pacienteAtualizado != null)
        {
            return Results.NoContent(); // Retorna o paciente atualizado
        }
        else
        {
            return
                Results.NotFound(
                    $"Paciente com ID {id} não encontrado."); // Retorna erro 404 se o paciente não foi encontrado
        }
    }).Produces<Paciente>().WithName("pacientes/update").WithOpenApi();


// Pacientes: Deleta um paciente
app.MapDelete("/pacientes/{id}", async (ServicePaciente servicePaciente, int id) =>
{
    var pacienteDeletado = await servicePaciente.DeletarPaciente(id);
    if (pacienteDeletado == null)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
}).Produces<Paciente>().WithName("pacientes/delete").WithOpenApi();

#endregion

#region Medicos

// Busca todos os Médicos
app.MapGet("/medicos", async (ServiceMedico serviceMedico) =>
{
    var medicos = await serviceMedico.BuscaTodosMedicos();

    var medicosDto = medicos
        .Select(m => new MedicoResponse(m.Id, m.Nome, m.GetEspecialidade(), m.Crm, m.GetDiaAtendimento()));

    return Results.Ok(medicosDto);
}).WithName("medicos").WithOpenApi();

// Busca Médico por id
app.MapGet("/medicos/{id}", async (ServiceMedico serviceMedico, int id) =>
{
    var medico = await serviceMedico.BuscaMedicoId(id);
    var medicoDto = new MedicoResponse(medico.Id, medico.Nome, medico.GetEspecialidade(), medico.Crm,
        medico.GetDiaAtendimento());
    return Results.Ok(medicoDto);
}).WithName("medicos/id").WithOpenApi();

// Cria um novo médico
app.MapPost("/medicos/", async (ServiceMedico serviceMedico, MedicoRequest medicoRequest) =>
{
    var especialidade = (int)Enum.Parse<EspecialidadeMedicoEnum>(medicoRequest.especialidade);
    var agenda = (int)Enum.Parse<DiasAtendimentoEnum>(medicoRequest.dia_atendimento);

    var medico = await serviceMedico.AdicionaNovoMedico(medicoRequest.nome, especialidade, medicoRequest.crm, agenda);
    return Results.Created($"/medicos/{medico.Id}", medico);
}).WithName("medicos/add").WithOpenApi();

// Atualiza informação Medico
app.MapPut("/medicos/{id}", async (ServiceMedico serviceMedico, MedicoRequest medicoRequest, int id) =>
{
    var especialidade = (int)Enum.Parse<EspecialidadeMedicoEnum>(medicoRequest.especialidade);
    var agenda = (int)Enum.Parse<DiasAtendimentoEnum>(medicoRequest.dia_atendimento);

    await serviceMedico.AtualizaMedico(id, medicoRequest.nome, especialidade, medicoRequest.crm, agenda);
    return Results.NoContent();
}).WithName("medicos/update").WithOpenApi();

// Deleta Medico
app.MapDelete("/medicos/{id}", async (ServiceMedico serviceMedico, int id) =>
{
    await serviceMedico.DeletaMedico(id);
    return Results.NoContent();
}).WithName("medicos/delete").WithOpenApi();

#endregion

app.Run();