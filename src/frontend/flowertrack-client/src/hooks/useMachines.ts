import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { machineService } from '../services/machineService';
import { useToast } from './useToast';
import type {
  CreateMachineRequest,
  UpdateMachineRequest,
  ChangeMachineStatusRequest,
} from '../types/api';

/**
 * Custom hook for machines data and operations
 */

/**
 * Get paginated machines list (service portal)
 */
export const useMachines = (params?: {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: string;
  organizationId?: string;
  sortBy?: string;
  sortDesc?: boolean;
}) => {
  return useQuery({
    queryKey: ['machines', params],
    queryFn: () => machineService.getMachines(params),
    staleTime: 30000, // 30 seconds
  });
};

/**
 * Get machines by organization ID
 */
export const useMachinesByOrganization = (
  organizationId: string,
  params?: {
    page?: number;
    pageSize?: number;
    status?: string;
  }
) => {
  return useQuery({
    queryKey: ['machines', 'organization', organizationId, params],
    queryFn: () => machineService.getMachinesByOrganization(organizationId, params),
    staleTime: 30000,
    enabled: !!organizationId,
  });
};

/**
 * Get single machine details
 */
export const useMachine = (id: string | undefined) => {
  return useQuery({
    queryKey: ['machines', id],
    queryFn: () => machineService.getMachine(id!),
    staleTime: 60000, // 1 minute
    enabled: !!id,
  });
};

/**
 * Machine mutations hook
 */
export const useMachineMutations = () => {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  const createMachine = useMutation({
    mutationFn: (data: CreateMachineRequest) => machineService.createMachine(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['machines'] });
      showToast('Machine registered successfully', 'success');
    },
    onError: (error: Error) => {
      showToast(error.message || 'Failed to register machine', 'error');
    },
  });

  const updateMachine = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateMachineRequest }) =>
      machineService.updateMachine(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['machines'] });
      queryClient.invalidateQueries({ queryKey: ['machines', variables.id] });
      showToast('Machine updated successfully', 'success');
    },
    onError: (error: Error) => {
      showToast(error.message || 'Failed to update machine', 'error');
    },
  });

  const changeMachineStatus = useMutation({
    mutationFn: ({ id, data }: { id: string; data: ChangeMachineStatusRequest }) =>
      machineService.changeMachineStatus(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['machines'] });
      queryClient.invalidateQueries({ queryKey: ['machines', variables.id] });
      showToast('Machine status updated successfully', 'success');
    },
    onError: (error: Error) => {
      showToast(error.message || 'Failed to update machine status', 'error');
    },
  });

  const deleteMachine = useMutation({
    mutationFn: (id: string) => machineService.deleteMachine(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['machines'] });
      showToast('Machine deleted successfully', 'success');
    },
    onError: (error: Error) => {
      showToast(error.message || 'Failed to delete machine', 'error');
    },
  });

  return {
    createMachine,
    updateMachine,
    changeMachineStatus,
    deleteMachine,
    isCreating: createMachine.isPending,
    isUpdating: updateMachine.isPending,
    isChangingStatus: changeMachineStatus.isPending,
    isDeleting: deleteMachine.isPending,
  };
};
