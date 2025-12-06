import { apiClient } from '../lib/apiClient';
import type { PaginatedResponse } from '../types/api';
// @obsolete DEV ONLY - Mock data import
import { isMockSession } from './mockData';

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

// @obsolete DEV ONLY - Mock service users
function getMockServiceUsers(): ServiceUser[] {
  return [
    {
      id: 'user-admin-1',
      email: 'admin@flowertrack.pl',
      firstName: 'Anna',
      lastName: 'Kowalska',
      fullName: 'Anna Kowalska',
      role: 'Admin',
      status: 'Active',
      lastActiveAt: new Date().toISOString(),
      joinedAt: '2023-01-15T10:00:00Z',
    },
    {
      id: 'user-tech-1',
      email: 'jan.serwisant@flowertrack.pl',
      firstName: 'Jan',
      lastName: 'Serwisant',
      fullName: 'Jan Serwisant',
      role: 'Technician',
      status: 'Active',
      lastActiveAt: new Date(Date.now() - 2 * 3600000).toISOString(),
      joinedAt: '2023-03-20T08:30:00Z',
    },
    {
      id: 'user-tech-2',
      email: 'anna.technik@flowertrack.pl',
      firstName: 'Anna',
      lastName: 'Technik',
      fullName: 'Anna Technik',
      role: 'Technician',
      status: 'Active',
      lastActiveAt: new Date(Date.now() - 24 * 3600000).toISOString(),
      joinedAt: '2023-06-01T14:00:00Z',
    },
    {
      id: 'user-tech-3',
      email: 'piotr.wisniewski@flowertrack.pl',
      firstName: 'Piotr',
      lastName: 'Wiśniewski',
      fullName: 'Piotr Wiśniewski',
      role: 'Technician',
      status: 'Inactive',
      lastActiveAt: '2024-09-15T10:00:00Z',
      joinedAt: '2022-11-01T09:00:00Z',
    },
  ];
}

export const serviceUserService = {
  /**
   * Get all service users with optional filters
   */
  async getServiceUsers(params?: ServiceUsersFilters): Promise<PaginatedResponse<ServiceUser>> {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 300));
      let users = getMockServiceUsers();
      
      // Apply filters
      if (params?.searchTerm) {
        const search = params.searchTerm.toLowerCase();
        users = users.filter(u => 
          u.fullName.toLowerCase().includes(search) ||
          u.email.toLowerCase().includes(search)
        );
      }
      if (params?.role && params.role !== 'All') {
        users = users.filter(u => u.role === params.role);
      }
      if (params?.status && params.status !== 'All') {
        users = users.filter(u => u.status === params.status);
      }
      
      return {
        items: users,
        totalCount: users.length,
        page: params?.page || 1,
        pageSize: params?.pageSize || 10,
        totalPages: Math.ceil(users.length / (params?.pageSize || 10)),
      };
    }

    const { data } = await apiClient.get<PaginatedResponse<ServiceUser>>('/service-users', {
      params,
    });
    return data;
  },

  /**
   * Invite a new service user
   */
  async inviteServiceUser(request: InviteServiceUserRequest): Promise<ServiceUser> {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 500));
      return {
        id: 'mock-user-' + Date.now(),
        email: request.email,
        firstName: request.firstName,
        lastName: request.lastName,
        fullName: `${request.firstName} ${request.lastName}`,
        role: request.role,
        status: 'Active',
        lastActiveAt: null,
        joinedAt: new Date().toISOString(),
      };
    }

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
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 300));
      const users = getMockServiceUsers();
      const user = users.find(u => u.id === userId) || users[0];
      return { ...user, role: request.role };
    }

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
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 300));
      const users = getMockServiceUsers();
      const user = users.find(u => u.id === userId) || users[0];
      return { ...user, status: request.status };
    }

    const { data } = await apiClient.put<ServiceUser>(
      `/service-users/${userId}/status`,
      request
    );
    return data;
  },
  /**
   * Reset a service user's password (admin only)
   */
  async resetUserPassword(userId: string): Promise<{ temporaryPassword: string }> {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 500));
      return { temporaryPassword: 'TempPass123!' + userId.slice(-4) };
    }

    const { data } = await apiClient.post<{ temporaryPassword: string }>(
      `/service-users/${userId}/reset-password`
    );
    return data;
  },
};
