import { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
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
import { Badge } from '../../components/ui/Badge';
import { TicketStatusBadge, TicketPriorityBadge } from '../../components/ui/Badge';
import type {
  MachineDto,
  TicketDto,
  OrganizationUserDto,
  UpdateOrganizationRequest,
  MachineStatus,
} from '../../types/api';
import './OrganizationDetailPage.css';

type TabType = 'overview' | 'machines' | 'team' | 'tickets';

/**
 * OrganizationDetailPage - Service portal page to view organization details
 * Features tabbed interface with Overview, Machines, Team, and Tickets sections
 */
export const OrganizationDetailPage = () => {
  const { t } = useTranslation();
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
    if (id && window.confirm(t('organizations.regenerateConfirm'))) {
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
    if (!id) return;

    if (window.confirm(t('organizations.deleteConfirm'))) {
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

  const getMachineStatusVariant = (
    status: MachineStatus
  ): 'success' | 'warning' | 'danger' | 'default' => {
    switch (status) {
      case 'Active':
        return 'success';
      case 'Maintenance':
        return 'warning';
      case 'Alarm':
        return 'danger';
      case 'Inactive':
        return 'default';
      default:
        return 'default';
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
        <h2>{t('errors.loadingFailed')}</h2>
        <p>{error?.message || t('errors.notFound')}</p>
        <Button onClick={() => navigate('/service/organizations')}>{t('common.back')}</Button>
      </div>
    );
  }

  /* Icons */
  const EmailIcon = () => (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      className="organizationDetail__icon"
    >
      <rect width="20" height="16" x="2" y="4" rx="2" />
      <path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7" />
    </svg>
  );

  const PhoneIcon = () => (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      className="organizationDetail__icon"
    >
      <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z" />
    </svg>
  );

  const MapPinIcon = () => (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      className="organizationDetail__icon"
    >
      <path d="M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0Z" />
      <circle cx="12" cy="10" r="3" />
    </svg>
  );

  const BuildingIcon = () => (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      className="organizationDetail__icon"
    >
      <rect width="16" height="20" x="4" y="2" rx="2" ry="2" />
      <path d="M9 22v-4h6v4" />
      <path d="M8 6h.01" />
      <path d="M16 6h.01" />
      <path d="M12 6h.01" />
      <path d="M12 10h.01" />
      <path d="M12 14h.01" />
      <path d="M16 10h.01" />
      <path d="M16 14h.01" />
      <path d="M8 10h.01" />
      <path d="M8 14h.01" />
    </svg>
  );

  const CalendarIcon = () => (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      className="organizationDetail__icon"
    >
      <rect width="18" height="18" x="3" y="4" rx="2" ry="2" />
      <line x1="16" x2="16" y1="2" y2="6" />
      <line x1="8" x2="8" y1="2" y2="6" />
      <line x1="3" x2="21" y1="10" y2="10" />
    </svg>
  );

  const ChartIcon = () => (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      className="organizationDetail__icon"
    >
      <path d="M3 3v18h18" />
      <path d="m19 9-5 5-4-4-3 3" />
    </svg>
  );

  const renderOverviewTab = () => (
    <div className="organizationDetail__overview">
      <div className="organizationDetail__infoGrid">
        <Card>
          <div className="organizationDetail__infoCard">
            <h3>
              <BuildingIcon /> {t('organizations.contactInformation')}
            </h3>
            <div className="organizationDetail__fieldGroup">
              <div className="organizationDetail__field">
                <span className="organizationDetail__label">
                  <EmailIcon /> {t('organizations.email')}
                </span>
                <span
                  className={
                    organization.contactEmail
                      ? 'organizationDetail__value'
                      : 'organizationDetail__value organizationDetail__value--empty'
                  }
                >
                  {organization.contactEmail || t('common.notProvided')}
                </span>
              </div>
              <div className="organizationDetail__field">
                <span className="organizationDetail__label">
                  <PhoneIcon /> {t('organizations.phone')}
                </span>
                <span
                  className={
                    organization.contactPhone
                      ? 'organizationDetail__value'
                      : 'organizationDetail__value organizationDetail__value--empty'
                  }
                >
                  {organization.contactPhone || t('common.notProvided')}
                </span>
              </div>
              <div className="organizationDetail__field">
                <span className="organizationDetail__label">
                  {t('organizations.serviceStatus')}
                </span>
                <span className="organizationDetail__value">
                  <Badge variant={organization.serviceStatus === 'Active' ? 'success' : 'warning'}>
                    {organization.serviceStatus || t('machines.statusActive')}
                  </Badge>
                </span>
              </div>
              <div className="organizationDetail__field">
                <span className="organizationDetail__label">{t('common.created')}</span>
                <span className="organizationDetail__value">
                  {format(new Date(organization.createdAt), 'MMM dd, yyyy')}
                </span>
              </div>
            </div>
          </div>
        </Card>

        <Card>
          <div className="organizationDetail__infoCard">
            <h3>
              <MapPinIcon /> {t('organizations.address')}
            </h3>
            <div className="organizationDetail__fieldGroup">
              <div className="organizationDetail__field">
                <span className="organizationDetail__label">{t('common.street')}</span>
                <span
                  className={
                    organization.address
                      ? 'organizationDetail__value'
                      : 'organizationDetail__value organizationDetail__value--empty'
                  }
                >
                  {organization.address || t('common.notProvided')}
                </span>
              </div>
              <div className="organizationDetail__field">
                <span className="organizationDetail__label">
                  {t('common.city')} / {t('common.postalCode')}
                </span>
                <span
                  className={
                    organization.city || organization.postalCode
                      ? 'organizationDetail__value'
                      : 'organizationDetail__value organizationDetail__value--empty'
                  }
                >
                  {organization.city || t('common.notProvided')}{' '}
                  {organization.postalCode ? `, ${organization.postalCode}` : ''}
                </span>
              </div>
              <div className="organizationDetail__field">
                <span className="organizationDetail__label">{t('common.country')}</span>
                <span
                  className={
                    organization.country
                      ? 'organizationDetail__value'
                      : 'organizationDetail__value organizationDetail__value--empty'
                  }
                >
                  {organization.country || t('common.notProvided')}
                </span>
              </div>
            </div>
          </div>
        </Card>

        <Card>
          <div className="organizationDetail__infoCard">
            <h3>
              <ChartIcon /> {t('organizations.statistics')}
            </h3>
            <div className="organizationDetail__statsGrid">
              <div className="organizationDetail__statItem">
                <span className="organizationDetail__statLabel">
                  {t('organizations.totalMachines')}
                </span>
                <span className="organizationDetail__statValue">{organization.machinesCount}</span>
              </div>
              <div className="organizationDetail__statItem">
                <span className="organizationDetail__statLabel">
                  {t('organizations.activeTickets')}
                </span>
                <span className="organizationDetail__statValue">
                  {organization.activeTicketsCount}
                </span>
              </div>
            </div>
            <div className="organizationDetail__field">
              <span className="organizationDetail__label">{t('organizations.machineStatus')}</span>
              <span className="organizationDetail__value">
                {organization.hasAlarmMachines ? (
                  <Badge variant="danger">⚠️ {t('organizations.hasAlarms')}</Badge>
                ) : (
                  <Badge variant="success">✓ {t('organizations.noAlarms')}</Badge>
                )}
              </span>
            </div>
          </div>
        </Card>

        <Card>
          <div className="organizationDetail__infoCard">
            <h3>
              <CalendarIcon /> {t('organizations.contractStart')} / {t('organizations.contractEnd')}
            </h3>
            <div className="organizationDetail__fieldGroup">
              <div className="organizationDetail__field">
                <span className="organizationDetail__label">
                  {t('organizations.contractStart')}
                </span>
                <span
                  className={
                    organization.contractStartDate
                      ? 'organizationDetail__value'
                      : 'organizationDetail__value organizationDetail__value--empty'
                  }
                >
                  {organization.contractStartDate
                    ? format(new Date(organization.contractStartDate), 'MMM dd, yyyy')
                    : t('common.notProvided')}
                </span>
              </div>
              <div className="organizationDetail__field">
                <span className="organizationDetail__label">{t('organizations.contractEnd')}</span>
                <span
                  className={
                    organization.contractEndDate
                      ? 'organizationDetail__value'
                      : 'organizationDetail__value organizationDetail__value--empty'
                  }
                >
                  {organization.contractEndDate
                    ? format(new Date(organization.contractEndDate), 'MMM dd, yyyy')
                    : t('common.notProvided')}
                </span>
              </div>
            </div>
          </div>
        </Card>
      </div>

      {organization.notes && (
        <Card className="organizationDetail__notesCard">
          <h3>{t('common.notes')}</h3>
          <p className="organizationDetail__notes">{organization.notes}</p>
        </Card>
      )}

      <div className="organizationDetail__actionsSection">
        <Button variant="secondary" onClick={() => setIsEditModalOpen(true)}>
          ✏️ {t('organizations.editOrganization')}
        </Button>
        <Button
          variant="danger"
          onClick={handleDeleteOrganization}
          disabled={isDeleting || organization.activeTicketsCount > 0}
          title={
            organization.activeTicketsCount > 0 ? t('organizations.cannotDeleteWithTickets') : ''
          }
        >
          {isDeleting ? t('common.deleting') : `🗑️ ${t('organizations.deleteOrganization')}`}
        </Button>
      </div>

      <Card className="organizationDetail__apiKeyCard">
        <h3>{t('organizations.apiKeyManagement')}</h3>
        <p className="organizationDetail__apiKeyDescription">
          {t('organizations.apiKeyDescription')}
        </p>
        <div className="organizationDetail__apiKeyRow">
          <div className="organizationDetail__apiKeyInput">
            <input
              type={showApiKey ? 'text' : 'password'}
              value={organization.apiKey || t('organizations.noApiKey')}
              readOnly
              className="organizationDetail__apiKeyField"
            />
            <Button variant="secondary" size="sm" onClick={() => setShowApiKey(!showApiKey)}>
              {showApiKey
                ? `🙈 ${t('organizations.hideApiKey')}`
                : `👁️ ${t('organizations.showApiKey')}`}
            </Button>
            {organization.apiKey && (
              <Button variant="secondary" size="sm" onClick={copyApiKey}>
                📋 {t('organizations.copyApiKey')}
              </Button>
            )}
          </div>
          <Button
            variant="danger"
            size="sm"
            onClick={handleRegenerateApiKey}
            disabled={isRegeneratingApiKey}
          >
            {isRegeneratingApiKey
              ? t('common.loading')
              : `🔄 ${t('organizations.regenerateApiKey')}`}
          </Button>
        </div>
      </Card>
    </div>
  );

  const renderMachinesTab = () => (
    <div className="organizationDetail__machines">
      <div className="organizationDetail__tabHeader">
        <h3>{t('machines.registeredMachines')}</h3>
        <Button variant="primary" onClick={() => setIsRegisterMachineModalOpen(true)}>
          + {t('machines.registerMachine')}
        </Button>
      </div>
      {machinesLoading ? (
        <div className="organizationDetail__tabLoading">
          <Loader />
        </div>
      ) : machines.length === 0 ? (
        <div className="organizationDetail__empty">
          <p>{t('machines.noMachines')}</p>
          <Button variant="primary" onClick={() => setIsRegisterMachineModalOpen(true)}>
            + {t('machines.registerFirstMachine')}
          </Button>
        </div>
      ) : (
        <div className="organizationDetail__tableWrapper">
          <table className="organizationDetail__table">
            <thead>
              <tr>
                <th>{t('machines.serialNumber')}</th>
                <th>{t('machines.model')}</th>
                <th>{t('common.status')}</th>
                <th>{t('machines.location')}</th>
                <th>{t('common.actions')}</th>
              </tr>
            </thead>
            <tbody>
              {machines.map((machine: MachineDto) => (
                <tr key={machine.id}>
                  <td className="organizationDetail__machineSerial">{machine.serialNumber}</td>
                  <td>{machine.model}</td>
                  <td>
                    <Badge variant={getMachineStatusVariant(machine.status)}>
                      {machine.status}
                    </Badge>
                  </td>
                  <td>{machine.location || '—'}</td>
                  <td>
                    <Button
                      variant="secondary"
                      size="sm"
                      onClick={() => navigate(`/service/machines/${machine.id}`)}
                    >
                      {t('common.viewDetails')}
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
          <p>{t('team.noMembers')}</p>
        </div>
      ) : (
        <div className="organizationDetail__tableWrapper">
          <table className="organizationDetail__table">
            <thead>
              <tr>
                <th>{t('users.fullName')}</th>
                <th>{t('users.email')}</th>
                <th>{t('users.role')}</th>
                <th>{t('common.status')}</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user: OrganizationUserDto) => (
                <tr key={user.id}>
                  <td className="organizationDetail__userName">{user.fullName}</td>
                  <td>{user.email}</td>
                  <td>
                    <Badge variant={user.isAdmin ? 'primary' : 'default'}>
                      {user.isAdmin ? t('users.administrator') : t('users.operator')}
                    </Badge>
                  </td>
                  <td>
                    <Badge variant={user.status === 'Active' ? 'success' : 'warning'}>
                      {user.status}
                    </Badge>
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
          <p>{t('tickets.noTickets') || 'No tickets found.'}</p>
        </div>
      ) : (
        <div className="organizationDetail__tableWrapper">
          <table className="organizationDetail__table">
            <thead>
              <tr>
                <th>{t('tickets.ticketNumber')}</th>
                <th>{t('tickets.title')}</th>
                <th>{t('common.status')}</th>
                <th>{t('common.priority')}</th>
                <th>{t('common.created')}</th>
                <th>{t('common.actions')}</th>
              </tr>
            </thead>
            <tbody>
              {tickets.map((ticket: TicketDto) => (
                <tr key={ticket.id}>
                  <td className="organizationDetail__ticketId">#{ticket.ticketNumber}</td>
                  <td className="organizationDetail__ticketTitle">{ticket.title}</td>
                  <td>
                    <TicketStatusBadge status={ticket.status} />
                  </td>
                  <td>
                    <TicketPriorityBadge priority={ticket.priority} />
                  </td>
                  <td>{format(new Date(ticket.createdAt), 'MMM dd, yyyy')}</td>
                  <td>
                    <Button
                      variant="secondary"
                      size="sm"
                      onClick={() => navigate(`/service/tickets/${ticket.id}`)}
                    >
                      {t('common.viewDetails')}
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
        <Link to="/service/organizations">{t('organizations.title')}</Link>
        <span className="organizationDetail__breadcrumbSeparator">/</span>
        <span className="organizationDetail__breadcrumbCurrent">{organization.name}</span>
      </div>

      {/* Header */}
      <div className="organizationDetail__header">
        <div>
          <h1 className="organizationDetail__title">{organization.name}</h1>
          <p className="organizationDetail__subtitle">ID: {organization.id.slice(0, 8)}</p>
        </div>
        <div className="organizationDetail__headerActions">
          <Button variant="secondary" onClick={() => navigate('/service/organizations')}>
            ← {t('common.back')}
          </Button>
        </div>
      </div>

      {/* Tabs */}
      <div className="organizationDetail__tabs">
        <button
          className={`organizationDetail__tab ${activeTab === 'overview' ? 'organizationDetail__tab--active' : ''}`}
          onClick={() => setActiveTab('overview')}
        >
          {t('organizations.overview')}
        </button>
        <button
          className={`organizationDetail__tab ${activeTab === 'machines' ? 'organizationDetail__tab--active' : ''}`}
          onClick={() => setActiveTab('machines')}
        >
          {t('organizations.machines')} ({organization.machinesCount})
        </button>
        <button
          className={`organizationDetail__tab ${activeTab === 'team' ? 'organizationDetail__tab--active' : ''}`}
          onClick={() => setActiveTab('team')}
        >
          {t('organizations.team')} ({users.length})
        </button>
        <button
          className={`organizationDetail__tab ${activeTab === 'tickets' ? 'organizationDetail__tab--active' : ''}`}
          onClick={() => setActiveTab('tickets')}
        >
          {t('tickets.title')} ({organization.activeTicketsCount})
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
