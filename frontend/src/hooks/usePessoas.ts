import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { pessoasApi } from '../api/pessoas';
import type { PessoaRequest } from '../api/pessoas';

export function usePessoas(params?: {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  tipo?: string;
}) {
  return useQuery({
    queryKey: ['pessoas', params],
    queryFn: () => pessoasApi.getAll(params).then((res) => res.data),
  });
}

export function usePessoa(id: string) {
  return useQuery({
    queryKey: ['pessoa', id],
    queryFn: () => pessoasApi.getById(id).then((res) => res.data),
    enabled: !!id,
  });
}

export function useCreatePessoa() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: PessoaRequest) => pessoasApi.create(data).then((res) => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pessoas'] });
    },
  });
}

export function useUpdatePessoa() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: PessoaRequest }) =>
      pessoasApi.update(id, data).then((res) => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pessoas'] });
    },
  });
}

export function useDeletePessoa() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => pessoasApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pessoas'] });
    },
  });
}
