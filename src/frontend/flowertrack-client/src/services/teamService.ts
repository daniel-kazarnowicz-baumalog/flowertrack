/**
 * Team Service
 * Handles API calls for organization team member management
 */

import { apiClient } from '../lib/apiClient';
import type {
  OrganizationUser,
  InviteTeamMemberRequest,
  UpdateMemberRoleRequest,
  PaginatedResponse,
} from '../types/api';

export interface TeamMemberParams {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
}

const teamService = {
  /**
   * Get all team members for an organization
   */
  getTeamMembers: async (
    orgId: string,
    params?: TeamMemberParams
  ): Promise<PaginatedResponse<OrganizationUser>> => {
    const { data } = await apiClient.get(`/organizations/${orgId}/users`, {
      params,
    });
    return data;
  },

  /**
   * Invite a new team member to the organization
   */
  inviteTeamMember: async (
    orgId: string,
    request: InviteTeamMemberRequest
  ): Promise<OrganizationUser> => {
    const { data } = await apiClient.post(`/organizations/${orgId}/users/invite`, request);
    return data;
  },

  /**
   * Update a team member's role
   */
  updateMemberRole: async (
    orgId: string,
    userId: string,
    request: UpdateMemberRoleRequest
  ): Promise<OrganizationUser> => {
    const { data } = await apiClient.put(`/organizations/${orgId}/users/${userId}/role`, request);
    return data;
  },

  /**
   * Remove a team member from the organization
   */
  removeMember: async (orgId: string, userId: string): Promise<void> => {
    await apiClient.delete(`/organizations/${orgId}/users/${userId}`);
  },
};

export default teamService;
