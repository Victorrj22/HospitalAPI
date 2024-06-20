namespace HospitalAPI.DTOs;

public record MedicoRequest(string nome, string especialidade, string crm, string dia_atendimento);