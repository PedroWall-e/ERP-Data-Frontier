import { useState, useEffect, useCallback, useRef } from 'react';
import {
  Box, Typography, Tabs, Tab, TextField, Button, Card, Select, MenuItem,
  FormControl, InputLabel, Alert, Snackbar, Chip, IconButton, alpha, Switch,
  FormControlLabel, Dialog, DialogTitle, DialogContent, DialogContentText,
  DialogActions, CircularProgress, Tooltip, InputAdornment,
} from '@mui/material';
import Grid from '@mui/material/Grid2';
import {
  Business as BusinessIcon, LocationOn as LocationIcon, VpnKey as CertIcon,
  Receipt as FiscalIcon, AccountBalance as BankIcon, Save as SaveIcon,
  CloudUpload as UploadIcon, Delete as DeleteIcon, CheckCircle as ValidIcon,
  Error as ExpiredIcon, Warning as WarningIcon, Visibility, VisibilityOff,
} from '@mui/icons-material';
import { useEmpresa, useSaveEmpresa, useUploadCertificado, useRemoverCertificado } from '../hooks/useEmpresa';
import type { ConfiguracaoEmpresaRequest } from '../api/empresa';

const UF_LIST = [
  'AC','AL','AM','AP','BA','CE','DF','ES','GO','MA','MG','MS','MT','PA',
  'PB','PE','PI','PR','RJ','RN','RO','RR','RS','SC','SE','SP','TO',
];

const CRT_OPTIONS = [
  { value: 1, label: '1 – Simples Nacional' },
  { value: 2, label: '2 – Simples Nacional (excesso)' },
  { value: 3, label: '3 – Regime Normal' },
];

const glassCard = {
  background: (t: any) => alpha(t.palette.background.paper, 0.6),
  backdropFilter: 'blur(20px)',
  border: (t: any) => `1px solid ${alpha(t.palette.divider, 0.08)}`,
  borderRadius: 3,
  p: 4,
  transition: 'all 0.3s ease',
};

interface TabPanelProps { children: React.ReactNode; value: number; index: number; }
function TabPanel({ children, value, index }: TabPanelProps) {
  return (
    <Box
      role="tabpanel"
      hidden={value !== index}
      sx={{
        opacity: value === index ? 1 : 0,
        transform: value === index ? 'translateY(0)' : 'translateY(8px)',
        transition: 'opacity 0.3s ease, transform 0.3s ease',
        pt: 3,
      }}
    >
      {value === index && children}
    </Box>
  );
}

const tabItems = [
  { label: 'Dados Gerais', icon: <BusinessIcon /> },
  { label: 'Endereço', icon: <LocationIcon /> },
  { label: 'Certificado Digital', icon: <CertIcon /> },
  { label: 'Fiscal', icon: <FiscalIcon /> },
  { label: 'Banco Inter (PIX)', icon: <BankIcon /> },
];

export default function EmpresaPage() {
  const { data: empresa, isLoading } = useEmpresa();
  const saveMut = useSaveEmpresa();
  const uploadCertMut = useUploadCertificado();
  const removeCertMut = useRemoverCertificado();

  const [activeTab, setActiveTab] = useState(0);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });
  const [showProducaoDialog, setShowProducaoDialog] = useState(false);
  const [certSenha, setCertSenha] = useState('');
  const [certFile, setCertFile] = useState<File | null>(null);
  const [showSecret, setShowSecret] = useState(false);
  const [isDragging, setIsDragging] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [form, setForm] = useState<ConfiguracaoEmpresaRequest>({
    razaoSocial: '', nomeFantasia: '', cnpj: '', inscricaoEstadual: '',
    inscricaoMunicipal: '', crt: 1, logradouro: '', numero: '', complemento: '',
    bairro: '', codigoIbge: '', municipio: '', uf: '', cep: '', telefone: '',
    email: '', ambienteFiscal: 2, serieNfe: 1, serieNfse: 1,
    interClientId: '', interClientSecret: '',
  });

  useEffect(() => {
    if (empresa) {
      setForm({
        razaoSocial: empresa.razaoSocial || '',
        nomeFantasia: empresa.nomeFantasia || '',
        cnpj: empresa.cnpj || '',
        inscricaoEstadual: empresa.inscricaoEstadual || '',
        inscricaoMunicipal: empresa.inscricaoMunicipal || '',
        crt: empresa.crt || 1,
        logradouro: empresa.logradouro || '',
        numero: empresa.numero || '',
        complemento: empresa.complemento || '',
        bairro: empresa.bairro || '',
        codigoIbge: empresa.codigoIbge || '',
        municipio: empresa.municipio || '',
        uf: empresa.uf || '',
        cep: empresa.cep || '',
        telefone: empresa.telefone || '',
        email: empresa.email || '',
        ambienteFiscal: empresa.ambienteFiscal || 2,
        serieNfe: empresa.serieNfe || 1,
        serieNfse: empresa.serieNfse || 1,
        interClientId: empresa.interClientId || '',
        interClientSecret: '',
      });
    }
  }, [empresa]);

  const update = (field: keyof ConfiguracaoEmpresaRequest, value: any) =>
    setForm(prev => ({ ...prev, [field]: value }));

  const handleSave = async () => {
    try {
      await saveMut.mutateAsync(form);
      setSnackbar({ open: true, message: 'Configurações salvas com sucesso!', severity: 'success' });
    } catch {
      setSnackbar({ open: true, message: 'Erro ao salvar configurações.', severity: 'error' });
    }
  };

  const handleUploadCert = async () => {
    if (!certFile || !certSenha) return;
    try {
      await uploadCertMut.mutateAsync({ file: certFile, senha: certSenha });
      setCertFile(null);
      setCertSenha('');
      setSnackbar({ open: true, message: 'Certificado enviado com sucesso!', severity: 'success' });
    } catch {
      setSnackbar({ open: true, message: 'Erro ao enviar certificado. Verifique a senha.', severity: 'error' });
    }
  };

  const handleRemoveCert = async () => {
    try {
      await removeCertMut.mutateAsync();
      setSnackbar({ open: true, message: 'Certificado removido.', severity: 'success' });
    } catch {
      setSnackbar({ open: true, message: 'Erro ao remover certificado.', severity: 'error' });
    }
  };

  const handleAmbienteChange = (checked: boolean) => {
    if (checked) {
      setShowProducaoDialog(true);
    } else {
      update('ambienteFiscal', 2);
    }
  };

  const confirmProducao = () => {
    update('ambienteFiscal', 1);
    setShowProducaoDialog(false);
  };

  const handleDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
    const file = e.dataTransfer.files[0];
    if (file && file.name.endsWith('.pfx')) setCertFile(file);
  }, []);

  const handleDragOver = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(true);
  }, []);

  const handleDragLeave = useCallback(() => setIsDragging(false), []);

  const certStatus = !empresa?.temCertificado
    ? { icon: <WarningIcon />, label: 'Ausente', color: '#FFA726' }
    : empresa.certificadoExpirado
      ? { icon: <ExpiredIcon />, label: 'Expirado', color: '#EF5350' }
      : { icon: <ValidIcon />, label: 'Válido', color: '#66BB6A' };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '60vh' }}>
        <CircularProgress sx={{ color: '#6C63FF' }} />
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" sx={{ fontWeight: 700 }}>Configurações da Empresa</Typography>
        <Typography variant="body2" color="text.secondary">
          Gerencie os dados cadastrais, certificados e configurações fiscais
        </Typography>
      </Box>

      <Card elevation={0} sx={{ ...glassCard, p: 0, overflow: 'visible' }}>
        <Tabs
          value={activeTab}
          onChange={(_, v) => setActiveTab(v)}
          variant="scrollable"
          scrollButtons="auto"
          sx={{
            px: 2, pt: 1,
            borderBottom: (t) => `1px solid ${alpha(t.palette.divider, 0.08)}`,
            '& .MuiTab-root': {
              textTransform: 'none', fontWeight: 500, fontSize: '0.9rem',
              minHeight: 56, gap: 1,
              transition: 'all 0.2s ease',
            },
            '& .Mui-selected': { fontWeight: 700 },
            '& .MuiTabs-indicator': {
              height: 3, borderRadius: '3px 3px 0 0',
              background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
            },
          }}
        >
          {tabItems.map((tab, i) => (
            <Tab key={i} icon={tab.icon} iconPosition="start" label={tab.label} />
          ))}
        </Tabs>

        <Box sx={{ p: { xs: 2, sm: 4 } }}>
          {/* Tab 1 – Dados Gerais */}
          <TabPanel value={activeTab} index={0}>
            <Grid container spacing={3}>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="CNPJ" value={form.cnpj}
                  onChange={(e) => update('cnpj', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <FormControl fullWidth>
                  <InputLabel>Regime Tributário (CRT)</InputLabel>
                  <Select value={form.crt} label="Regime Tributário (CRT)"
                    onChange={(e) => update('crt', e.target.value)}>
                    {CRT_OPTIONS.map(o => <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>)}
                  </Select>
                </FormControl>
              </Grid>
              <Grid size={12}>
                <TextField fullWidth label="Razão Social" value={form.razaoSocial}
                  onChange={(e) => update('razaoSocial', e.target.value)} />
              </Grid>
              <Grid size={12}>
                <TextField fullWidth label="Nome Fantasia" value={form.nomeFantasia}
                  onChange={(e) => update('nomeFantasia', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="Inscrição Estadual" value={form.inscricaoEstadual}
                  onChange={(e) => update('inscricaoEstadual', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="Inscrição Municipal" value={form.inscricaoMunicipal || ''}
                  onChange={(e) => update('inscricaoMunicipal', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="Telefone" value={form.telefone || ''}
                  onChange={(e) => update('telefone', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="E-mail" value={form.email || ''}
                  onChange={(e) => update('email', e.target.value)} />
              </Grid>
            </Grid>
            <Box sx={{ mt: 4, display: 'flex', justifyContent: 'flex-end' }}>
              <Button variant="contained" startIcon={<SaveIcon />} onClick={handleSave}
                disabled={saveMut.isPending}
                sx={{ px: 4, py: 1.2, borderRadius: 2, fontWeight: 600, textTransform: 'none',
                  background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
                  '&:hover': { background: 'linear-gradient(135deg, #5B52EE, #00C495)' } }}>
                {saveMut.isPending ? 'Salvando...' : 'Salvar Dados'}
              </Button>
            </Box>
          </TabPanel>

          {/* Tab 2 – Endereço */}
          <TabPanel value={activeTab} index={1}>
            <Grid container spacing={3}>
              <Grid size={{ xs: 12, sm: 9 }}>
                <TextField fullWidth label="Logradouro" value={form.logradouro}
                  onChange={(e) => update('logradouro', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 3 }}>
                <TextField fullWidth label="Número" value={form.numero}
                  onChange={(e) => update('numero', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="Complemento" value={form.complemento || ''}
                  onChange={(e) => update('complemento', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="Bairro" value={form.bairro}
                  onChange={(e) => update('bairro', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="Município" value={form.municipio}
                  onChange={(e) => update('municipio', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 3 }}>
                <FormControl fullWidth>
                  <InputLabel>UF</InputLabel>
                  <Select value={form.uf} label="UF"
                    onChange={(e) => update('uf', e.target.value)}>
                    {UF_LIST.map(uf => <MenuItem key={uf} value={uf}>{uf}</MenuItem>)}
                  </Select>
                </FormControl>
              </Grid>
              <Grid size={{ xs: 12, sm: 3 }}>
                <TextField fullWidth label="CEP" value={form.cep}
                  onChange={(e) => update('cep', e.target.value)} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth label="Código IBGE" value={form.codigoIbge}
                  onChange={(e) => update('codigoIbge', e.target.value)} />
              </Grid>
            </Grid>
            <Box sx={{ mt: 4, display: 'flex', justifyContent: 'flex-end' }}>
              <Button variant="contained" startIcon={<SaveIcon />} onClick={handleSave}
                disabled={saveMut.isPending}
                sx={{ px: 4, py: 1.2, borderRadius: 2, fontWeight: 600, textTransform: 'none',
                  background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
                  '&:hover': { background: 'linear-gradient(135deg, #5B52EE, #00C495)' } }}>
                {saveMut.isPending ? 'Salvando...' : 'Salvar Endereço'}
              </Button>
            </Box>
          </TabPanel>

          {/* Tab 3 – Certificado Digital */}
          <TabPanel value={activeTab} index={2}>
            {/* Current status */}
            <Card elevation={0} sx={{
              ...glassCard, mb: 3, display: 'flex', alignItems: 'center', gap: 2, flexWrap: 'wrap',
            }}>
              <Chip
                icon={certStatus.icon}
                label={certStatus.label}
                sx={{
                  bgcolor: alpha(certStatus.color, 0.12),
                  color: certStatus.color,
                  fontWeight: 700, fontSize: '0.9rem',
                  '& .MuiChip-icon': { color: certStatus.color },
                }}
              />
              {empresa?.temCertificado && (
                <>
                  <Box sx={{ flex: 1, minWidth: 200 }}>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      {empresa.certificadoNome}
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      Validade: {empresa.certificadoValidade
                        ? new Date(empresa.certificadoValidade).toLocaleDateString('pt-BR')
                        : '—'}
                    </Typography>
                  </Box>
                  <Tooltip title="Remover certificado">
                    <IconButton color="error" onClick={handleRemoveCert}
                      disabled={removeCertMut.isPending}>
                      <DeleteIcon />
                    </IconButton>
                  </Tooltip>
                </>
              )}
            </Card>

            {/* Upload area */}
            <Box
              onDrop={handleDrop}
              onDragOver={handleDragOver}
              onDragLeave={handleDragLeave}
              onClick={() => fileInputRef.current?.click()}
              sx={{
                border: (t) => `2px dashed ${isDragging ? '#6C63FF' : alpha(t.palette.divider, 0.2)}`,
                borderRadius: 3, p: 5, textAlign: 'center', cursor: 'pointer',
                bgcolor: (t) => isDragging ? alpha('#6C63FF', 0.05) : alpha(t.palette.background.paper, 0.3),
                transition: 'all 0.3s ease',
                '&:hover': { borderColor: '#6C63FF', bgcolor: (t) => alpha('#6C63FF', 0.03) },
              }}
            >
              <input
                type="file"
                accept=".pfx"
                ref={fileInputRef}
                hidden
                onChange={(e) => { if (e.target.files?.[0]) setCertFile(e.target.files[0]); }}
              />
              <UploadIcon sx={{ fontSize: 48, color: 'text.secondary', mb: 1 }} />
              <Typography variant="body1" sx={{ fontWeight: 600, mb: 0.5 }}>
                {certFile ? certFile.name : 'Arraste o certificado .pfx aqui'}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                ou clique para selecionar o arquivo
              </Typography>
            </Box>

            {certFile && (
              <Box sx={{ mt: 3, display: 'flex', gap: 2, alignItems: 'flex-end', flexWrap: 'wrap' }}>
                <TextField
                  label="Senha do Certificado"
                  type="password"
                  value={certSenha}
                  onChange={(e) => setCertSenha(e.target.value)}
                  sx={{ flex: 1, minWidth: 240 }}
                />
                <Button
                  variant="contained"
                  startIcon={uploadCertMut.isPending ? <CircularProgress size={18} /> : <UploadIcon />}
                  onClick={handleUploadCert}
                  disabled={uploadCertMut.isPending || !certSenha}
                  sx={{
                    px: 4, py: 1.5, borderRadius: 2, fontWeight: 600, textTransform: 'none',
                    background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
                    '&:hover': { background: 'linear-gradient(135deg, #5B52EE, #00C495)' },
                  }}
                >
                  Enviar Certificado
                </Button>
              </Box>
            )}
          </TabPanel>

          {/* Tab 4 – Fiscal */}
          <TabPanel value={activeTab} index={3}>
            <Card elevation={0} sx={{ ...glassCard, mb: 3 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: 2 }}>
                <Box>
                  <Typography variant="h6" sx={{ fontWeight: 600 }}>Ambiente Fiscal</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {form.ambienteFiscal === 1
                      ? 'Produção — NF-e com validade jurídica'
                      : 'Homologação — ambiente de testes'}
                  </Typography>
                </Box>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <Typography variant="body2" color="text.secondary">Homologação</Typography>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={form.ambienteFiscal === 1}
                        onChange={(_, checked) => handleAmbienteChange(checked)}
                        sx={{
                          '& .MuiSwitch-switchBase.Mui-checked': { color: '#66BB6A' },
                          '& .MuiSwitch-switchBase.Mui-checked + .MuiSwitch-track': { bgcolor: '#66BB6A' },
                        }}
                      />
                    }
                    label=""
                  />
                  <Typography variant="body2" sx={{
                    fontWeight: 600,
                    color: form.ambienteFiscal === 1 ? '#66BB6A' : '#FFA726',
                  }}>
                    {form.ambienteFiscal === 1 ? 'Produção' : 'Homologação'}
                  </Typography>
                </Box>
              </Box>
            </Card>

            <Grid container spacing={3}>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth type="number" label="Série NF-e" value={form.serieNfe}
                  onChange={(e) => update('serieNfe', parseInt(e.target.value) || 1)}
                  slotProps={{ input: { inputProps: { min: 1 } } }} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField fullWidth type="number" label="Série NFS-e" value={form.serieNfse}
                  onChange={(e) => update('serieNfse', parseInt(e.target.value) || 1)}
                  slotProps={{ input: { inputProps: { min: 1 } } }} />
              </Grid>
            </Grid>

            <Box sx={{ mt: 4, display: 'flex', justifyContent: 'flex-end' }}>
              <Button variant="contained" startIcon={<SaveIcon />} onClick={handleSave}
                disabled={saveMut.isPending}
                sx={{ px: 4, py: 1.2, borderRadius: 2, fontWeight: 600, textTransform: 'none',
                  background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
                  '&:hover': { background: 'linear-gradient(135deg, #5B52EE, #00C495)' } }}>
                {saveMut.isPending ? 'Salvando...' : 'Salvar Fiscal'}
              </Button>
            </Box>
          </TabPanel>

          {/* Tab 5 – Banco Inter */}
          <TabPanel value={activeTab} index={4}>
            <Alert severity="info" sx={{ mb: 3, borderRadius: 2 }}>
              Configure a integração com o Banco Inter para gerar cobranças PIX automaticamente.
            </Alert>

            <Grid container spacing={3}>
              <Grid size={12}>
                <TextField fullWidth label="Client ID" value={form.interClientId || ''}
                  onChange={(e) => update('interClientId', e.target.value)} />
              </Grid>
              <Grid size={12}>
                <TextField
                  fullWidth
                  label="Client Secret"
                  type={showSecret ? 'text' : 'password'}
                  value={form.interClientSecret || ''}
                  onChange={(e) => update('interClientSecret', e.target.value)}
                  slotProps={{
                    input: {
                      endAdornment: (
                        <InputAdornment position="end">
                          <IconButton onClick={() => setShowSecret(!showSecret)} edge="end" size="small">
                            {showSecret ? <VisibilityOff /> : <Visibility />}
                          </IconButton>
                        </InputAdornment>
                      ),
                    },
                  }}
                />
              </Grid>
            </Grid>

            <Box sx={{ mt: 4, display: 'flex', justifyContent: 'flex-end' }}>
              <Button variant="contained" startIcon={<SaveIcon />} onClick={handleSave}
                disabled={saveMut.isPending}
                sx={{ px: 4, py: 1.2, borderRadius: 2, fontWeight: 600, textTransform: 'none',
                  background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
                  '&:hover': { background: 'linear-gradient(135deg, #5B52EE, #00C495)' } }}>
                {saveMut.isPending ? 'Salvando...' : 'Salvar Integração'}
              </Button>
            </Box>
          </TabPanel>
        </Box>
      </Card>

      {/* Produção warning dialog */}
      <Dialog open={showProducaoDialog} onClose={() => setShowProducaoDialog(false)}>
        <DialogTitle sx={{ fontWeight: 700 }}>⚠️ Atenção — Ambiente de Produção</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Ao mudar para <strong>Produção</strong>, todas as NF-e emitidas terão{' '}
            <strong>validade jurídica</strong>. Certifique-se de que todos os dados cadastrais e o
            certificado digital estão corretos antes de prosseguir.
          </DialogContentText>
        </DialogContent>
        <DialogActions sx={{ p: 2.5 }}>
          <Button onClick={() => setShowProducaoDialog(false)} sx={{ textTransform: 'none' }}>
            Cancelar
          </Button>
          <Button variant="contained" color="warning" onClick={confirmProducao}
            sx={{ textTransform: 'none', fontWeight: 600 }}>
            Confirmar Produção
          </Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={snackbar.open} autoHideDuration={4000}
        onClose={() => setSnackbar(s => ({ ...s, open: false }))}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <Alert severity={snackbar.severity}
          onClose={() => setSnackbar(s => ({ ...s, open: false }))}
          sx={{ borderRadius: 2 }}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
