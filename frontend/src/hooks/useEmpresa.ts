import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { empresaApi } from '../api/empresa';
import type { ConfiguracaoEmpresaRequest } from '../api/empresa';

export function useEmpresa() {
  return useQuery({
    queryKey: ['empresa'],
    queryFn: () => empresaApi.get().then(res => res.data),
  });
}

export function useSaveEmpresa() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: ConfiguracaoEmpresaRequest) => empresaApi.save(data).then(res => res.data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['empresa'] }),
  });
}

export function useUploadCertificado() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ file, senha }: { file: File; senha: string }) =>
      empresaApi.uploadCertificado(file, senha).then(res => res.data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['empresa'] }),
  });
}

export function useRemoverCertificado() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: () => empresaApi.removerCertificado(),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['empresa'] }),
  });
}
