import { CircularProgress, Box } from '@mui/material'

interface LoadingSpinnerProps {
  fullScreen?: boolean
  size?: number
}

export default function LoadingSpinner({ fullScreen = false, size = 40 }: LoadingSpinnerProps) {
  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        ...(fullScreen && {
          position: 'fixed',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          bgcolor: 'rgba(255,255,255,0.8)',
          zIndex: 9999,
        }),
      }}
    >
      <CircularProgress size={size} />
    </Box>
  )
}
