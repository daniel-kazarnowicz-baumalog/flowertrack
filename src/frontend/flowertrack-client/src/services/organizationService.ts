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

/**
 * Organization Service
 * Handles all organization-related API calls
 */

export const organizationService = {
  /**
   * Get all organizations
   */
  async getAll(): Promise<OrganizationDto[]> {
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
    const response = await apiClient.get<OrganizationDto>(`/organizations/${id}`);
    return response.data;
  },

  /**
   * Onboard new organization (creates org + admin user, sends invitation)
   */
  async onboard(data: OnboardOrganizationRequest): Promise<OrganizationDto> {
    const response = await apiClient.post<OrganizationDto>('/organizations/onboard', data);
    return response.data;
  },

  /**
   * Update organization details
   */
  async update(id: string, data: UpdateOrganizationRequest): Promise<OrganizationDto> {
    const response = await apiClient.patch<OrganizationDto>(`/organizations/${id}`, data);
    return response.data;
  },

  /**
   * Regenerate organization API key
   */
  async regenerateApiKey(id: string): Promise<{ apiKey: string }> {
    const response = await apiClient.post<{ apiKey: string }>(
      `/organizations/${id}/regenerate-api-key`
    );
    return response.data;
  },

  /**
   * Get all machines for an organization
   */
  async getMachines(id: string): Promise<MachineDto[]> {
    const response = await apiClient.get<MachineDto[]>(`/organizations/${id}/machines`);
    return response.data;
  },

  /**
   * Get all tickets for an organization
   */
  async getTickets(id: string): Promise<TicketDto[]> {
    const response = await apiClient.get<TicketDto[]>(`/organizations/${id}/tickets`);
    return response.data;
  },

  /**
   * Get all users for an organization
   */
  async getUsers(id: string): Promise<OrganizationUserDto[]> {
    const response = await apiClient.get<OrganizationUserDto[]>(`/organizations/${id}/users`);
    return response.data;
  },

  /**
   * Delete organization (soft delete)
   */
  async delete(id: string): Promise<void> {
    await apiClient.delete(`/organizations/${id}`);
  },
};
