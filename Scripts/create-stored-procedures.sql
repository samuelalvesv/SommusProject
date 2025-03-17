CREATE DEFINER=`root`@`%` PROCEDURE `GetAlertasDengueByWeek`(IN AnoSemana INT)
BEGIN
    SELECT
        DataInicioSemanaEpidemiologicDaTimestamp,
        SemanaEpidemiologica,
        CasosEstimados,
        CasosEstimadosMinimo,
        CasosEstimadosMaximo,
        CasosNotificados,
        ProbabilidadeTransmissao,
        IncidenciaPor100kHabitantes,
        CodigoLocalidade,
        NivelRisco,
        Identificador,
        VersaoModelo,
        Mensagem,
        TaxaTransmissao,
        Populacao,
        TemperaturaMinima,
        UmidadeMaxima,
        AreaReceptiva,
        AreaTransmissao,
        NivelIncidencia,
        UmidadeMedia,
        UmidadeMinima,
        TemperaturaMedia,
        TemperaturaMaxima,
        CasosProvaveis,
        CasosProvaveisEstimados,
        CasosProvaveisEstimadosMinimo,
        CasosProvaveisEstimadosMaximo,
        CasosConfirmados,
        NotificacoesAcumuladasAno
    FROM ALERTA_DENGUE
    WHERE SemanaEpidemiologica = AnoSemana;
END
