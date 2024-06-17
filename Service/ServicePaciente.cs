using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Service
{
    public class ServicePaciente
    {
        private readonly DbgeralContext _dbgeralContext;

        public ServicePaciente(DbgeralContext dbgeralContext)
        {
            _dbgeralContext = dbgeralContext;
        }

        /// <summary>
        /// Busca todos os pacientes
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Paciente>> BuscarTodosPacientes()
        {
            return await _dbgeralContext.Pacientes.ToListAsync();
        }

        /// <summary>
        /// Busca paciente pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Paciente?> BuscarPacientePorId(int id)
        {
            return await _dbgeralContext.Pacientes.FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Cria novo paciente
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="data_Nasc"></param>
        /// <param name="peso"></param>
        /// <param name="altura"></param>
        /// <param name="telefone"></param>
        /// <param name="endereco"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Paciente> CriarNovoPaciente(string nome, DateOnly data_Nasc, decimal peso, decimal altura, int? telefone, int? endereco)
        {
            var novoPaciente = new Paciente()
            {
                Nome = nome,
                Data_Nasc = data_Nasc,
                Peso = peso,
                Altura = altura,
                Telefone = telefone,
                Endereco = endereco
            };

            var adicionaPaciente = await _dbgeralContext.Pacientes.AddAsync(novoPaciente);
            await _dbgeralContext.SaveChangesAsync();

            return adicionaPaciente.Entity;
        }

        /// <summary>
        /// Atualiza informações de um paciente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nome"></param>
        /// <param name="data_Nasc"></param>
        /// <param name="peso"></param>
        /// <param name="altura"></param>
        /// <param name="telefone"></param>
        /// <param name="endereco"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Paciente> AtualizarPaciente(int id, string nome, DateOnly data_Nasc, decimal peso, decimal altura, int? telefone, int? endereco)
        {
            var paciente = await _dbgeralContext.Pacientes.FirstOrDefaultAsync(p => p.Id == id);

            if (paciente == null)
            {
                throw new Exception($"Paciente com ID {id} não encontrado.");
            }

            paciente.Nome = nome;
            paciente.Data_Nasc = data_Nasc;
            paciente.Peso = peso;
            paciente.Altura = altura;
            paciente.Telefone = telefone;
            paciente.Endereco = endereco;

            await _dbgeralContext.SaveChangesAsync();
            return paciente;
        }

        /// <summary>
        /// Deleta um paciente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Paciente> DeletarPaciente(int id)
        {
            var paciente = await _dbgeralContext.Pacientes.FirstOrDefaultAsync(p => p.Id == id);

            if (paciente == null)
            {
                throw new Exception($"Paciente com ID {id} não encontrado.");
            }

            _dbgeralContext.Pacientes.Remove(paciente);
            await _dbgeralContext.SaveChangesAsync();
            return paciente;
        }
    }
}
