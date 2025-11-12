import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useOrganizations } from '../../hooks/useOrganizations';
import { Loader } from '../../components/ui/Loader';
import { Button } from '../../components/ui/Button';
import type { OrganizationDto } from '../../types/api';
import './OrganizationsListPage.css';

/**
 * OrganizationsListPage - Service portal page to manage all organizations
 * Shows table with search, filter, sort, and pagination
 */
export const OrganizationsListPage = () => {
  const navigate = useNavigate();
  const { organizations, isLoading, error } = useOrganizations();
  const [searchQuery, setSearchQuery] = useState('');
  const [filterAlarms, setFilterAlarms] = useState(false);
  const [filterActiveTickets, setFilterActiveTickets] = useState(false);
  const [sortField, setSortField] = useState<keyof OrganizationDto>('name');
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');

  // Filter organizations based on search and filters
  const filteredOrganizations = organizations?.filter((org: OrganizationDto) => {
    const matchesSearch =
      searchQuery === '' ||
      org.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      org.contactEmail.toLowerCase().includes(searchQuery.toLowerCase());

    const matchesAlarms = !filterAlarms || org.hasAlarmMachines;
    const matchesActiveTickets = !filterActiveTickets || org.activeTicketsCount > 0;

    return matchesSearch && matchesAlarms && matchesActiveTickets;
  });

  // Sort organizations
  const sortedOrganizations = filteredOrganizations?.sort(
    (a: OrganizationDto, b: OrganizationDto) => {
      let aValue = a[sortField];
      let bValue = b[sortField];

      // Handle undefined values
      if (aValue === undefined) return 1;
      if (bValue === undefined) return -1;

      // Handle different data types
      if (typeof aValue === 'string' && typeof bValue === 'string') {
        aValue = aValue.toLowerCase();
        bValue = bValue.toLowerCase();
      }

      if (aValue < bValue) return sortDirection === 'asc' ? -1 : 1;
      if (aValue > bValue) return sortDirection === 'asc' ? 1 : -1;
      return 0;
    }
  );

  const handleSort = (field: keyof OrganizationDto) => {
    if (sortField === field) {
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
    } else {
      setSortField(field);
      setSortDirection('asc');
    }
  };

  const getSortIcon = (field: keyof OrganizationDto) => {
    if (sortField !== field) return '⇅';
    return sortDirection === 'asc' ? '↑' : '↓';
  };

  if (isLoading) {
    return (
      <div className="organizationsListPage">
        <div className="organizationsListPage__loading">
          <Loader size="lg" />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="organizationsListPage">
        <div className="organizationsListPage__error">
          <h2>Error Loading Organizations</h2>
          <p>Failed to load organizations. Please try refreshing the page.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="organizationsListPage">
      {/* Header */}
      <div className="organizationsListPage__header">
        <div>
          <h1 className="organizationsListPage__title">Organizations</h1>
          <p className="organizationsListPage__subtitle">
            Manage client organizations and their settings
          </p>
        </div>
        <Button onClick={() => {}} variant="primary">
          + Onboard Organization
        </Button>
      </div>

      {/* Filters */}
      <div className="organizationsListPage__filters">
        <input
          type="text"
          placeholder="Search by name or email..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="organizationsListPage__searchInput"
        />
        <div className="organizationsListPage__filterGroup">
          <label className="organizationsListPage__filterLabel">
            <input
              type="checkbox"
              checked={filterAlarms}
              onChange={(e) => setFilterAlarms(e.target.checked)}
            />
            <span>Has Alarms</span>
          </label>
          <label className="organizationsListPage__filterLabel">
            <input
              type="checkbox"
              checked={filterActiveTickets}
              onChange={(e) => setFilterActiveTickets(e.target.checked)}
            />
            <span>Has Active Tickets</span>
          </label>
        </div>
      </div>

      {/* Results count */}
      <div className="organizationsListPage__results">
        Showing {sortedOrganizations?.length || 0} of {organizations?.length || 0} organizations
      </div>

      {/* Table */}
      {sortedOrganizations && sortedOrganizations.length > 0 ? (
        <div className="organizationsListPage__tableWrapper">
          <table className="organizationsListPage__table">
            <thead>
              <tr>
                <th onClick={() => handleSort('name')} className="organizationsListPage__sortable">
                  Organization {getSortIcon('name')}
                </th>
                <th
                  onClick={() => handleSort('contactEmail')}
                  className="organizationsListPage__sortable"
                >
                  Contact Email {getSortIcon('contactEmail')}
                </th>
                <th
                  onClick={() => handleSort('machinesCount')}
                  className="organizationsListPage__sortable"
                >
                  Machines {getSortIcon('machinesCount')}
                </th>
                <th
                  onClick={() => handleSort('activeTicketsCount')}
                  className="organizationsListPage__sortable"
                >
                  Active Tickets {getSortIcon('activeTicketsCount')}
                </th>
                <th>Status</th>
                <th
                  onClick={() => handleSort('createdAt')}
                  className="organizationsListPage__sortable"
                >
                  Created {getSortIcon('createdAt')}
                </th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {sortedOrganizations.map((org: OrganizationDto) => (
                <tr key={org.id} className="organizationsListPage__row">
                  <td className="organizationsListPage__orgName">{org.name}</td>
                  <td>{org.contactEmail}</td>
                  <td className="organizationsListPage__centered">{org.machinesCount}</td>
                  <td className="organizationsListPage__centered">{org.activeTicketsCount}</td>
                  <td>
                    {org.hasAlarmMachines && (
                      <span className="organizationsListPage__badge organizationsListPage__badge--alarm">
                        ⚠️ Alarm
                      </span>
                    )}
                  </td>
                  <td>{new Date(org.createdAt).toLocaleDateString()}</td>
                  <td>
                    <Button
                      variant="secondary"
                      size="sm"
                      onClick={() => navigate(`/service/organizations/${org.id}`)}
                    >
                      View Details
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <div className="organizationsListPage__empty">
          <p>No organizations found matching your filters.</p>
        </div>
      )}
    </div>
  );
};
