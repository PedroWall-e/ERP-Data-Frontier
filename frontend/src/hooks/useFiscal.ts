import { useMutation } from '@tanstack/react-query';
import { fiscalApi } from '../api/fiscal';
import type { InutilizacaoRequest } from '../api/fiscal';

export function useInutilizar() {
  return useMutation({
    mutationFn: (data: InutilizacaoRequest) =>
      fiscalApi.inutilizar(data).then(res => res.data),
  });
}
