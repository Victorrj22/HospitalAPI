using System;
using System.Collections.Generic;

namespace HospitalAPI.Models;

public partial class Paciente
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public int Idade { get; set; }

    public decimal Peso { get; set; }

    public decimal Altura { get; set; }

    public int? Telefone { get; set; }

    public int? Endereco { get; set; }

    public virtual ICollection<Consulta> Consulta { get; set; } = new List<Consulta>();

    public virtual ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();

    public virtual ICollection<Telefone> Telefones { get; set; } = new List<Telefone>();
}
