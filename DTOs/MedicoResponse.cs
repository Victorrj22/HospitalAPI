namespace HospitalAPI.DTOs;

public record MedicoResponse(int id, string nome, string especialidade, string crm, string dia_atendimento);