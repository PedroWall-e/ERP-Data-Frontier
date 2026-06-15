import apiClient from './client';

export interface Produto {
  id: string;
  nome: string;
  descricao?: string;
  codigo: string;
  codigoNcm?: string;
  precoUnitario: number;
  ativo: boolean;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface ProdutoRequest {
  nome: string;
  descricao?: string;
  codigo: string;
  codigoNcm?: string;
  precoUnitario: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export const produtosApi = {
  getAll: (params?: { pageNumber?: number; pageSize?: number; search?: string }) =>
    apiClient.get<PagedResult<Produto>>('/produtos', { params }),

  getById: (id: string) =>
    apiClient.get<Produto>(`/produtos/${id}`),

  create: (data: ProdutoRequest) =>
    apiClient.post<Produto>('/produtos', data),

  update: (id: string, data: ProdutoRequest) =>
    apiClient.put<Produto>(`/produtos/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/produtos/${id}`),
};
