import { useState } from 'react';
import {
  Box,
  Typography,
  Button,
  Card,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  TextField,
  InputAdornment,
  IconButton,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Grid,
  Tooltip,
  Skeleton,
  alpha,
  Snackbar,
  Alert,
  CircularProgress,
  Switch,
  FormControlLabel,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from '@mui/material';
import {
  Add as AddIcon,
  Search as SearchIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Close as CloseIcon,
} from '@mui/icons-material';
import { useProdutos, useCreateProduto, useUpdateProduto, useDeleteProduto } from '../hooks/useProdutos';
import type { ProdutoRequest, Produto } from '../api/produtos';

const UNIDADES = ['UN', 'HR', 'MES', 'DIA', 'KG', 'M', 'M2', 'M3', 'LT', 'PCT'];

const EMPTY_FORM: ProdutoRequest = {
  nome: '',
  descricao: '',
  codigo: '',
  codigoNcm: '',
  precoUnitario: 0,
  isServico: false,
  codigoServico: '',
  codigoCnae: '',
  codigoNbs: '',
  unidadeMedida: 'UN',
};

export default function ProdutosPage() {
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [search, setSearch] = useState('');
  const [searchDebounced, setSearchDebounced] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<ProdutoRequest>(EMPTY_FORM);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  const { data, isLoading } = useProdutos({
    pageNumber: page + 1,
    pageSize: rowsPerPage,
    search: searchDebounced || undefined,
  });

  const createMutation = useCreateProduto();
  const updateMutation = useUpdateProduto();
  const deleteMutation = useDeleteProduto();

  // Debounce search
  const handleSearchChange = (value: string) => {
    setSearch(value);
    setTimeout(() => {
      setSearchDebounced(value);
      setPage(0);
    }, 400);
  };

  const handleOpenCreate = () => {
    setForm(EMPTY_FORM);
    setEditingId(null);
    setDialogOpen(true);
  };

  const handleOpenEdit = (produto: Produto) => {
    setForm({
      nome: produto.nome,
      descricao: produto.descricao || '',
      codigo: produto.codigo,
      codigoNcm: produto.codigoNcm || '',
      precoUnitario: produto.precoUnitario,
      isServico: produto.isServico,
      codigoServico: produto.codigoServico || '',
      codigoCnae: produto.codigoCnae || '',
      codigoNbs: produto.codigoNbs || '',
      unidadeMedida: produto.unidadeMedida || 'UN',
    });
    setEditingId(produto.id);
    setDialogOpen(true);
  };

  const handleSave = async () => {
    try {
      if (editingId) {
        await updateMutation.mutateAsync({ id: editingId, data: form });
        setSnackbar({ open: true, message: 'Produto atualizado!', severity: 'success' });
      } else {
        await createMutation.mutateAsync(form);
        setSnackbar({ open: true, message: 'Produto criado!', severity: 'success' });
      }
      setDialogOpen(false);
    } catch (err: any) {
      setSnackbar({
        open: true,
        message: err.response?.data?.detail || 'Erro ao salvar produto.',
        severity: 'error',
      });
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await deleteMutation.mutateAsync(id);
      setSnackbar({ open: true, message: 'Produto desativado!', severity: 'success' });
    } catch {
      setSnackbar({ open: true, message: 'Erro ao desativar produto.', severity: 'error' });
    }
  };

  const handleChange = (field: keyof ProdutoRequest) => (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = field === 'precoUnitario' ? parseFloat(e.target.value) || 0 : e.target.value;
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const isSaving = createMutation.isPending || updateMutation.isPending;

  return (
    <Box>
      {/* Header */}
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          mb: 3,
          flexWrap: 'wrap',
          gap: 2,
          animation: 'fadeIn 0.4s ease-out',
        }}
      >
        <Box>
          <Typography variant="h4" sx={{ fontWeight: 700 }}>
            Produtos
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {data?.totalCount ?? 0} produtos cadastrados
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleOpenCreate}
          sx={{ px: 3 }}
        >
          Novo Produto
        </Button>
      </Box>

      {/* Search */}
      <TextField
        placeholder="Buscar por nome ou código..."
        value={search}
        onChange={(e) => handleSearchChange(e.target.value)}
        size="small"
        sx={{ mb: 2, width: { xs: '100%', sm: 360 } }}
        slotProps={{
          input: {
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon sx={{ color: 'text.secondary' }} />
              </InputAdornment>
            ),
          },
        }}
      />

      {/* Table */}
      <Card elevation={0} sx={{ animation: 'fadeIn 0.5s ease-out 0.1s both' }}>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Código</TableCell>
                <TableCell>Nome</TableCell>
                <TableCell align="center">Tipo</TableCell>
                <TableCell>NCM / Cód. Serviço</TableCell>
                <TableCell align="center">Unidade</TableCell>
                <TableCell align="right">Preço</TableCell>
                <TableCell align="center">Status</TableCell>
                <TableCell align="center">Ações</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {isLoading
                ? Array.from({ length: 5 }).map((_, i) => (
                    <TableRow key={i}>
                      {[1, 2, 3, 4, 5, 6, 7, 8].map((j) => (
                        <TableCell key={j}>
                          <Skeleton variant="text" />
                        </TableCell>
                      ))}
                    </TableRow>
                  ))
                : data?.items.map((produto) => (
                    <TableRow
                      key={produto.id}
                      sx={{
                        transition: 'background 0.15s',
                        '&:hover': { bgcolor: alpha('#6C63FF', 0.04) },
                      }}
                    >
                      <TableCell>
                        <Typography variant="body2" sx={{ fontWeight: 600, fontFamily: 'monospace' }}>
                          {produto.codigo}
                        </Typography>
                      </TableCell>
                      <TableCell>
                        <Typography variant="body2" sx={{ fontWeight: 500 }}>
                          {produto.nome}
                        </Typography>
                        {produto.descricao && (
                          <Typography variant="caption" color="text.secondary" noWrap sx={{ maxWidth: 300, display: 'block' }}>
                            {produto.descricao}
                          </Typography>
                        )}
                      </TableCell>
                      <TableCell align="center">
                        <Chip
                          label={produto.isServico ? '🔧 Serviço' : '📦 Produto'}
                          size="small"
                          sx={{
                            bgcolor: produto.isServico ? alpha('#00D9A6', 0.12) : alpha('#42A5F5', 0.12),
                            color: produto.isServico ? '#00D9A6' : '#42A5F5',
                            fontWeight: 600,
                            fontSize: '0.75rem',
                          }}
                        />
                      </TableCell>
                      <TableCell>
                        <Typography variant="body2" sx={{ fontFamily: 'monospace' }} color="text.secondary">
                          {produto.isServico
                            ? produto.codigoServico || '—'
                            : produto.codigoNcm || '—'}
                        </Typography>
                      </TableCell>
                      <TableCell align="center">
                        <Chip
                          label={produto.unidadeMedida || 'UN'}
                          size="small"
                          variant="outlined"
                          sx={{ fontFamily: 'monospace', fontWeight: 600, fontSize: '0.7rem' }}
                        />
                      </TableCell>
                      <TableCell align="right">
                        <Typography variant="body2" sx={{ fontWeight: 600 }}>
                          R$ {produto.precoUnitario.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                        </Typography>
                      </TableCell>
                      <TableCell align="center">
                        <Chip
                          label={produto.ativo ? 'Ativo' : 'Inativo'}
                          size="small"
                          sx={{
                            bgcolor: produto.ativo ? alpha('#00D9A6', 0.12) : alpha('#FF5C6C', 0.12),
                            color: produto.ativo ? '#00D9A6' : '#FF5C6C',
                            fontWeight: 600,
                          }}
                        />
                      </TableCell>
                      <TableCell align="center">
                        <Tooltip title="Editar">
                          <IconButton size="small" onClick={() => handleOpenEdit(produto)}>
                            <EditIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Desativar">
                          <IconButton
                            size="small"
                            onClick={() => handleDelete(produto.id)}
                            disabled={!produto.ativo}
                          >
                            <DeleteIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                      </TableCell>
                    </TableRow>
                  ))}
            </TableBody>
          </Table>
        </TableContainer>
        <TablePagination
          component="div"
          count={data?.totalCount ?? 0}
          page={page}
          onPageChange={(_, newPage) => setPage(newPage)}
          rowsPerPage={rowsPerPage}
          onRowsPerPageChange={(e) => {
            setRowsPerPage(parseInt(e.target.value, 10));
            setPage(0);
          }}
          rowsPerPageOptions={[5, 10, 25, 50]}
          labelRowsPerPage="Itens por página:"
          labelDisplayedRows={({ from, to, count }) => `${from}-${to} de ${count}`}
          sx={{ borderTop: (t) => `1px solid ${alpha(t.palette.text.secondary, 0.08)}` }}
        />
      </Card>

      {/* Create/Edit Dialog */}
      <Dialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        maxWidth="sm"
        fullWidth
        slotProps={{ paper: { sx: { borderRadius: 3 } } }}
      >
        <DialogTitle sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h6" sx={{ fontWeight: 700 }}>
            {editingId ? 'Editar Produto' : 'Novo Produto'}
          </Typography>
          <IconButton size="small" onClick={() => setDialogOpen(false)}>
            <CloseIcon />
          </IconButton>
        </DialogTitle>
        <DialogContent dividers>
          <Grid container spacing={2} sx={{ pt: 1 }}>
            {/* Toggle: É Serviço? */}
            <Grid size={12}>
              <FormControlLabel
                control={
                  <Switch
                    checked={form.isServico}
                    onChange={(e) => setForm((prev) => ({ ...prev, isServico: e.target.checked }))}
                    sx={{
                      '& .MuiSwitch-switchBase.Mui-checked': { color: '#00D9A6' },
                      '& .MuiSwitch-switchBase.Mui-checked + .MuiSwitch-track': { bgcolor: '#00D9A6' },
                    }}
                  />
                }
                label={
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      {form.isServico ? '🔧 É Serviço' : '📦 É Produto'}
                    </Typography>
                    <Chip
                      label={form.isServico ? 'Serviço' : 'Produto'}
                      size="small"
                      sx={{
                        bgcolor: form.isServico ? alpha('#00D9A6', 0.12) : alpha('#42A5F5', 0.12),
                        color: form.isServico ? '#00D9A6' : '#42A5F5',
                        fontWeight: 600,
                        fontSize: '0.7rem',
                      }}
                    />
                  </Box>
                }
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField fullWidth label="Código (SKU)" value={form.codigo} onChange={handleChange('codigo')} required />
            </Grid>

            {/* NCM - only for products */}
            {!form.isServico && (
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="NCM" value={form.codigoNcm} onChange={handleChange('codigoNcm')} slotProps={{ htmlInput: { maxLength: 8 } }} />
              </Grid>
            )}

            {/* Service fields - only for services */}
            {form.isServico && (
              <>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField fullWidth label="Código Serviço (LC 116)" value={form.codigoServico} onChange={handleChange('codigoServico')} />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField fullWidth label="Código CNAE" value={form.codigoCnae} onChange={handleChange('codigoCnae')} />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField fullWidth label="Código NBS" value={form.codigoNbs} onChange={handleChange('codigoNbs')} />
                </Grid>
              </>
            )}

            <Grid size={12}>
              <TextField fullWidth label="Nome" value={form.nome} onChange={handleChange('nome')} required />
            </Grid>
            <Grid size={12}>
              <TextField fullWidth label="Descrição" value={form.descricao} onChange={handleChange('descricao')} multiline rows={2} />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                fullWidth
                label="Preço Unitário (R$)"
                type="number"
                value={form.precoUnitario}
                onChange={handleChange('precoUnitario')}
                required
                slotProps={{ htmlInput: { step: '0.01', min: '0.01' } }}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <FormControl fullWidth>
                <InputLabel>Unidade de Medida</InputLabel>
                <Select
                  value={form.unidadeMedida}
                  label="Unidade de Medida"
                  onChange={(e) => setForm((prev) => ({ ...prev, unidadeMedida: e.target.value }))}
                >
                  {UNIDADES.map((u) => (
                    <MenuItem key={u} value={u}>{u}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={() => setDialogOpen(false)} color="inherit">
            Cancelar
          </Button>
          <Button variant="contained" onClick={handleSave} disabled={isSaving}>
            {isSaving ? <CircularProgress size={20} /> : editingId ? 'Salvar' : 'Criar'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar((s) => ({ ...s, open: false }))}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert
          severity={snackbar.severity}
          onClose={() => setSnackbar((s) => ({ ...s, open: false }))}
          sx={{ borderRadius: 2 }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
