import {
  Box,
  Typography,
  Grid,
  Card,
  CardContent,
  alpha,
  Skeleton,
} from '@mui/material';
import {
  Inventory2 as InventoryIcon,
  TrendingUp as TrendingUpIcon,
  AttachMoney as MoneyIcon,
  Category as CategoryIcon,
} from '@mui/icons-material';
import { useProdutos } from '../hooks/useProdutos';
import { useAuthStore } from '../stores/authStore';

interface StatCardProps {
  title: string;
  value: string | number;
  subtitle: string;
  icon: React.ReactNode;
  color: string;
  delay: number;
}

function StatCard({ title, value, subtitle, icon, color, delay }: StatCardProps) {
  return (
    <Card
      elevation={0}
      sx={{
        position: 'relative',
        overflow: 'hidden',
        animation: `fadeIn 0.5s ease-out ${delay}ms both`,
        transition: 'transform 0.2s ease, box-shadow 0.2s ease',
        '&:hover': {
          transform: 'translateY(-4px)',
          boxShadow: `0 8px 25px ${alpha(color, 0.25)}`,
        },
      }}
    >
      <CardContent sx={{ p: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <Box>
            <Typography variant="body2" color="text.secondary" sx={{ fontWeight: 500 }}>
              {title}
            </Typography>
            <Typography variant="h3" sx={{ fontWeight: 700, my: 1, color }}>
              {value}
            </Typography>
            <Typography variant="caption" color="text.secondary">
              {subtitle}
            </Typography>
          </Box>
          <Box
            sx={{
              p: 1.5,
              borderRadius: 2,
              bgcolor: alpha(color, 0.12),
              color,
              display: 'flex',
            }}
          >
            {icon}
          </Box>
        </Box>
      </CardContent>
      {/* Gradient accent line */}
      <Box
        sx={{
          position: 'absolute',
          bottom: 0,
          left: 0,
          right: 0,
          height: 3,
          background: `linear-gradient(90deg, ${color}, ${alpha(color, 0.3)})`,
        }}
      />
    </Card>
  );
}

export default function DashboardPage() {
  const { data, isLoading } = useProdutos({ pageSize: 100 });
  const nomeCompleto = useAuthStore((s) => s.nomeCompleto);

  const totalProdutos = data?.totalCount ?? 0;
  const produtosAtivos = data?.items?.filter((p) => p.ativo).length ?? 0;
  const valorTotal = data?.items?.reduce((sum, p) => sum + p.precoUnitario, 0) ?? 0;
  const precoMedio = totalProdutos > 0 ? valorTotal / totalProdutos : 0;

  return (
    <Box>
      <Box sx={{ mb: 4, animation: 'fadeIn 0.4s ease-out' }}>
        <Typography variant="h4" sx={{ fontWeight: 700 }}>
          Olá, {nomeCompleto?.split(' ')[0]} 👋
        </Typography>
        <Typography variant="body1" color="text.secondary" sx={{ mt: 0.5 }}>
          Aqui está o resumo do seu negócio
        </Typography>
      </Box>

      <Grid container spacing={3}>
        {isLoading ? (
          [1, 2, 3, 4].map((i) => (
            <Grid size={{ xs: 12, sm: 6, md: 3 }} key={i}>
              <Card elevation={0}>
                <CardContent sx={{ p: 3 }}>
                  <Skeleton variant="text" width="60%" />
                  <Skeleton variant="text" width="40%" height={50} />
                  <Skeleton variant="text" width="80%" />
                </CardContent>
              </Card>
            </Grid>
          ))
        ) : (
          <>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <StatCard
                title="Total de Produtos"
                value={totalProdutos}
                subtitle="Cadastrados no sistema"
                icon={<InventoryIcon />}
                color="#6C63FF"
                delay={0}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <StatCard
                title="Produtos Ativos"
                value={produtosAtivos}
                subtitle="Disponíveis para venda"
                icon={<TrendingUpIcon />}
                color="#00D9A6"
                delay={100}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <StatCard
                title="Valor Total"
                value={`R$ ${valorTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`}
                subtitle="Soma dos preços unitários"
                icon={<MoneyIcon />}
                color="#FFB547"
                delay={200}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <StatCard
                title="Preço Médio"
                value={`R$ ${precoMedio.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`}
                subtitle="Média dos produtos"
                icon={<CategoryIcon />}
                color="#FF5C6C"
                delay={300}
              />
            </Grid>
          </>
        )}
      </Grid>
    </Box>
  );
}
