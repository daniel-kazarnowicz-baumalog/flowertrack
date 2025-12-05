/**
 * React Query hooks for team member operations
 * Provides data fetching, caching, and mutation hooks for team management
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import type { UseQueryOptions } from '@tanstack/react-query';
import teamService from '../services/teamService';
import type { OrganizationUser, InviteTeamMemberRequest, PaginatedResponse } from '../types/api';

// ============================================================================
// Query Keys
// ============================================================================

export const teamKeys = {
  all: ['team-members'] as const,
  lists: () => [...teamKeys.all, 'list'] as const,
  list: (orgId: string, params?: { page?: number; pageSize?: number; searchTerm?: string }) =>
    [...teamKeys.lists(), orgId, params] as const,
};

// ============================================================================
// Query Hooks
// ============================================================================

/**
 * Fetch organization team members with pagination and search
 */
export const useTeamMembers = (
  orgId: string,
  params?: {
    page?: number;
    pageSize?: number;
    searchTerm?: string;
  },
  options?: Omit<
    UseQueryOptions<PaginatedResponse<OrganizationUser>, Error>,
    'queryKey' | 'queryFn'
  >
) => {
  return useQuery({
    queryKey: teamKeys.list(orgId, params),
    queryFn: () => teamService.getTeamMembers(orgId, params),
    enabled: !!orgId,
    ...options,
  });
};

// ============================================================================
// Mutation Hooks
// ============================================================================

/**
 * Invite a new team member
 */
export const useInviteTeamMember = (orgId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: InviteTeamMemberRequest) => teamService.inviteTeamMember(orgId, request),
    onSuccess: () => {
      // Invalidate all team member queries for this organization
      queryClient.invalidateQueries({ queryKey: teamKeys.lists() });
    },
  });
};

/**
 * Update a team member's role
 */
export const useUpdateMemberRole = (orgId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ userId, role }: { userId: string; role: 'Admin' | 'User' }) =>
      teamService.updateMemberRole(orgId, userId, { role }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: teamKeys.lists() });
    },
  });
};

/**
 * Remove a team member from organization
 */
export const useRemoveMember = (orgId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (userId: string) => teamService.removeMember(orgId, userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: teamKeys.lists() });
    },
  });
};
