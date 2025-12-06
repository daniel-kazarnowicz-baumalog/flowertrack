import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { TicketFilters, TicketTable, Pagination } from '../../components/tickets';
import { Button } from '../../components/ui/Button';
import { useTickets } from '../../hooks/useTickets';
import type { TicketFilters as TicketFiltersType } from '../../types/api';
import { useToast } from '../../hooks/useToast';
import './ServiceTicketsPage.css';

export const ServiceTicketsPage: React.FC = () => {
  const { t } = useTranslation();
  const { showToast } = useToast();
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
    setPage(1); // Reset to first page on filter change
  };

  const handleBulkAction = (_ticketIds: string[], action: 'assign' | 'status') => {
    if (action === 'assign') {
      showToast(t('tickets.bulkAssignComingSoon', 'Masowe przypisywanie będzie dostępne wkrótce'), 'info');
    } else if (action === 'status') {
      showToast(t('tickets.bulkStatusComingSoon', 'Masowa zmiana statusu będzie dostępna wkrótce'), 'info');
    }
    // TODO: Implement bulk action modals
  };

  if (error) {
    return (
      <div className="serviceTicketsPage">
        <div className="serviceTicketsPage__error">
          <h2>{t('errors.loadingFailed')}</h2>
          <p>{error.message}</p>
          <Button onClick={() => window.location.reload()}>{t('common.refresh', 'Odśwież stronę')}</Button>
        </div>
      </div>
    );
  }

  return (
    <div className="serviceTicketsPage">
      <div className="serviceTicketsPage__header">
        <h1>{t('tickets.title')}</h1>
        <div className="serviceTicketsPage__stats">
          {data && (
            <span className="serviceTicketsPage__count">
              {t('tickets.totalCount', 'Wszystkich zgłoszeń')}: <strong>{data.totalCount}</strong>
            </span>
          )}
        </div>
      </div>

      <TicketFilters
        onFilterChange={handleFilterChange}
        showOrganizationFilter={true}
        showAssigneeFilter={true}
        // TODO: Fetch organizations and assignees from API
        organizations={[]}
        assignees={[]}
      />

      <TicketTable
        tickets={data?.items || []}
        isLoading={isLoading}
        onBulkAction={handleBulkAction}
        showOrganization={true}
        showAssignee={true}
        basePath="/service/tickets"
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
