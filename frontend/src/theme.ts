import { createTheme, alpha } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    mode: 'dark',
    primary: {
      main: '#6C63FF',
      light: '#9D97FF',
      dark: '#4B44B2',
    },
    secondary: {
      main: '#00D9A6',
      light: '#33E3BC',
      dark: '#00A67E',
    },
    background: {
      default: '#0A0E1A',
      paper: '#111827',
    },
    error: {
      main: '#FF5C6C',
    },
    warning: {
      main: '#FFB547',
    },
    success: {
      main: '#00D9A6',
    },
    text: {
      primary: '#F1F5F9',
      secondary: '#94A3B8',
    },
    divider: alpha('#94A3B8', 0.12),
  },
  typography: {
    fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
    h1: { fontWeight: 700, letterSpacing: '-0.02em' },
    h2: { fontWeight: 700, letterSpacing: '-0.01em' },
    h3: { fontWeight: 600 },
    h4: { fontWeight: 600 },
    h5: { fontWeight: 600 },
    h6: { fontWeight: 600 },
    button: { textTransform: 'none', fontWeight: 600 },
  },
  shape: {
    borderRadius: 12,
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 10,
          padding: '10px 24px',
          fontSize: '0.95rem',
          transition: 'all 0.2s ease-in-out',
          '&.MuiButton-containedPrimary': {
            background: 'linear-gradient(135deg, #6C63FF 0%, #9D97FF 100%)',
            boxShadow: '0 4px 15px rgba(108, 99, 255, 0.4)',
            '&:hover': {
              background: 'linear-gradient(135deg, #5B53E0 0%, #8B85F0 100%)',
              boxShadow: '0 6px 20px rgba(108, 99, 255, 0.6)',
              transform: 'translateY(-1px)',
            },
          },
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          backgroundImage: 'none',
          backgroundColor: alpha('#111827', 0.8),
          backdropFilter: 'blur(20px)',
          border: `1px solid ${alpha('#94A3B8', 0.08)}`,
        },
      },
    },
    MuiTextField: {
      styleOverrides: {
        root: {
          '& .MuiOutlinedInput-root': {
            borderRadius: 10,
            '& fieldset': {
              borderColor: alpha('#94A3B8', 0.2),
            },
            '&:hover fieldset': {
              borderColor: alpha('#6C63FF', 0.5),
            },
            '&.Mui-focused fieldset': {
              borderColor: '#6C63FF',
            },
          },
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          backgroundImage: 'none',
        },
      },
    },
    MuiAppBar: {
      styleOverrides: {
        root: {
          backgroundImage: 'none',
          backgroundColor: alpha('#111827', 0.8),
          backdropFilter: 'blur(20px)',
          borderBottom: `1px solid ${alpha('#94A3B8', 0.08)}`,
        },
      },
    },
    MuiDrawer: {
      styleOverrides: {
        paper: {
          backgroundColor: '#0D1117',
          borderRight: `1px solid ${alpha('#94A3B8', 0.08)}`,
        },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        head: {
          fontWeight: 600,
          color: '#94A3B8',
          borderBottom: `1px solid ${alpha('#94A3B8', 0.12)}`,
        },
        body: {
          borderBottom: `1px solid ${alpha('#94A3B8', 0.06)}`,
        },
      },
    },
    MuiChip: {
      styleOverrides: {
        root: {
          borderRadius: 8,
          fontWeight: 500,
        },
      },
    },
  },
});

export default theme;
