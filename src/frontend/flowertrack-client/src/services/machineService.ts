import { apiClient } from '../lib/apiClient';
import type {
  MachineDto,
  CreateMachineRequest,
  UpdateMachineRequest,
  ChangeMachineStatusRequest,
  PaginatedResponse,
} from '../types/api';

/**
 * Machine service for API calls related to machines
 */

export const machineService = {
  /**
   * Get paginated list of machines (service portal - all organizations)
   */
  getMachines: async (params?: {
    page?: number;
    pageSize?: number;
    search?: string;
    status?: string;
    organizationId?: string;
    sortBy?: string;
    sortDesc?: boolean;
  }): Promise<PaginatedResponse<MachineDto>> => {
    const response = await apiClient.get<PaginatedResponse<MachineDto>>('/machines', {
      params: {
        page: params?.page || 1,
        pageSize: params?.pageSize || 10,
        search: params?.search,
        status: params?.status,
        organizationId: params?.organizationId,
        sortBy: params?.sortBy || 'serialNumber',
        sortDesc: params?.sortDesc || false,
      },
    });
    return response.data;
  },

  /**
   * Get machines for a specific organization
   */
  getMachinesByOrganization: async (
    organizationId: string,
    params?: {
      page?: number;
      pageSize?: number;
      status?: string;
    }
  ): Promise<PaginatedResponse<MachineDto>> => {
    const response = await apiClient.get<PaginatedResponse<MachineDto>>(
      `/organizations/${organizationId}/machines`,
      {
        params: {
          page: params?.page || 1,
          pageSize: params?.pageSize || 10,
          status: params?.status,
        },
      }
    );
    return response.data;
  },

  /**
   * Get machine details by ID
   */
  getMachine: async (id: string): Promise<MachineDto> => {
    const response = await apiClient.get<MachineDto>(`/machines/${id}`);
    return response.data;
  },

  /**
   * Register a new machine
   */
  createMachine: async (data: CreateMachineRequest): Promise<MachineDto> => {
    const response = await apiClient.post<MachineDto>('/machines', data);
    return response.data;
  },

  /**
   * Update machine details
   */
  updateMachine: async (id: string, data: UpdateMachineRequest): Promise<MachineDto> => {
    const response = await apiClient.put<MachineDto>(`/machines/${id}`, data);
    return response.data;
  },

  /**
   * Change machine status
   */
  changeMachineStatus: async (
    id: string,
    data: ChangeMachineStatusRequest
  ): Promise<MachineDto> => {
    const response = await apiClient.patch<MachineDto>(`/machines/${id}/status`, data);
    return response.data;
  },

  /**
   * Delete machine (soft delete - service admin only)
   */
  deleteMachine: async (id: string): Promise<void> => {
    await apiClient.delete(`/machines/${id}`);
  },
};
