import { useState, useEffect } from 'react';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Input } from '../../components/ui/Input';
import { Loader } from '../../components/ui/Loader';
import { ConfirmModal } from '../../components/ui/Modal';
import { Pagination } from '../../components/tickets/Pagination';
import { InviteTeamMemberModal } from '../../components/team';
import { useTeamMembers, useUpdateMemberRole, useRemoveMember } from '../../hooks/useTeam';
import { useAuth } from '../../contexts/AuthContext';
import { useToast } from '../../hooks/useToast';
import type { OrganizationUser, UserRole } from '../../types/api';
import { formatDistanceToNow } from 'date-fns';
import { getApiErrorMessage } from '../../lib/apiClient';
import './OrganizationTeamPage.css';

export function OrganizationTeamPage() {
  const { user } = useAuth();
  const { showToast } = useToast();

  // Get organization ID and admin status from user
  const organizationId = (user as { organizationId?: string })?.organizationId || '';
  const isAdmin = (user as { isAdmin?: boolean })?.isAdmin || false;

  // State
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState('');
  const [isInviteModalOpen, setIsInviteModalOpen] = useState(false);
  const [memberToRemove, setMemberToRemove] = useState<OrganizationUser | null>(null);

  // Debounce search
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearchTerm(searchTerm);
      setPage(1); // Reset to first page on search
    }, 300);
    return () => clearTimeout(timer);
  }, [searchTerm]);

  // Fetch team members
  const { data, isLoading, error, refetch } = useTeamMembers(organizationId, {
    page,
    pageSize,
    searchTerm: debouncedSearchTerm,
  });

  // Mutations
  const updateRoleMutation = useUpdateMemberRole(organizationId);
  const removeMemberMutation = useRemoveMember(organizationId);

  const handleRoleChange = async (userId: string, newRole: UserRole) => {
    // Prevent changing own role
    if (userId === user?.id) {
      showToast('You cannot change your own role', 'error');
      return;
    }

    try {
      await updateRoleMutation.mutateAsync({ userId, role: newRole });
      showToast('Role updated successfully', 'success');
    } catch (err) {
      showToast(getApiErrorMessage(err), 'error');
    }
  };

  const handleRemoveMember = async () => {
    if (!memberToRemove) return;

    // Prevent removing self
    if (memberToRemove.id === user?.id) {
      showToast('You cannot remove yourself from the organization', 'error');
      setMemberToRemove(null);
      return;
    }

    try {
      await removeMemberMutation.mutateAsync(memberToRemove.id);
      showToast('Team member removed successfully', 'success');
      setMemberToRemove(null);
    } catch (err) {
      const errorMessage = getApiErrorMessage(err);
      showToast(errorMessage, 'error');
      setMemberToRemove(null);
    }
  };

  const getRoleBadgeVariant = (role: UserRole) => {
    return role === 'Admin' ? 'primary' : 'success';
  };

  const getStatusBadgeVariant = (status: string) => {
    switch (status) {
      case 'Active':
        return 'success';
      case 'Invited':
        return 'warning';
      case 'Pending':
        return 'default';
      default:
        return 'default';
    }
  };

  if (isLoading) {
    return (
      <div className="team-page">
        <div className="team-page__loading">
          <Loader size="lg" text="Loading team members..." />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="team-page">
        <div className="team-page__error">
          <p>Failed to load team members</p>
          <p className="team-page__error-message">{getApiErrorMessage(error)}</p>
          <Button variant="primary" onClick={() => refetch()}>
            Retry
          </Button>
        </div>
      </div>
    );
  }

  const members = data?.items || [];
  const totalCount = data?.totalCount || 0;
  const totalPages = data?.totalPages || 1;

  return (
    <div className="team-page">
      {/* Header */}
      <div className="team-page__header">
        <div>
          <nav className="team-page__breadcrumb">
            <span>Dashboard</span>
            <span className="team-page__breadcrumb-separator">›</span>
            <span>Team</span>
          </nav>
          <h1 className="team-page__title">Team Members</h1>
        </div>
        {isAdmin && (
          <Button variant="primary" onClick={() => setIsInviteModalOpen(true)}>
            Invite Team Member
          </Button>
        )}
      </div>

      {/* Search and filters */}
      <div className="team-page__controls">
        <Input
          type="text"
          placeholder="Search by name or email..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="team-page__search"
        />
        <div className="team-page__page-size">
          <label htmlFor="pageSize">Items per page:</label>
          <select
            id="pageSize"
            value={pageSize}
            onChange={(e) => {
              setPageSize(Number(e.target.value));
              setPage(1);
            }}
            className="team-page__page-size-select"
          >
            <option value="10">10</option>
            <option value="25">25</option>
            <option value="50">50</option>
          </select>
        </div>
      </div>

      {/* Team members table */}
      {members.length === 0 ? (
        <div className="team-page__empty">
          <p className="team-page__empty-title">No team members yet</p>
          {isAdmin && (
            <>
              <p className="team-page__empty-description">
                Start building your team by inviting members
              </p>
              <Button variant="primary" onClick={() => setIsInviteModalOpen(true)}>
                Invite Team Member
              </Button>
            </>
          )}
        </div>
      ) : (
        <>
          <div className="team-page__table-container">
            <table className="team-table">
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Role</th>
                  <th>Status</th>
                  <th>Joined Date</th>
                  {isAdmin && <th>Actions</th>}
                </tr>
              </thead>
              <tbody>
                {members.map((member) => (
                  <tr key={member.id}>
                    <td className="team-table__name">
                      {member.firstName} {member.lastName}
                    </td>
                    <td className="team-table__email">{member.email}</td>
                    <td>
                      <Badge variant={getRoleBadgeVariant(member.role)}>
                        {member.role === 'Admin' && <span>👑 </span>}
                        {member.role}
                      </Badge>
                    </td>
                    <td>
                      <Badge variant={getStatusBadgeVariant(member.status)}>{member.status}</Badge>
                    </td>
                    <td className="team-table__date">
                      {formatDistanceToNow(new Date(member.joinedAt), { addSuffix: true })}
                    </td>
                    {isAdmin && (
                      <td className="team-table__actions">
                        <select
                          value={member.role}
                          onChange={(e) => handleRoleChange(member.id, e.target.value as UserRole)}
                          disabled={member.id === user?.id || updateRoleMutation.isPending}
                          className="team-table__role-select"
                        >
                          <option value="Admin">Admin</option>
                          <option value="User">User</option>
                        </select>
                        <Button
                          variant="danger"
                          size="sm"
                          onClick={() => setMemberToRemove(member)}
                          disabled={member.id === user?.id || removeMemberMutation.isPending}
                        >
                          Remove
                        </Button>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          <Pagination
            currentPage={page}
            totalPages={totalPages}
            totalItems={totalCount}
            pageSize={pageSize}
            onPageChange={setPage}
          />
        </>
      )}

      {/* Invite Modal */}
      <InviteTeamMemberModal
        isOpen={isInviteModalOpen}
        onClose={() => setIsInviteModalOpen(false)}
        organizationId={organizationId}
        onSuccess={() => refetch()}
      />

      {/* Remove Confirmation Modal */}
      <ConfirmModal
        isOpen={!!memberToRemove}
        onClose={() => setMemberToRemove(null)}
        onConfirm={handleRemoveMember}
        title="Remove Team Member?"
        message={`Are you sure you want to remove ${memberToRemove?.firstName} ${memberToRemove?.lastName} from your organization? They will lose access to all tickets and data.`}
        confirmText="Remove Member"
        cancelText="Cancel"
        variant="danger"
      />
    </div>
  );
}
