import { apiClient } from '../lib/apiClient';
import type {
  Organization,
  CreateOrganizationRequest,
  UpdateOrganizationRequest,
  Machine,
  Ticket,
  OrganizationUser,
} from '../types/api';

/**
 * Organization Service
 * Handles all organization-related API calls
 */

export const organizationService = {
  /**
   * Get all organizations
   */
  async getAll(): Promise<Organization[]> {
    const response = await apiClient.get<Organization[]>('/api/organizations');
    return response.data;
  },

  /**
   * Get organization by ID
   */
  async getById(id: string): Promise<Organization> {
    const response = await apiClient.get<Organization>(`/api/organizations/${id}`);
    return response.data;
  },

  /**
   * Onboard new organization (creates org + admin user, sends invitation)
   */
  async onboard(data: CreateOrganizationRequest): Promise<Organization> {
    const response = await apiClient.post<Organization>('/api/organizations/onboard', data);
    return response.data;
  },

  /**
   * Update organization details
   */
  async update(id: string, data: UpdateOrganizationRequest): Promise<Organization> {
    const response = await apiClient.patch<Organization>(`/api/organizations/${id}`, data);
    return response.data;
  },

  /**
   * Regenerate organization API key
   */
  async regenerateApiKey(id: string): Promise<{ apiKey: string }> {
    const response = await apiClient.post<{ apiKey: string }>(
      `/api/organizations/${id}/regenerate-api-key`
    );
    return response.data;
  },

  /**
   * Get all machines for an organization
   */
  async getMachines(id: string): Promise<Machine[]> {
    const response = await apiClient.get<Machine[]>(`/api/organizations/${id}/machines`);
    return response.data;
  },

  /**
   * Get all tickets for an organization
   */
  async getTickets(id: string): Promise<Ticket[]> {
    const response = await apiClient.get<Ticket[]>(`/api/organizations/${id}/tickets`);
    return response.data;
  },

  /**
   * Get all users for an organization
   */
  async getUsers(id: string): Promise<OrganizationUser[]> {
    const response = await apiClient.get<OrganizationUser[]>(`/api/organizations/${id}/users`);
    return response.data;
  },
};
