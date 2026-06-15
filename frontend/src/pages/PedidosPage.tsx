import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box, Typography, Button, Card, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, TablePagination, TextField, InputAdornment, IconButton,
  Chip, Skeleton, alpha, Snackbar, Alert, FormControl, InputLabel, Select,
  MenuItem, Tooltip,
} from '@mui/material';
import {
  Add as AddIcon, Search as SearchIcon, Visibility as ViewIcon,
  CheckCircle as ConfirmIcon, Cancel as CancelIcon,
} from '@mui/icons-material';
import { usePedidos, useUpdatePedidoStatus } from '../hooks/usePedidos';

const STATUS_MAP: Record<string, { label: string; color: string }> = {
  Rascunho: { label: 'Rascunho', color: '#9E9E9E' },
  Confirmado: { label: 'Confirmado', color: '#42A5F5' },
  Faturado: { label: 'Faturado', color: '#66BB6A' },
  Cancelado: { label: 'Cancelado', color: '#EF5350' },
};

const fmt = (v: number) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
const fmtDate = (d: string) => new Date(d).toLocaleDateString('pt-BR');

export default function PedidosPage() {
  const navigate = useNavigate();
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [search, setSearch] = useState('');
  const [searchDebounced, setSearchDebounced] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  const { data, isLoading } = usePedidos({
    pageNumber: page + 1, pageSize: rowsPerPage,
    search: searchDebounced || undefined,
    status: statusFilter || undefined,
  });
  const statusMut = useUpdatePedidoStatus();

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
                <TableCell align="right">Total</TableCell>
                <TableCell align="center">Ações</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {isLoading ? Array.from({ length: 5 }).map((_, i) => (
                <TableRow key={i}>{[1,2,3,4,5,6].map(j => <TableCell key={j}><Skeleton variant="text" /></TableCell>)}</TableRow>
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
                    <TableCell align="right">
                      <Typography variant="body2" sx={{ fontWeight: 600 }}>{fmt(p.valorTotalPedido)}</Typography>
                    </TableCell>
                    <TableCell align="center">
                      <Tooltip title="Ver detalhes"><IconButton size="small"><ViewIcon fontSize="small" /></IconButton></Tooltip>
                      {p.status === 'Rascunho' && (
                        <Tooltip title="Confirmar"><IconButton size="small" color="info" onClick={() => handleStatus(p.id, 'Confirmado')}><ConfirmIcon fontSize="small" /></IconButton></Tooltip>
                      )}
                      {p.status !== 'Cancelado' && p.status !== 'Faturado' && (
                        <Tooltip title="Cancelar"><IconButton size="small" color="error" onClick={() => handleStatus(p.id, 'Cancelado')}><CancelIcon fontSize="small" /></IconButton></Tooltip>
                      )}
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

      <Snackbar open={snackbar.open} autoHideDuration={4000}
        onClose={() => setSnackbar(s => ({ ...s, open: false }))} anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <Alert severity={snackbar.severity} onClose={() => setSnackbar(s => ({ ...s, open: false }))} sx={{ borderRadius: 2 }}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
}
