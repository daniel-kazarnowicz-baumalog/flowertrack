// Export all custom hooks

// Attachments
export { 
  attachmentKeys,
  useTicketAttachments, 
  useUploadAttachment, 
  useDeleteAttachment,
  downloadAttachmentFile 
} from './useAttachments';

// Comments
export { 
  commentKeys,
  useTicketComments, 
  useAddComment, 
  useAddNote,
  useUpdateComment, 
  useDeleteComment 
} from './useComments';

// Dashboard
export { 
  useServiceDashboardStats, 
  useClientDashboardStats, 
  useTicketTrends 
} from './useDashboard';

// Machines
export { 
  useMachines, 
  useMachine, 
  useMachinesByOrganization,
  useMachineMutations 
} from './useMachines';

// Organizations
export { 
  useOrganizations, 
  useOrganization,
  useOrganizationMachines,
  useOrganizationTickets,
  useOrganizationUsers
} from './useOrganizations';

// Service Users
export { 
  useServiceUsers, 
  useInviteServiceUser, 
  useUpdateServiceUserRole,
  useUpdateServiceUserStatus 
} from './useServiceUsers';

// Team
export { 
  teamKeys,
  useTeamMembers, 
  useInviteTeamMember, 
  useUpdateMemberRole, 
  useRemoveMember 
} from './useTeam';

// Tickets
export { 
  ticketKeys,
  useTickets, 
  useTicket, 
  useTicketHistory,
  useCreateTicket, 
  useUpdateTicket, 
  useChangeTicketStatus,
  useAssignTicket,
  useBulkAssignTickets,
  useBulkChangeStatus
} from './useTickets';

// Toast
export { useToast, ToastContext } from './useToast';

// Media query hooks
export { 
  useMediaQuery, 
  breakpoints,
  useIsMobile, 
  useIsTablet, 
  useIsDesktop,
  useIsLargeDesktop,
  usePrefersColorScheme,
  usePrefersReducedMotion,
  usePrefersContrast
} from './useMediaQuery';
