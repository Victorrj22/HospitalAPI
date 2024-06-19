using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HospitalAPI.Service;

public class ServiceMedico
{
    private readonly DbgeralContext _dbgeralContext;

    public ServiceMedico(DbgeralContext dbgeralContext)
    {
        _dbgeralContext = dbgeralContext;
    }
    
    public async Task<List<Medico>> BuscaTodosMedicos()
    {
        return await _dbgeralContext.Medicos.ToListAsync();
    }

    public async Task<List<Medico>> BuscaMedicoId(int id)
    {
        var medico = await _dbgeralContext.Medicos.Where(m => m.Id == id).ToListAsync();
        if (medico.Count == 0)
        {
            throw new Exception("Não existe um médico com esse Id");
        }
        return medico;
    }

    public async Task<Medico> AdicionaNovoMedico(string nome, int especialidade, string crm,int agenda)
    {
        //Mapeia os dias da semana com o Enum
        var agendaMap = new Dictionary<int, int>
        {
            { 1, (int)DiasAtendimentoEnum.Segunda },
            { 2, (int)DiasAtendimentoEnum.Terça },
            { 3, (int)DiasAtendimentoEnum.Quarta },
            { 4, (int)DiasAtendimentoEnum.Quinta },
            { 5, (int)DiasAtendimentoEnum.Sexta },
        };
        
        // Verifica se a data é válida
        if (!agendaMap.TryGetValue(agenda, out var diaAtendimento))
        {
            throw new Exception("O valor para a agenda não é válido. Escolha um dia de Segunda-Sexta (1-5)");
        }
        
        var especialidadeMap = new Dictionary<int, int>
        {
            { 1, (int)EspecialidadeMedicoEnum.Geral },
            { 2, (int)EspecialidadeMedicoEnum.Cirurgião },
            { 3, (int)EspecialidadeMedicoEnum.Cardiologista },
            { 4, (int)EspecialidadeMedicoEnum.Ortopedista },
            { 5, (int)EspecialidadeMedicoEnum.Psiquiatra },
        };
        
        // Verifica se a data é válida
        if (!especialidadeMap.TryGetValue(especialidade, out var especialidadeEnum))
        {
            throw new Exception("O valor para a agenda não é válido. Escolha um valor de 1-5");
        }
        
        var medico = new Medico()
        {
            Nome = nome,
            Especialidade = especialidadeEnum,
            Crm = crm,
            Agenda = diaAtendimento
        };

        var novoMedico = await _dbgeralContext.Medicos.AddAsync(medico);
        await _dbgeralContext.SaveChangesAsync();
        return novoMedico.Entity;
    }

    public async Task<Medico> AtualizaMedico(int id,string nome, int especialidade, string crm,int agenda)
    {
        var medico = await _dbgeralContext.Medicos.FirstOrDefaultAsync(m => m.Id == id);

        if (medico == null)
        {
            throw new Exception("Não existe um médico com o Id informado.");
        }
        //Mapeia os dias da semana com o Enum
        var agendaMap = new Dictionary<int, int>
        {
            { 1, (int)DiasAtendimentoEnum.Segunda },
            { 2, (int)DiasAtendimentoEnum.Terça },
            { 3, (int)DiasAtendimentoEnum.Quarta },
            { 4, (int)DiasAtendimentoEnum.Quinta },
            { 5, (int)DiasAtendimentoEnum.Sexta },
        };
        
        // Verifica se a data é válida
        if (!agendaMap.TryGetValue(agenda, out var diaAtendimento))
        {
            throw new Exception("O valor para a agenda não é válido. Escolha um dia de Segunda-Sexta (1-5)");
        }
        
        var especialidadeMap = new Dictionary<int, int>
        {
            { 1, (int)EspecialidadeMedicoEnum.Geral },
            { 2, (int)EspecialidadeMedicoEnum.Cirurgião },
            { 3, (int)EspecialidadeMedicoEnum.Cardiologista },
            { 4, (int)EspecialidadeMedicoEnum.Ortopedista },
            { 5, (int)EspecialidadeMedicoEnum.Psiquiatra },
        };
        
        // Verifica se a data é válida
        if (!especialidadeMap.TryGetValue(especialidade, out var especialidadeEnum))
        {
            throw new Exception("O valor para a agenda não é válido. Escolha um valor de 1-5");
        }

        medico.Nome = nome;
        medico.Agenda = diaAtendimento;
        medico.Especialidade = especialidadeEnum;
        medico.Crm = crm;
        await _dbgeralContext.SaveChangesAsync();
        return medico;
    }

    public async Task<Medico> DeletaMedico(int id)
    {
        var medico = await _dbgeralContext.Medicos.FirstOrDefaultAsync(m => m.Id == id);
        
        if (medico == null)
        {
            throw new Exception("Não existe um médico com o Id informado.");
        }

        _dbgeralContext.Medicos.Remove(medico);
        await _dbgeralContext.SaveChangesAsync();
        return medico;
    }
}