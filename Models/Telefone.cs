using System;
using System.Collections.Generic;

namespace HospitalAPI.Models;

public partial class Telefone
{
    public int Id { get; set; }

    public int? IdPaciente { get; set; }

    public string Ddd { get; set; } = null!;

    public string Numero { get; set; } = null!;

    public virtual Paciente? IdPacienteNavigation { get; set; }
}
