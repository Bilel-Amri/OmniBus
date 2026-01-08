import { Skeleton, Box, Card, CardContent, Grid } from '@mui/material';

interface SkeletonLoadingProps {
  variant?: 'list' | 'card' | 'table' | 'detail';
  count?: number;
}

export default function SkeletonLoading({ variant = 'card', count = 3 }: SkeletonLoadingProps) {
  if (variant === 'list') {
    return (
      <Box>
        {Array.from({ length: count }).map((_, index) => (
          <Box key={index} sx={{ mb: 2 }}>
            <Skeleton variant="rectangular" height={80} sx={{ borderRadius: 1 }} />
          </Box>
        ))}
      </Box>
    );
  }

  if (variant === 'table') {
    return (
      <Box>
        <Skeleton variant="rectangular" height={50} sx={{ mb: 2 }} />
        {Array.from({ length: count }).map((_, index) => (
          <Skeleton key={index} variant="rectangular" height={40} sx={{ mb: 1 }} />
        ))}
      </Box>
    );
  }

  if (variant === 'detail') {
    return (
      <Box>
        <Skeleton variant="text" height={60} width="60%" sx={{ mb: 2 }} />
        <Grid container spacing={2}>
          <Grid item xs={12} md={6}>
            <Skeleton variant="rectangular" height={200} sx={{ borderRadius: 2 }} />
          </Grid>
          <Grid item xs={12} md={6}>
            <Skeleton variant="text" height={40} />
            <Skeleton variant="text" height={40} />
            <Skeleton variant="text" height={40} />
            <Skeleton variant="text" height={40} width="80%" />
          </Grid>
        </Grid>
      </Box>
    );
  }

  // Default: card variant
  return (
    <Grid container spacing={3}>
      {Array.from({ length: count }).map((_, index) => (
        <Grid item xs={12} sm={6} md={4} key={index}>
          <Card>
            <Skeleton variant="rectangular" height={140} />
            <CardContent>
              <Skeleton variant="text" height={30} />
              <Skeleton variant="text" height={20} width="80%" />
              <Skeleton variant="text" height={20} width="60%" />
            </CardContent>
          </Card>
        </Grid>
      ))}
    </Grid>
  );
}
