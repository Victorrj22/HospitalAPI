using HospitalAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static HospitalAPI.Models.DiasAtendimentoEnum;

namespace HospitalAPI.Service;

public class ServiceConsulta
{
    private readonly DbgeralContext _dbgeralContext;

    public ServiceConsulta(DbgeralContext dbgeralContext)
    {
        _dbgeralContext = dbgeralContext;
    }

    #region Consultas

    /// <summary>
    /// Busca Todos as consultas
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Consulta>> BuscarTodasConsultas()
    {
        return await _dbgeralContext.Consultas.ToListAsync();
    }

    /// <summary>
    /// Busca consulta pelo Id do Paciente
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Consulta>> BuscarConsultaId(int id)
    {
        return await _dbgeralContext.Consultas.Where(c => c.Id == id).ToListAsync();
    }

    /// <summary>
    /// Cria nova consulta
    /// </summary>
    /// <param name="nomePaciente"></param>
    /// <param name="nomeMedico"></param>
    /// <param name="dataConsulta"></param>
    /// <param name="metodoPagamento"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Consulta> CriarNovaConsulta(string nomePaciente, string nomeMedico, DateTime dataConsulta, string metodoPagamento)
    {
        //Puxa o dia da Semana
        DayOfWeek diaSemana = dataConsulta.DayOfWeek;
        
        //Mapeia os dias da semana com o Enum
        var diaAtendimentoMap = new Dictionary<DayOfWeek, int>
        {
            { DayOfWeek.Monday, (int)DiasAtendimentoEnum.Segunda },
            { DayOfWeek.Tuesday, (int)DiasAtendimentoEnum.Terça },
            { DayOfWeek.Wednesday, (int)DiasAtendimentoEnum.Quarta },
            { DayOfWeek.Thursday, (int)DiasAtendimentoEnum.Quinta },
            { DayOfWeek.Friday, (int)DiasAtendimentoEnum.Sexta },
        };
        
        // Verifica se a data é válida
        if (!diaAtendimentoMap.TryGetValue(diaSemana, out var diaAtendimento))
        {
            throw new Exception("O Dia da semana não é válido. Escolha um dia de Segunda-Sexta");
        }
        
        //Mapeia os métodos de pagamento com o Enum
        var metodoPagamentoMap = new Dictionary<string, MetodoPagamentoEnum>(StringComparer.OrdinalIgnoreCase)
        {
            { "dinheiro", MetodoPagamentoEnum.Dinheiro },
            { "débito", MetodoPagamentoEnum.Débito },
            { "credito", MetodoPagamentoEnum.Crédito },
            { "plano", MetodoPagamentoEnum.Plano },
        };

        // Verifica se o método de pagamento é válido
        if (!metodoPagamentoMap.TryGetValue(metodoPagamento, out var metodoPagamentoId))
        {
            throw new Exception("Método de pagamento inválido.");
        }
        
        //Busca o médico com o mesmo nome passado no parâmetro
        var medico = await _dbgeralContext.Medicos.FirstOrDefaultAsync(m => m.Nome.ToLower().Contains(nomeMedico.ToLower()));
        //Busca o paciente com o mesmo nome passado no parâmetro
        var paciente = await _dbgeralContext.Pacientes.FirstOrDefaultAsync(p => p.Nome.ToLower().Contains(nomePaciente.ToLower()));
        
        if (medico != null)
        {
            var consultasExistentesMedico = await _dbgeralContext.Consultas
                .Where(c => c.IdMedico == medico.Id && c.Data.Date == dataConsulta.Date)
                .ToListAsync();
            
            // Verifica se já existe uma consulta no mesmo dia e com menos de 1 hora de diferença
            foreach (var consulta in consultasExistentesMedico)
            {
                var diferenca = (consulta.Data - dataConsulta).TotalMinutes;
                if (Math.Abs(diferenca) < 60)
                {
                    throw new Exception("Já existe uma consulta marcada para esse médico no horário escolhido. Por favor, escolha um outro horário.");
                }
            }
            
            if (medico.Agenda == diaAtendimento)
            {
                var novaConsulta = new Consulta()
                {
                    IdPaciente = paciente!.Id,
                    IdMedico = medico.Id,
                    Data = dataConsulta,
                    MetodoPagamento = (int)metodoPagamentoId,
                    Valor = (decimal)150.00,
                    Cancelada = false
                };
                
                var adicionaConsulta = await _dbgeralContext.Consultas.AddAsync(novaConsulta);
                await _dbgeralContext.SaveChangesAsync();

                return adicionaConsulta.Entity;
            }
            else
            {
                throw new Exception("O médico não atende no dia escolhido. Escolha outro dia");
            }
        }
        else
        {
            throw new Exception($"Não foi encontrado nenhum médico com o nome {nomeMedico}");
        }
    }

    /// <summary>
    /// Atualiza a data de uma consulta
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dataConsulta"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Consulta> AtualizaData(int id, DateTime dataConsulta)
    {
        var consulta = await _dbgeralContext.Consultas.FirstOrDefaultAsync(c => c.Id == id);
        
        if (consulta == null)
        {
            throw new Exception($"Consulta com ID {id} não encontrada.");
        }
        
        var medico = await _dbgeralContext.Medicos.FirstOrDefaultAsync(m => m.Id == consulta!.IdMedico);
        
        //Puxa o dia da Semana
        DayOfWeek diaSemana = dataConsulta.DayOfWeek;
        
        //Mapeia os dias da semana com o Enum
        var diaAtendimentoMap = new Dictionary<DayOfWeek, int>
        {
            { DayOfWeek.Monday, (int)DiasAtendimentoEnum.Segunda },
            { DayOfWeek.Tuesday, (int)DiasAtendimentoEnum.Terça },
            { DayOfWeek.Wednesday, (int)DiasAtendimentoEnum.Quarta },
            { DayOfWeek.Thursday, (int)DiasAtendimentoEnum.Quinta },
            { DayOfWeek.Friday, (int)DiasAtendimentoEnum.Sexta },
        };
        
        // Verifica se a data é válida
        if (!diaAtendimentoMap.TryGetValue(diaSemana, out var diaAtendimento))
        {
            throw new Exception("O Dia da semana não é válido. Escolha um dia de Segunda-Sexta");
        }

        if (medico!.Agenda == diaAtendimento)
        {
            consulta.Data = dataConsulta;
            await _dbgeralContext.SaveChangesAsync();
            return consulta;
        }
        else
        {
            throw new Exception("O médico não atende no dia escolhido. Escolha outro dia");
        }
        
    }

    
    /// <summary>
    /// Deleta uma consulta
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Consulta> DeletaConsulta(int id)
    {
        var consulta = await _dbgeralContext.Consultas.FirstOrDefaultAsync(c => c.Id == id);
        
        if (consulta == null)
        {
            throw new Exception($"Consulta com ID {id} não encontrada.");
        }
        
        _dbgeralContext.Consultas.Remove(consulta);
        await _dbgeralContext.SaveChangesAsync();
        return consulta;
    }
    #endregion
    
}