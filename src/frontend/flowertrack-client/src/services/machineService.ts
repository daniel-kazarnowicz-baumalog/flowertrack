import { apiClient } from '../lib/apiClient';
import type {
  MachineDto,
  CreateMachineRequest,
  UpdateMachineRequest,
  ChangeMachineStatusRequest,
  PaginatedResponse,
} from '../types/api';
// @obsolete DEV ONLY - Mock data import
import { isMockSession, getMockMachines } from './mockData';

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
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 300));
      const mockData = getMockMachines();
      let items = [...mockData.items];
      
      // Apply filters
      if (params?.search) {
        const search = params.search.toLowerCase();
        items = items.filter(m => 
          m.name.toLowerCase().includes(search) ||
          m.serialNumber.toLowerCase().includes(search)
        );
      }
      if (params?.status) {
        items = items.filter(m => m.status === params.status);
      }
      if (params?.organizationId) {
        items = items.filter(m => m.organizationId === params.organizationId);
      }
      
      return {
        items: items as unknown as MachineDto[],
        totalCount: items.length,
        page: params?.page || 1,
        pageSize: params?.pageSize || 10,
        totalPages: Math.ceil(items.length / (params?.pageSize || 10)),
      };
    }

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
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 300));
      const mockData = getMockMachines();
      const items = mockData.items.filter(m => m.organizationId === organizationId);
      return {
        items: items as unknown as MachineDto[],
        totalCount: items.length,
        page: params?.page || 1,
        pageSize: params?.pageSize || 10,
        totalPages: Math.ceil(items.length / (params?.pageSize || 10)),
      };
    }

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
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 200));
      const mockData = getMockMachines();
      const machine = mockData.items.find(m => m.id === id) || mockData.items[0];
      return machine as unknown as MachineDto;
    }

    const response = await apiClient.get<MachineDto>(`/machines/${id}`);
    return response.data;
  },

  /**
   * Register a new machine
   */
  createMachine: async (data: CreateMachineRequest): Promise<MachineDto> => {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 500));
      return {
        id: 'mock-machine-' + Date.now(),
        ...data,
        status: 'Active',
        createdAt: new Date().toISOString(),
      } as unknown as MachineDto;
    }

    const response = await apiClient.post<MachineDto>('/machines', data);
    return response.data;
  },

  /**
   * Update machine details
   */
  updateMachine: async (id: string, data: UpdateMachineRequest): Promise<MachineDto> => {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 300));
      return { id, ...data } as unknown as MachineDto;
    }

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
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 300));
      return { id, status: data.status } as unknown as MachineDto;
    }

    const response = await apiClient.patch<MachineDto>(`/machines/${id}/status`, data);
    return response.data;
  },

  /**
   * Delete machine (soft delete - service admin only)
   */
  deleteMachine: async (id: string): Promise<void> => {
    // @obsolete DEV ONLY - Mock data
    if (isMockSession()) {
      await new Promise(resolve => setTimeout(resolve, 300));
      console.log('Mock delete machine:', id);
      return;
    }

    await apiClient.delete(`/machines/${id}`);
  },
};
