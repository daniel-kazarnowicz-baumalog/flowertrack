import { useState, useMemo } from 'react';
import { Navigate } from 'react-router-dom';
import { formatDistanceToNow } from 'date-fns';
import { useAuth } from '../../contexts/AuthContext';
import {
  useServiceUsers,
  useUpdateServiceUserRole,
  useUpdateServiceUserStatus,
} from '../../hooks/useServiceUsers';
import { InviteTechnicianModal } from '../../components/users';
import { ServiceResetPasswordModal } from '../../components/users/ServiceResetPasswordModal';
import { Loader, Button, Badge, ConfirmModal, Input } from '../../components/ui';
import type { ServiceUser } from '../../services/serviceUserService';
import './ServiceUsersListPage.css';

/**
 * ServiceUsersListPage - Admin-only page for managing service users
 */
export const ServiceUsersListPage = () => {
  const { user } = useAuth();
  const [searchTerm, setSearchTerm] = useState('');
  const [roleFilter, setRoleFilter] = useState<'All' | 'Admin' | 'Technician'>('All');
  const [statusFilter, setStatusFilter] = useState<'All' | 'Active' | 'Inactive'>('Active');
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(25);
  const [isInviteModalOpen, setIsInviteModalOpen] = useState(false);
  const [resetModal, setResetModal] = useState<{
    isOpen: boolean;
    user: ServiceUser | null;
  }>({
    isOpen: false,
    user: null,
  });

  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    type: 'role' | 'status' | null;
    user: ServiceUser | null;
    newValue?: 'Admin' | 'Technician' | 'Active' | 'Inactive';
  }>({
    isOpen: false,
    type: null,
    user: null,
  });

  const updateRoleMutation = useUpdateServiceUserRole();
  const updateStatusMutation = useUpdateServiceUserStatus();

  const { data, isLoading, error, refetch } = useServiceUsers({
    page,
    pageSize,
    searchTerm: searchTerm || undefined,
    role: roleFilter,
    status: statusFilter,
  });

  // API returns array directly, not paginated response
  const users = data || [];
  const totalCount = users.length;
  const totalPages = Math.ceil(totalCount / pageSize) || 1;

  // Check if user is the last active admin
  const isLastAdmin = (userId: string) => {
    const activeAdmins = users.filter((u) => u.role === 'Admin' && u.status === 'Active');
    return activeAdmins.length === 1 && activeAdmins[0].id === userId;
  };

  const getConfirmModalContent = useMemo(() => {
    if (!confirmModal.user) return { title: '', message: '' };

    if (confirmModal.type === 'role') {
      const oldRole = confirmModal.user.role;
      const newRole = confirmModal.newValue;
      return {
        title: 'Change User Role?',
        message: `Change ${confirmModal.user.fullName}'s role from ${oldRole} to ${newRole}? This will update their access permissions.`,
      };
    } else if (confirmModal.type === 'status') {
      const isDeactivating = confirmModal.newValue === 'Inactive';
      return {
        title: isDeactivating ? 'Deactivate User?' : 'Activate User?',
        message: isDeactivating
          ? `Deactivate ${confirmModal.user.fullName}? They will lose access to the system but their data will be preserved. You can reactivate them later.`
          : `Activate ${confirmModal.user.fullName}? They will regain access to the system.`,
      };
    }

    return { title: '', message: '' };
  }, [confirmModal]);

  // Check service access - must be after all hooks
  if (!user || user.role !== 'service') {
    return <Navigate to="/service/dashboard" replace />;
  }

  // Determine if user can modify another user
  const isCurrentUser = (userId: string) => userId === user.id;

  const handleRoleChange = (serviceUser: ServiceUser, newRole: 'Admin' | 'Technician') => {
    if (isCurrentUser(serviceUser.id) || !user.isAdmin) {
      return; // Should be disabled in UI
    }

    setConfirmModal({
      isOpen: true,
      type: 'role',
      user: serviceUser,
      newValue: newRole,
    });
  };

  const handleStatusToggle = (serviceUser: ServiceUser) => {
    if (isCurrentUser(serviceUser.id) || !user.isAdmin) {
      return; // Should be disabled in UI
    }

    if (serviceUser.status === 'Active' && isLastAdmin(serviceUser.id)) {
      return; // Should be disabled in UI
    }

    const newStatus = serviceUser.status === 'Active' ? 'Inactive' : 'Active';
    setConfirmModal({
      isOpen: true,
      type: 'status',
      user: serviceUser,
      newValue: newStatus,
    });
  };

  const handleResetPassword = (serviceUser: ServiceUser) => {
    setResetModal({
      isOpen: true,
      user: serviceUser,
    });
  };

  const handleConfirm = () => {
    if (!confirmModal.user) return;

    if (confirmModal.type === 'role' && confirmModal.newValue) {
      updateRoleMutation.mutate({
        userId: confirmModal.user.id,
        role: confirmModal.newValue as 'Admin' | 'Technician',
      });
    } else if (confirmModal.type === 'status' && confirmModal.newValue) {
      updateStatusMutation.mutate({
        userId: confirmModal.user.id,
        status: confirmModal.newValue as 'Active' | 'Inactive',
      });
    }

    setConfirmModal({ isOpen: false, type: null, user: null });
  };

  const formatLastActive = (lastActiveAt: string | null) => {
    if (!lastActiveAt) return 'Never';
    try {
      return formatDistanceToNow(new Date(lastActiveAt), { addSuffix: true });
    } catch {
      return 'Unknown';
    }
  };

  const formatJoinedDate = (joinedAt: string) => {
    try {
      return new Date(joinedAt).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
      });
    } catch {
      return 'Unknown';
    }
  };

  const getRoleBadgeVariant = (role: string) => {
    return role === 'Admin' ? 'primary' : 'success';
  };

  const getStatusBadgeVariant = (status: string) => {
    return status === 'Active' ? 'success' : 'default';
  };

  if (isLoading) {
    return (
      <div className="serviceUsersListPage">
        <div className="serviceUsersListPage__loading">
          <Loader size="lg" />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="serviceUsersListPage">
        <div className="serviceUsersListPage__error">
          <h2>Error Loading Service Users</h2>
          <p>Failed to load service users. Please try again.</p>
          <Button onClick={() => refetch()}>Retry</Button>
        </div>
      </div>
    );
  }

  return (
    <div className="serviceUsersListPage">
      <div className="serviceUsersListPage__header">
        <h1 className="serviceUsersListPage__title">Service Team</h1>
        <Button variant="primary" onClick={() => setIsInviteModalOpen(true)}>
          Invite Technician
        </Button>
      </div>

      <div className="serviceUsersListPage__filters">
        <div className="serviceUsersListPage__searchWrapper">
          <Input
            type="text"
            placeholder="Search by name or email..."
            value={searchTerm}
            onChange={(e) => {
              setSearchTerm(e.target.value);
              setPage(1); // Reset to first page on search
            }}
          />
        </div>

        <div className="serviceUsersListPage__filterGroup">
          <label htmlFor="roleFilter">Role:</label>
          <select
            id="roleFilter"
            value={roleFilter}
            onChange={(e) => {
              setRoleFilter(e.target.value as 'All' | 'Admin' | 'Technician');
              setPage(1);
            }}
            className="serviceUsersListPage__select"
          >
            <option value="All">All</option>
            <option value="Admin">Admin</option>
            <option value="Technician">Technician</option>
          </select>
        </div>

        <div className="serviceUsersListPage__filterGroup">
          <label htmlFor="statusFilter">Status:</label>
          <select
            id="statusFilter"
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value as 'All' | 'Active' | 'Inactive');
              setPage(1);
            }}
            className="serviceUsersListPage__select"
          >
            <option value="All">All</option>
            <option value="Active">Active</option>
            <option value="Inactive">Inactive</option>
          </select>
        </div>
      </div>

      {users.length === 0 ? (
        <div className="serviceUsersListPage__empty">
          <p>No service users found.</p>
          {(searchTerm || roleFilter !== 'All' || statusFilter !== 'Active') && (
            <Button
              variant="ghost"
              onClick={() => {
                setSearchTerm('');
                setRoleFilter('All');
                setStatusFilter('Active');
                setPage(1);
              }}
            >
              Reset Filters
            </Button>
          )}
        </div>
      ) : (
        <>
          <div className="serviceUsersListPage__tableWrapper">
            <table className="serviceUsersListPage__table">
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Role</th>
                  <th>Status</th>
                  <th>Last Active</th>
                  <th>Joined Date</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {users.map((serviceUser) => (
                  <tr key={serviceUser.id}>
                    <td className="serviceUsersListPage__nameCell">{serviceUser.fullName}</td>
                    <td>{serviceUser.email}</td>
                    <td>
                      <Badge variant={getRoleBadgeVariant(serviceUser.role)}>
                        {serviceUser.role === 'Admin' ? '👑 ' : '🔧 '}
                        {serviceUser.role}
                      </Badge>
                    </td>
                    <td>
                      <Badge variant={getStatusBadgeVariant(serviceUser.status)}>
                        {serviceUser.status}
                      </Badge>
                    </td>
                    <td>{formatLastActive(serviceUser.lastActiveAt)}</td>
                    <td>{formatJoinedDate(serviceUser.joinedAt)}</td>
                    <td>
                      <div className="serviceUsersListPage__actions">
                        <select
                          value={serviceUser.role}
                          onChange={(e) =>
                            handleRoleChange(serviceUser, e.target.value as 'Admin' | 'Technician')
                          }
                          disabled={!user.isAdmin || isCurrentUser(serviceUser.id)}
                          className="serviceUsersListPage__roleSelect"
                          aria-label={`Change role for ${serviceUser.fullName}`}
                        >
                          <option value="Technician">Technician</option>
                          <option value="Admin">Admin</option>
                        </select>

                        {user.isAdmin && (
                          <button
                            className="serviceUsersListPage__iconButton"
                            onClick={() => handleResetPassword(serviceUser)}
                            title="Reset Password"
                            disabled={serviceUser.status !== 'Active'}
                          >
                            Key
                          </button>
                        )}

                        <Button
                          variant={serviceUser.status === 'Active' ? 'danger' : 'primary'}
                          size="sm"
                          onClick={() => handleStatusToggle(serviceUser)}
                          disabled={
                            !user.isAdmin ||
                            isCurrentUser(serviceUser.id) ||
                            (serviceUser.status === 'Active' && isLastAdmin(serviceUser.id))
                          }
                        >
                          {serviceUser.status === 'Active' ? 'Deactivate' : 'Activate'}
                        </Button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="serviceUsersListPage__pagination">
            <div className="serviceUsersListPage__paginationInfo">
              Showing {(page - 1) * pageSize + 1} to {Math.min(page * pageSize, totalCount)} of{' '}
              {totalCount} users
            </div>

            <div className="serviceUsersListPage__paginationControls">
              <label htmlFor="pageSize">Items per page:</label>
              <select
                id="pageSize"
                value={pageSize}
                onChange={(e) => {
                  setPageSize(Number(e.target.value));
                  setPage(1);
                }}
                className="serviceUsersListPage__pageSizeSelect"
              >
                <option value={10}>10</option>
                <option value={25}>25</option>
                <option value={50}>50</option>
              </select>

              <div className="serviceUsersListPage__pageButtons">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  disabled={page === 1}
                >
                  Previous
                </Button>
                <span className="serviceUsersListPage__pageNumber">
                  Page {page} of {totalPages}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                  disabled={page === totalPages}
                >
                  Next
                </Button>
              </div>
            </div>
          </div>
        </>
      )}

      <InviteTechnicianModal
        isOpen={isInviteModalOpen}
        onClose={() => setIsInviteModalOpen(false)}
        onSuccess={() => refetch()}
      />

      {resetModal.user && (
        <ServiceResetPasswordModal
          isOpen={resetModal.isOpen}
          onClose={() => setResetModal({ isOpen: false, user: null })}
          userId={resetModal.user.id}
          userName={resetModal.user.fullName}
        />
      )}

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false, type: null, user: null })}
        onConfirm={handleConfirm}
        title={getConfirmModalContent.title}
        message={getConfirmModalContent.message}
        confirmText={
          confirmModal.type === 'role'
            ? 'Change Role'
            : confirmModal.newValue === 'Inactive'
              ? 'Deactivate'
              : 'Activate'
        }
        variant={
          confirmModal.type === 'status' && confirmModal.newValue === 'Inactive'
            ? 'danger'
            : 'primary'
        }
      />
    </div>
  );
};
