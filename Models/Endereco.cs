using System;
using System.Collections.Generic;

namespace HospitalAPI.Models;

public partial class Endereco
{
    public int Id { get; set; }

    public int? IdPaciente { get; set; }

    public string Rua { get; set; } = null!;

    public int Numero { get; set; }

    public string Bairro { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual Paciente? IdPacienteNavigation { get; set; }
}
