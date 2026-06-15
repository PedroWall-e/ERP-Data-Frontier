import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
  token: string | null;
  email: string | null;
  nomeCompleto: string | null;
  tenantId: string | null;
  tenantTier: string | null;
  expiresAt: string | null;
  isAuthenticated: boolean;
  login: (data: {
    token: string;
    email: string;
    nomeCompleto: string;
    tenantId: string;
    tenantTier: string;
    expiresAt: string;
  }) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      email: null,
      nomeCompleto: null,
      tenantId: null,
      tenantTier: null,
      expiresAt: null,
      isAuthenticated: false,
      login: (data) =>
        set({
          token: data.token,
          email: data.email,
          nomeCompleto: data.nomeCompleto,
          tenantId: data.tenantId,
          tenantTier: data.tenantTier,
          expiresAt: data.expiresAt,
          isAuthenticated: true,
        }),
      logout: () =>
        set({
          token: null,
          email: null,
          nomeCompleto: null,
          tenantId: null,
          tenantTier: null,
          expiresAt: null,
          isAuthenticated: false,
        }),
    }),
    {
      name: 'datafrontier-auth',
    }
  )
);
