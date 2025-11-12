import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useMachines, useMachineMutations } from '../../hooks/useMachines';
import { RegisterMachineModal } from '../../components/machines';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Loader } from '../../components/ui/Loader';
import { Pagination } from '../../components/tickets/Pagination';
import type { MachineStatus, CreateMachineRequest } from '../../types/api';
import './MachinesListPage.css';

/**
 * Machines List Page - Service Portal
 * Displays all machines across all organizations
 */
export const MachinesListPage = () => {
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
  useState(() => {
    const handler = setTimeout(() => {
      setDebouncedSearch(search);
      setPage(1); // Reset to first page on search
    }, 300);
    return () => clearTimeout(handler);
  });

  const { data, isLoading, error } = useMachines({
    page,
    pageSize,
    search: debouncedSearch,
    status: statusFilter === 'All' ? undefined : statusFilter,
    sortBy,
    sortDesc,
  });

  const { createMachine, isCreating } = useMachineMutations();

  const handleRegisterMachine = (machineData: CreateMachineRequest) => {
    createMachine.mutate(machineData, {
      onSuccess: (newMachine: { id: string }) => {
        setIsRegisterModalOpen(false);
        navigate(`/service/machines/${newMachine.id}`);
      },
    });
  };

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

  if (error) {
    return (
      <div className="machinesList">
        <div className="machinesList__header">
          <h1>Machines</h1>
        </div>
        <div className="machinesList__error">
          <p>Failed to load machines: {error.message}</p>
          <Button onClick={() => window.location.reload()}>Retry</Button>
        </div>
      </div>
    );
  }

  return (
    <div className="machinesList">
      <div className="machinesList__header">
        <h1>Machines</h1>
        <Button onClick={() => setIsRegisterModalOpen(true)}>➕ Register Machine</Button>
      </div>

      {/* Filters */}
      <div className="machinesList__filters">
        <Input
          type="text"
          placeholder="Search by serial number or model..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="machinesList__search"
        />

        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="machinesList__select"
        >
          <option value="All">All Statuses</option>
          <option value="Active">Active</option>
          <option value="Maintenance">Maintenance</option>
          <option value="Alarm">Alarm</option>
          <option value="Inactive">Inactive</option>
        </select>

        <select
          value={pageSize}
          onChange={(e) => {
            setPageSize(Number(e.target.value));
            setPage(1);
          }}
          className="machinesList__select"
        >
          <option value={10}>10 per page</option>
          <option value={25}>25 per page</option>
          <option value={50}>50 per page</option>
        </select>
      </div>

      {/* Table */}
      {isLoading ? (
        <div className="machinesList__loading">
          <Loader size="lg" />
          <p>Loading machines...</p>
        </div>
      ) : !data || data.items.length === 0 ? (
        <div className="machinesList__empty">
          <div className="machinesList__emptyIcon">🏭</div>
          <h2>No Machines Found</h2>
          <p>
            {search || statusFilter !== 'All'
              ? "Try adjusting your filters to find what you're looking for."
              : 'Get started by registering your first machine.'}
          </p>
          {!search && statusFilter === 'All' && (
            <Button onClick={() => setIsRegisterModalOpen(true)}>Register First Machine</Button>
          )}
        </div>
      ) : (
        <>
          <div className="machinesList__tableWrapper">
            <table className="machinesList__table">
              <thead>
                <tr>
                  <th onClick={() => handleSort('serialNumber')}>
                    Serial Number {sortBy === 'serialNumber' && (sortDesc ? '↓' : '↑')}
                  </th>
                  <th onClick={() => handleSort('model')}>
                    Model {sortBy === 'model' && (sortDesc ? '↓' : '↑')}
                  </th>
                  <th onClick={() => handleSort('status')}>
                    Status {sortBy === 'status' && (sortDesc ? '↓' : '↑')}
                  </th>
                  <th onClick={() => handleSort('organizationName')}>
                    Organization {sortBy === 'organizationName' && (sortDesc ? '↓' : '↑')}
                  </th>
                  <th>Location</th>
                  <th>Active Tickets</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((machine: { id: string; serialNumber: string; model: string; status: MachineStatus; organizationId: string; organizationName: string; location?: string; activeTicketsCount: number }) => (
                  <tr key={machine.id}>
                    <td>
                      <strong>{machine.serialNumber}</strong>
                    </td>
                    <td>{machine.model || '—'}</td>
                    <td>
                      <Badge variant={getMachineStatusVariant(machine.status)}>
                        {machine.status}
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
                        <Button size="small" variant="secondary">
                          View Details
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
              Showing {(page - 1) * pageSize + 1} to {Math.min(page * pageSize, data.totalCount)} of{' '}
              {data.totalCount} machines
            </div>
            <Pagination
              currentPage={page}
              totalPages={data.totalPages}
              totalItems={data.totalCount}
              pageSize={pageSize}
              onPageChange={setPage}
            />
          </div>
        </>
      )}

      {/* Register Machine Modal */}
      <RegisterMachineModal
        isOpen={isRegisterModalOpen}
        onClose={() => setIsRegisterModalOpen(false)}
        onSubmit={handleRegisterMachine}
        isLoading={isCreating}
      />
    </div>
  );
};
