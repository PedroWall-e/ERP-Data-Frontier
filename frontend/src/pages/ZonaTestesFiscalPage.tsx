import { useState } from 'react';
import {
  Box,
  Typography,
  Card,
  Tabs,
  Tab,
  TextField,
  Button,
  Chip,
  alpha,
  Alert,
  CircularProgress,
  Autocomplete,
  Divider,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Skeleton,
} from '@mui/material';
import {
  CheckCircle as CheckIcon,
  Cancel as CancelIcon,
  Science as ScienceIcon,
  ElectricBolt as BoltIcon,
  Description as NfseIcon,
  Info as InfoIcon,
  Block as BlockIcon,
} from '@mui/icons-material';
import { usePedidos, useFaturarPedido, useFaturarServico } from '../hooks/usePedidos';
import { useEmpresa } from '../hooks/useEmpresa';
import { useInutilizar } from '../hooks/useFiscal';
import type { Pedido } from '../api/pedidos';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel({ children, value, index }: TabPanelProps) {
  return (
    <Box
      role="tabpanel"
      hidden={value !== index}
      sx={{ py: 3, animation: value === index ? 'fadeIn 0.3s ease-out' : 'none' }}
    >
      {value === index && children}
    </Box>
  );
}

const fmt = (v: number) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });

export default function ZonaTestesFiscalPage() {
  const [tab, setTab] = useState(0);
  const [selectedPedidoNfe, setSelectedPedidoNfe] = useState<Pedido | null>(null);
  const [selectedPedidoNfse, setSelectedPedidoNfse] = useState<Pedido | null>(null);
  const [nfeResult, setNfeResult] = useState<any>(null);
  const [nfseResult, setNfseResult] = useState<any>(null);

  // Inutilização state
  const [inutSerie, setInutSerie] = useState(1);
  const [inutNumInicio, setInutNumInicio] = useState(1);
  const [inutNumFim, setInutNumFim] = useState(1);
  const [inutJustificativa, setInutJustificativa] = useState('');
  const [inutResult, setInutResult] = useState<any>(null);

  const { data: pedidosData, isLoading: pedidosLoading } = usePedidos({ pageSize: 100 });
  const { data: empresa, isLoading: empresaLoading } = useEmpresa();
  const faturarMut = useFaturarPedido();
  const faturarServicoMut = useFaturarServico();
  const inutilizarMut = useInutilizar();

  const confirmados = pedidosData?.items.filter(p => p.status === 'Confirmado') ?? [];

  const handleFaturarNfe = async () => {
    if (!selectedPedidoNfe) return;
    try {
      const result = await faturarMut.mutateAsync(selectedPedidoNfe.id);
      setNfeResult({ success: true, data: result });
    } catch (err: any) {
      setNfeResult({ success: false, error: err.response?.data?.detail || err.message || 'Erro desconhecido' });
    }
  };

  const handleFaturarNfse = async () => {
    if (!selectedPedidoNfse) return;
    try {
      const result = await faturarServicoMut.mutateAsync(selectedPedidoNfse.id);
      setNfseResult({ success: true, data: result });
    } catch (err: any) {
      setNfseResult({ success: false, error: err.response?.data?.detail || err.message || 'Erro desconhecido' });
    }
  };

  const handleInutilizar = async () => {
    if (inutJustificativa.length < 15) return;
    try {
      const result = await inutilizarMut.mutateAsync({
        serie: inutSerie,
        numeroInicio: inutNumInicio,
        numeroFim: inutNumFim,
        justificativa: inutJustificativa,
      });
      setInutResult({ success: true, data: result });
    } catch (err: any) {
      setInutResult({ success: false, error: err.response?.data?.detail || err.message || 'Erro desconhecido' });
    }
  };

  const renderPedidoSummary = (pedido: Pedido | null) => {
    if (!pedido) return null;
    return (
      <Card
        elevation={0}
        sx={{
          mt: 2,
          p: 2.5,
          background: (t) => `linear-gradient(135deg, ${alpha(t.palette.primary.main, 0.05)}, ${alpha('#00D9A6', 0.05)})`,
          border: (t) => `1px solid ${alpha(t.palette.primary.main, 0.1)}`,
          borderRadius: 2,
        }}
      >
        <Box sx={{ display: 'flex', justifyContent: 'space-between', flexWrap: 'wrap', gap: 1, mb: 1.5 }}>
          <Box>
            <Typography variant="body2" color="text.secondary">Pedido</Typography>
            <Typography variant="h6" sx={{ fontWeight: 700, fontFamily: 'monospace' }}>{pedido.numeroPedido}</Typography>
          </Box>
          <Box>
            <Typography variant="body2" color="text.secondary">Cliente</Typography>
            <Typography variant="body1" sx={{ fontWeight: 600 }}>{pedido.pessoaNome}</Typography>
            <Typography variant="caption" color="text.secondary" sx={{ fontFamily: 'monospace' }}>{pedido.pessoaCpfCnpj}</Typography>
          </Box>
          <Box>
            <Typography variant="body2" color="text.secondary">Total</Typography>
            <Typography variant="h6" sx={{ fontWeight: 700, color: '#00D9A6' }}>{fmt(pedido.valorTotalPedido)}</Typography>
          </Box>
        </Box>
        <Divider sx={{ my: 1.5, borderColor: alpha('#94A3B8', 0.1) }} />
        <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
          Itens ({pedido.itens.length}):
        </Typography>
        {pedido.itens.map((item, i) => (
          <Box key={item.id} sx={{ display: 'flex', justifyContent: 'space-between', py: 0.5 }}>
            <Typography variant="body2">
              {i + 1}. {item.produtoNome}
              <Typography component="span" variant="caption" color="text.secondary" sx={{ ml: 1 }}>
                x{item.quantidade}
              </Typography>
            </Typography>
            <Typography variant="body2" sx={{ fontWeight: 600, fontFamily: 'monospace' }}>
              {fmt(item.valorTotal)}
            </Typography>
          </Box>
        ))}
      </Card>
    );
  };

  const renderResult = (result: any, type: 'NF-e' | 'NFS-e') => {
    if (!result) return null;
    return (
      <Box sx={{ mt: 3, animation: 'fadeIn 0.4s ease-out' }}>
        {result.success ? (
          <>
            <Alert severity="success" sx={{ mb: 2, borderRadius: 2 }}>
              {type} gerada com sucesso! 🎉
            </Alert>
            <Card
              elevation={0}
              sx={{
                p: 2,
                bgcolor: alpha('#0A0E1A', 0.6),
                border: (t) => `1px solid ${alpha(t.palette.primary.main, 0.15)}`,
                borderRadius: 2,
              }}
            >
              <Typography variant="caption" color="text.secondary" sx={{ mb: 1, display: 'block' }}>
                Resposta do servidor:
              </Typography>
              <Box
                component="pre"
                sx={{
                  fontFamily: '"JetBrains Mono", "Fira Code", monospace',
                  fontSize: '0.8rem',
                  color: '#00D9A6',
                  m: 0,
                  p: 2,
                  borderRadius: 1.5,
                  bgcolor: alpha('#000', 0.3),
                  overflow: 'auto',
                  maxHeight: 400,
                  whiteSpace: 'pre-wrap',
                  wordBreak: 'break-all',
                  '&::-webkit-scrollbar': { width: 6 },
                  '&::-webkit-scrollbar-track': { bgcolor: 'transparent' },
                  '&::-webkit-scrollbar-thumb': { bgcolor: alpha('#6C63FF', 0.3), borderRadius: 3 },
                }}
              >
                {JSON.stringify(result.data, null, 2)}
              </Box>
            </Card>
          </>
        ) : (
          <Alert severity="error" sx={{ borderRadius: 2 }}>
            <Typography variant="body2" sx={{ fontWeight: 600 }}>Erro ao gerar {type}</Typography>
            <Typography variant="body2" sx={{ mt: 0.5, fontFamily: 'monospace', fontSize: '0.8rem' }}>
              {result.error}
            </Typography>
          </Alert>
        )}
      </Box>
    );
  };

  return (
    <Box sx={{ animation: 'fadeIn 0.4s ease-out' }}>
      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Typography
          variant="h4"
          sx={{
            fontWeight: 800,
            mb: 0.5,
            background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
            WebkitBackgroundClip: 'text',
            WebkitTextFillColor: 'transparent',
            display: 'inline-block',
          }}
        >
          🧪 Zona de Testes Fiscal
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Teste a emissão de NF-e e NFS-e em ambiente controlado
        </Typography>
      </Box>

      {empresa && empresa.ambienteFiscal === 2 && (
        <Alert
          severity="warning"
          icon={<InfoIcon />}
          sx={{
            mb: 3,
            borderRadius: 2,
            border: (t) => `1px solid ${alpha(t.palette.warning.main, 0.3)}`,
            bgcolor: (t) => alpha(t.palette.warning.main, 0.06),
          }}
        >
          <Typography variant="body2" sx={{ fontWeight: 600 }}>
            ⚠️ Ambiente de HOMOLOGAÇÃO ativo — Documentos emitidos aqui NÃO têm validade fiscal.
          </Typography>
        </Alert>
      )}

      {/* Main Card */}
      <Card
        elevation={0}
        sx={{
          borderRadius: 3,
          overflow: 'hidden',
          background: (t) => alpha(t.palette.background.paper, 0.7),
          backdropFilter: 'blur(20px)',
          border: (t) => `1px solid ${alpha(t.palette.divider, 0.08)}`,
        }}
      >
        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tabs
            value={tab}
            onChange={(_, v) => setTab(v)}
            sx={{
              px: 2,
              '& .MuiTab-root': {
                textTransform: 'none',
                fontWeight: 600,
                fontSize: '0.9rem',
                minHeight: 56,
              },
              '& .MuiTabs-indicator': {
                height: 3,
                borderRadius: '3px 3px 0 0',
                background: 'linear-gradient(90deg, #6C63FF, #00D9A6)',
              },
            }}
          >
            <Tab icon={<BoltIcon sx={{ fontSize: 18 }} />} iconPosition="start" label="NF-e (Produto)" />
            <Tab icon={<NfseIcon sx={{ fontSize: 18 }} />} iconPosition="start" label="NFS-e (Serviço)" />
            <Tab icon={<ScienceIcon sx={{ fontSize: 18 }} />} iconPosition="start" label="Status" />
          </Tabs>
        </Box>

        <Box sx={{ p: 3 }}>
          {/* Tab 1 - NF-e */}
          <TabPanel value={tab} index={0}>
            <Typography variant="h6" sx={{ fontWeight: 700, mb: 2 }}>
              ⚡ Emitir NF-e (Nota Fiscal Eletrônica)
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Selecione um pedido com status <strong>Confirmado</strong> para gerar o INI e transmitir a NF-e.
            </Typography>

            <Autocomplete
              options={confirmados}
              getOptionLabel={(o) => `${o.numeroPedido} — ${o.pessoaNome} — ${fmt(o.valorTotalPedido)}`}
              value={selectedPedidoNfe}
              onChange={(_, val) => { setSelectedPedidoNfe(val); setNfeResult(null); }}
              loading={pedidosLoading}
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Selecionar Pedido"
                  placeholder="Buscar pedido..."
                  size="small"
                />
              )}
              sx={{ mb: 2, maxWidth: 600 }}
            />

            {renderPedidoSummary(selectedPedidoNfe)}

            {selectedPedidoNfe && (
              <Box sx={{ mt: 3 }}>
                <Button
                  variant="contained"
                  size="large"
                  onClick={handleFaturarNfe}
                  disabled={faturarMut.isPending}
                  startIcon={faturarMut.isPending ? <CircularProgress size={18} color="inherit" /> : <BoltIcon />}
                  sx={{
                    textTransform: 'none',
                    fontWeight: 700,
                    px: 4,
                    py: 1.5,
                    borderRadius: 2,
                    background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
                    boxShadow: `0 4px 20px ${alpha('#6C63FF', 0.3)}`,
                    '&:hover': {
                      background: 'linear-gradient(135deg, #5B52EE, #00C495)',
                      boxShadow: `0 6px 24px ${alpha('#6C63FF', 0.4)}`,
                    },
                    transition: 'all 0.2s ease',
                  }}
                >
                  {faturarMut.isPending ? 'Gerando INI e transmitindo...' : 'Gerar INI NF-e'}
                </Button>
              </Box>
            )}

            {renderResult(nfeResult, 'NF-e')}
          </TabPanel>

          {/* Tab 2 - NFS-e */}
          <TabPanel value={tab} index={1}>
            <Typography variant="h6" sx={{ fontWeight: 700, mb: 2 }}>
              📋 Emitir NFS-e (Nota Fiscal de Serviço)
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Selecione um pedido com status <strong>Confirmado</strong> para gerar a NFS-e via NFS-e Nacional.
            </Typography>

            <Autocomplete
              options={confirmados}
              getOptionLabel={(o) => `${o.numeroPedido} — ${o.pessoaNome} — ${fmt(o.valorTotalPedido)}`}
              value={selectedPedidoNfse}
              onChange={(_, val) => { setSelectedPedidoNfse(val); setNfseResult(null); }}
              loading={pedidosLoading}
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Selecionar Pedido"
                  placeholder="Buscar pedido..."
                  size="small"
                />
              )}
              sx={{ mb: 2, maxWidth: 600 }}
            />

            {renderPedidoSummary(selectedPedidoNfse)}

            {selectedPedidoNfse && (
              <Box sx={{ mt: 3 }}>
                <Button
                  variant="contained"
                  size="large"
                  onClick={handleFaturarNfse}
                  disabled={faturarServicoMut.isPending}
                  startIcon={faturarServicoMut.isPending ? <CircularProgress size={18} color="inherit" /> : <NfseIcon />}
                  sx={{
                    textTransform: 'none',
                    fontWeight: 700,
                    px: 4,
                    py: 1.5,
                    borderRadius: 2,
                    background: 'linear-gradient(135deg, #AB47BC, #6C63FF)',
                    boxShadow: `0 4px 20px ${alpha('#AB47BC', 0.3)}`,
                    '&:hover': {
                      background: 'linear-gradient(135deg, #9C27B0, #5B52EE)',
                      boxShadow: `0 6px 24px ${alpha('#AB47BC', 0.4)}`,
                    },
                    transition: 'all 0.2s ease',
                  }}
                >
                  {faturarServicoMut.isPending ? 'Gerando INI e transmitindo...' : 'Gerar INI NFS-e'}
                </Button>
              </Box>
            )}

            {renderResult(nfseResult, 'NFS-e')}
          </TabPanel>

          {/* Tab 3 - Status */}
          <TabPanel value={tab} index={2}>
            <Typography variant="h6" sx={{ fontWeight: 700, mb: 3 }}>
              🔍 Status do Ambiente Fiscal
            </Typography>

            {empresaLoading ? (
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                {[1, 2, 3].map((i) => <Skeleton key={i} variant="rounded" height={60} />)}
              </Box>
            ) : (
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
                {/* Ambiente Card */}
                <Card
                  elevation={0}
                  sx={{
                    p: 3,
                    borderRadius: 2,
                    background: (t) => `linear-gradient(135deg, ${alpha(
                      empresa?.ambienteFiscal === 1 ? '#66BB6A' : '#FFA726', 0.08
                    )}, ${alpha(
                      empresa?.ambienteFiscal === 1 ? '#66BB6A' : '#FFA726', 0.02
                    )})`,
                    border: (t) => `1px solid ${alpha(
                      empresa?.ambienteFiscal === 1 ? '#66BB6A' : '#FFA726', 0.2
                    )}`,
                  }}
                >
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Box
                      sx={{
                        width: 48,
                        height: 48,
                        borderRadius: 2,
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        background: `linear-gradient(135deg, ${
                          empresa?.ambienteFiscal === 1 ? '#66BB6A' : '#FFA726'
                        }, ${
                          empresa?.ambienteFiscal === 1 ? '#43A047' : '#FB8C00'
                        })`,
                        fontSize: '1.5rem',
                      }}
                    >
                      {empresa?.ambienteFiscal === 1 ? '🏭' : '🧪'}
                    </Box>
                    <Box>
                      <Typography variant="body2" color="text.secondary">Ambiente Fiscal</Typography>
                      <Typography variant="h6" sx={{ fontWeight: 700 }}>
                        {empresa?.ambienteLabel || (empresa?.ambienteFiscal === 1 ? 'Produção' : 'Homologação')}
                      </Typography>
                    </Box>
                    <Chip
                      label={empresa?.ambienteFiscal === 1 ? 'PRODUÇÃO' : 'HOMOLOGAÇÃO'}
                      size="small"
                      sx={{
                        ml: 'auto',
                        fontWeight: 700,
                        letterSpacing: '0.04em',
                        bgcolor: alpha(empresa?.ambienteFiscal === 1 ? '#66BB6A' : '#FFA726', 0.15),
                        color: empresa?.ambienteFiscal === 1 ? '#66BB6A' : '#FFA726',
                      }}
                    />
                  </Box>
                </Card>

                {/* Certificate Status Card */}
                <Card
                  elevation={0}
                  sx={{
                    p: 3,
                    borderRadius: 2,
                    background: (t) => `linear-gradient(135deg, ${alpha(t.palette.primary.main, 0.05)}, ${alpha('#00D9A6', 0.03)})`,
                    border: (t) => `1px solid ${alpha(t.palette.divider, 0.1)}`,
                  }}
                >
                  <Typography variant="subtitle1" sx={{ fontWeight: 700, mb: 0.5 }}>
                    📜 Certificado Digital
                  </Typography>
                  {empresa?.temCertificado ? (
                    <Box sx={{ mt: 1 }}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                        <CheckIcon sx={{ color: '#66BB6A', fontSize: 20 }} />
                        <Typography variant="body2" sx={{ fontWeight: 500 }}>Instalado</Typography>
                      </Box>
                      {empresa.certificadoNome && (
                        <Typography variant="body2" color="text.secondary" sx={{ fontFamily: 'monospace', fontSize: '0.8rem' }}>
                          {empresa.certificadoNome}
                        </Typography>
                      )}
                      {empresa.certificadoValidade && (
                        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                          Validade: {new Date(empresa.certificadoValidade).toLocaleDateString('pt-BR')}
                          {empresa.certificadoExpirado && (
                            <Chip label="EXPIRADO" size="small" sx={{ ml: 1, bgcolor: alpha('#EF5350', 0.12), color: '#EF5350', fontWeight: 700, fontSize: '0.65rem' }} />
                          )}
                        </Typography>
                      )}
                    </Box>
                  ) : (
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mt: 1 }}>
                      <CancelIcon sx={{ color: '#EF5350', fontSize: 20 }} />
                      <Typography variant="body2" color="error">Nenhum certificado instalado</Typography>
                    </Box>
                  )}
                </Card>

                {/* Checklist */}
                <Card
                  elevation={0}
                  sx={{
                    p: 3,
                    borderRadius: 2,
                    background: (t) => `linear-gradient(135deg, ${alpha(t.palette.background.paper, 0.8)}, ${alpha(t.palette.background.paper, 0.5)})`,
                    border: (t) => `1px solid ${alpha(t.palette.divider, 0.1)}`,
                  }}
                >
                  <Typography variant="subtitle1" sx={{ fontWeight: 700, mb: 2 }}>
                    ✅ Checklist de Componentes
                  </Typography>
                  <List disablePadding>
                    <ListItem
                      sx={{
                        px: 2, py: 1.2, borderRadius: 1.5, mb: 1,
                        bgcolor: (t) => alpha(empresa?.temCertificado ? '#66BB6A' : '#EF5350', 0.06),
                      }}
                    >
                      <ListItemIcon sx={{ minWidth: 36 }}>
                        {empresa?.temCertificado
                          ? <CheckIcon sx={{ color: '#66BB6A' }} />
                          : <CancelIcon sx={{ color: '#EF5350' }} />}
                      </ListItemIcon>
                      <ListItemText
                        primary="Certificado Digital A1"
                        secondary={empresa?.temCertificado ? 'Instalado e válido' : 'Não configurado'}
                        slotProps={{
                          primary: { sx: { fontWeight: 600, fontSize: '0.9rem' } },
                          secondary: { sx: { fontSize: '0.8rem' } },
                        }}
                      />
                    </ListItem>

                    <ListItem
                      sx={{
                        px: 2, py: 1.2, borderRadius: 1.5, mb: 1,
                        bgcolor: alpha('#EF5350', 0.06),
                      }}
                    >
                      <ListItemIcon sx={{ minWidth: 36 }}>
                        <CancelIcon sx={{ color: '#EF5350' }} />
                      </ListItemIcon>
                      <ListItemText
                        primary="ACBr DLLs"
                        secondary="Não detectadas no servidor"
                        slotProps={{
                          primary: { sx: { fontWeight: 600, fontSize: '0.9rem' } },
                          secondary: { sx: { fontSize: '0.8rem' } },
                        }}
                      />
                    </ListItem>

                    <ListItem
                      sx={{
                        px: 2, py: 1.2, borderRadius: 1.5,
                        bgcolor: alpha('#EF5350', 0.06),
                      }}
                    >
                      <ListItemIcon sx={{ minWidth: 36 }}>
                        <CancelIcon sx={{ color: '#EF5350' }} />
                      </ListItemIcon>
                      <ListItemText
                        primary="Schemas XML (NF-e / NFS-e)"
                        secondary="Não detectados no servidor"
                        slotProps={{
                          primary: { sx: { fontWeight: 600, fontSize: '0.9rem' } },
                          secondary: { sx: { fontSize: '0.8rem' } },
                        }}
                      />
                    </ListItem>
                  </List>
                </Card>

                {empresa?.ambienteFiscal === 2 && (
                  <Alert
                    severity="info"
                    sx={{
                      borderRadius: 2,
                      border: (t) => `1px solid ${alpha(t.palette.info.main, 0.2)}`,
                    }}
                  >
                    <Typography variant="body2">
                      Em <strong>Homologação</strong>, os documentos fiscais são transmitidos para o ambiente de testes da SEFAZ.
                      Nenhum documento emitido aqui possui validade fiscal.
                      Para mudar para <strong>Produção</strong>, acesse Configurações → Empresa.
                    </Typography>
                  </Alert>
                )}

                {/* Inutilização de Numeração */}
                <Card
                  elevation={0}
                  sx={{
                    p: 3,
                    borderRadius: 2,
                    background: (t) => `linear-gradient(135deg, ${alpha('#EF5350', 0.05)}, ${alpha('#FFA726', 0.03)})`,
                    border: (t) => `1px solid ${alpha('#EF5350', 0.15)}`,
                  }}
                >
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 2 }}>
                    <Box
                      sx={{
                        width: 40,
                        height: 40,
                        borderRadius: 1.5,
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        background: 'linear-gradient(135deg, #EF5350, #FFA726)',
                        fontSize: '1.2rem',
                      }}
                    >
                      <BlockIcon sx={{ color: '#fff', fontSize: 22 }} />
                    </Box>
                    <Box>
                      <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                        Inutilização de Numeração
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        Inutilize faixas de numeração NF-e que não serão utilizadas
                      </Typography>
                    </Box>
                  </Box>

                  <Divider sx={{ my: 2, borderColor: alpha('#94A3B8', 0.1) }} />

                  <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', mb: 2 }}>
                    <TextField
                      label="Série"
                      type="number"
                      size="small"
                      value={inutSerie}
                      onChange={(e) => setInutSerie(Number(e.target.value))}
                      sx={{ width: 120 }}
                      slotProps={{ input: { inputProps: { min: 1 } } }}
                    />
                    <TextField
                      label="Número Início"
                      type="number"
                      size="small"
                      value={inutNumInicio}
                      onChange={(e) => setInutNumInicio(Number(e.target.value))}
                      sx={{ width: 160 }}
                      slotProps={{ input: { inputProps: { min: 1 } } }}
                    />
                    <TextField
                      label="Número Fim"
                      type="number"
                      size="small"
                      value={inutNumFim}
                      onChange={(e) => setInutNumFim(Number(e.target.value))}
                      sx={{ width: 160 }}
                      slotProps={{ input: { inputProps: { min: 1 } } }}
                    />
                  </Box>

                  <TextField
                    label="Justificativa"
                    multiline
                    rows={3}
                    fullWidth
                    size="small"
                    value={inutJustificativa}
                    onChange={(e) => setInutJustificativa(e.target.value)}
                    error={inutJustificativa.length > 0 && inutJustificativa.length < 15}
                    helperText={
                      inutJustificativa.length > 0 && inutJustificativa.length < 15
                        ? `Mínimo 15 caracteres (${inutJustificativa.length}/15)`
                        : `${inutJustificativa.length}/15 caracteres`
                    }
                    placeholder="Descreva o motivo da inutilização..."
                    sx={{ mb: 2 }}
                  />

                  <Button
                    variant="contained"
                    size="large"
                    onClick={handleInutilizar}
                    disabled={inutJustificativa.length < 15 || inutilizarMut.isPending}
                    startIcon={inutilizarMut.isPending ? <CircularProgress size={18} color="inherit" /> : <BlockIcon />}
                    sx={{
                      textTransform: 'none',
                      fontWeight: 700,
                      px: 4,
                      py: 1.5,
                      borderRadius: 2,
                      background: 'linear-gradient(135deg, #EF5350, #FFA726)',
                      boxShadow: `0 4px 20px ${alpha('#EF5350', 0.3)}`,
                      '&:hover': {
                        background: 'linear-gradient(135deg, #E53935, #FB8C00)',
                        boxShadow: `0 6px 24px ${alpha('#EF5350', 0.4)}`,
                      },
                      '&:disabled': { opacity: 0.5 },
                      transition: 'all 0.2s ease',
                    }}
                  >
                    {inutilizarMut.isPending ? 'Inutilizando...' : 'Inutilizar Numeração'}
                  </Button>

                  {inutResult && (
                    <Box sx={{ mt: 3, animation: 'fadeIn 0.4s ease-out' }}>
                      {inutResult.success ? (
                        <>
                          <Alert severity="success" sx={{ mb: 2, borderRadius: 2 }}>
                            Numeração inutilizada com sucesso! 🔒
                          </Alert>
                          <Card
                            elevation={0}
                            sx={{
                              p: 2,
                              bgcolor: alpha('#0A0E1A', 0.6),
                              border: (t) => `1px solid ${alpha(t.palette.primary.main, 0.15)}`,
                              borderRadius: 2,
                            }}
                          >
                            <Typography variant="caption" color="text.secondary" sx={{ mb: 1, display: 'block' }}>
                              Resposta do servidor:
                            </Typography>
                            <Box
                              component="pre"
                              sx={{
                                fontFamily: '"JetBrains Mono", "Fira Code", monospace',
                                fontSize: '0.8rem',
                                color: '#00D9A6',
                                m: 0,
                                p: 2,
                                borderRadius: 1.5,
                                bgcolor: alpha('#000', 0.3),
                                overflow: 'auto',
                                maxHeight: 300,
                                whiteSpace: 'pre-wrap',
                                wordBreak: 'break-all',
                                '&::-webkit-scrollbar': { width: 6 },
                                '&::-webkit-scrollbar-track': { bgcolor: 'transparent' },
                                '&::-webkit-scrollbar-thumb': { bgcolor: alpha('#6C63FF', 0.3), borderRadius: 3 },
                              }}
                            >
                              {JSON.stringify(inutResult.data, null, 2)}
                            </Box>
                          </Card>
                        </>
                      ) : (
                        <Alert severity="error" sx={{ borderRadius: 2 }}>
                          <Typography variant="body2" sx={{ fontWeight: 600 }}>Erro na inutilização</Typography>
                          <Typography variant="body2" sx={{ mt: 0.5, fontFamily: 'monospace', fontSize: '0.8rem' }}>
                            {inutResult.error}
                          </Typography>
                        </Alert>
                      )}
                    </Box>
                  )}
                </Card>
              </Box>
            )}
          </TabPanel>
        </Box>
      </Card>
    </Box>
  );
}
