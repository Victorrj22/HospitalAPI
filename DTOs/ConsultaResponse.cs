namespace HospitalAPI.DTOs;

public record ConsultaResponse(int id, string paciente_nome, int paciente_id, string medico_nome, int medico_id, DateTime data_marcada, decimal valor, string metodo_pagamento, bool cancelada);