import { useState, useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box, Typography, Button, Card, CardContent, TextField, Autocomplete,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
  IconButton, Grid, Divider, Snackbar, Alert, CircularProgress, alpha,
  InputAdornment, Tooltip, Chip,
} from '@mui/material';
import {
  ArrowBack as BackIcon, Add as AddIcon, Delete as DeleteIcon,
  ShoppingCart as CartIcon, Save as SaveIcon,
} from '@mui/icons-material';
import { useCreatePedido } from '../hooks/usePedidos';
import { usePessoas } from '../hooks/usePessoas';
import { useProdutos } from '../hooks/useProdutos';
import type { Pessoa } from '../api/pessoas';
import type { Produto } from '../api/produtos';

interface ItemForm {
  produtoId: string;
  produtoNome: string;
  produtoCodigo: string;
  quantidade: number;
  valorUnitario: number;
  valorDesconto: number;
  valorTotal: number;
}

const fmt = (v: number) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });

export default function NovoPedidoPage() {
  const navigate = useNavigate();
  const createMut = useCreatePedido();

  // Client search
  const [clienteInput, setClienteInput] = useState('');
  const [clienteDebounced, setClienteDebounced] = useState('');
  const [selectedCliente, setSelectedCliente] = useState<Pessoa | null>(null);

  // Product search for add-item
  const [produtoInput, setProdutoInput] = useState('');
  const [produtoDebounced, setProdutoDebounced] = useState('');
  const [selectedProduto, setSelectedProduto] = useState<Produto | null>(null);

  // Add item form
  const [quantidade, setQuantidade] = useState<number>(1);
  const [valorUnitario, setValorUnitario] = useState<number>(0);
  const [valorDesconto, setValorDesconto] = useState<number>(0);

  // Order items & notes
  const [itens, setItens] = useState<ItemForm[]>([]);
  const [observacoes, setObservacoes] = useState('');
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  // Debounce searches
  useEffect(() => {
    const t = setTimeout(() => setClienteDebounced(clienteInput), 300);
    return () => clearTimeout(t);
  }, [clienteInput]);
  useEffect(() => {
    const t = setTimeout(() => setProdutoDebounced(produtoInput), 300);
    return () => clearTimeout(t);
  }, [produtoInput]);

  const { data: clientesData, isLoading: clientesLoading } = usePessoas({
    search: clienteDebounced || undefined, pageSize: 15, tipo: 'cliente',
  });
  const { data: produtosData, isLoading: produtosLoading } = useProdutos({
    search: produtoDebounced || undefined, pageSize: 15,
  });

  // When product selected, fill unit price
  useEffect(() => {
    if (selectedProduto) setValorUnitario(selectedProduto.precoUnitario);
  }, [selectedProduto]);

  // Totals
  const totals = useMemo(() => {
    const produtos = itens.reduce((s, i) => s + i.valorUnitario * i.quantidade, 0);
    const descontos = itens.reduce((s, i) => s + i.valorDesconto, 0);
    return { produtos, descontos, total: produtos - descontos };
  }, [itens]);

  const handleAddItem = () => {
    if (!selectedProduto || quantidade <= 0) return;
    const bruto = valorUnitario * quantidade;
    const item: ItemForm = {
      produtoId: selectedProduto.id,
      produtoNome: selectedProduto.nome,
      produtoCodigo: selectedProduto.codigo,
      quantidade,
      valorUnitario,
      valorDesconto,
      valorTotal: bruto - valorDesconto,
    };
    setItens(prev => [...prev, item]);
    setSelectedProduto(null); setProdutoInput('');
    setQuantidade(1); setValorUnitario(0); setValorDesconto(0);
  };

  const handleRemoveItem = (idx: number) => setItens(prev => prev.filter((_, i) => i !== idx));

  const handleSave = async () => {
    if (!selectedCliente) { setSnackbar({ open: true, message: 'Selecione um cliente.', severity: 'error' }); return; }
    if (itens.length === 0) { setSnackbar({ open: true, message: 'Adicione ao menos um item.', severity: 'error' }); return; }
    try {
      await createMut.mutateAsync({
        pessoaId: selectedCliente.id,
        observacoes: observacoes || undefined,
        itens: itens.map(i => ({
          produtoId: i.produtoId,
          quantidade: i.quantidade,
          valorUnitario: i.valorUnitario,
          valorDesconto: i.valorDesconto,
        })),
      });
      setSnackbar({ open: true, message: 'Pedido criado com sucesso!', severity: 'success' });
      setTimeout(() => navigate('/pedidos'), 1200);
    } catch (err: any) {
      setSnackbar({ open: true, message: err.response?.data?.detail || 'Erro ao criar pedido.', severity: 'error' });
    }
  };

  return (
    <Box>
      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
        <IconButton onClick={() => navigate('/pedidos')}><BackIcon /></IconButton>
        <Box sx={{ flex: 1 }}>
          <Typography variant="h4" sx={{ fontWeight: 700 }}>Novo Pedido</Typography>
          <Typography variant="body2" color="text.secondary">Preencha os dados e adicione os itens</Typography>
        </Box>
        <Button variant="contained" startIcon={createMut.isPending ? <CircularProgress size={18} color="inherit" /> : <SaveIcon />}
          onClick={handleSave} disabled={createMut.isPending || !selectedCliente || itens.length === 0}
          size="large" sx={{ px: 4 }}>
          Salvar Pedido
        </Button>
      </Box>

      {/* Client & Notes */}
      <Card elevation={0} sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 2 }}>Dados do Pedido</Typography>
          <Grid container spacing={2}>
            <Grid size={{ xs: 12, md: 6 }}>
              <Autocomplete
                options={clientesData?.items ?? []}
                getOptionLabel={(o: Pessoa) => `${o.razaoSocial} (${o.cpfCnpj})`}
                value={selectedCliente}
                onChange={(_, v) => setSelectedCliente(v)}
                onInputChange={(_, v) => setClienteInput(v)}
                loading={clientesLoading}
                filterOptions={(x) => x}
                isOptionEqualToValue={(o, v) => o.id === v.id}
                renderInput={(params) => <TextField {...params} label="Cliente *" placeholder="Buscar por nome ou CPF/CNPJ..." />}
                noOptionsText="Nenhum cliente encontrado"
              />
            </Grid>
            <Grid size={{ xs: 12, md: 6 }}>
              <TextField fullWidth label="Observações" value={observacoes}
                onChange={(e) => setObservacoes(e.target.value)} multiline rows={1} />
            </Grid>
          </Grid>
        </CardContent>
      </Card>

      {/* Add Item */}
      <Card elevation={0} sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 2 }}>Adicionar Item</Typography>
          <Grid container spacing={2} alignItems="center">
            <Grid size={{ xs: 12, md: 4 }}>
              <Autocomplete
                options={produtosData?.items ?? []}
                getOptionLabel={(o: Produto) => `${o.codigo} - ${o.nome}`}
                value={selectedProduto}
                onChange={(_, v) => setSelectedProduto(v)}
                onInputChange={(_, v) => setProdutoInput(v)}
                loading={produtosLoading}
                filterOptions={(x) => x}
                isOptionEqualToValue={(o, v) => o.id === v.id}
                renderInput={(params) => <TextField {...params} label="Produto" placeholder="Buscar produto..." />}
                noOptionsText="Nenhum produto encontrado"
              />
            </Grid>
            <Grid size={{ xs: 6, md: 2 }}>
              <TextField fullWidth label="Qtd" type="number" value={quantidade}
                onChange={(e) => setQuantidade(Math.max(0, Number(e.target.value)))}
                slotProps={{ htmlInput: { min: 0.0001, step: 1 } }} />
            </Grid>
            <Grid size={{ xs: 6, md: 2 }}>
              <TextField fullWidth label="Valor Unit." type="number" value={valorUnitario}
                onChange={(e) => setValorUnitario(Math.max(0, Number(e.target.value)))}
                slotProps={{ input: { startAdornment: <InputAdornment position="start">R$</InputAdornment> } }} />
            </Grid>
            <Grid size={{ xs: 6, md: 2 }}>
              <TextField fullWidth label="Desconto" type="number" value={valorDesconto}
                onChange={(e) => setValorDesconto(Math.max(0, Number(e.target.value)))}
                slotProps={{ input: { startAdornment: <InputAdornment position="start">R$</InputAdornment> } }} />
            </Grid>
            <Grid size={{ xs: 6, md: 2 }}>
              <Button variant="outlined" fullWidth startIcon={<AddIcon />} onClick={handleAddItem}
                disabled={!selectedProduto || quantidade <= 0} sx={{ height: 56 }}>
                Adicionar
              </Button>
            </Grid>
          </Grid>
        </CardContent>
      </Card>

      {/* Items Table */}
      <Card elevation={0} sx={{ mb: 3 }}>
        <CardContent sx={{ pb: '16px !important' }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
            <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>
              Itens do Pedido
            </Typography>
            {itens.length > 0 && <Chip icon={<CartIcon />} label={`${itens.length} ite${itens.length > 1 ? 'ns' : 'm'}`} size="small" color="primary" variant="outlined" />}
          </Box>

          {itens.length === 0 ? (
            <Box sx={{ textAlign: 'center', py: 6, color: 'text.secondary' }}>
              <CartIcon sx={{ fontSize: 48, mb: 1, opacity: 0.3 }} />
              <Typography>Nenhum item adicionado</Typography>
            </Box>
          ) : (
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>#</TableCell>
                    <TableCell>Produto</TableCell>
                    <TableCell align="right">Qtd</TableCell>
                    <TableCell align="right">Unitário</TableCell>
                    <TableCell align="right">Desconto</TableCell>
                    <TableCell align="right">Subtotal</TableCell>
                    <TableCell align="center" />
                  </TableRow>
                </TableHead>
                <TableBody>
                  {itens.map((item, idx) => (
                    <TableRow key={idx} sx={{ '&:hover': { bgcolor: alpha('#6C63FF', 0.04) } }}>
                      <TableCell>{idx + 1}</TableCell>
                      <TableCell>
                        <Typography variant="body2" sx={{ fontWeight: 500 }}>{item.produtoNome}</Typography>
                        <Typography variant="caption" color="text.secondary">{item.produtoCodigo}</Typography>
                      </TableCell>
                      <TableCell align="right">{item.quantidade}</TableCell>
                      <TableCell align="right">{fmt(item.valorUnitario)}</TableCell>
                      <TableCell align="right">{item.valorDesconto > 0 ? fmt(item.valorDesconto) : '\u2014'}</TableCell>
                      <TableCell align="right">
                        <Typography variant="body2" sx={{ fontWeight: 600 }}>{fmt(item.valorTotal)}</Typography>
                      </TableCell>
                      <TableCell align="center">
                        <Tooltip title="Remover"><IconButton size="small" color="error" onClick={() => handleRemoveItem(idx)}><DeleteIcon fontSize="small" /></IconButton></Tooltip>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          )}

          {itens.length > 0 && (
            <>
              <Divider sx={{ my: 2 }} />
              <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 0.5 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', width: 280 }}>
                  <Typography color="text.secondary">Subtotal Produtos:</Typography>
                  <Typography sx={{ fontWeight: 500 }}>{fmt(totals.produtos)}</Typography>
                </Box>
                {totals.descontos > 0 && (
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', width: 280 }}>
                    <Typography color="text.secondary">Total Descontos:</Typography>
                    <Typography sx={{ fontWeight: 500, color: 'error.main' }}>-{fmt(totals.descontos)}</Typography>
                  </Box>
                )}
                <Divider sx={{ width: 280, my: 1 }} />
                <Box sx={{ display: 'flex', justifyContent: 'space-between', width: 280 }}>
                  <Typography variant="h6" sx={{ fontWeight: 700 }}>TOTAL:</Typography>
                  <Typography variant="h6" sx={{ fontWeight: 700, color: 'primary.main' }}>{fmt(totals.total)}</Typography>
                </Box>
              </Box>
            </>
          )}
        </CardContent>
      </Card>

      <Snackbar open={snackbar.open} autoHideDuration={4000}
        onClose={() => setSnackbar(s => ({ ...s, open: false }))} anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <Alert severity={snackbar.severity} onClose={() => setSnackbar(s => ({ ...s, open: false }))} sx={{ borderRadius: 2 }}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
}
