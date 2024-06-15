using System;
using System.Collections.Generic;

namespace HospitalAPI.Models;

public partial class Medico
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public int Especialidade { get; set; }

    public string Crm { get; set; } = null!;

    public int Agenda { get; set; }

    public virtual ICollection<Consulta> Consulta { get; set; } = new List<Consulta>();
}
