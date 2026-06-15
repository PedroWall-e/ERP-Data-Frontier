import apiClient from './client';
import type { PagedResult } from './produtos';

export interface PedidoItemImpostoRequest {
  nomeImposto: string;
  baseCalculo: number;
  aliquota: number;
  valorImposto: number;
}

export interface PedidoItemRequest {
  produtoId: string;
  quantidade: number;
  valorUnitario: number;
  valorDesconto: number;
  impostos?: PedidoItemImpostoRequest[];
}

export interface PedidoRequest {
  pessoaId: string;
  observacoes?: string;
  itens: PedidoItemRequest[];
}

export interface PedidoItemImpostoResponse {
  id: string;
  nomeImposto: string;
  baseCalculo: number;
  aliquota: number;
  valorImposto: number;
}

export interface PedidoItemResponse {
  id: string;
  produtoId: string;
  produtoNome: string;
  produtoCodigo: string;
  quantidade: number;
  valorUnitario: number;
  valorDesconto: number;
  valorTotal: number;
  impostos: PedidoItemImpostoResponse[];
}

export interface Pedido {
  id: string;
  numeroPedido: string;
  dataEmissao: string;
  status: string;
  pessoaId: string;
  pessoaNome: string;
  pessoaCpfCnpj: string;
  valorTotalProdutos: number;
  valorTotalDesconto: number;
  valorTotalImpostos: number;
  valorTotalPedido: number;
  observacoes?: string;
  itens: PedidoItemResponse[];
  criadoEm: string;
  // NF-e
  numeroNfe?: number;
  chaveAcessoNfe?: string;
  caminhoPdfDanfe?: string;
  // NFS-e
  numeroNfse?: number;
  codigoVerificacaoNfse?: string;
  linkNfseNacional?: string;
}

export const pedidosApi = {
  getAll: (params?: { pageNumber?: number; pageSize?: number; search?: string; status?: string }) =>
    apiClient.get<PagedResult<Pedido>>('/pedidos', { params }),

  getById: (id: string) =>
    apiClient.get<Pedido>(`/pedidos/${id}`),

  create: (data: PedidoRequest) =>
    apiClient.post<Pedido>('/pedidos', data),

  updateStatus: (id: string, status: string) =>
    apiClient.patch<Pedido>(`/pedidos/${id}/status`, { status }),

  faturar: (id: string) =>
    apiClient.post(`/pedidos/${id}/faturar`),

  downloadDanfe: (id: string) =>
    apiClient.get(`/pedidos/${id}/danfe`, { responseType: 'blob' }),

  faturarServico: (id: string) =>
    apiClient.post(`/pedidos/${id}/faturar-servico`),

  cancelarNfe: (id: string, justificativa: string) =>
    apiClient.post(`/pedidos/${id}/cancelar-nfe`, { justificativa }),

  cartaCorrecao: (id: string, texto: string) =>
    apiClient.post(`/pedidos/${id}/carta-correcao`, { texto }),

  consultarSefaz: (id: string) =>
    apiClient.get(`/pedidos/${id}/consultar-sefaz`),

  downloadXml: (id: string) =>
    apiClient.get(`/pedidos/${id}/xml`, { responseType: 'blob' }),
};
