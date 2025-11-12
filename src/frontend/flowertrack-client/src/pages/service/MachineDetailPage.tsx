import { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { format } from 'date-fns';
import { useMachine, useMachineMutations } from '../../hooks/useMachines';
import { useTickets } from '../../hooks/useTickets';
import { EditMachineModal } from '../../components/machines';
import { Loader } from '../../components/ui/Loader';
import { Button } from '../../components/ui/Button';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import type { UpdateMachineRequest, MachineStatus } from '../../types/api';
import './MachineDetailPage.css';

/**
 * MachineDetailPage - Service portal page to view machine details
 * Features overview, service history, and ticket management
 */
export const MachineDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [showStatusDropdown, setShowStatusDropdown] = useState(false);

  const { data: machine, isLoading, error, refetch } = useMachine(id);
  const { data: ticketsData, isLoading: ticketsLoading } = useTickets({
    machineId: id,
    page: 1,
    pageSize: 10,
  });

  const { updateMachine, changeMachineStatus, isUpdating, isChangingStatus } =
    useMachineMutations();

  const handleEditMachine = (data: UpdateMachineRequest) => {
    if (id) {
      updateMachine.mutate(
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

  const handleStatusChange = (newStatus: MachineStatus) => {
    if (id) {
      changeMachineStatus.mutate(
        { id, data: { status: newStatus } },
        {
          onSuccess: () => {
            setShowStatusDropdown(false);
            refetch();
          },
        }
      );
    }
  };

  if (isLoading) {
    return (
      <div className="machineDetail__loading">
        <Loader size="lg" />
      </div>
    );
  }

  if (error || !machine) {
    return (
      <div className="machineDetail__error">
        <h2>Error Loading Machine</h2>
        <p>{error?.message || 'Machine not found'}</p>
        <Button onClick={() => navigate('/service/machines')}>Back to Machines</Button>
      </div>
    );
  }

  const tickets = ticketsData?.items || [];
  const statusOptions: MachineStatus[] = ['Active', 'Maintenance', 'Alarm', 'Inactive'];

  return (
    <div className="machineDetail">
      {/* Breadcrumb */}
      <nav className="machineDetail__breadcrumb">
        <Link to="/service/machines" className="machineDetail__breadcrumbLink">
          Machines
        </Link>
        <span className="machineDetail__breadcrumbSeparator">/</span>
        <span className="machineDetail__breadcrumbCurrent">{machine.serialNumber}</span>
      </nav>

      {/* Header */}
      <div className="machineDetail__header">
        <div className="machineDetail__headerLeft">
          <h1 className="machineDetail__title">{machine.serialNumber}</h1>
          <div className="machineDetail__subtitle">
            <span>{machine.model}</span>
            <span className="machineDetail__separator">•</span>
            <Link
              to={`/service/organizations/${machine.organizationId}`}
              className="machineDetail__orgLink"
            >
              {machine.organizationName}
            </Link>
          </div>
        </div>
        <div className="machineDetail__headerRight">
          <div className="machineDetail__statusControl">
            <Button
              variant="secondary"
              onClick={() => setShowStatusDropdown(!showStatusDropdown)}
              disabled={isChangingStatus}
            >
              Change Status
            </Button>
            {showStatusDropdown && (
              <div className="machineDetail__statusDropdown">
                {statusOptions.map((status) => (
                  <div
                    key={status}
                    className="machineDetail__statusOption"
                    onClick={() => handleStatusChange(status)}
                  >
                    <Badge variant={getStatusVariant(status)}>{status}</Badge>
                  </div>
                ))}
              </div>
            )}
          </div>
          <Button variant="primary" onClick={() => setIsEditModalOpen(true)}>
            Edit Details
          </Button>
        </div>
      </div>

      {/* Overview Section */}
      <div className="machineDetail__overview">
        <Card>
          <div className="machineDetail__overviewGrid">
            <div className="machineDetail__overviewItem">
              <span className="machineDetail__label">Status</span>
              <Badge variant={getStatusVariant(machine.status)}>{machine.status}</Badge>
            </div>
            <div className="machineDetail__overviewItem">
              <span className="machineDetail__label">Location</span>
              <span className="machineDetail__value">{machine.location || 'Not specified'}</span>
            </div>
            <div className="machineDetail__overviewItem">
              <span className="machineDetail__label">Installation Date</span>
              <span className="machineDetail__value">
                {machine.installationDate
                  ? format(new Date(machine.installationDate), 'MMM dd, yyyy')
                  : 'Not specified'}
              </span>
            </div>
            <div className="machineDetail__overviewItem">
              <span className="machineDetail__label">Active Tickets</span>
              <span className="machineDetail__value">{machine.activeTicketsCount}</span>
            </div>
            <div className="machineDetail__overviewItem">
              <span className="machineDetail__label">Registered</span>
              <span className="machineDetail__value">
                {format(new Date(machine.createdAt), 'MMM dd, yyyy')}
              </span>
            </div>
            <div className="machineDetail__overviewItem">
              <span className="machineDetail__label">Last Updated</span>
              <span className="machineDetail__value">
                {format(new Date(machine.updatedAt), 'MMM dd, yyyy HH:mm')}
              </span>
            </div>
          </div>
          {machine.notes && (
            <div className="machineDetail__notes">
              <span className="machineDetail__label">Notes</span>
              <p className="machineDetail__notesText">{machine.notes}</p>
            </div>
          )}
        </Card>
      </div>

      {/* Related Tickets Section */}
      <div className="machineDetail__section">
        <div className="machineDetail__sectionHeader">
          <h2 className="machineDetail__sectionTitle">Service History</h2>
          <Button
            variant="secondary"
            onClick={() =>
              navigate(
                `/service/tickets/new?machineId=${machine.id}&organizationId=${machine.organizationId}`
              )
            }
          >
            Create Ticket
          </Button>
        </div>

        <Card>
          {ticketsLoading ? (
            <div className="machineDetail__loading">
              <Loader size="md" />
            </div>
          ) : tickets.length === 0 ? (
            <div className="machineDetail__emptyState">
              <p className="machineDetail__emptyText">No service tickets yet</p>
              <p className="machineDetail__emptySubtext">
                Create a ticket to start tracking service history for this machine
              </p>
            </div>
          ) : (
            <div className="machineDetail__ticketsTable">
              <table className="machineDetail__table">
                <thead>
                  <tr>
                    <th>Ticket #</th>
                    <th>Title</th>
                    <th>Priority</th>
                    <th>Status</th>
                    <th>Created</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {tickets.map((ticket) => (
                    <tr key={ticket.id}>
                      <td>
                        <code className="machineDetail__ticketNumber">#{ticket.ticketNumber}</code>
                      </td>
                      <td>{ticket.title}</td>
                      <td>
                        <Badge variant={getPriorityVariant(ticket.priority)}>
                          {ticket.priority}
                        </Badge>
                      </td>
                      <td>
                        <Badge variant={getTicketStatusVariant(ticket.status)}>
                          {ticket.status}
                        </Badge>
                      </td>
                      <td>{format(new Date(ticket.createdAt), 'MMM dd, yyyy')}</td>
                      <td>
                        <Link
                          to={`/service/tickets/${ticket.id}`}
                          className="machineDetail__viewLink"
                        >
                          View Details
                        </Link>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </Card>
      </div>

      {/* Edit Modal */}
      <EditMachineModal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        onSubmit={handleEditMachine}
        machine={machine}
        isLoading={isUpdating}
      />
    </div>
  );
};

// Helper functions for badge variants
function getStatusVariant(status: MachineStatus): 'success' | 'warning' | 'danger' | 'default' {
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
}

function getPriorityVariant(priority: string): 'danger' | 'warning' | 'default' | 'success' {
  switch (priority.toLowerCase()) {
    case 'critical':
      return 'danger';
    case 'high':
      return 'warning';
    case 'medium':
      return 'default';
    case 'low':
      return 'success';
    default:
      return 'default';
  }
}

function getTicketStatusVariant(status: string): 'default' | 'warning' | 'success' | 'danger' {
  switch (status.toLowerCase()) {
    case 'new':
      return 'default';
    case 'in progress':
      return 'warning';
    case 'resolved':
      return 'success';
    case 'closed':
      return 'default';
    default:
      return 'default';
  }
}
