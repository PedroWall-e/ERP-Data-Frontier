import { useState } from 'react';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  Typography,
  Alert,
  CircularProgress,
  Grid,
  InputAdornment,
  IconButton,
  alpha,
} from '@mui/material';
import {
  Visibility,
  VisibilityOff,
  Business as BusinessIcon,
} from '@mui/icons-material';
import { authApi } from '../api/auth';

export default function RegisterPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    email: '',
    senha: '',
    nomeCompleto: '',
    nomeTenant: '',
    cnpj: '',
  });
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (field: string) => (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm((prev) => ({ ...prev, [field]: e.target.value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await authApi.register(form);
      navigate('/login', {
        state: { message: 'Conta criada com sucesso! Faça login para continuar.' },
      });
    } catch (err: any) {
      setError(
        err.response?.data?.detail ||
        err.response?.data?.error ||
        'Erro ao criar conta.'
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        background: `
          radial-gradient(ellipse at 80% 50%, ${alpha('#6C63FF', 0.15)} 0%, transparent 50%),
          radial-gradient(ellipse at 20% 80%, ${alpha('#00D9A6', 0.1)} 0%, transparent 50%),
          #0A0E1A
        `,
        p: 2,
      }}
    >
      <Box
        sx={{
          width: '100%',
          maxWidth: 520,
          animation: 'fadeIn 0.6s ease-out',
        }}
      >
        <Box sx={{ textAlign: 'center', mb: 4 }}>
          <Typography
            variant="h3"
            sx={{
              background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
              WebkitBackgroundClip: 'text',
              WebkitTextFillColor: 'transparent',
              fontWeight: 800,
              letterSpacing: '-0.03em',
              mb: 1,
            }}
          >
            DataFrontier
          </Typography>
        </Box>

        <Card elevation={0} sx={{ borderRadius: 3 }}>
          <CardContent sx={{ p: 4 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
              <BusinessIcon color="primary" />
              <Typography variant="h5" sx={{ fontWeight: 700 }}>
                Criar Conta
              </Typography>
            </Box>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Registre sua empresa e comece a usar
            </Typography>

            {error && (
              <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }} onClose={() => setError('')}>
                {error}
              </Alert>
            )}

            <form onSubmit={handleSubmit}>
              <Typography variant="subtitle2" color="text.secondary" sx={{ mb: 1, fontWeight: 600 }}>
                Dados da Empresa
              </Typography>
              <Grid container spacing={2} sx={{ mb: 2 }}>
                <Grid size={12}>
                  <TextField
                    fullWidth
                    label="Nome da Empresa"
                    value={form.nomeTenant}
                    onChange={handleChange('nomeTenant')}
                    required
                  />
                </Grid>
                <Grid size={12}>
                  <TextField
                    fullWidth
                    label="CNPJ (apenas dígitos)"
                    value={form.cnpj}
                    onChange={handleChange('cnpj')}
                    required
                    slotProps={{ htmlInput: { maxLength: 14, pattern: '\\d{14}' } }}
                    placeholder="12345678000190"
                  />
                </Grid>
              </Grid>

              <Typography variant="subtitle2" color="text.secondary" sx={{ mb: 1, fontWeight: 600 }}>
                Dados do Administrador
              </Typography>
              <Grid container spacing={2}>
                <Grid size={12}>
                  <TextField
                    fullWidth
                    label="Nome Completo"
                    value={form.nomeCompleto}
                    onChange={handleChange('nomeCompleto')}
                    required
                  />
                </Grid>
                <Grid size={12}>
                  <TextField
                    fullWidth
                    label="E-mail"
                    type="email"
                    value={form.email}
                    onChange={handleChange('email')}
                    required
                  />
                </Grid>
                <Grid size={12}>
                  <TextField
                    fullWidth
                    label="Senha (mín. 8 caracteres)"
                    type={showPassword ? 'text' : 'password'}
                    value={form.senha}
                    onChange={handleChange('senha')}
                    required
                    slotProps={{
                      htmlInput: { minLength: 8 },
                      input: {
                        endAdornment: (
                          <InputAdornment position="end">
                            <IconButton onClick={() => setShowPassword(!showPassword)} edge="end" size="small">
                              {showPassword ? <VisibilityOff /> : <Visibility />}
                            </IconButton>
                          </InputAdornment>
                        ),
                      },
                    }}
                  />
                </Grid>
              </Grid>

              <Button
                fullWidth
                type="submit"
                variant="contained"
                size="large"
                disabled={loading}
                sx={{ mt: 3, py: 1.5 }}
              >
                {loading ? <CircularProgress size={24} color="inherit" /> : 'Criar Conta'}
              </Button>
            </form>

            <Box sx={{ textAlign: 'center', mt: 3 }}>
              <Typography variant="body2" color="text.secondary">
                Já tem uma conta?{' '}
                <Typography
                  component={RouterLink}
                  to="/login"
                  variant="body2"
                  color="primary.main"
                  sx={{ textDecoration: 'none', fontWeight: 600, '&:hover': { textDecoration: 'underline' } }}
                >
                  Fazer login
                </Typography>
              </Typography>
            </Box>
          </CardContent>
        </Card>
      </Box>
    </Box>
  );
}
