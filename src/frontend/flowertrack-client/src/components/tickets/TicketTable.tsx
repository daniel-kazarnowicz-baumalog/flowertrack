import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Badge, getStatusColor, getPriorityColor } from '../ui/Badge';
import { Button } from '../ui/Button';
import { Loader } from '../ui/Loader';
import type { TicketDto } from '../../types/api';
import { formatDistanceToNow } from 'date-fns';
import { pl } from 'date-fns/locale';
import './TicketTable.css';

interface TicketTableProps {
  tickets: TicketDto[];
  isLoading?: boolean;
  onBulkAction?: (ticketIds: string[], action: 'assign' | 'status') => void;
  showOrganization?: boolean;
  showAssignee?: boolean;
  basePath: string; // '/service/tickets' or '/client/tickets'
}

export const TicketTable: React.FC<TicketTableProps> = ({
  tickets,
  isLoading = false,
  onBulkAction,
  showOrganization = false,
  showAssignee = false,
  basePath,
}) => {
  const navigate = useNavigate();
  const [selectedIds, setSelectedIds] = useState<string[]>([]);

  const toggleSelectAll = () => {
    if (selectedIds.length === tickets.length) {
      setSelectedIds([]);
    } else {
      setSelectedIds(tickets.map((t) => t.id));
    }
  };

  const toggleSelect = (id: string) => {
    setSelectedIds((prev) => (prev.includes(id) ? prev.filter((i) => i !== id) : [...prev, id]));
  };

  const handleRowClick = (ticketId: string, e: React.MouseEvent) => {
    // Don't navigate if clicking checkbox
    const target = e.target as HTMLElement;
    if (target.tagName === 'INPUT' && (target as HTMLInputElement).type === 'checkbox') {
      return;
    }
    navigate(`${basePath}/${ticketId}`);
  };

  if (isLoading) {
    return (
      <div className="ticket-table__loading">
        <Loader size="lg" text="Ładowanie zgłoszeń..." />
      </div>
    );
  }

  if (tickets.length === 0) {
    return (
      <div className="ticketTable__empty">
        <p>Brak zgłoszeń spełniających kryteria</p>
      </div>
    );
  }

  return (
    <div className="ticketTable">
      {selectedIds.length > 0 && onBulkAction && (
        <div className="ticketTable__bulkActions">
          <span className="ticketTable__bulkCount">Zaznaczono: {selectedIds.length}</span>
          <Button
            variant="outline"
            size="small"
            onClick={() => onBulkAction(selectedIds, 'assign')}
          >
            Przypisz
          </Button>
          <Button
            variant="outline"
            size="small"
            onClick={() => onBulkAction(selectedIds, 'status')}
          >
            Zmień status
          </Button>
          <Button variant="outline" size="small" onClick={() => setSelectedIds([])}>
            Anuluj
          </Button>
        </div>
      )}

      <div className="ticketTable__container">
        <table className="ticketTable__table">
          <thead>
            <tr>
              {onBulkAction && (
                <th className="ticketTable__checkboxCell">
                  <input
                    type="checkbox"
                    checked={selectedIds.length === tickets.length && tickets.length > 0}
                    onChange={toggleSelectAll}
                  />
                </th>
              )}
              <th>Numer</th>
              <th>Tytuł</th>
              <th>Status</th>
              <th>Priorytet</th>
              {showOrganization && <th>Organizacja</th>}
              <th>Maszyna</th>
              {showAssignee && <th>Przypisane do</th>}
              <th>Utworzono</th>
            </tr>
          </thead>
          <tbody>
            {tickets.map((ticket) => (
              <tr
                key={ticket.id}
                className="ticketTable__row"
                onClick={(e) => handleRowClick(ticket.id, e)}
              >
                {onBulkAction && (
                  <td className="ticketTable__checkboxCell" onClick={(e) => e.stopPropagation()}>
                    <input
                      type="checkbox"
                      checked={selectedIds.includes(ticket.id)}
                      onChange={() => toggleSelect(ticket.id)}
                    />
                  </td>
                )}
                <td className="ticketTable__number">{ticket.ticketNumber}</td>
                <td className="ticketTable__title">
                  <div className="ticketTable__titleText">{ticket.title}</div>
                  <div className="ticketTable__creator">
                    Utworzone przez: {ticket.createdByName}
                  </div>
                </td>
                <td>
                  <Badge variant={getStatusColor(ticket.status)}>{ticket.status}</Badge>
                </td>
                <td>
                  <Badge variant={getPriorityColor(ticket.priority)}>{ticket.priority}</Badge>
                </td>
                {showOrganization && (
                  <td className="ticketTable__organization">{ticket.organizationName}</td>
                )}
                <td className="ticketTable__machine">
                  <div>{ticket.machineModel}</div>
                  <div className="ticketTable__serialNumber">{ticket.machineSerialNumber}</div>
                </td>
                {showAssignee && (
                  <td className="ticketTable__assignee">
                    {ticket.assignedToName || (
                      <span className="ticketTable__unassigned">Nieprzypisane</span>
                    )}
                  </td>
                )}
                <td className="ticketTable__date">
                  {formatDistanceToNow(new Date(ticket.createdAt), {
                    addSuffix: true,
                    locale: pl,
                  })}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};
