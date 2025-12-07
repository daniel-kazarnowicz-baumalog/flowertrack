import { apiClient } from '../lib/apiClient';
import type {
  OrganizationDto,
  OnboardOrganizationRequest,
  UpdateOrganizationRequest,
  MachineDto,
  TicketDto,
  OrganizationUserDto,
  PaginatedResponse,
} from '../types/api';
// @obsolete DEV ONLY - Mock data import
import { isMockSession, getMockOrganizations } from './mockData';

/**
 * Organization Service
 * Handles all organization-related API calls
 */

export const organizationService = {
  /**
   * Get all organizations
   */
  async getAll(): Promise<OrganizationDto[]> {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise((resolve) => setTimeout(resolve, 300));
      const mockData = getMockOrganizations();
      return mockData.items as unknown as OrganizationDto[];
    }
    const response = await apiClient.get<PaginatedResponse<OrganizationDto> | OrganizationDto[]>(
      '/organizations'
    );

    if ('items' in response.data && Array.isArray(response.data.items)) {
      return response.data.items;
    }

    return response.data as OrganizationDto[];
  },

  /**
   * Get organization by ID
   */
  async getById(id: string): Promise<OrganizationDto> {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise((resolve) => setTimeout(resolve, 300));
      const mockData = getMockOrganizations();
      const org = mockData.items.find((o) => o.id === id) || mockData.items[0];
      return org as unknown as OrganizationDto;
    }
    const response = await apiClient.get<OrganizationDto>(`/organizations/${id}`);
    return response.data;
  },

  /**
   * Onboard new organization (creates org + admin user, sends invitation)
   */
  async onboard(data: OnboardOrganizationRequest): Promise<OrganizationDto> {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise((resolve) => setTimeout(resolve, 500));
      return {
        id: 'mock-org-' + Date.now(),
        name: data.organizationName,
        contactEmail: data.adminEmail,
        ...data,
      } as unknown as OrganizationDto;
    }
    const response = await apiClient.post<OrganizationDto>('/organizations/onboard', data);
    return response.data;
  },

  /**
   * Update organization details
   */
  async update(id: string, data: UpdateOrganizationRequest): Promise<OrganizationDto> {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise((resolve) => setTimeout(resolve, 300));
      return { id, ...data } as unknown as OrganizationDto;
    }
    const response = await apiClient.patch<OrganizationDto>(`/organizations/${id}`, data);
    return response.data;
  },

  /**
   * Regenerate organization API key
   */
  async regenerateApiKey(id: string): Promise<{ apiKey: string }> {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise((resolve) => setTimeout(resolve, 300));
      return { apiKey: 'mock-api-key-' + id + '-' + Date.now() };
    }
    const response = await apiClient.post<{ apiKey: string }>(
      `/organizations/${id}/regenerate-api-key`
    );
    return response.data;
  },

  /**
   * Get all machines for an organization
   */
  async getMachines(id: string): Promise<MachineDto[]> {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise((resolve) => setTimeout(resolve, 300));
      const { getMockMachines } = await import('./mockData');
      const mockData = getMockMachines();
      return mockData.items.filter((m) => m.organizationId === id) as unknown as MachineDto[];
    }
    const response = await apiClient.get<MachineDto[]>(`/organizations/${id}/machines`);
    return response.data;
  },

  /**
   * Get all tickets for an organization
   */
  async getTickets(id: string): Promise<TicketDto[]> {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise((resolve) => setTimeout(resolve, 300));
      const { getMockTickets } = await import('./mockData');
      const mockData = getMockTickets();
      return mockData.items.filter((t) => t.organizationId === id) as TicketDto[];
    }
    const response = await apiClient.get<TicketDto[]>(`/organizations/${id}/tickets`);
    return response.data;
  },

  /**
   * Get all users for an organization
   */
  async getUsers(id: string): Promise<OrganizationUserDto[]> {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise((resolve) => setTimeout(resolve, 300));
      return [
        {
          id: 'user-1',
          email: 'admin@org.pl',
          firstName: 'Jan',
          lastName: 'Kowalski',
          fullName: 'Jan Kowalski',
          organizationId: id,
          organizationName: 'Org Name',
          isAdmin: true,
          status: 'Active',
        },
      ] as OrganizationUserDto[];
    }
    const response = await apiClient.get<OrganizationUserDto[]>(`/organizations/${id}/users`);
    return response.data;
  },
};
