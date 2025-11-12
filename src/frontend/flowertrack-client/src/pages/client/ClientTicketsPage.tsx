import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  TicketFilters,
  TicketTable,
  Pagination,
  CreateTicketModal,
} from '../../components/tickets';
import { Button } from '../../components/ui/Button';
import { useTickets, useCreateTicket } from '../../hooks/useTickets';
import { useToast } from '../../hooks/useToast';
import type { TicketFilters as TicketFiltersType, CreateTicketRequest } from '../../types/api';
import './ClientTicketsPage.css';

// Mock machines - TODO: Replace with real API call
const MOCK_MACHINES = [
  { id: '1', model: 'Baumalog X500', serialNumber: 'BML2024001' },
  { id: '2', model: 'Baumalog Pro 2000', serialNumber: 'BML2024002' },
];

export const ClientTicketsPage: React.FC = () => {
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [page, setPage] = useState(1);
  const [filters, setFilters] = useState<TicketFiltersType>({});
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const pageSize = 20;

  const { data, isLoading, error } = useTickets({
    ...filters,
    page,
    pageSize,
  });

  const createTicketMutation = useCreateTicket();

  const handleFilterChange = (newFilters: TicketFiltersType) => {
    setFilters(newFilters);
    setPage(1);
  };

  const handleCreateTicket = async (data: CreateTicketRequest) => {
    try {
      const ticket = await createTicketMutation.mutateAsync(data);
      showToast('Zgłoszenie utworzone', 'success');
      setIsCreateModalOpen(false);
      navigate(`/client/tickets/${ticket.id}`);
    } catch {
      showToast('Nie udało się utworzyć zgłoszenia', 'error');
    }
  };

  if (error) {
    return (
      <div className="clientTicketsPage">
        <div className="clientTicketsPage__error">
          <h2>Błąd ładowania zgłoszeń</h2>
          <p>{error.message}</p>
          <Button onClick={() => window.location.reload()}>Odśwież stronę</Button>
        </div>
      </div>
    );
  }

  return (
    <div className="clientTicketsPage">
      <div className="clientTicketsPage__header">
        <h1>Moje Zgłoszenia</h1>
        <Button onClick={() => setIsCreateModalOpen(true)}>+ Nowe Zgłoszenie</Button>
      </div>

      <TicketFilters
        onFilterChange={handleFilterChange}
        showOrganizationFilter={false}
        showAssigneeFilter={false}
      />

      <TicketTable
        tickets={data?.items || []}
        isLoading={isLoading}
        showOrganization={false}
        showAssignee={true}
        basePath="/client/tickets"
      />

      {data && data.totalPages > 1 && (
        <Pagination
          currentPage={page}
          totalPages={data.totalPages}
          totalItems={data.totalCount}
          pageSize={pageSize}
          onPageChange={setPage}
        />
      )}

      <CreateTicketModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        machines={MOCK_MACHINES}
        onSubmit={handleCreateTicket}
        isLoading={createTicketMutation.isPending}
        isFetchingMachines={false}
      />
    </div>
  );
};
