import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box, Typography, Button, Card, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, TablePagination, TextField, InputAdornment, IconButton,
  Chip, Skeleton, alpha, Snackbar, Alert, FormControl, InputLabel, Select,
  MenuItem, Tooltip, Dialog, DialogTitle, DialogContent, DialogContentText,
  DialogActions, Link, Menu,
} from '@mui/material';
import {
  Add as AddIcon, Search as SearchIcon, Visibility as ViewIcon,
  CheckCircle as ConfirmIcon, Cancel as CancelIcon,
  ElectricBolt as FaturarIcon, PictureAsPdf as DanfeIcon,
  Description as NfseIcon, MoreVert as MoreVertIcon,
} from '@mui/icons-material';
import {
  usePedidos, useUpdatePedidoStatus, useFaturarPedido, useDownloadDanfe,
  useFaturarServico, useCancelarNfe, useCartaCorrecao, useConsultarSefaz, useDownloadXml,
} from '../hooks/usePedidos';

const STATUS_MAP: Record<string, { label: string; color: string }> = {
  Rascunho: { label: 'Rascunho', color: '#9E9E9E' },
  Confirmado: { label: 'Confirmado', color: '#42A5F5' },
  Faturado: { label: 'Faturado', color: '#66BB6A' },
  Cancelado: { label: 'Cancelado', color: '#EF5350' },
};

const fmt = (v: number) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
const fmtDate = (d: string) => new Date(d).toLocaleDateString('pt-BR');
const truncChave = (c: string) => c.length > 20 ? `${c.slice(0, 10)}...${c.slice(-10)}` : c;

export default function PedidosPage() {
  const navigate = useNavigate();
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [search, setSearch] = useState('');
  const [searchDebounced, setSearchDebounced] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' | 'info' });
  const [faturarDialog, setFaturarDialog] = useState<string | null>(null);
  const [faturarServicoDialog, setFaturarServicoDialog] = useState<string | null>(null);

  // NF-e actions state
  const [actionsAnchor, setActionsAnchor] = useState<null | HTMLElement>(null);
  const [actionsTargetId, setActionsTargetId] = useState<string | null>(null);
  const [cancelarDialog, setCancelarDialog] = useState<string | null>(null);
  const [cancelarJustificativa, setCancelarJustificativa] = useState('');
  const [cceDialog, setCceDialog] = useState<string | null>(null);
  const [cceTexto, setCceTexto] = useState('');

  const { data, isLoading } = usePedidos({
    pageNumber: page + 1, pageSize: rowsPerPage,
    search: searchDebounced || undefined,
    status: statusFilter || undefined,
  });
  const statusMut = useUpdatePedidoStatus();
  const faturarMut = useFaturarPedido();
  const faturarServicoMut = useFaturarServico();
  const danfeMut = useDownloadDanfe();
  const cancelarNfeMut = useCancelarNfe();
  const cartaCorrecaoMut = useCartaCorrecao();
  const consultarSefazMut = useConsultarSefaz();
  const downloadXmlMut = useDownloadXml();

  const handleSearchChange = (value: string) => {
    setSearch(value);
    setTimeout(() => { setSearchDebounced(value); setPage(0); }, 400);
  };

  const handleStatus = async (id: string, status: string) => {
    try {
      await statusMut.mutateAsync({ id, status });
      setSnackbar({ open: true, message: `Pedido ${status.toLowerCase()}!`, severity: 'success' });
    } catch { setSnackbar({ open: true, message: 'Erro ao atualizar status.', severity: 'error' }); }
  };

  const handleFaturar = async () => {
    if (!faturarDialog) return;
    try {
      await faturarMut.mutateAsync(faturarDialog);
      setSnackbar({ open: true, message: 'NF-e emitida com sucesso! 🎉', severity: 'success' });
    } catch {
      setSnackbar({ open: true, message: 'Erro ao faturar pedido.', severity: 'error' });
    } finally {
      setFaturarDialog(null);
    }
  };

  const handleFaturarServico = async () => {
    if (!faturarServicoDialog) return;
    try {
      await faturarServicoMut.mutateAsync(faturarServicoDialog);
      setSnackbar({ open: true, message: 'NFS-e emitida com sucesso! 📋', severity: 'success' });
    } catch {
      setSnackbar({ open: true, message: 'Erro ao emitir NFS-e.', severity: 'error' });
    } finally {
      setFaturarServicoDialog(null);
    }
  };

  const handleDownloadDanfe = async (id: string) => {
    try {
      await danfeMut.mutateAsync(id);
    } catch {
      setSnackbar({ open: true, message: 'Erro ao baixar DANFE.', severity: 'error' });
    }
  };

  // NF-e actions handlers
  const handleActionsOpen = (event: React.MouseEvent<HTMLElement>, pedidoId: string) => {
    setActionsAnchor(event.currentTarget);
    setActionsTargetId(pedidoId);
  };

  const handleActionsClose = () => {
    setActionsAnchor(null);
    setActionsTargetId(null);
  };

  const handleCancelarNfe = async () => {
    if (!cancelarDialog || cancelarJustificativa.length < 15) return;
    try {
      await cancelarNfeMut.mutateAsync({ id: cancelarDialog, justificativa: cancelarJustificativa });
      setSnackbar({ open: true, message: 'NF-e cancelada com sucesso! ❌', severity: 'success' });
    } catch {
      setSnackbar({ open: true, message: 'Erro ao cancelar NF-e.', severity: 'error' });
    } finally {
      setCancelarDialog(null);
      setCancelarJustificativa('');
    }
  };

  const handleCartaCorrecao = async () => {
    if (!cceDialog || cceTexto.length < 15) return;
    try {
      await cartaCorrecaoMut.mutateAsync({ id: cceDialog, texto: cceTexto });
      setSnackbar({ open: true, message: 'Carta de Correção enviada com sucesso! 📝', severity: 'success' });
    } catch {
      setSnackbar({ open: true, message: 'Erro ao enviar Carta de Correção.', severity: 'error' });
    } finally {
      setCceDialog(null);
      setCceTexto('');
    }
  };

  const handleConsultarSefaz = async (id: string) => {
    handleActionsClose();
    try {
      const result = await consultarSefazMut.mutateAsync(id);
      const msg = typeof result === 'object' && result?.mensagem
        ? result.mensagem
        : 'Consulta realizada com sucesso!';
      setSnackbar({ open: true, message: `🔍 SEFAZ: ${msg}`, severity: 'info' });
    } catch {
      setSnackbar({ open: true, message: 'Erro ao consultar SEFAZ.', severity: 'error' });
    }
  };

  const handleDownloadXml = async (id: string) => {
    handleActionsClose();
    try {
      await downloadXmlMut.mutateAsync(id);
      setSnackbar({ open: true, message: 'XML baixado com sucesso! 📄', severity: 'success' });
    } catch {
      setSnackbar({ open: true, message: 'Erro ao baixar XML.', severity: 'error' });
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3, flexWrap: 'wrap', gap: 2 }}>
        <Box>
          <Typography variant="h4" sx={{ fontWeight: 700 }}>Pedidos de Venda</Typography>
          <Typography variant="body2" color="text.secondary">{data?.totalCount ?? 0} pedidos</Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate('/pedidos/novo')}>Novo Pedido</Button>
      </Box>

      <Box sx={{ display: 'flex', gap: 2, mb: 2, flexWrap: 'wrap' }}>
        <TextField placeholder="Buscar por número ou cliente..." value={search}
          onChange={(e) => handleSearchChange(e.target.value)} size="small" sx={{ width: { xs: '100%', sm: 360 } }}
          slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon sx={{ color: 'text.secondary' }} /></InputAdornment> } }} />
        <FormControl size="small" sx={{ minWidth: 160 }}>
          <InputLabel>Status</InputLabel>
          <Select value={statusFilter} label="Status" onChange={(e) => { setStatusFilter(e.target.value); setPage(0); }}>
            <MenuItem value="">Todos</MenuItem>
            <MenuItem value="Rascunho">Rascunho</MenuItem>
            <MenuItem value="Confirmado">Confirmado</MenuItem>
            <MenuItem value="Faturado">Faturado</MenuItem>
            <MenuItem value="Cancelado">Cancelado</MenuItem>
          </Select>
        </FormControl>
      </Box>

      <Card elevation={0}>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Número</TableCell>
                <TableCell>Cliente</TableCell>
                <TableCell>Data</TableCell>
                <TableCell align="center">Status</TableCell>
                <TableCell align="center">Nº NF-e</TableCell>
                <TableCell align="center">Nº NFS-e</TableCell>
                <TableCell align="right">Total</TableCell>
                <TableCell align="center">Ações</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {isLoading ? Array.from({ length: 5 }).map((_, i) => (
                <TableRow key={i}>{[1,2,3,4,5,6,7,8].map(j => <TableCell key={j}><Skeleton variant="text" /></TableCell>)}</TableRow>
              )) : data?.items.map((p) => {
                const st = STATUS_MAP[p.status] || STATUS_MAP.Rascunho;
                return (
                  <TableRow key={p.id} sx={{ transition: 'background 0.15s', '&:hover': { bgcolor: alpha('#6C63FF', 0.04) } }}>
                    <TableCell>
                      <Typography variant="body2" sx={{ fontWeight: 600, fontFamily: 'monospace' }}>{p.numeroPedido}</Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" sx={{ fontWeight: 500 }}>{p.pessoaNome}</Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" color="text.secondary">{fmtDate(p.dataEmissao)}</Typography>
                    </TableCell>
                    <TableCell align="center">
                      <Chip label={st.label} size="small"
                        sx={{ bgcolor: alpha(st.color, 0.12), color: st.color, fontWeight: 600 }} />
                    </TableCell>
                    <TableCell align="center">
                      {p.numeroNfe ? (
                        <Tooltip title={p.chaveAcessoNfe ? truncChave(p.chaveAcessoNfe) : ''}>
                          <Chip
                            label={String(p.numeroNfe)}
                            size="small"
                            variant="outlined"
                            sx={{ fontFamily: 'monospace', fontWeight: 600, borderColor: alpha('#66BB6A', 0.4), color: '#66BB6A' }}
                          />
                        </Tooltip>
                      ) : (
                        <Typography variant="body2" color="text.disabled">—</Typography>
                      )}
                    </TableCell>
                    <TableCell align="center">
                      {p.numeroNfse ? (
                        <Tooltip title={p.codigoVerificacaoNfse ? `Cód. Verificação: ${p.codigoVerificacaoNfse}` : ''}>
                          <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 0.5 }}>
                            <Chip
                              label={String(p.numeroNfse)}
                              size="small"
                              variant="outlined"
                              sx={{ fontFamily: 'monospace', fontWeight: 600, borderColor: alpha('#AB47BC', 0.4), color: '#AB47BC' }}
                            />
                            {p.linkNfseNacional && (
                              <Link
                                href={p.linkNfseNacional}
                                target="_blank"
                                rel="noopener"
                                sx={{ fontSize: '0.65rem', color: '#42A5F5', textDecoration: 'none', '&:hover': { textDecoration: 'underline' } }}
                              >
                                Ver NFS-e ↗
                              </Link>
                            )}
                          </Box>
                        </Tooltip>
                      ) : (
                        <Typography variant="body2" color="text.disabled">—</Typography>
                      )}
                    </TableCell>
                    <TableCell align="right">
                      <Typography variant="body2" sx={{ fontWeight: 600 }}>{fmt(p.valorTotalPedido)}</Typography>
                    </TableCell>
                    <TableCell align="center">
                      <Box sx={{ display: 'flex', justifyContent: 'center', gap: 0.5 }}>
                        <Tooltip title="Ver detalhes"><IconButton size="small"><ViewIcon fontSize="small" /></IconButton></Tooltip>
                        {p.status === 'Rascunho' && (
                          <Tooltip title="Confirmar"><IconButton size="small" color="info" onClick={() => handleStatus(p.id, 'Confirmado')}><ConfirmIcon fontSize="small" /></IconButton></Tooltip>
                        )}
                        {p.status === 'Confirmado' && (
                          <>
                            <Tooltip title="Faturar (emitir NF-e)">
                              <IconButton size="small" onClick={() => setFaturarDialog(p.id)}
                                sx={{ color: '#FFA726' }}>
                                <FaturarIcon fontSize="small" />
                              </IconButton>
                            </Tooltip>
                            <Tooltip title="Faturar NFS-e (Serviço)">
                              <IconButton size="small" onClick={() => setFaturarServicoDialog(p.id)}
                                sx={{ color: '#AB47BC' }}>
                                <NfseIcon fontSize="small" />
                              </IconButton>
                            </Tooltip>
                          </>
                        )}
                        {p.chaveAcessoNfe && (
                          <Tooltip title="Baixar DANFE">
                            <IconButton size="small" onClick={() => handleDownloadDanfe(p.id)}
                              disabled={danfeMut.isPending}
                              sx={{ color: '#EF5350' }}>
                              <DanfeIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        )}
                        {p.status === 'Faturado' && p.chaveAcessoNfe && (
                          <Tooltip title="Ações NF-e">
                            <IconButton size="small" onClick={(e) => handleActionsOpen(e, p.id)}
                              sx={{ color: '#94A3B8' }}>
                              <MoreVertIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        )}
                        {p.status !== 'Cancelado' && p.status !== 'Faturado' && (
                          <Tooltip title="Cancelar"><IconButton size="small" color="error" onClick={() => handleStatus(p.id, 'Cancelado')}><CancelIcon fontSize="small" /></IconButton></Tooltip>
                        )}
                      </Box>
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>
        <TablePagination component="div" count={data?.totalCount ?? 0} page={page}
          onPageChange={(_, p) => setPage(p)} rowsPerPage={rowsPerPage}
          onRowsPerPageChange={(e) => { setRowsPerPage(parseInt(e.target.value, 10)); setPage(0); }}
          rowsPerPageOptions={[5, 10, 25]} labelRowsPerPage="Itens por página:"
          labelDisplayedRows={({ from, to, count }) => `${from}-${to} de ${count}`}
          sx={{ borderTop: (t) => `1px solid ${alpha(t.palette.text.secondary, 0.08)}` }} />
      </Card>

      {/* NF-e Actions Menu */}
      <Menu
        anchorEl={actionsAnchor}
        open={Boolean(actionsAnchor)}
        onClose={handleActionsClose}
        slotProps={{
          paper: {
            sx: {
              bgcolor: (t: any) => alpha(t.palette.background.paper, 0.95),
              backdropFilter: 'blur(20px)',
              border: (t: any) => `1px solid ${alpha(t.palette.divider, 0.12)}`,
              borderRadius: 2,
              minWidth: 220,
              boxShadow: `0 8px 32px ${alpha('#000', 0.3)}`,
            },
          },
        }}
      >
        <MenuItem
          onClick={() => {
            setCancelarDialog(actionsTargetId);
            handleActionsClose();
          }}
          sx={{ py: 1.2, fontSize: '0.9rem', gap: 1.5 }}
        >
          ❌ Cancelar NF-e
        </MenuItem>
        <MenuItem
          onClick={() => {
            setCceDialog(actionsTargetId);
            handleActionsClose();
          }}
          sx={{ py: 1.2, fontSize: '0.9rem', gap: 1.5 }}
        >
          📝 Carta de Correção
        </MenuItem>
        <MenuItem
          onClick={() => actionsTargetId && handleConsultarSefaz(actionsTargetId)}
          disabled={consultarSefazMut.isPending}
          sx={{ py: 1.2, fontSize: '0.9rem', gap: 1.5 }}
        >
          🔍 Consultar SEFAZ
        </MenuItem>
        <MenuItem
          onClick={() => actionsTargetId && handleDownloadXml(actionsTargetId)}
          disabled={downloadXmlMut.isPending}
          sx={{ py: 1.2, fontSize: '0.9rem', gap: 1.5 }}
        >
          📄 Download XML
        </MenuItem>
      </Menu>

      {/* Faturar NF-e confirmation dialog */}
      <Dialog open={!!faturarDialog} onClose={() => setFaturarDialog(null)}>
        <DialogTitle sx={{ fontWeight: 700 }}>⚡ Faturar Pedido</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Deseja emitir a <strong>NF-e</strong> para este pedido? Esta ação não pode ser desfeita.
            O pedido passará para o status <strong>Faturado</strong>.
          </DialogContentText>
        </DialogContent>
        <DialogActions sx={{ p: 2.5 }}>
          <Button onClick={() => setFaturarDialog(null)} sx={{ textTransform: 'none' }}>Cancelar</Button>
          <Button variant="contained" onClick={handleFaturar} disabled={faturarMut.isPending}
            sx={{
              textTransform: 'none', fontWeight: 600,
              background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
              '&:hover': { background: 'linear-gradient(135deg, #5B52EE, #00C495)' },
            }}>
            {faturarMut.isPending ? 'Emitindo...' : 'Confirmar Faturamento'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Faturar NFS-e confirmation dialog */}
      <Dialog open={!!faturarServicoDialog} onClose={() => setFaturarServicoDialog(null)}>
        <DialogTitle sx={{ fontWeight: 700 }}>📋 Faturar NFS-e (Serviço)</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Deseja emitir a <strong>NFS-e</strong> para os itens de serviço deste pedido?
            A nota fiscal de serviço será transmitida via NFS-e Nacional.
          </DialogContentText>
        </DialogContent>
        <DialogActions sx={{ p: 2.5 }}>
          <Button onClick={() => setFaturarServicoDialog(null)} sx={{ textTransform: 'none' }}>Cancelar</Button>
          <Button variant="contained" onClick={handleFaturarServico} disabled={faturarServicoMut.isPending}
            sx={{
              textTransform: 'none', fontWeight: 600,
              background: 'linear-gradient(135deg, #AB47BC, #6C63FF)',
              '&:hover': { background: 'linear-gradient(135deg, #9C27B0, #5B52EE)' },
            }}>
            {faturarServicoMut.isPending ? 'Emitindo...' : 'Confirmar NFS-e'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Cancelar NF-e dialog */}
      <Dialog
        open={!!cancelarDialog}
        onClose={() => { setCancelarDialog(null); setCancelarJustificativa(''); }}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle sx={{ fontWeight: 700, color: '#EF5350' }}>❌ Cancelar NF-e</DialogTitle>
        <DialogContent>
          <DialogContentText sx={{ mb: 2 }}>
            Informe a justificativa para o cancelamento da NF-e. A justificativa deve conter no mínimo <strong>15 caracteres</strong>.
          </DialogContentText>
          <TextField
            label="Justificativa do cancelamento"
            multiline
            rows={4}
            fullWidth
            value={cancelarJustificativa}
            onChange={(e) => setCancelarJustificativa(e.target.value)}
            error={cancelarJustificativa.length > 0 && cancelarJustificativa.length < 15}
            helperText={
              cancelarJustificativa.length > 0 && cancelarJustificativa.length < 15
                ? `Mínimo 15 caracteres (${cancelarJustificativa.length}/15)`
                : `${cancelarJustificativa.length}/15 caracteres`
            }
            placeholder="Descreva o motivo do cancelamento..."
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions sx={{ p: 2.5 }}>
          <Button
            onClick={() => { setCancelarDialog(null); setCancelarJustificativa(''); }}
            sx={{ textTransform: 'none' }}
          >
            Voltar
          </Button>
          <Button
            variant="contained"
            onClick={handleCancelarNfe}
            disabled={cancelarJustificativa.length < 15 || cancelarNfeMut.isPending}
            sx={{
              textTransform: 'none', fontWeight: 600,
              background: 'linear-gradient(135deg, #EF5350, #E53935)',
              '&:hover': { background: 'linear-gradient(135deg, #E53935, #C62828)' },
              '&:disabled': { opacity: 0.5 },
            }}
          >
            {cancelarNfeMut.isPending ? 'Cancelando...' : 'Confirmar Cancelamento'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Carta de Correção dialog */}
      <Dialog
        open={!!cceDialog}
        onClose={() => { setCceDialog(null); setCceTexto(''); }}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle sx={{ fontWeight: 700, color: '#42A5F5' }}>📝 Carta de Correção (CC-e)</DialogTitle>
        <DialogContent>
          <DialogContentText sx={{ mb: 2 }}>
            Informe o texto da correção. O texto deve conter no mínimo <strong>15 caracteres</strong>.
            A CC-e não permite alteração de valores fiscais, quantidade ou dados do destinatário.
          </DialogContentText>
          <TextField
            label="Texto da correção"
            multiline
            rows={4}
            fullWidth
            value={cceTexto}
            onChange={(e) => setCceTexto(e.target.value)}
            error={cceTexto.length > 0 && cceTexto.length < 15}
            helperText={
              cceTexto.length > 0 && cceTexto.length < 15
                ? `Mínimo 15 caracteres (${cceTexto.length}/15)`
                : `${cceTexto.length}/15 caracteres`
            }
            placeholder="Descreva a correção a ser aplicada..."
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions sx={{ p: 2.5 }}>
          <Button
            onClick={() => { setCceDialog(null); setCceTexto(''); }}
            sx={{ textTransform: 'none' }}
          >
            Voltar
          </Button>
          <Button
            variant="contained"
            onClick={handleCartaCorrecao}
            disabled={cceTexto.length < 15 || cartaCorrecaoMut.isPending}
            sx={{
              textTransform: 'none', fontWeight: 600,
              background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
              '&:hover': { background: 'linear-gradient(135deg, #5B52EE, #00C495)' },
              '&:disabled': { opacity: 0.5 },
            }}
          >
            {cartaCorrecaoMut.isPending ? 'Enviando...' : 'Enviar CC-e'}
          </Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={snackbar.open} autoHideDuration={4000}
        onClose={() => setSnackbar(s => ({ ...s, open: false }))} anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <Alert severity={snackbar.severity} onClose={() => setSnackbar(s => ({ ...s, open: false }))} sx={{ borderRadius: 2 }}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
}
