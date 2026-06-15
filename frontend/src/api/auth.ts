import apiClient from './client';

export interface LoginRequest {
  email: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  tenantId: string;
  tenantTier: string;
  email: string;
  nomeCompleto: string;
}

export interface RegisterRequest {
  email: string;
  senha: string;
  nomeCompleto: string;
  nomeTenant: string;
  cnpj: string;
}

export interface RegisterResponse {
  usuarioId: string;
  tenantId: string;
  email: string;
  nomeTenant: string;
}

export interface MeResponse {
  userId: string;
  email: string;
  name: string;
  tenantId: string;
  tenantTier: string;
}

export const authApi = {
  login: (data: LoginRequest) =>
    apiClient.post<LoginResponse>('/auth/login', data),

  register: (data: RegisterRequest) =>
    apiClient.post<RegisterResponse>('/auth/register', data),

  me: () =>
    apiClient.get<MeResponse>('/auth/me'),
};
