using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HospitalAPI.Models;

public partial class Consulta
{
    public int Id { get; set; }

    public int? IdPaciente { get; set; }

    public int? IdMedico { get; set; }

    public DateTime Data { get; set; }

    public decimal Valor { get; set; }

    public int MetodoPagamento { get; set; }

    public bool Cancelada { get; set; }

    [JsonIgnore]
    public virtual Medico? IdMedicoNavigation { get; set; }

    [JsonIgnore]
    public virtual Paciente? IdPacienteNavigation { get; set; }

    public string GetMetodoPagamento()
    {
        return ((MetodoPagamentoEnum)MetodoPagamento).ToString();
    }
}
