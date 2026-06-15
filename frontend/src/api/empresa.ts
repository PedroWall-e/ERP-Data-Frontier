import apiClient from './client';

export interface ConfiguracaoEmpresaRequest {
  razaoSocial: string;
  nomeFantasia: string;
  cnpj: string;
  inscricaoEstadual: string;
  inscricaoMunicipal?: string;
  crt: number;
  logradouro: string;
  numero: string;
  complemento?: string;
  bairro: string;
  codigoIbge: string;
  municipio: string;
  uf: string;
  cep: string;
  telefone?: string;
  email?: string;
  ambienteFiscal: number;
  serieNfe: number;
  serieNfse: number;
  interClientId?: string;
  interClientSecret?: string;
}

export interface ConfiguracaoEmpresaResponse {
  id: string;
  razaoSocial: string;
  nomeFantasia: string;
  cnpj: string;
  inscricaoEstadual: string;
  inscricaoMunicipal?: string;
  crt: number;
  logradouro: string;
  numero: string;
  complemento?: string;
  bairro: string;
  codigoIbge: string;
  municipio: string;
  uf: string;
  cep: string;
  telefone?: string;
  email?: string;
  temCertificado: boolean;
  certificadoNome?: string;
  certificadoValidade?: string;
  certificadoExpirado: boolean;
  ambienteFiscal: number;
  ambienteLabel: string;
  serieNfe: number;
  serieNfse: number;
  temInterConfig: boolean;
  interClientId?: string;
  criadoEm: string;
  atualizadoEm: string;
}

export const empresaApi = {
  get: () => apiClient.get<ConfiguracaoEmpresaResponse>('/empresa'),
  save: (data: ConfiguracaoEmpresaRequest) => apiClient.put<ConfiguracaoEmpresaResponse>('/empresa', data),
  uploadCertificado: (file: File, senha: string) => {
    const formData = new FormData();
    formData.append('arquivo', file);
    formData.append('senha', senha);
    return apiClient.post<ConfiguracaoEmpresaResponse>('/empresa/certificado', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
  removerCertificado: () => apiClient.delete('/empresa/certificado'),
};
