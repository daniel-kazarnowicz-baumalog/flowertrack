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
    const response = await apiClient.get<PaginatedResponse<MachineDto> | MachineDto[]>(
      '/machines',
      {
        params: {
          page: params?.page || 1,
          pageSize: params?.pageSize || 10,
          searchTerm: params?.search || undefined,
          status: params?.status === 'All' ? undefined : params?.status, // Handle 'All' explicitly if passed
          organizationId: params?.organizationId,
          sortBy: params?.sortBy || 'serialNumber',
          sortDesc: params?.sortDesc || false,
        },
      }
    );

    if (Array.isArray(response.data)) {
      // Backend returned a flat list, perform client-side pagination and sorting
      let allItems = [...response.data];

      // Client-side sorting
      if (params?.sortBy) {
        allItems.sort((a, b) => {
          const field = params.sortBy as keyof MachineDto;
          const valA = a[field];
          const valB = b[field];

          if (valA === valB) return 0;
          if (valA === undefined || valA === null) return 1;
          if (valB === undefined || valB === null) return -1;

          if (typeof valA === 'string' && typeof valB === 'string') {
            return params.sortDesc ? valB.localeCompare(valA) : valA.localeCompare(valB);
          }

          if (valA < valB) return params.sortDesc ? 1 : -1;
          if (valA > valB) return params.sortDesc ? -1 : 1;
          return 0;
        });
      }

      const page = params?.page || 1;
      const pageSize = params?.pageSize || 10;
      const totalCount = allItems.length;
      const totalPages = Math.ceil(totalCount / pageSize);

      const startIndex = (page - 1) * pageSize;
      const endIndex = Math.min(startIndex + pageSize, totalCount);
      const paginatedItems = allItems.slice(startIndex, endIndex);

      return {
        items: paginatedItems,
        totalCount,
        page,
        pageSize,
        totalPages,
      };
    }

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
