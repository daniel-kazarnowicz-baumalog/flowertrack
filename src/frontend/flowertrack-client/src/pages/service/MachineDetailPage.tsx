/**
 * MachineDetailPage - Service Portal page to view machine details
 * Features detailed machine information, service history, and edit functionality.
 * Modernized UI/UX.
 */

import { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { format, formatDistanceToNow } from 'date-fns';
import { pl, enUS } from 'date-fns/locale';
import { useTranslation } from 'react-i18next';
import { useMachine } from '../../hooks/useMachines';
import { useTickets } from '../../hooks/useTickets';
import { EditMachineModal } from '../../components/machines';
import { Loader } from '../../components/ui/Loader';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import type { MachineStatus, TicketStatus } from '../../types/api';
import './MachineDetailPage.css';

/**
 * Machine Detail Page - Service Portal
 * Displays comprehensive machine information and service history
 */
export const MachineDetailPage = () => {
  const { t, i18n } = useTranslation();
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const { data: machine, isLoading, error, refetch } = useMachine(id);
  const { data: ticketsData } = useTickets({ machineId: id, pageSize: 100 });

  const dateLocale = i18n.language === 'pl' ? pl : enUS;

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

  const getTicketStatusVariant = (
    status: TicketStatus
  ): 'success' | 'warning' | 'danger' | 'info' | 'default' => {
    switch (status) {
      case 'New':
        return 'info';
      case 'Accepted':
      case 'InProgress':
        return 'warning';
      case 'Resolved':
      case 'Closed':
        return 'success';
      case 'Reopened':
        return 'danger';
      default:
        return 'default';
    }
  };

  // Helper for timeline marker styling
  const getTimelineMarkerClass = (status: TicketStatus) => {
    switch (status) {
      case 'New':
        return 'new';
      case 'Accepted':
      case 'InProgress':
      case 'Reopened':
        return 'active';
      case 'Resolved':
        return 'resolved';
      case 'Closed':
        return 'closed';
      default:
        return '';
    }
  };

  const getMachineStatusLabel = (status: string) => {
    switch (status) {
      case 'Active':
        return t('machines.statusActive');
      case 'Maintenance':
        return t('machines.statusMaintenance');
      case 'Alarm':
        return t('machines.statusAlarm');
      case 'Inactive':
        return t('machines.statusInactive');
      default:
        return status;
    }
  };

  if (isLoading) {
    return (
      <div className="machineDetail__loading">
        <Loader size="lg" />
        <p>{t('machines.machineDetails')}...</p>
      </div>
    );
  }

  if (error || !machine) {
    return (
      <div className="machineDetail__error">
        <h2>{t('errors.notFound')}</h2>
        <p>{error?.message || t('errors.somethingWentWrong')}</p>
        <div className="machineDetail__errorActions">
          <Button onClick={() => navigate('/service/machines')}>
            {t('machines.backToMachines')}
          </Button>
          <Button variant="ghost" onClick={() => refetch()}>
            {t('common.refresh')}
          </Button>
        </div>
      </div>
    );
  }

  const tickets = ticketsData?.items || [];
  const activeTickets = tickets.filter((t) => t.status !== 'Closed' && t.status !== 'Resolved');

  return (
    <div className="machineDetail">
      {/* Breadcrumb */}
      <nav className="machineDetail__breadcrumb">
        <Link to="/service/machines">{t('machines.title')}</Link>
        <span className="machineDetail__breadcrumbSep">›</span>
        <span>{machine.serialNumber}</span>
      </nav>

      {/* Header */}
      <div className="machineDetail__header">
        <div className="machineDetail__headerLeft">
          <h1>{machine.serialNumber}</h1>
          <p className="machineDetail__subtitle">{machine.model}</p>
        </div>
        <div className="machineDetail__headerRight">
          <Badge variant={getMachineStatusVariant(machine.status)} size="lg">
            {getMachineStatusLabel(machine.status)}
          </Badge>
          <Button onClick={() => setIsEditModalOpen(true)}>{t('machines.editMachine')}</Button>
        </div>
      </div>

      <div className="machineDetail__content">
        {/* Main Content Area */}
        <div className="machineDetail__main">
          {/* Service History */}
          <div className="machineDetail__card">
            <div className="machineDetail__cardHeader">
              <h2>{t('machines.machineHistory')}</h2>
            </div>
            <div className="machineDetail__cardContent">
              {tickets.length === 0 ? (
                <div className="machineDetail__empty">
                  <p>{t('machines.noMachines')}</p>{' '}
                  {/* Reusing noMachines but maybe should be noTickets? Using general 'no data' equivalent */}
                </div>
              ) : (
                <div className="machineDetail__timeline">
                  {tickets.map((ticket) => (
                    <div key={ticket.id} className="machineDetail__timelineItem">
                      <div
                        className={`machineDetail__timelineMarker ${getTimelineMarkerClass(ticket.status)}`}
                      >
                        {/* Could put icon here */}
                      </div>
                      <div className="machineDetail__timelineContent">
                        <div className="machineDetail__timelineHeader">
                          <div>
                            <div className="machineDetail__ticketNumber">
                              #{ticket.ticketNumber}
                            </div>
                            <h3 className="machineDetail__ticketTitle">{ticket.title}</h3>
                          </div>
                          <Badge variant={getTicketStatusVariant(ticket.status)} size="sm">
                            {ticket.status}
                          </Badge>
                        </div>
                        <Link
                          to={`/service/tickets/${ticket.id}`}
                          className="machineDetail__link"
                          style={{ fontSize: '0.875rem' }}
                        >
                          {t('machines.viewTicketDetails')} →
                        </Link>
                        <div className="machineDetail__timelineFooter">
                          <span>
                            {t('machines.createdAgo', {
                              time: formatDistanceToNow(new Date(ticket.createdAt), {
                                addSuffix: true,
                                locale: dateLocale,
                              }),
                            })}
                          </span>
                          {ticket.resolvedAt && (
                            <>
                              <span>•</span>
                              <span>
                                {t('machines.resolvedAgo', {
                                  time: formatDistanceToNow(new Date(ticket.resolvedAt), {
                                    addSuffix: true,
                                    locale: dateLocale,
                                  }),
                                })}
                              </span>
                            </>
                          )}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          {/* Notes & Specs (if any) */}
          {machine.notes && (
            <div className="machineDetail__card">
              <div className="machineDetail__cardHeader">
                <h2>{t('machines.notesAndSpecifications')}</h2>
              </div>
              <div className="machineDetail__cardContent">
                <p className="machineDetail__notes">{machine.notes}</p>
              </div>
            </div>
          )}
        </div>

        {/* Sidebar */}
        <div className="machineDetail__sidebar">
          {/* Machine Info */}
          <div className="machineDetail__card">
            <div className="machineDetail__cardHeader">
              <h2>{t('organizations.overview')}</h2>
            </div>
            <div className="machineDetail__cardContent">
              <div className="machineDetail__infoList">
                <div className="machineDetail__infoItem">
                  <span className="machineDetail__label">{t('machines.organization')}</span>
                  <Link
                    to={`/service/organizations/${machine.organizationId}`}
                    className="machineDetail__link"
                  >
                    {machine.organizationName}
                  </Link>
                </div>
                <div className="machineDetail__infoItem">
                  <span className="machineDetail__label">{t('machines.location')}</span>
                  <span className="machineDetail__value">{machine.location || '—'}</span>
                </div>
                <div className="machineDetail__infoItem">
                  <span className="machineDetail__label">{t('machines.installationDate')}</span>
                  <span className="machineDetail__value">
                    {machine.installationDate
                      ? format(new Date(machine.installationDate), 'MMM dd, yyyy', {
                          locale: dateLocale,
                        })
                      : '—'}
                  </span>
                </div>
                <div className="machineDetail__infoItem">
                  <span className="machineDetail__label">{t('machines.lastUpdated')}</span>
                  <span className="machineDetail__value">
                    {formatDistanceToNow(new Date(machine.updatedAt), {
                      addSuffix: true,
                      locale: dateLocale,
                    })}
                  </span>
                </div>
              </div>
            </div>
          </div>

          {/* Stats Grid */}
          <div className="machineDetail__statsGrid">
            <div className="machineDetail__stat">
              <span className="machineDetail__statLabel">{t('machines.activeTickets')}</span>
              <span
                className="machineDetail__statValue"
                style={{ color: activeTickets.length > 0 ? 'var(--color-amber-500)' : 'inherit' }}
              >
                {activeTickets.length}
              </span>
            </div>
            <div className="machineDetail__stat">
              <span className="machineDetail__statLabel">{t('machines.totalTickets')}</span>
              <span className="machineDetail__statValue">{tickets.length}</span>
            </div>
          </div>
        </div>
      </div>

      {/* Edit Modal */}
      <EditMachineModal
        isOpen={isEditModalOpen}
        machine={machine}
        onClose={() => setIsEditModalOpen(false)}
        onSuccess={() => {
          setIsEditModalOpen(false);
          refetch();
        }}
      />
    </div>
  );
};
