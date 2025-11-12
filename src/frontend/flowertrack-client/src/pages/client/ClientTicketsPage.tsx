import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { TicketFilters, TicketTable, Pagination } from '../../components/tickets';
import { Button } from '../../components/ui/Button';
import { useTickets } from '../../hooks/useTickets';
import type { TicketFilters as TicketFiltersType } from '../../types/api';
import './ClientTicketsPage.css';

export const ClientTicketsPage: React.FC = () => {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [filters, setFilters] = useState<TicketFiltersType>({});
  const pageSize = 20;

  const { data, isLoading, error } = useTickets({
    ...filters,
    page,
    pageSize,
  });

  const handleFilterChange = (newFilters: TicketFiltersType) => {
    setFilters(newFilters);
    setPage(1);
  };

  const handleCreateTicket = () => {
    navigate('/client/tickets/new');
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
        <Button onClick={handleCreateTicket}>+ Nowe Zgłoszenie</Button>
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
    </div>
  );
};
