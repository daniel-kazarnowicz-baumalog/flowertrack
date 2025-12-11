import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useMachines } from '../../hooks/useMachines';
import { RegisterMachineModal } from '../../components/machines';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Loader } from '../../components/ui/Loader';
import { Pagination } from '../../components/tickets/Pagination';
import type { MachineStatus } from '../../types/api';
import './MachinesListPage.css';

/**
 * Machines List Page - Service Portal
 * Displays all machines across all organizations
 */
export const MachinesListPage = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('All');
  const [sortBy, setSortBy] = useState('serialNumber');
  const [sortDesc, setSortDesc] = useState(false);
  const [isRegisterModalOpen, setIsRegisterModalOpen] = useState(false);

  // Debounced search
  const [debouncedSearch, setDebouncedSearch] = useState('');
  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedSearch(search);
      setPage(1); // Reset to first page on search
    }, 300);
    return () => clearTimeout(handler);
  }, [search]);

  const { data, isLoading, error } = useMachines({
    page,
    pageSize,
    search: debouncedSearch,
    status: statusFilter === 'All' ? undefined : statusFilter,
    sortBy,
    sortDesc,
  });

  const handleSort = (column: string) => {
    if (sortBy === column) {
      setSortDesc(!sortDesc);
    } else {
      setSortBy(column);
      setSortDesc(false);
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

  const getStatusLabel = (status: string) => {
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

  if (error) {
    return (
      <div className="machinesList">
        <div className="machinesList__header">
          <h1>{t('machines.title')}</h1>
        </div>
        <div className="machinesList__error">
          <p>
            {t('errors.loadingFailed')}: {error.message}
          </p>
          <Button onClick={() => window.location.reload()}>{t('common.refresh')}</Button>
        </div>
      </div>
    );
  }

  return (
    <div className="machinesList">
      <div className="machinesList__header">
        <h1>{t('machines.title')}</h1>
        <Button onClick={() => setIsRegisterModalOpen(true)}>
          ➕ {t('machines.registerMachine')}
        </Button>
      </div>

      {/* Filters */}
      <div className="machinesList__filters">
        <Input
          type="text"
          placeholder={t('machines.searchPlaceholder')}
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="machinesList__search"
        />

        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="machinesList__select"
        >
          <option value="All">{t('machines.allStatuses')}</option>
          <option value="Active">{t('machines.statusActive')}</option>
          <option value="Maintenance">{t('machines.statusMaintenance')}</option>
          <option value="Alarm">{t('machines.statusAlarm')}</option>
          <option value="Inactive">{t('machines.statusInactive')}</option>
        </select>

        <select
          value={pageSize}
          onChange={(e) => {
            setPageSize(Number(e.target.value));
            setPage(1);
          }}
          className="machinesList__select"
        >
          <option value={10}>10 {t('common.perPage')}</option>
          <option value={25}>25 {t('common.perPage')}</option>
          <option value={50}>50 {t('common.perPage')}</option>
        </select>
      </div>

      {/* Table */}
      {isLoading ? (
        <div className="machinesList__loading">
          <Loader size="lg" />
          <p>{t('common.loading')}</p>
        </div>
      ) : !data?.items?.length ? (
        <div className="machinesList__empty">
          <div className="machinesList__emptyIcon">🏭</div>
          <h2>{t('machines.notFoundTitle')}</h2>
          <p>
            {search || statusFilter !== 'All'
              ? t('machines.notFoundFilterHint')
              : t('machines.notFoundStartHint')}
          </p>
          {!search && statusFilter === 'All' && (
            <Button onClick={() => setIsRegisterModalOpen(true)}>
              {t('machines.registerFirstMachine')}
            </Button>
          )}
        </div>
      ) : (
        <>
          <div className="machinesList__tableWrapper">
            <table className="machinesList__table">
              <thead>
                <tr>
                  <th onClick={() => handleSort('serialNumber')}>
                    {t('machines.serialNumber')}{' '}
                    {sortBy === 'serialNumber' && (sortDesc ? '↓' : '↑')}
                  </th>
                  <th onClick={() => handleSort('model')}>
                    {t('machines.model')} {sortBy === 'model' && (sortDesc ? '↓' : '↑')}
                  </th>
                  <th onClick={() => handleSort('status')}>
                    {t('common.status')} {sortBy === 'status' && (sortDesc ? '↓' : '↑')}
                  </th>
                  <th onClick={() => handleSort('organizationName')}>
                    {t('machines.organization')}{' '}
                    {sortBy === 'organizationName' && (sortDesc ? '↓' : '↑')}
                  </th>
                  <th>{t('machines.location')}</th>
                  <th>{t('machines.activeTickets')}</th>
                  <th>{t('common.actions')}</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((machine) => (
                  <tr key={machine.id}>
                    <td>
                      <strong>{machine.serialNumber}</strong>
                    </td>
                    <td>{machine.model || '—'}</td>
                    <td>
                      <Badge variant={getMachineStatusVariant(machine.status)}>
                        {getStatusLabel(machine.status)}
                      </Badge>
                    </td>
                    <td>
                      <Link
                        to={`/service/organizations/${machine.organizationId}`}
                        className="machinesList__orgLink"
                      >
                        {machine.organizationName}
                      </Link>
                    </td>
                    <td>{machine.location || '—'}</td>
                    <td>
                      {machine.activeTicketsCount > 0 ? (
                        <Badge variant="warning">{machine.activeTicketsCount}</Badge>
                      ) : (
                        '—'
                      )}
                    </td>
                    <td>
                      <Link to={`/service/machines/${machine.id}`}>
                        <Button size="sm" variant="secondary">
                          {t('machines.viewDetails')}
                        </Button>
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="machinesList__footer">
            <div className="machinesList__info">
              {t('machines.showingResults', {
                from: (page - 1) * pageSize + 1,
                to: Math.min(page * pageSize, data?.totalCount ?? 0),
                total: data?.totalCount ?? 0,
              })}
            </div>
            <Pagination
              currentPage={page}
              totalPages={data?.totalPages ?? 1}
              totalItems={data?.totalCount ?? 0}
              pageSize={pageSize}
              onPageChange={setPage}
            />
          </div>
        </>
      )}

      <RegisterMachineModal
        isOpen={isRegisterModalOpen}
        onClose={() => setIsRegisterModalOpen(false)}
        onSuccess={(machine) => {
          setIsRegisterModalOpen(false);
          navigate(`/service/machines/${machine.id}`);
        }}
      />
    </div>
  );
};
