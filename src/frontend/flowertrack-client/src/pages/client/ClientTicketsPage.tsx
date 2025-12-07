import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../../components/ui/Button';
import { Loader, Card } from '../../components/ui';
import { useTickets, useCreateTicket } from '../../hooks/useTickets';
import { useToast } from '../../hooks/useToast';
import './ClientTicketsPage.css';

export const ClientTicketsPage: React.FC = () => {
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [page, setPage] = useState(1);
  const pageSize = 20;

  const { data, isLoading, error } = useTickets({
    page,
    pageSize,
  });

  const createTicketMutation = useCreateTicket();

  const handleNewTicket = () => {
    // TODO: Open create ticket modal
    showToast('Funkcja tworzenia zgłoszenia w przygotowaniu', 'info');
  };

  if (error) {
    return (
      <div className="clientTicketsPage">
        <div className="clientTicketsPage__error">
          <h2>Błąd ładowania zgłoszeń</h2>
          <p>{(error as Error).message}</p>
          <Button onClick={() => window.location.reload()}>Odśwież stronę</Button>
        </div>
      </div>
    );
  }

  return (
    <div className="clientTicketsPage">
      <div className="clientTicketsPage__header">
        <h1>
          <span>🎫</span>
          Moje Zgłoszenia
        </h1>
        <Button
          variant="primary"
          onClick={handleNewTicket}
          disabled={createTicketMutation.isPending}
        >
          <span>+</span>
          Nowe Zgłoszenie
        </Button>
      </div>

      <div className="clientTicketsPage__content">
        {isLoading ? (
          <div className="clientTicketsPage__loader">
            <Loader size="lg" />
          </div>
        ) : data?.items && data.items.length > 0 ? (
          <div className="clientTicketsPage__list">
            {data.items.map((ticket) => (
              <Card
                key={ticket.id}
                hoverable
                onClick={() => navigate(`/client/tickets/${ticket.id}`)}
              >
                <div className="ticketCard">
                  <div className="ticketCard__header">
                    <span className="ticketCard__number">{ticket.ticketNumber}</span>
                    <span
                      className={`ticketCard__priority ticketCard__priority--${ticket.priority?.toLowerCase()}`}
                    >
                      {ticket.priority}
                    </span>
                  </div>
                  <h3 className="ticketCard__title">{ticket.title}</h3>
                  <p className="ticketCard__description">{ticket.description}</p>
                  <div className="ticketCard__footer">
                    <span
                      className={`ticketCard__status ticketCard__status--${ticket.status?.toLowerCase().replace(' ', '-')}`}
                    >
                      {ticket.status}
                    </span>
                    <span className="ticketCard__date">
                      {new Date(ticket.createdAt).toLocaleDateString('pl-PL')}
                    </span>
                  </div>
                </div>
              </Card>
            ))}
          </div>
        ) : (
          <div className="clientTicketsPage__empty">
            <span className="clientTicketsPage__emptyIcon">📭</span>
            <h3>Brak zgłoszeń</h3>
            <p>Nie masz jeszcze żadnych zgłoszeń serwisowych</p>
            <Button variant="primary" onClick={handleNewTicket}>
              Utwórz pierwsze zgłoszenie
            </Button>
          </div>
        )}

        {data && data.totalPages > 1 && (
          <div className="clientTicketsPage__pagination">
            <Button variant="ghost" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}>
              ← Poprzednia
            </Button>
            <span>
              Strona {page} z {data.totalPages}
            </span>
            <Button
              variant="ghost"
              disabled={page >= data.totalPages}
              onClick={() => setPage((p) => p + 1)}
            >
              Następna →
            </Button>
          </div>
        )}
      </div>
    </div>
  );
};
