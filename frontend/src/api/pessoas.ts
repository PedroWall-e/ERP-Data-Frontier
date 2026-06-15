import apiClient from './client';
import type { PagedResult } from './produtos';

export interface EnderecoRequest {
  logradouro: string;
  numero: string;
  complemento?: string;
  bairro: string;
  cep: string;
  cidade: string;
  uf: string;
  codigoIbge: string;
}

export interface EnderecoResponse {
  id: string;
  logradouro: string;
  numero: string;
  complemento?: string;
  bairro: string;
  cep: string;
  cidade: string;
  uf: string;
  codigoIbge: string;
}

export interface PessoaRequest {
  tipoPessoa: string;
  cpfCnpj: string;
  razaoSocial: string;
  nomeFantasia?: string;
  inscricaoEstadual?: string;
  indicadorIE: string;
  isCliente: boolean;
  isFornecedor: boolean;
  email?: string;
  telefone?: string;
  endereco?: EnderecoRequest;
}

export interface Pessoa {
  id: string;
  tipoPessoa: string;
  cpfCnpj: string;
  razaoSocial: string;
  nomeFantasia?: string;
  inscricaoEstadual?: string;
  indicadorIE: string;
  isCliente: boolean;
  isFornecedor: boolean;
  email?: string;
  telefone?: string;
  ativo: boolean;
  endereco?: EnderecoResponse;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface ViaCepResponse {
  cep: string;
  logradouro: string;
  complemento: string;
  bairro: string;
  localidade: string;
  uf: string;
  ibge: string;
  erro?: boolean;
}

export const pessoasApi = {
  getAll: (params?: { pageNumber?: number; pageSize?: number; search?: string; tipo?: string }) =>
    apiClient.get<PagedResult<Pessoa>>('/pessoas', { params }),

  getById: (id: string) =>
    apiClient.get<Pessoa>(`/pessoas/${id}`),

  create: (data: PessoaRequest) =>
    apiClient.post<Pessoa>('/pessoas', data),

  update: (id: string, data: PessoaRequest) =>
    apiClient.put<Pessoa>(`/pessoas/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/pessoas/${id}`),
};

export const fetchCep = async (cep: string): Promise<ViaCepResponse | null> => {
  try {
    const clean = cep.replace(/\D/g, '');
    if (clean.length !== 8) return null;
    const response = await fetch(`https://viacep.com.br/ws/${clean}/json/`);
    const data = await response.json();
    if (data.erro) return null;
    return data;
  } catch {
    return null;
  }
};
