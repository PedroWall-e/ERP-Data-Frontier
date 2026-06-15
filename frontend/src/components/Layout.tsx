import { useState } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import {
  Box,
  Drawer,
  AppBar,
  Toolbar,
  Typography,
  IconButton,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Avatar,
  Chip,
  Tooltip,
  Divider,
  alpha,
  useTheme,
} from '@mui/material';
import {
  Menu as MenuIcon,
  Dashboard as DashboardIcon,
  Inventory2 as InventoryIcon,
  People as PeopleIcon,
  ShoppingCart as ShoppingCartIcon,
  Receipt as ReceiptIcon,
  Settings as SettingsIcon,
  Logout as LogoutIcon,
  ChevronLeft as ChevronLeftIcon,
} from '@mui/icons-material';
import { useAuthStore } from '../stores/authStore';
import { useEmpresa } from '../hooks/useEmpresa';

const DRAWER_WIDTH = 260;
const DRAWER_WIDTH_COLLAPSED = 72;

const menuItems = [
  { text: 'Dashboard', icon: <DashboardIcon />, path: '/' },
  { text: 'Produtos', icon: <InventoryIcon />, path: '/produtos' },
  { text: 'Pessoas', icon: <PeopleIcon />, path: '/pessoas' },
  { text: 'Pedidos', icon: <ShoppingCartIcon />, path: '/pedidos' },
  { text: 'Fiscal', icon: <ReceiptIcon />, path: '/fiscal' },
  { text: 'Configurações', icon: <SettingsIcon />, path: '/empresa' },
];

export default function Layout() {
  const [drawerOpen, setDrawerOpen] = useState(true);
  const navigate = useNavigate();
  const location = useLocation();
  const theme = useTheme();
  const { nomeCompleto, email, tenantTier, logout } = useAuthStore();
  const { data: empresa } = useEmpresa();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const currentWidth = drawerOpen ? DRAWER_WIDTH : DRAWER_WIDTH_COLLAPSED;

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: 'background.default' }}>
      {/* Sidebar */}
      <Drawer
        variant="permanent"
        sx={{
          width: currentWidth,
          flexShrink: 0,
          transition: 'width 0.3s ease',
          '& .MuiDrawer-paper': {
            width: currentWidth,
            transition: 'width 0.3s ease',
            overflowX: 'hidden',
          },
        }}
      >
        <Toolbar
          sx={{
            px: 2,
            justifyContent: drawerOpen ? 'space-between' : 'center',
          }}
        >
          {drawerOpen && (
            <Typography
              variant="h6"
              sx={{
                background: 'linear-gradient(135deg, #6C63FF, #00D9A6)',
                WebkitBackgroundClip: 'text',
                WebkitTextFillColor: 'transparent',
                fontWeight: 800,
                letterSpacing: '-0.03em',
              }}
            >
              DataFrontier
            </Typography>
          )}
          <IconButton onClick={() => setDrawerOpen(!drawerOpen)} size="small">
            {drawerOpen ? <ChevronLeftIcon /> : <MenuIcon />}
          </IconButton>
        </Toolbar>

        <Divider sx={{ borderColor: alpha('#94A3B8', 0.08) }} />

        <List sx={{ px: 1, py: 2, flex: 1 }}>
          {menuItems.map((item) => {
            const isActive = location.pathname === item.path;
            return (
              <Tooltip
                key={item.text}
                title={!drawerOpen ? item.text : ''}
                placement="right"
              >
                <ListItemButton
          onClick={() => !item.disabled && navigate(item.path)}
                  disabled={!!item.disabled}
                  sx={{
                    borderRadius: 2,
                    mb: 0.5,
                    minHeight: 48,
                    justifyContent: drawerOpen ? 'initial' : 'center',
                    px: 2,
                    ...(isActive && {
                      bgcolor: alpha(theme.palette.primary.main, 0.15),
                      '&:hover': {
                        bgcolor: alpha(theme.palette.primary.main, 0.25),
                      },
                    }),
                  }}
                >
                  <ListItemIcon
                    sx={{
                      minWidth: 0,
                      mr: drawerOpen ? 2 : 'auto',
                      color: isActive ? 'primary.main' : 'text.secondary',
                    }}
                  >
                    {item.icon}
                  </ListItemIcon>
                  {drawerOpen && (
                    <ListItemText
                      primary={item.text}
                      slotProps={{
                        primary: {
                          sx: {
                            fontSize: '0.9rem',
                            fontWeight: isActive ? 600 : 400,
                            color: isActive ? 'primary.main' : 'text.primary',
                          },
                        },
                      }}
                    />
                  )}
                </ListItemButton>
              </Tooltip>
            );
          })}
        </List>

        <Divider sx={{ borderColor: alpha('#94A3B8', 0.08) }} />

        {/* User info */}
        <Box sx={{ p: 2 }}>
          {drawerOpen ? (
            <Box
              sx={{
                display: 'flex',
                alignItems: 'center',
                gap: 1.5,
                p: 1.5,
                borderRadius: 2,
                bgcolor: alpha('#94A3B8', 0.06),
              }}
            >
              <Avatar
                sx={{
                  width: 36,
                  height: 36,
                  bgcolor: 'primary.main',
                  fontSize: '0.85rem',
                  fontWeight: 700,
                }}
              >
                {nomeCompleto?.charAt(0).toUpperCase()}
              </Avatar>
              <Box sx={{ flex: 1, minWidth: 0 }}>
                <Typography
                  variant="body2"
                  noWrap
                  sx={{ fontWeight: 600 }}
                >
                  {nomeCompleto}
                </Typography>
                <Typography
                  variant="caption"
                  color="text.secondary"
                  noWrap
                  sx={{ display: 'block' }}
                >
                  {email}
                </Typography>
              </Box>
              <Tooltip title="Sair">
                <IconButton size="small" onClick={handleLogout}>
                  <LogoutIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            </Box>
          ) : (
            <Tooltip title="Sair" placement="right">
              <IconButton onClick={handleLogout} sx={{ mx: 'auto', display: 'block' }}>
                <LogoutIcon />
              </IconButton>
            </Tooltip>
          )}
        </Box>
      </Drawer>

      {/* Main content */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          display: 'flex',
          flexDirection: 'column',
          minHeight: '100vh',
        }}
      >
        <AppBar position="sticky" elevation={0}>
          <Toolbar>
            <Box sx={{ flex: 1 }} />
            {empresa && empresa.ambienteFiscal === 2 && (
              <Chip
                label="⚠️ HOMOLOGAÇÃO"
                size="small"
                sx={{
                  mr: 1.5,
                  fontWeight: 700,
                  letterSpacing: '0.03em',
                  bgcolor: (t) => alpha(t.palette.warning.main, 0.12),
                  color: 'warning.main',
                  border: (t) => `1px solid ${alpha(t.palette.warning.main, 0.3)}`,
                  animation: 'pulse 2s ease-in-out infinite',
                  '@keyframes pulse': {
                    '0%, 100%': { opacity: 1 },
                    '50%': { opacity: 0.7 },
                  },
                }}
              />
            )}
            <Chip
              label={tenantTier === 'Enterprise' ? 'Enterprise' : 'SaaS'}
              size="small"
              color={tenantTier === 'Enterprise' ? 'secondary' : 'primary'}
              variant="outlined"
              sx={{ fontWeight: 600, letterSpacing: '0.03em' }}
            />
          </Toolbar>
        </AppBar>

        <Box sx={{ flex: 1, p: 3 }}>
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
}
