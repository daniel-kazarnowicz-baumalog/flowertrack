import { useState, useMemo } from 'react';
import { Navigate } from 'react-router-dom';
import { formatDistanceToNow, format, isValid } from 'date-fns';
import { useTranslation } from 'react-i18next';
import { pl, enUS } from 'date-fns/locale';
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
  const { t, i18n } = useTranslation();
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

  const dateLocale = i18n.language === 'pl' ? pl : enUS;

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
        title: t('users.changeRoleTitle'),
        message: t('users.changeRoleMessage', {
          name: confirmModal.user.fullName,
          oldRole,
          newRole,
        }),
      };
    } else if (confirmModal.type === 'status') {
      const isDeactivating = confirmModal.newValue === 'Inactive';
      return {
        title: isDeactivating ? t('users.deactivateTitle') : t('users.activateTitle'),
        message: isDeactivating
          ? t('users.deactivateMessage', { name: confirmModal.user.fullName })
          : t('users.activateMessage', { name: confirmModal.user.fullName }),
      };
    }

    return { title: '', message: '' };
  }, [confirmModal, t]);

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
    if (!lastActiveAt) return t('users.never');
    try {
      const date = new Date(lastActiveAt);
      if (!isValid(date)) return t('users.unknown');
      return formatDistanceToNow(date, { addSuffix: true, locale: dateLocale });
    } catch {
      return t('users.unknown');
    }
  };

  const formatJoinedDate = (joinedAt: string) => {
    if (!joinedAt) return '—';
    try {
      const date = new Date(joinedAt);
      if (!isValid(date)) return t('users.unknown');
      return format(date, 'MMM dd, yyyy', { locale: dateLocale });
    } catch {
      return t('users.unknown');
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
          <h2>{t('errors.loadingFailed')}</h2>
          <p>{error.message || t('errors.somethingWentWrong')}</p>
          <Button onClick={() => refetch()}>{t('common.refresh')}</Button>
        </div>
      </div>
    );
  }

  return (
    <div className="serviceUsersListPage">
      <div className="serviceUsersListPage__header">
        <h1 className="serviceUsersListPage__title">{t('users.serviceTeam')}</h1>
        <Button variant="primary" onClick={() => setIsInviteModalOpen(true)}>
          {t('users.inviteTechnician')}
        </Button>
      </div>

      <div className="serviceUsersListPage__filters">
        <div className="serviceUsersListPage__searchWrapper">
          <Input
            type="text"
            placeholder={t('organizations.searchPlaceholder')}
            value={searchTerm}
            onChange={(e) => {
              setSearchTerm(e.target.value);
              setPage(1); // Reset to first page on search
            }}
          />
        </div>

        <div className="serviceUsersListPage__filterGroup">
          <label htmlFor="roleFilter">{t('users.role')}:</label>
          <select
            id="roleFilter"
            value={roleFilter}
            onChange={(e) => {
              setRoleFilter(e.target.value as 'All' | 'Admin' | 'Technician');
              setPage(1);
            }}
            className="serviceUsersListPage__select"
          >
            <option value="All">{t('common.all')}</option>
            <option value="Admin">{t('users.administrator')}</option>
            <option value="Technician">{t('users.technician')}</option>
          </select>
        </div>

        <div className="serviceUsersListPage__filterGroup">
          <label htmlFor="statusFilter">{t('common.status')}:</label>
          <select
            id="statusFilter"
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value as 'All' | 'Active' | 'Inactive');
              setPage(1);
            }}
            className="serviceUsersListPage__select"
          >
            <option value="All">{t('common.all')}</option>
            <option value="Active">{t('users.active')}</option>
            <option value="Inactive">{t('users.inactive')}</option>
          </select>
        </div>
      </div>

      {users.length === 0 ? (
        <div className="serviceUsersListPage__empty">
          <p>{t('users.noUsersFound')}</p>
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
              {t('users.resetFilters')}
            </Button>
          )}
        </div>
      ) : (
        <>
          <div className="serviceUsersListPage__tableWrapper">
            <table className="serviceUsersListPage__table">
              <thead>
                <tr>
                  <th>{t('users.fullName')}</th>
                  <th>{t('users.email')}</th>
                  <th>{t('users.role')}</th>
                  <th>{t('common.status')}</th>
                  <th>{t('users.lastActive')}</th>
                  <th>{t('users.joinedDate')}</th>
                  <th>{t('common.actions')}</th>
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
                        {serviceUser.role === 'Admin'
                          ? t('users.administrator')
                          : t('users.technician')}
                      </Badge>
                    </td>
                    <td>
                      <Badge variant={getStatusBadgeVariant(serviceUser.status)}>
                        {serviceUser.status === 'Active' ? t('users.active') : t('users.inactive')}
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
                          <option value="Technician">{t('users.technician')}</option>
                          <option value="Admin">{t('users.administrator')}</option>
                        </select>

                        {user.isAdmin && (
                          <button
                            className="serviceUsersListPage__iconButton"
                            onClick={() => handleResetPassword(serviceUser)}
                            title={t('users.resetPassword')}
                            disabled={serviceUser.status !== 'Active'}
                          >
                            🔑
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
                          {serviceUser.status === 'Active'
                            ? t('users.deactivate')
                            : t('users.activate')}
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
              {t('machines.showingResults', {
                from: (page - 1) * pageSize + 1,
                to: Math.min(page * pageSize, totalCount),
                total: totalCount,
              })}
            </div>

            <div className="serviceUsersListPage__paginationControls">
              <label htmlFor="pageSize">{t('common.perPage')}:</label>{' '}
              {/* Check if this makes sense '10 per page' or 'Items per page: 10'. The UI has label 'Items per page:'. 'common.perPage' is 'na stronę' / 'per page'. So '10 na stronę' is OK in select, but label? Providing 'itemsPerPage' key might be better. */}
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
                  {t('common.previous')}
                </Button>
                <span className="serviceUsersListPage__pageNumber">
                  {i18n.language === 'pl'
                    ? `Strona ${page} z ${totalPages}`
                    : `Page ${page} of ${totalPages}`}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                  disabled={page === totalPages}
                >
                  {t('common.next')}
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
            ? t('users.changeRole')
            : confirmModal.newValue === 'Inactive'
              ? t('users.deactivate')
              : t('users.activate')
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
