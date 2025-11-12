/**
 * ClientTicketDetailPage - Detailed view of a ticket for client users
 * Simplified version with tabs for Overview, Comments, and Attachments
 */

import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { formatDistanceToNow } from 'date-fns';
import { pl } from 'date-fns/locale';
import { useTicket } from '../../hooks/useTickets';
import { CommentThread, AttachmentGallery } from '../../components/tickets';
import { Button } from '../../components/ui/Button';
import { Badge, getStatusColor, getPriorityColor } from '../../components/ui/Badge';
import { Card } from '../../components/ui/Card';
import { Loader } from '../../components/ui/Loader';
import styles from './ClientTicketDetailPage.module.css';

type TabType = 'overview' | 'comments' | 'attachments';

export const ClientTicketDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [activeTab, setActiveTab] = useState<TabType>('overview');

  const { data: ticket, isLoading: isLoadingTicket } = useTicket(id || '');

  if (isLoadingTicket) {
    return (
      <div className={styles.loadingContainer}>
        <Loader size="lg" />
        <p>Ładowanie zgłoszenia...</p>
      </div>
    );
  }

  if (!ticket) {
    return (
      <div className={styles.errorContainer}>
        <h2>Nie znaleziono zgłoszenia</h2>
        <Button onClick={() => navigate('/client/tickets')}>Powrót do listy zgłoszeń</Button>
      </div>
    );
  }

  return (
    <div className={styles.page}>
      {/* Header */}
      <div className={styles.header}>
        <div className={styles.headerTop}>
          <div className={styles.headerLeft}>
            <button onClick={() => navigate('/client/tickets')} className={styles.backButton}>
              ← Powrót
            </button>
            <h1 className={styles.title}>
              {ticket.ticketNumber} - {ticket.title}
            </h1>
          </div>
          <div className={styles.headerRight}>
            <Badge variant={getStatusColor(ticket.status)}>{ticket.status}</Badge>
            <Badge variant={getPriorityColor(ticket.priority)}>{ticket.priority}</Badge>
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className={styles.tabs}>
        <button
          className={`${styles.tab} ${activeTab === 'overview' ? styles.activeTab : ''}`}
          onClick={() => setActiveTab('overview')}
        >
          📋 Przegląd
        </button>
        <button
          className={`${styles.tab} ${activeTab === 'comments' ? styles.activeTab : ''}`}
          onClick={() => setActiveTab('comments')}
        >
          💬 Komentarze
        </button>
        <button
          className={`${styles.tab} ${activeTab === 'attachments' ? styles.activeTab : ''}`}
          onClick={() => setActiveTab('attachments')}
        >
          📎 Załączniki
        </button>
      </div>

      {/* Tab Content */}
      <div className={styles.content}>
        {activeTab === 'overview' && (
          <div className={styles.overview}>
            <div className={styles.mainInfo}>
              <Card>
                <h2 className={styles.sectionTitle}>Szczegóły zgłoszenia</h2>
                <div className={styles.infoRow}>
                  <span className={styles.label}>Opis:</span>
                  <span className={styles.value}>{ticket.description}</span>
                </div>
                <div className={styles.infoRow}>
                  <span className={styles.label}>Maszyna:</span>
                  <span className={styles.value}>
                    {ticket.machineModel} (S/N: {ticket.machineSerialNumber})
                  </span>
                </div>
                <div className={styles.infoRow}>
                  <span className={styles.label}>Utworzone:</span>
                  <span className={styles.value}>
                    {formatDistanceToNow(new Date(ticket.createdAt), {
                      addSuffix: true,
                      locale: pl,
                    })}
                  </span>
                </div>
                {ticket.assignedToName && (
                  <div className={styles.infoRow}>
                    <span className={styles.label}>Przypisany technik:</span>
                    <span className={styles.value}>{ticket.assignedToName}</span>
                  </div>
                )}
                {ticket.resolvedAt && (
                  <div className={styles.infoRow}>
                    <span className={styles.label}>Rozwiązane:</span>
                    <span className={styles.value}>
                      {formatDistanceToNow(new Date(ticket.resolvedAt), {
                        addSuffix: true,
                        locale: pl,
                      })}
                    </span>
                  </div>
                )}
                {ticket.closedAt && (
                  <div className={styles.infoRow}>
                    <span className={styles.label}>Zamknięte:</span>
                    <span className={styles.value}>
                      {formatDistanceToNow(new Date(ticket.closedAt), {
                        addSuffix: true,
                        locale: pl,
                      })}
                    </span>
                  </div>
                )}
              </Card>
            </div>
          </div>
        )}

        {activeTab === 'comments' && (
          <Card>
            <h2 className={styles.sectionTitle}>Komentarze</h2>
            <CommentThread ticketId={ticket.id} isServiceUser={false} />
          </Card>
        )}

        {activeTab === 'attachments' && (
          <Card>
            <h2 className={styles.sectionTitle}>Załączniki</h2>
            <AttachmentGallery ticketId={ticket.id} />
          </Card>
        )}
      </div>
    </div>
  );
};
