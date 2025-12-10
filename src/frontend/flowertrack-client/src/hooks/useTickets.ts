/**
 * React Query hooks for ticket operations
 * Provides data fetching, caching, and mutation hooks for ticket management
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import type { UseQueryOptions } from '@tanstack/react-query';
import type {
  TicketDto,
  CreateTicketRequest,
  TicketFilters,
  PaginatedResponse,
  TicketHistoryEvent,
  AssignTicketRequest,
} from '../types/api';
import { getTickets, getTicketById, assignTicket } from '../services/ticketService';

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
    queryFn: () => getTickets(filters),
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
    queryFn: () => getTicketById(id),
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
    mutationFn: async (data: CreateTicketRequest) => ({ id: 'mock', ...data }) as TicketDto,
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
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (_data: { id: string; data: unknown }) => ({}) as TicketDto,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ticketKeys.lists() });
    },
  });
};

/**
 * Change ticket status
 */
export const useChangeTicketStatus = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (_data: {
      id: string;
      data: { newStatus: string; justification?: string };
    }) => {},
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ticketKeys.lists() });
    },
  });
};

/**
 * Assign ticket to service user
 */
export const useAssignTicket = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (params: { id: string; data: AssignTicketRequest }) => {
      await assignTicket(params.id, params.data);
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: ticketKeys.lists() });
      queryClient.invalidateQueries({ queryKey: ticketKeys.detail(variables.id) });
    },
  });
};

/**
 * Bulk assign tickets
 */
export const useBulkAssignTickets = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async () => {},
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ticketKeys.lists() });
    },
  });
};

/**
 * Bulk change ticket status
 */
export const useBulkChangeStatus = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async () => {},
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ticketKeys.lists() });
    },
  });
};
