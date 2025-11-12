/**
 * React Query hooks for ticket operations
 * Provides data fetching, caching, and mutation hooks for ticket management
 */

import { useQuery, useMutation, useQueryClient, UseQueryOptions } from '@tanstack/react-query';
import ticketService from '../services/ticketService';
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
 * @example
 * const { data, isLoading, error } = useTickets({
 *   status: ['New', 'InProgress'],
 *   priority: ['High', 'Critical'],
 *   page: 1,
 *   pageSize: 20
 * });
 */
export const useTickets = (
  filters: TicketFilters = {},
  options?: Omit<UseQueryOptions<PaginatedResponse<TicketDto>, Error>, 'queryKey' | 'queryFn'>
) => {
  return useQuery({
    queryKey: ticketKeys.list(filters),
    queryFn: () => ticketService.getTickets(filters),
    staleTime: 2 * 60 * 1000, // 2 minutes
    ...options,
  });
};

/**
 * Fetch single ticket by ID
 * @example
 * const { data: ticket, isLoading } = useTicket('ticket-id-123');
 */
export const useTicket = (
  id: string,
  options?: Omit<UseQueryOptions<TicketDto, Error>, 'queryKey' | 'queryFn'>
) => {
  return useQuery({
    queryKey: ticketKeys.detail(id),
    queryFn: () => ticketService.getTicketById(id),
    enabled: !!id,
    staleTime: 1 * 60 * 1000, // 1 minute
    ...options,
  });
};

/**
 * Fetch ticket history/timeline events
 * @example
 * const { data: history, isLoading } = useTicketHistory('ticket-id-123');
 */
export const useTicketHistory = (
  ticketId: string,
  options?: Omit<UseQueryOptions<TicketHistoryEvent[], Error>, 'queryKey' | 'queryFn'>
) => {
  return useQuery({
    queryKey: ticketKeys.history(ticketId),
    queryFn: () => ticketService.getTicketHistory(ticketId),
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
 * @example
 * const createTicket = useCreateTicket();
 * createTicket.mutate({ machineId: '...', title: '...', description: '...', priority: 'High' }, {
 *   onSuccess: (ticket) => navigate(`/client/tickets/${ticket.id}`)
 * });
 */
export const useCreateTicket = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateTicketRequest) => ticketService.createTicket(data),
    onSuccess: () => {
      // Invalidate ticket lists to refetch with new ticket
      queryClient.invalidateQueries({ queryKey: ticketKeys.lists() });
    },
  });
};

/**
 * Update ticket details (title, description, priority)
 * @example
 * const updateTicket = useUpdateTicket();
 * updateTicket.mutate({ id: 'ticket-id', data: { title: 'Updated title' } });
 */
export const useUpdateTicket = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateTicketRequest }) =>
      ticketService.updateTicket(id, data),
    onSuccess: (updatedTicket, variables) => {
      // Update ticket detail in cache
      queryClient.setQueryData(ticketKeys.detail(variables.id), updatedTicket);
      // Invalidate lists to refetch
      queryClient.invalidateQueries({ queryKey: ticketKeys.lists() });
    },
  });
};

/**
 * Change ticket status
 * @example
 * const changeStatus = useChangeTicketStatus();
 * changeStatus.mutate({ id: 'ticket-id', data: { newStatus: 'InProgress', justification: '...' } });
 */
export const useChangeTicketStatus = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: ChangeTicketStatusRequest }) =>
      ticketService.changeTicketStatus(id, data),
    onSuccess: (_, variables) => {
      // Invalidate ticket detail and lists to refetch updated data
      queryClient.invalidateQueries({ queryKey: ticketKeys.detail(variables.id) });
      queryClient.invalidateQueries({ queryKey: ticketKeys.lists() });
      queryClient.invalidateQueries({ queryKey: ticketKeys.history(variables.id) });
    },
  });
};

/**
 * Assign ticket to service user
 * @example
 * const assignTicket = useAssignTicket();
 * assignTicket.mutate({ id: 'ticket-id', data: { serviceUserId: 'user-id' } });
 */
export const useAssignTicket = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: AssignTicketRequest }) =>
      ticketService.assignTicket(id, data),
    onSuccess: (_, variables) => {
      // Invalidate ticket detail and lists
      queryClient.invalidateQueries({ queryKey: ticketKeys.detail(variables.id) });
      queryClient.invalidateQueries({ queryKey: ticketKeys.lists() });
      queryClient.invalidateQueries({ queryKey: ticketKeys.history(variables.id) });
    },
  });
};

/**
 * Bulk assign tickets
 * @example
 * const bulkAssign = useBulkAssignTickets();
 * bulkAssign.mutate({ ticketIds: ['id1', 'id2'], serviceUserId: 'user-id' });
 */
export const useBulkAssignTickets = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      ticketIds,
      serviceUserId,
    }: {
      ticketIds: string[];
      serviceUserId: string | null;
    }) => ticketService.bulkAssignTickets(ticketIds, serviceUserId),
    onSuccess: () => {
      // Invalidate all ticket queries after bulk operation
      queryClient.invalidateQueries({ queryKey: ticketKeys.all });
    },
  });
};

/**
 * Bulk change ticket status
 * @example
 * const bulkChangeStatus = useBulkChangeStatus();
 * bulkChangeStatus.mutate({ ticketIds: ['id1', 'id2'], newStatus: 'Closed', justification: '...' });
 */
export const useBulkChangeStatus = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      ticketIds,
      newStatus,
      justification,
    }: {
      ticketIds: string[];
      newStatus: string;
      justification?: string;
    }) => ticketService.bulkChangeStatus(ticketIds, newStatus, justification),
    onSuccess: () => {
      // Invalidate all ticket queries after bulk operation
      queryClient.invalidateQueries({ queryKey: ticketKeys.all });
    },
  });
};
