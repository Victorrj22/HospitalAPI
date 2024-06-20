namespace HospitalAPI.DTOs;

public record PacienteResponse(int id, string nome, DateTime data_nascimento, decimal peso, decimal altura);