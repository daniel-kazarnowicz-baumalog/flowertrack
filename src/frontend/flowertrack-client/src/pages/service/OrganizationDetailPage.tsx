import { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { format } from 'date-fns';
import {
  useOrganization,
  useOrganizationMachines,
  useOrganizationTickets,
  useOrganizationUsers,
  useOrganizations,
} from '../../hooks/useOrganizations';
import { EditOrganizationModal } from '../../components/organizations';
import { RegisterMachineModal } from '../../components/machines';
import { Loader } from '../../components/ui/Loader';
import { Button } from '../../components/ui/Button';
import { Card } from '../../components/ui/Card';
import type {
  MachineDto,
  TicketDto,
  OrganizationUserDto,
  UpdateOrganizationRequest,
} from '../../types/api';
import './OrganizationDetailPage.css';

type TabType = 'overview' | 'machines' | 'team' | 'tickets';

/**
 * OrganizationDetailPage - Service portal page to view organization details
 * Features tabbed interface with Overview, Machines, Team, and Tickets sections
 */
export const OrganizationDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<TabType>('overview');
  const [showApiKey, setShowApiKey] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isRegisterMachineModalOpen, setIsRegisterMachineModalOpen] = useState(false);

  const { organization, isLoading, error, refetch } = useOrganization(id);
  const {
    machines,
    isLoading: machinesLoading,
    refetch: refetchMachines,
  } = useOrganizationMachines(id);
  const { tickets, isLoading: ticketsLoading } = useOrganizationTickets(id);
  const { users, isLoading: usersLoading } = useOrganizationUsers(id);
  const {
    updateOrganization,
    isUpdating,
    deleteOrganization,
    isDeleting,
    regenerateApiKey,
    isRegeneratingApiKey,
  } = useOrganizations();

  const handleRegenerateApiKey = () => {
    if (
      id &&
      window.confirm(
        'Are you sure you want to regenerate the API key? The old key will stop working.'
      )
    ) {
      regenerateApiKey(id);
    }
  };

  const handleEditOrganization = (data: UpdateOrganizationRequest) => {
    if (id) {
      updateOrganization(
        { id, data },
        {
          onSuccess: () => {
            setIsEditModalOpen(false);
            refetch();
          },
        }
      );
    }
  };

  const handleDeleteOrganization = () => {
    if (
      id &&
      window.confirm(
        'Are you sure you want to delete this organization? This action cannot be undone.'
      )
    ) {
      deleteOrganization(id, {
        onSuccess: () => {
          navigate('/service/organizations');
        },
      });
    }
  };

  const handleMachineRegistered = () => {
    refetchMachines();
    refetch();
    setIsRegisterMachineModalOpen(false);
  };

  const copyApiKey = () => {
    if (organization?.apiKey) {
      navigator.clipboard.writeText(organization.apiKey);
    }
  };

  if (isLoading) {
    return (
      <div className="organizationDetail__loading">
        <Loader size="lg" />
      </div>
    );
  }

  if (error || !organization) {
    return (
      <div className="organizationDetail__error">
        <h2>Error Loading Organization</h2>
        <p>{error?.message || 'Organization not found'}</p>
        <Button onClick={() => navigate('/service/organizations')}>Back to Organizations</Button>
      </div>
    );
  }

  const renderOverviewTab = () => (
    <div className="organizationDetail__overview">
      <div className="organizationDetail__infoGrid">
        <Card>
          <div className="organizationDetail__infoCard">
            <h3>Contact Information</h3>
            <div className="organizationDetail__infoRow">
              <span className="organizationDetail__label">Email:</span>
              <span className="organizationDetail__value">{organization.contactEmail || '—'}</span>
            </div>
            <div className="organizationDetail__infoRow">
              <span className="organizationDetail__label">Phone:</span>
              <span className="organizationDetail__value">{organization.contactPhone || '—'}</span>
            </div>
            <div className="organizationDetail__infoRow">
              <span className="organizationDetail__label">Service Status:</span>
              <span className="organizationDetail__value">
                <span
                  className={`organizationDetail__statusBadge organizationDetail__statusBadge--${organization.serviceStatus?.toLowerCase() || 'active'}`}
                >
                  {organization.serviceStatus || 'Active'}
                </span>
              </span>
            </div>
            <div className="organizationDetail__infoRow">
              <span className="organizationDetail__label">Created:</span>
              <span className="organizationDetail__value">
                {format(new Date(organization.createdAt), 'MMM dd, yyyy')}
              </span>
            </div>
          </div>
        </Card>

        <Card>
          <div className="organizationDetail__infoCard">
            <h3>Address</h3>
            <div className="organizationDetail__infoRow">
              <span className="organizationDetail__label">Street:</span>
              <span className="organizationDetail__value">{organization.address || '—'}</span>
            </div>
            <div className="organizationDetail__infoRow">
              <span className="organizationDetail__label">City:</span>
              <span className="organizationDetail__value">{organization.city || '—'}</span>
            </div>
            <div className="organizationDetail__infoRow">
              <span className="organizationDetail__label">Postal Code:</span>
              <span className="organizationDetail__value">{organization.postalCode || '—'}</span>
            </div>
            <div className="organizationDetail__infoRow">
              <span className="organizationDetail__label">Country:</span>
              <span className="organizationDetail__value">{organization.country || '—'}</span>
            </div>
          </div>
        </Card>

        <Card>
          <div className="organizationDetail__infoCard">
            <h3>Statistics</h3>
            <div className="organizationDetail__statRow">
              <span className="organizationDetail__statLabel">Total Machines</span>
              <span className="organizationDetail__statValue">{organization.machinesCount}</span>
            </div>
            <div className="organizationDetail__statRow">
              <span className="organizationDetail__statLabel">Active Tickets</span>
              <span className="organizationDetail__statValue">
                {organization.activeTicketsCount}
              </span>
            </div>
            <div className="organizationDetail__statRow">
              <span className="organizationDetail__statLabel">Machine Status</span>
              <span className="organizationDetail__statValue">
                {organization.hasAlarmMachines ? (
                  <span className="organizationDetail__alarmBadge">⚠️ Has Alarms</span>
                ) : (
                  <span className="organizationDetail__okBadge">✓ OK</span>
                )}
              </span>
            </div>
          </div>
        </Card>

        <Card>
          <div className="organizationDetail__infoCard">
            <h3>Contract Information</h3>
            <div className="organizationDetail__infoRow">
              <span className="organizationDetail__label">Contract Start:</span>
              <span className="organizationDetail__value">
                {organization.contractStartDate
                  ? format(new Date(organization.contractStartDate), 'MMM dd, yyyy')
                  : '—'}
              </span>
            </div>
            <div className="organizationDetail__infoRow">
              <span className="organizationDetail__label">Contract End:</span>
              <span className="organizationDetail__value">
                {organization.contractEndDate
                  ? format(new Date(organization.contractEndDate), 'MMM dd, yyyy')
                  : '—'}
              </span>
            </div>
          </div>
        </Card>
      </div>

      {organization.notes && (
        <Card className="organizationDetail__notesCard">
          <h3>Notes</h3>
          <p className="organizationDetail__notes">{organization.notes}</p>
        </Card>
      )}

      <div className="organizationDetail__actionsSection">
        <Button variant="secondary" onClick={() => setIsEditModalOpen(true)}>
          ✏️ Edit Organization
        </Button>
        <Button
          variant="danger"
          onClick={handleDeleteOrganization}
          disabled={isDeleting || organization.activeTicketsCount > 0}
          title={
            organization.activeTicketsCount > 0
              ? 'Cannot delete organization with active tickets'
              : ''
          }
        >
          {isDeleting ? 'Deleting...' : '🗑️ Delete Organization'}
        </Button>
      </div>

      <Card className="organizationDetail__apiKeyCard">
        <h3>API Key Management</h3>
        <p className="organizationDetail__apiKeyDescription">
          API key allows this organization to integrate with external systems.
        </p>
        <div className="organizationDetail__apiKeyRow">
          <div className="organizationDetail__apiKeyInput">
            <input
              type={showApiKey ? 'text' : 'password'}
              value={organization.apiKey || 'No API key generated'}
              readOnly
              className="organizationDetail__apiKeyField"
            />
            <Button variant="secondary" size="small" onClick={() => setShowApiKey(!showApiKey)}>
              {showApiKey ? '🙈 Hide' : '👁️ Show'}
            </Button>
            {organization.apiKey && (
              <Button variant="secondary" size="small" onClick={copyApiKey}>
                📋 Copy
              </Button>
            )}
          </div>
          <Button
            variant="danger"
            size="small"
            onClick={handleRegenerateApiKey}
            disabled={isRegeneratingApiKey}
          >
            {isRegeneratingApiKey ? 'Regenerating...' : '🔄 Regenerate'}
          </Button>
        </div>
      </Card>
    </div>
  );

  const renderMachinesTab = () => (
    <div className="organizationDetail__machines">
      <div className="organizationDetail__tabHeader">
        <h3>Registered Machines</h3>
        <Button variant="primary" onClick={() => setIsRegisterMachineModalOpen(true)}>
          + Register Machine
        </Button>
      </div>
      {machinesLoading ? (
        <div className="organizationDetail__tabLoading">
          <Loader />
        </div>
      ) : machines.length === 0 ? (
        <div className="organizationDetail__empty">
          <p>No machines registered for this organization.</p>
          <Button variant="primary" onClick={() => setIsRegisterMachineModalOpen(true)}>
            + Register First Machine
          </Button>
        </div>
      ) : (
        <div className="organizationDetail__tableWrapper">
          <table className="organizationDetail__table">
            <thead>
              <tr>
                <th>Serial Number</th>
                <th>Model</th>
                <th>Status</th>
                <th>Location</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {machines.map((machine: MachineDto) => (
                <tr key={machine.id}>
                  <td className="organizationDetail__machineSerial">{machine.serialNumber}</td>
                  <td>{machine.model}</td>
                  <td>
                    <span
                      className={`organizationDetail__statusBadge organizationDetail__statusBadge--${machine.status.toLowerCase()}`}
                    >
                      {machine.status}
                    </span>
                  </td>
                  <td>{machine.location || '—'}</td>
                  <td>
                    <Button
                      variant="secondary"
                      size="small"
                      onClick={() => navigate(`/service/machines/${machine.id}`)}
                    >
                      View Details
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );

  const renderTeamTab = () => (
    <div className="organizationDetail__team">
      {usersLoading ? (
        <div className="organizationDetail__tabLoading">
          <Loader />
        </div>
      ) : users.length === 0 ? (
        <div className="organizationDetail__empty">
          <p>No team members found.</p>
        </div>
      ) : (
        <div className="organizationDetail__tableWrapper">
          <table className="organizationDetail__table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Role</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user: OrganizationUserDto) => (
                <tr key={user.id}>
                  <td className="organizationDetail__userName">{user.fullName}</td>
                  <td>{user.email}</td>
                  <td>
                    <span
                      className={`organizationDetail__roleBadge ${user.isAdmin ? 'organizationDetail__roleBadge--admin' : ''}`}
                    >
                      {user.isAdmin ? 'Admin' : 'User'}
                    </span>
                  </td>
                  <td>
                    <span
                      className={`organizationDetail__statusBadge organizationDetail__statusBadge--${user.status.toLowerCase()}`}
                    >
                      {user.status}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );

  const renderTicketsTab = () => (
    <div className="organizationDetail__tickets">
      {ticketsLoading ? (
        <div className="organizationDetail__tabLoading">
          <Loader />
        </div>
      ) : tickets.length === 0 ? (
        <div className="organizationDetail__empty">
          <p>No tickets found for this organization.</p>
        </div>
      ) : (
        <div className="organizationDetail__tableWrapper">
          <table className="organizationDetail__table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Title</th>
                <th>Status</th>
                <th>Priority</th>
                <th>Created</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {tickets.map((ticket: TicketDto) => (
                <tr key={ticket.id}>
                  <td className="organizationDetail__ticketId">#{ticket.id.slice(0, 8)}</td>
                  <td className="organizationDetail__ticketTitle">{ticket.title}</td>
                  <td>
                    <span
                      className={`organizationDetail__statusBadge organizationDetail__statusBadge--${ticket.status.toLowerCase()}`}
                    >
                      {ticket.status}
                    </span>
                  </td>
                  <td>
                    <span
                      className={`organizationDetail__priorityBadge organizationDetail__priorityBadge--${ticket.priority.toLowerCase()}`}
                    >
                      {ticket.priority}
                    </span>
                  </td>
                  <td>{format(new Date(ticket.createdAt), 'MMM dd, yyyy')}</td>
                  <td>
                    <Button
                      variant="secondary"
                      size="small"
                      onClick={() => navigate(`/service/tickets/${ticket.id}`)}
                    >
                      View
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );

  return (
    <div className="organizationDetail">
      {/* Breadcrumb Navigation */}
      <div className="organizationDetail__breadcrumb">
        <Link to="/service/organizations">Organizations</Link>
        <span className="organizationDetail__breadcrumbSeparator">/</span>
        <span className="organizationDetail__breadcrumbCurrent">{organization.name}</span>
      </div>

      {/* Header */}
      <div className="organizationDetail__header">
        <div>
          <h1 className="organizationDetail__title">{organization.name}</h1>
          <p className="organizationDetail__subtitle">
            Organization ID: {organization.id.slice(0, 8)}
          </p>
        </div>
        <div className="organizationDetail__headerActions">
          <Button variant="secondary" onClick={() => navigate('/service/organizations')}>
            ← Back
          </Button>
        </div>
      </div>

      {/* Tabs */}
      <div className="organizationDetail__tabs">
        <button
          className={`organizationDetail__tab ${activeTab === 'overview' ? 'organizationDetail__tab--active' : ''}`}
          onClick={() => setActiveTab('overview')}
        >
          Overview
        </button>
        <button
          className={`organizationDetail__tab ${activeTab === 'machines' ? 'organizationDetail__tab--active' : ''}`}
          onClick={() => setActiveTab('machines')}
        >
          Machines ({organization.machinesCount})
        </button>
        <button
          className={`organizationDetail__tab ${activeTab === 'team' ? 'organizationDetail__tab--active' : ''}`}
          onClick={() => setActiveTab('team')}
        >
          Team ({users.length})
        </button>
        <button
          className={`organizationDetail__tab ${activeTab === 'tickets' ? 'organizationDetail__tab--active' : ''}`}
          onClick={() => setActiveTab('tickets')}
        >
          Tickets ({organization.activeTicketsCount})
        </button>
      </div>

      {/* Tab Content */}
      <div className="organizationDetail__tabContent">
        {activeTab === 'overview' && renderOverviewTab()}
        {activeTab === 'machines' && renderMachinesTab()}
        {activeTab === 'team' && renderTeamTab()}
        {activeTab === 'tickets' && renderTicketsTab()}
      </div>

      {/* Edit Modal */}
      <EditOrganizationModal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        onSubmit={handleEditOrganization}
        organization={organization}
        isLoading={isUpdating}
      />

      {/* Register Machine Modal */}
      <RegisterMachineModal
        isOpen={isRegisterMachineModalOpen}
        onClose={() => setIsRegisterMachineModalOpen(false)}
        onSuccess={handleMachineRegistered}
        organizationId={id!}
      />
    </div>
  );
};
