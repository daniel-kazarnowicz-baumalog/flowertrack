/**
 * React Query hooks for ticket attachments
 */

import { useQuery, useMutation, useQueryClient, UseQueryOptions } from '@tanstack/react-query';
import ticketService from '../services/ticketService';
import type { AttachmentDto } from '../types/api';
import { ticketKeys } from './useTickets';

// ============================================================================
// Query Keys
// ============================================================================

export const attachmentKeys = {
  all: ['attachments'] as const,
  forTicket: (ticketId: string) => [...attachmentKeys.all, ticketId] as const,
};

// ============================================================================
// Query Hooks
// ============================================================================

/**
 * Fetch all attachments for a ticket
 * @example
 * const { data: attachments, isLoading } = useTicketAttachments('ticket-id-123');
 */
export const useTicketAttachments = (
  ticketId: string,
  options?: Omit<UseQueryOptions<AttachmentDto[], Error>, 'queryKey' | 'queryFn'>
) => {
  return useQuery({
    queryKey: attachmentKeys.forTicket(ticketId),
    queryFn: () => ticketService.getTicketAttachments(ticketId),
    enabled: !!ticketId,
    staleTime: 2 * 60 * 1000, // 2 minutes (attachments don't change often)
    ...options,
  });
};

// ============================================================================
// Mutation Hooks
// ============================================================================

/**
 * Upload attachment to ticket
 * @example
 * const uploadAttachment = useUploadAttachment();
 * uploadAttachment.mutate({ ticketId: 'ticket-id', file: fileObject });
 */
export const useUploadAttachment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ ticketId, file }: { ticketId: string; file: File }) =>
      ticketService.uploadAttachment(ticketId, file),
    onSuccess: (_, variables) => {
      // Invalidate attachments list and ticket history
      queryClient.invalidateQueries({ queryKey: attachmentKeys.forTicket(variables.ticketId) });
      queryClient.invalidateQueries({ queryKey: ticketKeys.history(variables.ticketId) });
    },
  });
};

/**
 * Delete attachment
 * @example
 * const deleteAttachment = useDeleteAttachment();
 * deleteAttachment.mutate({ ticketId: 'ticket-id', attachmentId: 'attachment-id' });
 */
export const useDeleteAttachment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ ticketId, attachmentId }: { ticketId: string; attachmentId: string }) =>
      ticketService.deleteAttachment(ticketId, attachmentId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: attachmentKeys.forTicket(variables.ticketId) });
      queryClient.invalidateQueries({ queryKey: ticketKeys.history(variables.ticketId) });
    },
  });
};

/**
 * Download attachment file
 * Note: This is not a React Query hook, but a helper function
 * @example
 * const blob = await downloadAttachmentFile('ticket-id', 'attachment-id');
 * const url = URL.createObjectURL(blob);
 * window.open(url);
 */
export const downloadAttachmentFile = async (
  ticketId: string,
  attachmentId: string
): Promise<Blob> => {
  return ticketService.downloadAttachment(ticketId, attachmentId);
};
