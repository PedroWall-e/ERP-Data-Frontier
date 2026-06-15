import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { pedidosApi } from '../api/pedidos';
import type { PedidoRequest } from '../api/pedidos';

export function usePedidos(params?: {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  status?: string;
}) {
  return useQuery({
    queryKey: ['pedidos', params],
    queryFn: () => pedidosApi.getAll(params).then((res) => res.data),
  });
}

export function usePedido(id: string) {
  return useQuery({
    queryKey: ['pedido', id],
    queryFn: () => pedidosApi.getById(id).then((res) => res.data),
    enabled: !!id,
  });
}

export function useCreatePedido() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: PedidoRequest) => pedidosApi.create(data).then((res) => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pedidos'] });
    },
  });
}

export function useUpdatePedidoStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, status }: { id: string; status: string }) =>
      pedidosApi.updateStatus(id, status).then((res) => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pedidos'] });
    },
  });
}

export function useFaturarPedido() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => pedidosApi.faturar(id).then(res => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pedidos'] });
    },
  });
}

export function useDownloadDanfe() {
  return useMutation({
    mutationFn: async (id: string) => {
      const response = await pedidosApi.downloadDanfe(id);
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `DANFE-${id}.pdf`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
    },
  });
}

export function useFaturarServico() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => pedidosApi.faturarServico(id).then(res => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pedidos'] });
    },
  });
}

export function useCancelarNfe() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, justificativa }: { id: string; justificativa: string }) =>
      pedidosApi.cancelarNfe(id, justificativa).then(res => res.data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['pedidos'] }),
  });
}

export function useCartaCorrecao() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, texto }: { id: string; texto: string }) =>
      pedidosApi.cartaCorrecao(id, texto).then(res => res.data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['pedidos'] }),
  });
}

export function useConsultarSefaz() {
  return useMutation({
    mutationFn: (id: string) =>
      pedidosApi.consultarSefaz(id).then(res => res.data),
  });
}

export function useDownloadXml() {
  return useMutation({
    mutationFn: async (id: string) => {
      const response = await pedidosApi.downloadXml(id);
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `NF-e-${id}.xml`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
    },
  });
}
