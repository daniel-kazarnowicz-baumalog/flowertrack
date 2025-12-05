/**
 * React Query hooks for dashboard data
 */

import { useQuery } from '@tanstack/react-query';
import {
  getServiceDashboardStats,
  getClientDashboardStats,
  getTicketTrends,
  type ServiceDashboardStats,
  type ClientDashboardStats,
  type TicketTrend,
} from '../services/dashboardService';

/**
 * Hook to fetch service dashboard statistics
 */
export function useServiceDashboardStats(userId?: string) {
  return useQuery<ServiceDashboardStats>({
    queryKey: ['dashboard', 'service', userId],
    queryFn: () => getServiceDashboardStats(userId),
    staleTime: 60000, // Refresh every 60 seconds
    refetchInterval: 60000, // Auto-refresh
  });
}

/**
 * Hook to fetch client dashboard statistics
 */
export function useClientDashboardStats(organizationId: string) {
  return useQuery<ClientDashboardStats>({
    queryKey: ['dashboard', 'client', organizationId],
    queryFn: () => getClientDashboardStats(organizationId),
    staleTime: 60000,
    refetchInterval: 60000,
    enabled: !!organizationId,
  });
}

/**
 * Hook to fetch ticket trends over time
 */
export function useTicketTrends(organizationId?: string) {
  return useQuery<TicketTrend[]>({
    queryKey: ['dashboard', 'trends', organizationId],
    queryFn: () => getTicketTrends(organizationId),
    staleTime: 300000, // 5 minutes for trends
    enabled: true,
  });
}
