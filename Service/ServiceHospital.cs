using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Service;

public class ServiceHospital
{
    private readonly DbgeralContext _dbgeralContext;

    public ServiceHospital(DbgeralContext dbgeralContext)
    {
        _dbgeralContext = dbgeralContext;
    }

    public async Task<IEnumerable<Consulta>> BuscarTodasConsultas()
    {
        return await _dbgeralContext.Consultas.ToListAsync();
    }
}