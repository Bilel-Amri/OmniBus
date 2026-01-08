import { Alert, AlertTitle, Box, Button } from '@mui/material';
import { Error as ErrorIcon, Refresh } from '@mui/icons-material';

interface ErrorMessageProps {
  message?: string;
  title?: string;
  onRetry?: () => void;
  fullHeight?: boolean;
}

export default function ErrorMessage({
  message = 'Something went wrong. Please try again.',
  title = 'Error',
  onRetry,
  fullHeight = false,
}: ErrorMessageProps) {
  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: fullHeight ? '60vh' : 'auto',
        p: 3,
      }}
    >
      <Alert
        severity="error"
        icon={<ErrorIcon />}
        sx={{ maxWidth: 600 }}
        action={
          onRetry && (
            <Button color="inherit" size="small" onClick={onRetry} startIcon={<Refresh />}>
              Retry
            </Button>
          )
        }
      >
        <AlertTitle>{title}</AlertTitle>
        {message}
      </Alert>
    </Box>
  );
}
