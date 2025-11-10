import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { organizationService } from '../services/organizationService';
import { useToast } from './useToast';
import type { OnboardOrganizationRequest, UpdateOrganizationRequest } from '../types/api';

/**
 * React Query hooks for Organizations
 */

/**
 * Hook to get all organizations
 */
export const useOrganizations = () => {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  // List all organizations
  const {
    data: organizations,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['organizations'],
    queryFn: organizationService.getAll,
  });

  // Create organization mutation
  const createMutation = useMutation({
    mutationFn: (data: OnboardOrganizationRequest) => organizationService.onboard(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['organizations'] });
      showToast('Organization onboarded successfully', 'success');
    },
    onError: (error: unknown) => {
      const message =
        (error as { response?: { data?: { message?: string } } }).response?.data?.message ||
        'Failed to onboard organization';
      showToast(message, 'error');
    },
  });

  // Update organization mutation
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateOrganizationRequest }) =>
      organizationService.update(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['organizations'] });
      queryClient.invalidateQueries({ queryKey: ['organizations', variables.id] });
      showToast('Organization updated successfully', 'success');
    },
    onError: (error: unknown) => {
      const message =
        (error as { response?: { data?: { message?: string } } }).response?.data?.message ||
        'Failed to update organization';
      showToast(message, 'error');
    },
  });

  // Regenerate API key mutation
  const regenerateApiKeyMutation = useMutation({
    mutationFn: (id: string) => organizationService.regenerateApiKey(id),
    onSuccess: (data, id) => {
      queryClient.invalidateQueries({ queryKey: ['organizations', id] });
      showToast('API key regenerated successfully', 'success');
      // Copy to clipboard
      navigator.clipboard.writeText(data.apiKey);
      showToast('New API key copied to clipboard', 'info');
    },
    onError: (error: unknown) => {
      const message =
        (error as { response?: { data?: { message?: string } } }).response?.data?.message ||
        'Failed to regenerate API key';
      showToast(message, 'error');
    },
  });

  return {
    organizations: organizations || [],
    isLoading,
    error,
    refetch,
    createOrganization: createMutation.mutate,
    createOrganizationAsync: createMutation.mutateAsync,
    isCreating: createMutation.isPending,
    updateOrganization: updateMutation.mutate,
    updateOrganizationAsync: updateMutation.mutateAsync,
    isUpdating: updateMutation.isPending,
    regenerateApiKey: regenerateApiKeyMutation.mutate,
    isRegeneratingApiKey: regenerateApiKeyMutation.isPending,
  };
};

/**
 * Hook to get a single organization by ID
 */
export const useOrganization = (id: string | undefined) => {
  const {
    data: organization,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['organizations', id],
    queryFn: () => organizationService.getById(id!),
    enabled: !!id,
  });

  return {
    organization,
    isLoading,
    error,
    refetch,
  };
};

/**
 * Hook to get organization machines
 */
export const useOrganizationMachines = (organizationId: string | undefined) => {
  const {
    data: machines,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['organizations', organizationId, 'machines'],
    queryFn: () => organizationService.getMachines(organizationId!),
    enabled: !!organizationId,
  });

  return {
    machines: machines || [],
    isLoading,
    error,
    refetch,
  };
};

/**
 * Hook to get organization tickets
 */
export const useOrganizationTickets = (organizationId: string | undefined) => {
  const {
    data: tickets,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['organizations', organizationId, 'tickets'],
    queryFn: () => organizationService.getTickets(organizationId!),
    enabled: !!organizationId,
  });

  return {
    tickets: tickets || [],
    isLoading,
    error,
    refetch,
  };
};

/**
 * Hook to get organization users
 */
export const useOrganizationUsers = (organizationId: string | undefined) => {
  const {
    data: users,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['organizations', organizationId, 'users'],
    queryFn: () => organizationService.getUsers(organizationId!),
    enabled: !!organizationId,
  });

  return {
    users: users || [],
    isLoading,
    error,
    refetch,
  };
};
