import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useOrganizations } from '../../hooks/useOrganizations';
import { OnboardOrganizationModal } from '../../components/organizations';
import { Loader } from '../../components/ui/Loader';
import { Button } from '../../components/ui/Button';
import type { OrganizationDto, OnboardOrganizationRequest } from '../../types/api';
import './OrganizationsListPage.css';

/**
 * OrganizationsListPage - Service portal page to manage all organizations
 * Shows table with search, filter, sort, and pagination
 */
export const OrganizationsListPage = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { organizations, isLoading, error, createOrganizationAsync, isCreating } =
    useOrganizations();
  const [searchQuery, setSearchQuery] = useState('');
  const [filterAlarms, setFilterAlarms] = useState(false);
  const [filterActiveTickets, setFilterActiveTickets] = useState(false);
  const [sortField, setSortField] = useState<keyof OrganizationDto>('name');
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');
  const [isModalOpen, setIsModalOpen] = useState(false);

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

  const handleOnboardOrganization = async (data: OnboardOrganizationRequest) => {
    try {
      const organization = await createOrganizationAsync(data);
      setIsModalOpen(false);
      // Navigate only if we got a valid organization with ID
      if (organization?.id) {
        navigate(`/service/organizations/${organization.id}`);
      }
    } catch (error: any) {
      // Error is already handled by the mutation with toast
      console.error('Failed to onboard organization:', error);
      console.error('Error response:', error.response?.data);
      console.error('Error status:', error.response?.status);
      // Don't navigate on error
      return;
    }
  };

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
          <h2>{t('errors.loadingFailed')}</h2>
          <p>{t('errors.tryRefresh')}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="organizationsListPage">
      {/* Header */}
      <div className="organizationsListPage__header">
        <div>
          <h1 className="organizationsListPage__title">{t('organizations.title')}</h1>
          <p className="organizationsListPage__subtitle">{t('organizations.subtitle')}</p>
        </div>
        <Button onClick={() => setIsModalOpen(true)} variant="primary">
          + {t('organizations.onboardOrganization')}
        </Button>
      </div>

      {/* Filters */}
      <div className="organizationsListPage__filters">
        <input
          type="text"
          placeholder={t('organizations.searchPlaceholder')}
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
            <span>{t('organizations.hasAlarms')}</span>
          </label>
          <label className="organizationsListPage__filterLabel">
            <input
              type="checkbox"
              checked={filterActiveTickets}
              onChange={(e) => setFilterActiveTickets(e.target.checked)}
            />
            {/* Using activeTickets translation which works for "Has Active Tickets" context in PL ("Aktywne zgłoszenia") */}
            <span>{t('organizations.activeTickets')}</span>
          </label>
        </div>
      </div>

      {/* Results count */}
      <div className="organizationsListPage__results">
        {t('organizations.showingResults', {
          count: sortedOrganizations?.length || 0,
          total: organizations?.length || 0,
        })}
      </div>

      {/* Table */}
      {sortedOrganizations && sortedOrganizations.length > 0 ? (
        <div className="organizationsListPage__tableWrapper">
          <table className="organizationsListPage__table">
            <thead>
              <tr>
                <th onClick={() => handleSort('name')} className="organizationsListPage__sortable">
                  {t('organizations.name')} {getSortIcon('name')}
                </th>
                <th
                  onClick={() => handleSort('contactEmail')}
                  className="organizationsListPage__sortable"
                >
                  {t('organizations.contactEmail')} {getSortIcon('contactEmail')}
                </th>
                <th
                  onClick={() => handleSort('machinesCount')}
                  className="organizationsListPage__sortable"
                >
                  {t('organizations.machines')} {getSortIcon('machinesCount')}
                </th>
                <th
                  onClick={() => handleSort('activeTicketsCount')}
                  className="organizationsListPage__sortable"
                >
                  {t('organizations.activeTickets')} {getSortIcon('activeTicketsCount')}
                </th>
                <th>{t('common.status')}</th>
                <th
                  onClick={() => handleSort('createdAt')}
                  className="organizationsListPage__sortable"
                >
                  {t('common.created')} {getSortIcon('createdAt')}
                </th>
                <th>{t('common.actions')}</th>
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
                        ⚠️ {t('machines.statusAlarm')}
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
                      {t('common.viewDetails')}
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <div className="organizationsListPage__empty">
          <p>{t('organizations.noOrganizations')}</p>
        </div>
      )}

      {/* Onboard Modal */}
      <OnboardOrganizationModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleOnboardOrganization}
        isLoading={isCreating}
      />
    </div>
  );
};
