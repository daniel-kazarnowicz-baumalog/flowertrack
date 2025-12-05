import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { serviceUserService } from '../services/serviceUserService';
import { useToast } from './useToast';
import type { InviteServiceUserRequest, ServiceUsersFilters } from '../services/serviceUserService';

/**
 * React Query hooks for Service Users
 */

/**
 * Hook to get all service users with optional filters
 */
export const useServiceUsers = (params?: ServiceUsersFilters) => {
  return useQuery({
    queryKey: ['service-users', params],
    queryFn: () => serviceUserService.getServiceUsers(params),
  });
};

/**
 * Hook to invite a new service user
 */
export const useInviteServiceUser = () => {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: (request: InviteServiceUserRequest) =>
      serviceUserService.inviteServiceUser(request),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['service-users'] });
      showToast(`Invitation sent to ${variables.email}`, 'success');
    },
    onError: (error: unknown) => {
      const message =
        (error as { response?: { data?: { message?: string } } }).response?.data?.message ||
        'Failed to invite service user';
      showToast(message, 'error');
    },
  });
};

/**
 * Hook to update a service user's role
 */
export const useUpdateServiceUserRole = () => {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: ({ userId, role }: { userId: string; role: 'Admin' | 'Technician' }) =>
      serviceUserService.updateServiceUserRole(userId, { role }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['service-users'] });
      showToast('User role updated successfully', 'success');
    },
    onError: (error: unknown) => {
      const message =
        (error as { response?: { data?: { message?: string } } }).response?.data?.message ||
        'Failed to update user role';
      showToast(message, 'error');
    },
  });
};

/**
 * Hook to update a service user's status (activate/deactivate)
 */
export const useUpdateServiceUserStatus = () => {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: ({ userId, status }: { userId: string; status: 'Active' | 'Inactive' }) =>
      serviceUserService.updateServiceUserStatus(userId, { status }),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['service-users'] });
      const action = variables.status === 'Active' ? 'activated' : 'deactivated';
      showToast(`User ${action} successfully`, 'success');
    },
    onError: (error: unknown) => {
      const message =
        (error as { response?: { data?: { message?: string } } }).response?.data?.message ||
        'Failed to update user status';
      showToast(message, 'error');
    },
  });
};
