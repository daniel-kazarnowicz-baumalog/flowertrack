/**
 * Ticket Service - API operations for ticket management
 * Handles CRUD operations, status changes, assignments, comments, and attachments
 */

import { apiClient } from '../lib/apiClient';
import type {
  TicketDto,
  CreateTicketRequest,
  UpdateTicketRequest,
  ChangeTicketStatusRequest,
  AssignTicketRequest,
  TicketFilters,
  PaginatedResponse,
  TicketHistoryEvent,
  CommentDto,
  CreateCommentRequest,
  UpdateCommentRequest,
  CreateNoteRequest,
  AttachmentDto,
} from '../types/api';

/**
 * Fetch paginated tickets with optional filters
 */
export const getTickets = async (
  filters: TicketFilters = {}
): Promise<PaginatedResponse<TicketDto>> => {
  const params = new URLSearchParams();

  // Add filter parameters
  if (filters.status && filters.status.length > 0) {
    filters.status.forEach((status) => params.append('status', status));
  }
  if (filters.priority && filters.priority.length > 0) {
    filters.priority.forEach((priority) => params.append('priority', priority));
  }
  if (filters.organizationId) {
    params.append('organizationId', filters.organizationId);
  }
  if (filters.machineId) {
    params.append('machineId', filters.machineId);
  }
  if (filters.assignedToId) {
    params.append('assignedToId', filters.assignedToId);
  }
  if (filters.search) {
    params.append('search', filters.search);
  }
  if (filters.page) {
    params.append('page', filters.page.toString());
  }
  if (filters.pageSize) {
    params.append('pageSize', filters.pageSize.toString());
  }
  if (filters.sortBy) {
    params.append('sortBy', filters.sortBy);
  }
  if (filters.sortDirection) {
    params.append('sortDirection', filters.sortDirection);
  }

  const response = await apiClient.get<PaginatedResponse<TicketDto>>(
    `/tickets?${params.toString()}`
  );
  return {
    ...response.data,
    items: response.data.items.map(mapTicketEnums),
  };
};

/**
 * Fetch single ticket by ID
 */
export const getTicketById = async (id: string): Promise<TicketDto> => {
  const response = await apiClient.get<TicketDto>(`/tickets/${id}`);
  return mapTicketEnums(response.data);
};

/**
 * Create new ticket (client portal)
 */
export const createTicket = async (data: CreateTicketRequest): Promise<TicketDto> => {
  // Map priority string to int for backend
  const priorityToIntMap: Record<string, number> = {
    Low: 0,
    Medium: 1,
    High: 2,
    Critical: 3,
  };

  const requestData = {
    ...data,
    priority: priorityToIntMap[data.priority] ?? 1, // Default to Medium if unknown
  };

  const response = await apiClient.post<TicketDto>('/tickets', requestData);
  return mapTicketEnums(response.data);
};

/**
 * Update ticket details (title, description, priority)
 */
export const updateTicket = async (id: string, data: UpdateTicketRequest): Promise<TicketDto> => {
  const response = await apiClient.patch<TicketDto>(`/tickets/${id}`, data);
  return response.data;
};

/**
 * Map frontend status string to backend integer
 */
const statusToInt: Record<string, number> = {
  New: 0,
  Accepted: 1,
  InProgress: 2,
  Resolved: 3,
  Closed: 4,
  Reopened: 5,
};

/**
 * Change ticket status with optional justification
 */
export const changeTicketStatus = async (
  id: string,
  data: ChangeTicketStatusRequest
): Promise<void> => {
  // Convert frontend string status to backend integer
  const backendData = {
    status: statusToInt[data.newStatus] ?? 0,
    reason: data.justification,
  };
  await apiClient.patch(`/tickets/${id}/status`, backendData);
};

/**
 * Assign ticket to service user (or unassign if serviceUserId is null)
 */
export const assignTicket = async (id: string, data: AssignTicketRequest): Promise<void> => {
  await apiClient.patch(`/tickets/${id}/assign`, data);
};

/**
 * Fetch ticket history/timeline events
 */
export const getTicketHistory = async (ticketId: string): Promise<TicketHistoryEvent[]> => {
  const response = await apiClient.get<TicketHistoryEvent[]>(`/tickets/${ticketId}/history`);
  return response.data;
};

// ============================================================================
// Comment Operations
// ============================================================================

/**
 * Fetch all comments for a ticket (includes public and internal notes based on user role)
 */
export const getTicketComments = async (ticketId: string): Promise<CommentDto[]> => {
  const response = await apiClient.get<CommentDto[]>(`/tickets/${ticketId}/comments`);
  return response.data;
};

/**
 * Add public comment to ticket (visible to client)
 */
export const addComment = async (
  ticketId: string,
  data: CreateCommentRequest
): Promise<CommentDto> => {
  const response = await apiClient.post<CommentDto>(`/tickets/${ticketId}/comments`, data);
  return response.data;
};

/**
 * Add internal note to ticket (only visible to service team)
 */
export const addNote = async (ticketId: string, data: CreateNoteRequest): Promise<CommentDto> => {
  const response = await apiClient.post<CommentDto>(`/tickets/${ticketId}/notes`, data);
  return response.data;
};

/**
 * Update existing comment (only within time limit)
 */
export const updateComment = async (
  ticketId: string,
  commentId: string,
  data: UpdateCommentRequest
): Promise<CommentDto> => {
  const response = await apiClient.patch<CommentDto>(
    `/tickets/${ticketId}/comments/${commentId}`,
    data
  );
  return response.data;
};

/**
 * Delete comment (only within time limit)
 */
export const deleteComment = async (ticketId: string, commentId: string): Promise<void> => {
  await apiClient.delete(`/tickets/${ticketId}/comments/${commentId}`);
};

// ============================================================================
// Attachment Operations
// ============================================================================

/**
 * Fetch all attachments for a ticket
 */
export const getTicketAttachments = async (ticketId: string): Promise<AttachmentDto[]> => {
  const response = await apiClient.get<AttachmentDto[]>(`/tickets/${ticketId}/attachments`);
  return response.data;
};

/**
 * Upload attachment to ticket
 */
export const uploadAttachment = async (ticketId: string, file: File): Promise<AttachmentDto> => {
  const formData = new FormData();
  formData.append('file', file);

  const response = await apiClient.post<AttachmentDto>(
    `/tickets/${ticketId}/attachments`,
    formData,
    {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    }
  );
  return response.data;
};

/**
 * Download attachment file
 */
export const downloadAttachment = async (ticketId: string, attachmentId: string): Promise<Blob> => {
  const response = await apiClient.get(`/tickets/${ticketId}/attachments/${attachmentId}`, {
    responseType: 'blob',
  });
  return response.data;
};

/**
 * Delete attachment
 */
export const deleteAttachment = async (ticketId: string, attachmentId: string): Promise<void> => {
  await apiClient.delete(`/tickets/${ticketId}/attachments/${attachmentId}`);
};

// ============================================================================
// Bulk Operations (for service portal)
// ============================================================================

/**
 * Bulk assign tickets to service user
 */
export const bulkAssignTickets = async (
  ticketIds: string[],
  serviceUserId: string | null
): Promise<void> => {
  await apiClient.post('/tickets/bulk/assign', {
    ticketIds,
    serviceUserId,
  });
};

/**
 * Bulk change ticket status
 */
export const bulkChangeStatus = async (
  ticketIds: string[],
  newStatus: string,
  justification?: string
): Promise<void> => {
  await apiClient.post('/tickets/bulk/status', {
    ticketIds,
    newStatus,
    justification,
  });
};

/**
 * Map backend integer enums to frontend string unions
 */
function mapTicketEnums(ticket: any): TicketDto {
  const statusMap: Record<number, string> = {
    0: 'New',
    1: 'Accepted',
    2: 'InProgress',
    3: 'Resolved',
    4: 'Closed',
    5: 'Reopened',
  };
  const priorityMap: Record<number, string> = {
    0: 'Low',
    1: 'Medium',
    2: 'High',
    3: 'Critical',
  };

  return {
    ...ticket,
    status: typeof ticket.status === 'number' ? statusMap[ticket.status] || 'New' : ticket.status,
    priority:
      typeof ticket.priority === 'number'
        ? priorityMap[ticket.priority] || 'Medium'
        : ticket.priority,
  };
}

export default {
  getTickets,
  getTicketById,
  createTicket,
  updateTicket,
  changeTicketStatus,
  assignTicket,
  getTicketHistory,
  getTicketComments,
  addComment,
  addNote,
  updateComment,
  deleteComment,
  getTicketAttachments,
  uploadAttachment,
  downloadAttachment,
  deleteAttachment,
  bulkAssignTickets,
  bulkChangeStatus,
};
