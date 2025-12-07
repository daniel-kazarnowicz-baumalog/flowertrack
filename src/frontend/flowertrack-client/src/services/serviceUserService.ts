import { apiClient } from '../lib/apiClient';
import type { PaginatedResponse } from '../types/api';

/**
 * Service User Service
 * Handles all service user administration API calls
 */

export interface ServiceUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  role: 'Admin' | 'Technician';
  status: 'Active' | 'Inactive';
  lastActiveAt: string | null;
  joinedAt: string;
}

export interface InviteServiceUserRequest {
  email: string;
  firstName: string;
  lastName: string;
  role: 'Admin' | 'Technician';
}

export interface UpdateServiceUserRoleRequest {
  role: 'Admin' | 'Technician';
}

export interface UpdateServiceUserStatusRequest {
  status: 'Active' | 'Inactive';
}

export interface ServiceUsersFilters {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
  role?: 'Admin' | 'Technician' | 'All';
  status?: 'Active' | 'Inactive' | 'All';
}

export const serviceUserService = {
  /**
   * Get all service users with optional filters
   */
  async getServiceUsers(params?: ServiceUsersFilters): Promise<PaginatedResponse<ServiceUser>> {
    const { data } = await apiClient.get<PaginatedResponse<ServiceUser>>('/admin/users/service', {
      params,
    });
    return data;
  },

  /**
   * Invite a new service user
   */
  async inviteServiceUser(request: InviteServiceUserRequest): Promise<ServiceUser> {
    const { data } = await apiClient.post<ServiceUser>('/service-users/invite', request);
    return data;
  },

  /**
   * Update a service user's role
   */
  async updateServiceUserRole(
    userId: string,
    request: UpdateServiceUserRoleRequest
  ): Promise<ServiceUser> {
    const { data } = await apiClient.put<ServiceUser>(`/service-users/${userId}/role`, request);
    return data;
  },

  /**
   * Update a service user's status (activate/deactivate)
   */
  async updateServiceUserStatus(
    userId: string,
    request: UpdateServiceUserStatusRequest
  ): Promise<ServiceUser> {
    const { data } = await apiClient.put<ServiceUser>(`/service-users/${userId}/status`, request);
    return data;
  },

  /**
   * Reset a service user's password (admin only)
   */
  async resetUserPassword(userId: string): Promise<{ temporaryPassword: string }> {
    const { data } = await apiClient.post<{ temporaryPassword: string }>(
      `/service-users/${userId}/reset-password`
    );
    return data;
  },
};
