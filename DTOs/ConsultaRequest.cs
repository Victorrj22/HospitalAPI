namespace HospitalAPI.DTOs;

public record ConsultaRequest(int paciente_id, int medico_id, DateTime data_marcada, decimal valor, string metodo_pagamento, bool cancelada);