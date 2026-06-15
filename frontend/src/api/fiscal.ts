import apiClient from './client';

export interface InutilizacaoRequest {
  serie: number;
  numeroInicio: number;
  numeroFim: number;
  justificativa: string;
}

export interface FiscalResponse {
  sucesso: boolean;
  numeroNfe?: number;
  chaveAcesso?: string;
  protocolo?: string;
  caminhoPdf?: string;
  caminhoXml?: string;
  mensagem?: string;
  codigoRetorno?: number;
  iniGerado?: string;
}

export const fiscalApi = {
  inutilizar: (data: InutilizacaoRequest) =>
    apiClient.post<FiscalResponse>('/fiscal/inutilizar', data),
};
