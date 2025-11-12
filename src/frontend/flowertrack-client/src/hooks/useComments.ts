/**
 * React Query hooks for ticket comments and notes
 */

import { useQuery, useMutation, useQueryClient, UseQueryOptions } from '@tanstack/react-query';
import ticketService from '../services/ticketService';
import type {
  CommentDto,
  CreateCommentRequest,
  UpdateCommentRequest,
  CreateNoteRequest,
} from '../types/api';
import { ticketKeys } from './useTickets';

// ============================================================================
// Query Keys
// ============================================================================

export const commentKeys = {
  all: ['comments'] as const,
  forTicket: (ticketId: string) => [...commentKeys.all, ticketId] as const,
};

// ============================================================================
// Query Hooks
// ============================================================================

/**
 * Fetch all comments for a ticket
 * @example
 * const { data: comments, isLoading } = useTicketComments('ticket-id-123');
 */
export const useTicketComments = (
  ticketId: string,
  options?: Omit<UseQueryOptions<CommentDto[], Error>, 'queryKey' | 'queryFn'>
) => {
  return useQuery({
    queryKey: commentKeys.forTicket(ticketId),
    queryFn: () => ticketService.getTicketComments(ticketId),
    enabled: !!ticketId,
    staleTime: 30 * 1000, // 30 seconds
    ...options,
  });
};

// ============================================================================
// Mutation Hooks
// ============================================================================

/**
 * Add public comment to ticket
 * @example
 * const addComment = useAddComment();
 * addComment.mutate({ ticketId: 'ticket-id', data: { content: 'Comment text' } });
 */
export const useAddComment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ ticketId, data }: { ticketId: string; data: CreateCommentRequest }) =>
      ticketService.addComment(ticketId, data),
    onSuccess: (_, variables) => {
      // Invalidate comments list and ticket history
      queryClient.invalidateQueries({ queryKey: commentKeys.forTicket(variables.ticketId) });
      queryClient.invalidateQueries({ queryKey: ticketKeys.history(variables.ticketId) });
    },
  });
};

/**
 * Add internal note to ticket (service users only)
 * @example
 * const addNote = useAddNote();
 * addNote.mutate({ ticketId: 'ticket-id', data: { content: 'Internal note' } });
 */
export const useAddNote = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ ticketId, data }: { ticketId: string; data: CreateNoteRequest }) =>
      ticketService.addNote(ticketId, data),
    onSuccess: (_, variables) => {
      // Invalidate comments list and ticket history
      queryClient.invalidateQueries({ queryKey: commentKeys.forTicket(variables.ticketId) });
      queryClient.invalidateQueries({ queryKey: ticketKeys.history(variables.ticketId) });
    },
  });
};

/**
 * Update existing comment
 * @example
 * const updateComment = useUpdateComment();
 * updateComment.mutate({ ticketId: 'ticket-id', commentId: 'comment-id', data: { content: 'Updated' } });
 */
export const useUpdateComment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      ticketId,
      commentId,
      data,
    }: {
      ticketId: string;
      commentId: string;
      data: UpdateCommentRequest;
    }) => ticketService.updateComment(ticketId, commentId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: commentKeys.forTicket(variables.ticketId) });
      queryClient.invalidateQueries({ queryKey: ticketKeys.history(variables.ticketId) });
    },
  });
};

/**
 * Delete comment
 * @example
 * const deleteComment = useDeleteComment();
 * deleteComment.mutate({ ticketId: 'ticket-id', commentId: 'comment-id' });
 */
export const useDeleteComment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ ticketId, commentId }: { ticketId: string; commentId: string }) =>
      ticketService.deleteComment(ticketId, commentId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: commentKeys.forTicket(variables.ticketId) });
      queryClient.invalidateQueries({ queryKey: ticketKeys.history(variables.ticketId) });
    },
  });
};
