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
