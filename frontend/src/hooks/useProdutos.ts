import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { produtosApi } from '../api/produtos';
import type { ProdutoRequest } from '../api/produtos';

export function useProdutos(params?: {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
}) {
  return useQuery({
    queryKey: ['produtos', params],
    queryFn: () => produtosApi.getAll(params).then((res) => res.data),
  });
}

export function useProduto(id: string) {
  return useQuery({
    queryKey: ['produto', id],
    queryFn: () => produtosApi.getById(id).then((res) => res.data),
    enabled: !!id,
  });
}

export function useCreateProduto() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: ProdutoRequest) =>
      produtosApi.create(data).then((res) => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['produtos'] });
    },
  });
}

export function useUpdateProduto() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: ProdutoRequest }) =>
      produtosApi.update(id, data).then((res) => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['produtos'] });
    },
  });
}

export function useDeleteProduto() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => produtosApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['produtos'] });
    },
  });
}
