/**
 * MachineDetailPage - Service Portal page to view machine details
 * Features detailed machine information, service history, and edit functionality
 */

import { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { format, formatDistanceToNow } from 'date-fns';
import { useMachine } from '../../hooks/useMachines';
import { useTickets } from '../../hooks/useTickets';
import { EditMachineModal } from '../../components/machines';
import { Loader } from '../../components/ui/Loader';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Card } from '../../components/ui/Card';
import type { MachineStatus, TicketStatus } from '../../types/api';
import './MachineDetailPage.css';

/**
 * Machine Detail Page - Service Portal
 * Displays comprehensive machine information and service history
 */
export const MachineDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const { data: machine, isLoading, error, refetch } = useMachine(id);
  const { data: ticketsData } = useTickets({ machineId: id, pageSize: 100 });

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

  if (isLoading) {
    return (
      <div className="machineDetail__loading">
        <Loader size="lg" />
        <p>Loading machine details...</p>
      </div>
    );
  }

  if (error || !machine) {
    return (
      <div className="machineDetail__error">
        <h2>Machine Not Found</h2>
        <p>{error?.message || 'The machine you are looking for does not exist.'}</p>
        <div className="machineDetail__errorActions">
          <Button onClick={() => navigate('/service/machines')}>Back to Machines</Button>
          <Button variant="ghost" onClick={() => refetch()}>
            Retry
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
        <Link to="/service/machines">Machines</Link>
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
          <Badge variant={getMachineStatusVariant(machine.status)}>{machine.status}</Badge>
          <Button onClick={() => setIsEditModalOpen(true)}>Edit Machine</Button>
        </div>
      </div>

      {/* Overview Card */}
      <Card>
        <div className="machineDetail__section">
          <h2>Overview</h2>
          <div className="machineDetail__overviewGrid">
            <div className="machineDetail__field">
              <span className="machineDetail__label">Organization:</span>
              <Link
                to={`/service/organizations/${machine.organizationId}`}
                className="machineDetail__link"
              >
                {machine.organizationName}
              </Link>
            </div>
            <div className="machineDetail__field">
              <span className="machineDetail__label">Location:</span>
              <span className="machineDetail__value">{machine.location || '—'}</span>
            </div>
            <div className="machineDetail__field">
              <span className="machineDetail__label">Installation Date:</span>
              <span className="machineDetail__value">
                {machine.installationDate
                  ? format(new Date(machine.installationDate), 'MMM dd, yyyy')
                  : '—'}
              </span>
            </div>
            <div className="machineDetail__field">
              <span className="machineDetail__label">Last Updated:</span>
              <span className="machineDetail__value">
                {formatDistanceToNow(new Date(machine.updatedAt), { addSuffix: true })}
              </span>
            </div>
            <div className="machineDetail__field">
              <span className="machineDetail__label">Active Tickets:</span>
              <span className="machineDetail__value">
                {activeTickets.length > 0 ? (
                  <Link
                    to={`/service/tickets?machineId=${machine.id}&status=New,Accepted,InProgress,Reopened`}
                    className="machineDetail__link"
                  >
                    <Badge variant="warning">{activeTickets.length}</Badge>
                  </Link>
                ) : (
                  '—'
                )}
              </span>
            </div>
            <div className="machineDetail__field">
              <span className="machineDetail__label">Total Tickets:</span>
              <span className="machineDetail__value">
                {tickets.length > 0 ? (
                  <Link
                    to={`/service/tickets?machineId=${machine.id}`}
                    className="machineDetail__link"
                  >
                    {tickets.length}
                  </Link>
                ) : (
                  '0'
                )}
              </span>
            </div>
          </div>
        </div>
      </Card>

      {/* Service History */}
      <Card>
        <div className="machineDetail__section">
          <h2>Service History</h2>
          {tickets.length === 0 ? (
            <div className="machineDetail__empty">
              <p>No service history yet</p>
            </div>
          ) : (
            <div className="machineDetail__timeline">
              {tickets.map((ticket) => (
                <div key={ticket.id} className="machineDetail__timelineItem">
                  <div className="machineDetail__timelineMarker" />
                  <div className="machineDetail__timelineContent">
                    <div className="machineDetail__timelineHeader">
                      <Link
                        to={`/service/tickets/${ticket.id}`}
                        className="machineDetail__ticketLink"
                      >
                        <strong>{ticket.ticketNumber}</strong>
                      </Link>
                      <Badge variant={getTicketStatusVariant(ticket.status)}>{ticket.status}</Badge>
                    </div>
                    <p className="machineDetail__ticketTitle">{ticket.title}</p>
                    <div className="machineDetail__timelineFooter">
                      <span className="machineDetail__timelineDate">
                        Created{' '}
                        {formatDistanceToNow(new Date(ticket.createdAt), { addSuffix: true })}
                      </span>
                      {ticket.resolvedAt && (
                        <>
                          <span className="machineDetail__timelineSep">•</span>
                          <span className="machineDetail__timelineDate">
                            Resolved{' '}
                            {formatDistanceToNow(new Date(ticket.resolvedAt), { addSuffix: true })}
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
      </Card>

      {/* Specifications Section (if notes exist) */}
      {machine.notes && (
        <Card>
          <div className="machineDetail__section">
            <h2>Notes & Specifications</h2>
            <p className="machineDetail__notes">{machine.notes}</p>
          </div>
        </Card>
      )}

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
