namespace HospitalAPI.DTOs;

public record PacienteRequest(string nome, DateTime data_nascimento, decimal peso, decimal altura, int? telefone, int? endereco);