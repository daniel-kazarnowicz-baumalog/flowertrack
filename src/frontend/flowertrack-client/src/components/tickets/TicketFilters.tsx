import { useState, useEffect } from 'react';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import type {
  TicketFilters as TicketFiltersType,
  TicketStatus,
  TicketPriority,
} from '../../types/api';
import './TicketFilters.css';

interface TicketFiltersProps {
  onFilterChange: (filters: TicketFiltersType) => void;
  showOrganizationFilter?: boolean;
  showAssigneeFilter?: boolean;
  organizations?: Array<{ id: string; name: string }>;
  assignees?: Array<{ id: string; name: string }>;
}

const STATUS_OPTIONS: TicketStatus[] = [
  'New',
  'Accepted',
  'InProgress',
  'Resolved',
  'Closed',
  'Reopened',
];
const PRIORITY_OPTIONS: TicketPriority[] = ['Low', 'Medium', 'High', 'Critical'];

export const TicketFilters: React.FC<TicketFiltersProps> = ({
  onFilterChange,
  showOrganizationFilter = false,
  showAssigneeFilter = false,
  organizations = [],
  assignees = [],
}) => {
  const [search, setSearch] = useState('');
  const [selectedStatuses, setSelectedStatuses] = useState<TicketStatus[]>([]);
  const [selectedPriorities, setSelectedPriorities] = useState<TicketPriority[]>([]);
  const [selectedOrganization, setSelectedOrganization] = useState('');
  const [selectedAssignee, setSelectedAssignee] = useState('');
  const [isExpanded, setIsExpanded] = useState(false);

  useEffect(() => {
    const filters: TicketFiltersType = {
      search: search || undefined,
      status: selectedStatuses.length > 0 ? selectedStatuses : undefined,
      priority: selectedPriorities.length > 0 ? selectedPriorities : undefined,
      organizationId: selectedOrganization || undefined,
      assignedToId: selectedAssignee === 'unassigned' ? 'null' : selectedAssignee || undefined,
    };
    onFilterChange(filters);
  }, [
    search,
    selectedStatuses,
    selectedPriorities,
    selectedOrganization,
    selectedAssignee,
    onFilterChange,
  ]);

  const toggleStatus = (status: TicketStatus) => {
    setSelectedStatuses((prev) =>
      prev.includes(status) ? prev.filter((s) => s !== status) : [...prev, status]
    );
  };

  const togglePriority = (priority: TicketPriority) => {
    setSelectedPriorities((prev) =>
      prev.includes(priority) ? prev.filter((p) => p !== priority) : [...prev, priority]
    );
  };

  const resetFilters = () => {
    setSearch('');
    setSelectedStatuses([]);
    setSelectedPriorities([]);
    setSelectedOrganization('');
    setSelectedAssignee('');
  };

  const activeFilterCount =
    (search ? 1 : 0) +
    selectedStatuses.length +
    selectedPriorities.length +
    (selectedOrganization ? 1 : 0) +
    (selectedAssignee ? 1 : 0);

  return (
    <div className="ticketFilters">
      <div className="ticketFilters__header">
        <Input
          type="text"
          placeholder="Szukaj po numerze lub tytule zgłoszenia..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="ticketFilters__search"
        />
        <div className="ticketFilters__actions">
          <Button
            variant="outline"
            onClick={() => setIsExpanded(!isExpanded)}
            className="ticketFilters__toggleButton"
          >
            {isExpanded ? '▲' : '▼'} Filtry {activeFilterCount > 0 && `(${activeFilterCount})`}
          </Button>
          {activeFilterCount > 0 && (
            <Button variant="outline" onClick={resetFilters}>
              Wyczyść filtry
            </Button>
          )}
        </div>
      </div>

      {isExpanded && (
        <div className="ticketFilters__panel">
          <div className="ticketFilters__section">
            <label className="ticketFilters__label">Status:</label>
            <div className="ticketFilters__options">
              {STATUS_OPTIONS.map((status) => (
                <button
                  key={status}
                  type="button"
                  className={`ticketFilters__chip ${selectedStatuses.includes(status) ? 'ticketFilters__chip--active' : ''}`}
                  onClick={() => toggleStatus(status)}
                >
                  {status}
                </button>
              ))}
            </div>
          </div>

          <div className="ticketFilters__section">
            <label className="ticketFilters__label">Priorytet:</label>
            <div className="ticketFilters__options">
              {PRIORITY_OPTIONS.map((priority) => (
                <button
                  key={priority}
                  type="button"
                  className={`ticketFilters__chip ${selectedPriorities.includes(priority) ? 'ticketFilters__chip--active' : ''} ticketFilters__chip--${priority.toLowerCase()}`}
                  onClick={() => togglePriority(priority)}
                >
                  {priority}
                </button>
              ))}
            </div>
          </div>

          {showOrganizationFilter && organizations.length > 0 && (
            <div className="ticketFilters__section">
              <label className="ticketFilters__label">Organizacja:</label>
              <select
                value={selectedOrganization}
                onChange={(e) => setSelectedOrganization(e.target.value)}
                className="ticketFilters__select"
              >
                <option value="">Wszystkie organizacje</option>
                {organizations.map((org) => (
                  <option key={org.id} value={org.id}>
                    {org.name}
                  </option>
                ))}
              </select>
            </div>
          )}

          {showAssigneeFilter && (
            <div className="ticketFilters__section">
              <label className="ticketFilters__label">Przypisane do:</label>
              <select
                value={selectedAssignee}
                onChange={(e) => setSelectedAssignee(e.target.value)}
                className="ticketFilters__select"
              >
                <option value="">Wszyscy</option>
                <option value="me">Moje zgłoszenia</option>
                <option value="unassigned">Nieprzypisane</option>
                {assignees.map((assignee) => (
                  <option key={assignee.id} value={assignee.id}>
                    {assignee.name}
                  </option>
                ))}
              </select>
            </div>
          )}
        </div>
      )}
    </div>
  );
};
