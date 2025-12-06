/**
 * React Query hooks for ticket operations
 * Provides data fetching, caching, and mutation hooks for ticket management
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import type { UseQueryOptions } from '@tanstack/react-query';
// import {
//   getTickets,
//   getTicketById,
//   getTicketHistory,
//   createTicket,
//   updateTicket,
//   changeTicketStatus,
//   assignTicket,
//   bulkAssignTickets,
//   bulkChangeStatus
// } from '../services/ticketService';
import type {
  TicketDto,
  CreateTicketRequest,
  UpdateTicketRequest,
  ChangeTicketStatusRequest,
  AssignTicketRequest,
  TicketFilters,
  PaginatedResponse,
  TicketHistoryEvent,
} from '../types/api';

// ============================================================================
// Query Keys
// ============================================================================

export const ticketKeys = {
  all: ['tickets'] as const,
  lists: () => [...ticketKeys.all, 'list'] as const,
  list: (filters: TicketFilters) => [...ticketKeys.lists(), filters] as const,
  details: () => [...ticketKeys.all, 'detail'] as const,
  detail: (id: string) => [...ticketKeys.details(), id] as const,
  history: (id: string) => [...ticketKeys.detail(id), 'history'] as const,
};

// ============================================================================
// Query Hooks
// ============================================================================

/**
 * Fetch paginated tickets with filters
 */
export const useTickets = (
  filters: TicketFilters = {},
  options?: Omit<UseQueryOptions<PaginatedResponse<TicketDto>, Error>, 'queryKey' | 'queryFn'>
) => {
  return useQuery({
    queryKey: ticketKeys.list(filters),
    queryFn: async () => {
      // HARD MOCK
      await new Promise(r => setTimeout(r, 100));
      return { items: [], totalCount: 0, pageNumber: 1, pageSize: 20 } as PaginatedResponse<TicketDto>;
    },
    staleTime: 2 * 60 * 1000, // 2 minutes
    ...options,
  });
};

/**
 * Fetch single ticket by ID
 */
export const useTicket = (
  id: string,
  options?: Omit<UseQueryOptions<TicketDto, Error>, 'queryKey' | 'queryFn'>
) => {
  return useQuery({
    queryKey: ticketKeys.detail(id),
    queryFn: async () => ({ id } as TicketDto),
    enabled: !!id,
    staleTime: 1 * 60 * 1000, // 1 minute
    ...options,
  });
};

/**
 * Fetch ticket history/timeline events
 */
export const useTicketHistory = (
  ticketId: string,
  options?: Omit<UseQueryOptions<TicketHistoryEvent[], Error>, 'queryKey' | 'queryFn'>
) => {
  return useQuery({
    queryKey: ticketKeys.history(ticketId),
    queryFn: async () => [],
    enabled: !!ticketId,
    staleTime: 30 * 1000, // 30 seconds
    ...options,
  });
};

// ============================================================================
// Mutation Hooks
// ============================================================================

/**
 * Create new ticket (client portal)
 */
export const useCreateTicket = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: CreateTicketRequest) => ({ id: 'mock', ...data } as TicketDto),
    onSuccess: () => {
      // Invalidate ticket lists to refetch with new ticket
      queryClient.invalidateQueries({ queryKey: ticketKeys.lists() });
    },
  });
};

/**
 * Update ticket details (title, description, priority)
 */
export const useUpdateTicket = () => {
  // Mock
  const queryClient = useQueryClient();
  return useMutation({ mutationFn: async () => ({}) as any });
};

/**
 * Change ticket status
 */
export const useChangeTicketStatus = () => {
  // Mock
  const queryClient = useQueryClient();
  return useMutation({ mutationFn: async () => { } });
};

/**
 * Assign ticket to service user
 */
export const useAssignTicket = () => {
  // Mock
  const queryClient = useQueryClient();
  return useMutation({ mutationFn: async () => { } });
};

/**
 * Bulk assign tickets
 */
export const useBulkAssignTickets = () => {
  // Mock
  const queryClient = useQueryClient();
  return useMutation({ mutationFn: async () => { } });
};

/**
 * Bulk change ticket status
 */
export const useBulkChangeStatus = () => {
  // Mock
  const queryClient = useQueryClient();
  return useMutation({ mutationFn: async () => { } });
};
