import { useState, useCallback } from 'react';
import {
  Box, Typography, Button, Card, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, TablePagination, TextField, InputAdornment, IconButton,
  Chip, Dialog, DialogTitle, DialogContent, DialogActions, Grid, Tooltip,
  Skeleton, alpha, Snackbar, Alert, CircularProgress, FormControl, InputLabel,
  Select, MenuItem, RadioGroup, FormControlLabel, Radio, FormLabel, Checkbox,
  Divider, Tab, Tabs,
} from '@mui/material';
import {
  Add as AddIcon, Search as SearchIcon, Edit as EditIcon, Delete as DeleteIcon,
  Close as CloseIcon, LocationOn as LocationIcon, Person as PersonIcon,
} from '@mui/icons-material';
import { usePessoas, useCreatePessoa, useUpdatePessoa, useDeletePessoa } from '../hooks/usePessoas';
import { fetchCep } from '../api/pessoas';
import type { PessoaRequest, Pessoa } from '../api/pessoas';

// ── Helpers ────────────────────────────────────────────────────────
const formatCpfCnpj = (v: string) => {
  const d = v.replace(/\D/g, '');
  if (d.length <= 11) return d.replace(/(\d{3})(\d{3})(\d{3})(\d{0,2})/, '$1.$2.$3-$4').replace(/[-.]+$/, '');
  return d.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{0,2})/, '$1.$2.$3/$4-$5').replace(/[-./]+$/, '');
};
const formatCep = (v: string) => v.replace(/\D/g, '').replace(/(\d{5})(\d{0,3})/, '$1-$2').replace(/-$/, '');
const formatTel = (v: string) => {
  const d = v.replace(/\D/g, '');
  if (d.length <= 10) return d.replace(/(\d{2})(\d{4})(\d{0,4})/, '($1) $2-$3').replace(/[()-\s]+$/, '');
  return d.replace(/(\d{2})(\d{5})(\d{0,4})/, '($1) $2-$3').replace(/[()-\s]+$/, '');
};
const onlyDigits = (v: string) => v.replace(/\D/g, '');

const UF_LIST = ['AC','AL','AM','AP','BA','CE','DF','ES','GO','MA','MG','MS','MT','PA','PB','PE','PI','PR','RJ','RN','RO','RR','RS','SC','SE','SP','TO'];

interface TabPanelProps { children?: React.ReactNode; index: number; value: number; }
function TabPanel({ children, value, index }: TabPanelProps) {
  return value === index ? <Box sx={{ pt: 2 }}>{children}</Box> : null;
}

const EMPTY_FORM: PessoaRequest = {
  tipoPessoa: 'Juridica', cpfCnpj: '', razaoSocial: '', nomeFantasia: '',
  inscricaoEstadual: '', indicadorIE: 'NaoContribuinte',
  isCliente: true, isFornecedor: false, email: '', telefone: '',
  endereco: { logradouro: '', numero: '', complemento: '', bairro: '', cep: '', cidade: '', uf: '', codigoIbge: '' },
};

export default function PessoasPage() {
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [search, setSearch] = useState('');
  const [searchDebounced, setSearchDebounced] = useState('');
  const [tipoFilter, setTipoFilter] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<PessoaRequest>(EMPTY_FORM);
  const [tab, setTab] = useState(0);
  const [cepLoading, setCepLoading] = useState(false);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  const { data, isLoading } = usePessoas({
    pageNumber: page + 1, pageSize: rowsPerPage,
    search: searchDebounced || undefined,
    tipo: tipoFilter || undefined,
  });
  const createMut = useCreatePessoa();
  const updateMut = useUpdatePessoa();
  const deleteMut = useDeletePessoa();

  const handleSearchChange = (value: string) => {
    setSearch(value);
    setTimeout(() => { setSearchDebounced(value); setPage(0); }, 400);
  };

  const handleOpenCreate = () => { setForm(EMPTY_FORM); setEditingId(null); setTab(0); setDialogOpen(true); };

  const handleOpenEdit = (p: Pessoa) => {
    setForm({
      tipoPessoa: p.tipoPessoa, cpfCnpj: p.cpfCnpj, razaoSocial: p.razaoSocial,
      nomeFantasia: p.nomeFantasia || '', inscricaoEstadual: p.inscricaoEstadual || '',
      indicadorIE: p.indicadorIE, isCliente: p.isCliente, isFornecedor: p.isFornecedor,
      email: p.email || '', telefone: p.telefone || '',
      endereco: p.endereco ? { ...p.endereco } : EMPTY_FORM.endereco,
    });
    setEditingId(p.id); setTab(0); setDialogOpen(true);
  };

  const handleSave = async () => {
    try {
      const payload = { ...form, cpfCnpj: onlyDigits(form.cpfCnpj), telefone: form.telefone ? onlyDigits(form.telefone) : undefined,
        endereco: form.endereco?.cep ? { ...form.endereco, cep: onlyDigits(form.endereco.cep) } : undefined };
      if (editingId) {
        await updateMut.mutateAsync({ id: editingId, data: payload });
        setSnackbar({ open: true, message: 'Pessoa atualizada!', severity: 'success' });
      } else {
        await createMut.mutateAsync(payload);
        setSnackbar({ open: true, message: 'Pessoa cadastrada!', severity: 'success' });
      }
      setDialogOpen(false);
    } catch (err: any) {
      setSnackbar({ open: true, message: err.response?.data?.detail || 'Erro ao salvar.', severity: 'error' });
    }
  };

  const handleDelete = async (id: string) => {
    try { await deleteMut.mutateAsync(id); setSnackbar({ open: true, message: 'Pessoa desativada!', severity: 'success' }); }
    catch { setSnackbar({ open: true, message: 'Erro ao desativar.', severity: 'error' }); }
  };

  const setField = (field: string, value: any) => setForm(prev => ({ ...prev, [field]: value }));
  const setEndereco = (field: string, value: string) =>
    setForm(prev => ({ ...prev, endereco: { ...prev.endereco!, [field]: value } }));

  const handleCepBlur = useCallback(async () => {
    const cep = onlyDigits(form.endereco?.cep || '');
    if (cep.length !== 8) return;
    setCepLoading(true);
    const data = await fetchCep(cep);
    if (data) {
      setForm(prev => ({
        ...prev,
        endereco: {
          ...prev.endereco!,
          logradouro: data.logradouro || prev.endereco!.logradouro,
          bairro: data.bairro || prev.endereco!.bairro,
          cidade: data.localidade || prev.endereco!.cidade,
          uf: data.uf || prev.endereco!.uf,
          codigoIbge: data.ibge || prev.endereco!.codigoIbge,
          complemento: data.complemento || prev.endereco!.complemento,
        },
      }));
    }
    setCepLoading(false);
  }, [form.endereco?.cep]);

  const isSaving = createMut.isPending || updateMut.isPending;
  const isPF = form.tipoPessoa === 'Fisica';

  return (
    <Box>
      {/* Header */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3, flexWrap: 'wrap', gap: 2, animation: 'fadeIn 0.4s ease-out' }}>
        <Box>
          <Typography variant="h4" sx={{ fontWeight: 700 }}>Pessoas</Typography>
          <Typography variant="body2" color="text.secondary">
            {data?.totalCount ?? 0} pessoas cadastradas
          </Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={handleOpenCreate}>Nova Pessoa</Button>
      </Box>

      {/* Filters */}
      <Box sx={{ display: 'flex', gap: 2, mb: 2, flexWrap: 'wrap' }}>
        <TextField placeholder="Buscar por nome, CPF/CNPJ ou e-mail..." value={search}
          onChange={(e) => handleSearchChange(e.target.value)} size="small" sx={{ width: { xs: '100%', sm: 360 } }}
          slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon sx={{ color: 'text.secondary' }} /></InputAdornment> } }} />
        <FormControl size="small" sx={{ minWidth: 160 }}>
          <InputLabel>Tipo</InputLabel>
          <Select value={tipoFilter} label="Tipo" onChange={(e) => { setTipoFilter(e.target.value); setPage(0); }}>
            <MenuItem value="">Todos</MenuItem>
            <MenuItem value="cliente">Clientes</MenuItem>
            <MenuItem value="fornecedor">Fornecedores</MenuItem>
          </Select>
        </FormControl>
      </Box>

      {/* Table */}
      <Card elevation={0} sx={{ animation: 'fadeIn 0.5s ease-out 0.1s both' }}>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>CPF/CNPJ</TableCell>
                <TableCell>Razão Social</TableCell>
                <TableCell>Tipo</TableCell>
                <TableCell>Cidade/UF</TableCell>
                <TableCell align="center">Papel</TableCell>
                <TableCell align="center">Status</TableCell>
                <TableCell align="center">Ações</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {isLoading ? Array.from({ length: 5 }).map((_, i) => (
                <TableRow key={i}>{[1,2,3,4,5,6,7].map(j => <TableCell key={j}><Skeleton variant="text" /></TableCell>)}</TableRow>
              )) : data?.items.map((p) => (
                <TableRow key={p.id} sx={{ transition: 'background 0.15s', '&:hover': { bgcolor: alpha('#6C63FF', 0.04) } }}>
                  <TableCell>
                    <Typography variant="body2" sx={{ fontFamily: 'monospace', fontWeight: 500 }}>{formatCpfCnpj(p.cpfCnpj)}</Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" sx={{ fontWeight: 500 }}>{p.razaoSocial}</Typography>
                    {p.nomeFantasia && <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>{p.nomeFantasia}</Typography>}
                  </TableCell>
                  <TableCell>
                    <Chip label={p.tipoPessoa === 'Fisica' ? 'PF' : 'PJ'} size="small" variant="outlined"
                      color={p.tipoPessoa === 'Fisica' ? 'info' : 'secondary'} sx={{ fontWeight: 600 }} />
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" color="text.secondary">
                      {p.endereco ? `${p.endereco.cidade}/${p.endereco.uf}` : '—'}
                    </Typography>
                  </TableCell>
                  <TableCell align="center">
                    <Box sx={{ display: 'flex', gap: 0.5, justifyContent: 'center' }}>
                      {p.isCliente && <Chip label="Cliente" size="small" sx={{ bgcolor: alpha('#6C63FF', 0.12), color: '#6C63FF', fontWeight: 600 }} />}
                      {p.isFornecedor && <Chip label="Fornec." size="small" sx={{ bgcolor: alpha('#FFB547', 0.12), color: '#FFB547', fontWeight: 600 }} />}
                    </Box>
                  </TableCell>
                  <TableCell align="center">
                    <Chip label={p.ativo ? 'Ativo' : 'Inativo'} size="small"
                      sx={{ bgcolor: p.ativo ? alpha('#00D9A6', 0.12) : alpha('#FF5C6C', 0.12), color: p.ativo ? '#00D9A6' : '#FF5C6C', fontWeight: 600 }} />
                  </TableCell>
                  <TableCell align="center">
                    <Tooltip title="Editar"><IconButton size="small" onClick={() => handleOpenEdit(p)}><EditIcon fontSize="small" /></IconButton></Tooltip>
                    <Tooltip title="Desativar"><IconButton size="small" onClick={() => handleDelete(p.id)} disabled={!p.ativo}><DeleteIcon fontSize="small" /></IconButton></Tooltip>
                  </TableCell>
                </TableRow>
              ))}
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

      {/* Dialog */}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="md" fullWidth
        slotProps={{ paper: { sx: { borderRadius: 3 } } }}>
        <DialogTitle sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', fontWeight: 700 }}>
          {editingId ? 'Editar Pessoa' : 'Nova Pessoa'}
          <IconButton size="small" onClick={() => setDialogOpen(false)}><CloseIcon /></IconButton>
        </DialogTitle>
        <DialogContent dividers>
          <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 1 }}>
            <Tab icon={<PersonIcon />} iconPosition="start" label="Dados Gerais" />
            <Tab icon={<LocationIcon />} iconPosition="start" label="Endereço" />
          </Tabs>

          {/* Tab 0: Dados Gerais */}
          <TabPanel value={tab} index={0}>
            <Grid container spacing={2}>
              <Grid size={12}>
                <FormControl><FormLabel sx={{ mb: 1 }}>Tipo de Pessoa</FormLabel>
                  <RadioGroup row value={form.tipoPessoa} onChange={(e) => setField('tipoPessoa', e.target.value)}>
                    <FormControlLabel value="Juridica" control={<Radio />} label="Pessoa Jurídica (CNPJ)" />
                    <FormControlLabel value="Fisica" control={<Radio />} label="Pessoa Física (CPF)" />
                  </RadioGroup>
                </FormControl>
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label={isPF ? 'CPF' : 'CNPJ'} value={formatCpfCnpj(form.cpfCnpj)}
                  onChange={(e) => setField('cpfCnpj', onlyDigits(e.target.value))}
                  required slotProps={{ htmlInput: { maxLength: isPF ? 14 : 18 } }} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label={isPF ? 'Nome Completo' : 'Razão Social'}
                  value={form.razaoSocial} onChange={(e) => setField('razaoSocial', e.target.value)} required />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="Nome Fantasia" value={form.nomeFantasia}
                  onChange={(e) => setField('nomeFantasia', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="E-mail" type="email" value={form.email}
                  onChange={(e) => setField('email', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 4 }}>
                <TextField fullWidth label="Telefone" value={formatTel(form.telefone || '')}
                  onChange={(e) => setField('telefone', onlyDigits(e.target.value))}
                  slotProps={{ htmlInput: { maxLength: 15 } }} />
              </Grid>
              <Grid size={{ xs: 12, sm: 4 }}>
                <TextField fullWidth label="Inscrição Estadual" value={form.inscricaoEstadual}
                  onChange={(e) => setField('inscricaoEstadual', e.target.value)}
                  disabled={form.indicadorIE === 'Isento' || form.indicadorIE === 'NaoContribuinte'} />
              </Grid>
              <Grid size={{ xs: 12, sm: 4 }}>
                <FormControl fullWidth>
                  <InputLabel>Indicador IE</InputLabel>
                  <Select value={form.indicadorIE} label="Indicador IE" onChange={(e) => {
                    setField('indicadorIE', e.target.value);
                    if (e.target.value !== 'ContribuinteICMS') setField('inscricaoEstadual', '');
                  }}>
                    <MenuItem value="ContribuinteICMS">1 — Contribuinte ICMS</MenuItem>
                    <MenuItem value="Isento">2 — Isento</MenuItem>
                    <MenuItem value="NaoContribuinte">9 — Não Contribuinte</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
              <Grid size={12}>
                <Divider sx={{ my: 1 }} />
                <FormControlLabel control={<Checkbox checked={form.isCliente} onChange={(e) => setField('isCliente', e.target.checked)} />} label="Cliente" />
                <FormControlLabel control={<Checkbox checked={form.isFornecedor} onChange={(e) => setField('isFornecedor', e.target.checked)} />} label="Fornecedor" />
              </Grid>
            </Grid>
          </TabPanel>

          {/* Tab 1: Endereço */}
          <TabPanel value={tab} index={1}>
            <Grid container spacing={2}>
              <Grid size={{ xs: 12, sm: 4 }}>
                <TextField fullWidth label="CEP" value={formatCep(form.endereco?.cep || '')}
                  onChange={(e) => setEndereco('cep', onlyDigits(e.target.value))}
                  onBlur={handleCepBlur} slotProps={{ htmlInput: { maxLength: 9 },
                    input: { endAdornment: cepLoading ? <CircularProgress size={18} /> : null } }} />
              </Grid>
              <Grid size={{ xs: 12, sm: 8 }}>
                <TextField fullWidth label="Logradouro" value={form.endereco?.logradouro || ''}
                  onChange={(e) => setEndereco('logradouro', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 3 }}>
                <TextField fullWidth label="Número" value={form.endereco?.numero || ''}
                  onChange={(e) => setEndereco('numero', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 4 }}>
                <TextField fullWidth label="Complemento" value={form.endereco?.complemento || ''}
                  onChange={(e) => setEndereco('complemento', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 5 }}>
                <TextField fullWidth label="Bairro" value={form.endereco?.bairro || ''}
                  onChange={(e) => setEndereco('bairro', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 5 }}>
                <TextField fullWidth label="Cidade" value={form.endereco?.cidade || ''}
                  onChange={(e) => setEndereco('cidade', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 3 }}>
                <FormControl fullWidth>
                  <InputLabel>UF</InputLabel>
                  <Select value={form.endereco?.uf || ''} label="UF" onChange={(e) => setEndereco('uf', e.target.value)}>
                    {UF_LIST.map(uf => <MenuItem key={uf} value={uf}>{uf}</MenuItem>)}
                  </Select>
                </FormControl>
              </Grid>
              <Grid size={{ xs: 12, sm: 4 }}>
                <TextField fullWidth label="Código IBGE" value={form.endereco?.codigoIbge || ''}
                  onChange={(e) => setEndereco('codigoIbge', onlyDigits(e.target.value))}
                  slotProps={{ htmlInput: { maxLength: 7 } }}
                  helperText="Preenchido automaticamente pelo CEP" />
              </Grid>
            </Grid>
          </TabPanel>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={() => setDialogOpen(false)} color="inherit">Cancelar</Button>
          <Button variant="contained" onClick={handleSave} disabled={isSaving}>
            {isSaving ? <CircularProgress size={20} /> : editingId ? 'Salvar' : 'Cadastrar'}
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
