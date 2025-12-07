/**
 * ServiceTicketDetailPage - Detailed view of a ticket for service users
 * Includes tabs for Overview, Timeline, Comments, and Attachments
 */

import React, { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { formatDistanceToNow } from 'date-fns';
import { pl } from 'date-fns/locale';
import {
  useTicket,
  useUpdateTicket,
  useChangeTicketStatus,
  useAssignTicket,
  useTicketHistory,
} from '../../hooks/useTickets';
import { useServiceUsers } from '../../hooks/useServiceUsers';
import { useToast } from '../../hooks/useToast';
import {
  Timeline,
  CommentThread,
  AttachmentGallery,
  StatusChangeModal,
  AssignmentDropdown,
} from '../../components/tickets';
import { Button } from '../../components/ui/Button';
import { Badge, getStatusColor, getPriorityColor } from '../../components/ui/Badge';
import { Card } from '../../components/ui/Card';
import { Loader } from '../../components/ui/Loader';
import type { TicketStatus } from '../../types/api';
import styles from './ServiceTicketDetailPage.module.css';

type TabType = 'overview' | 'timeline' | 'comments' | 'attachments';

export const ServiceTicketDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();

  const [activeTab, setActiveTab] = useState<TabType>('overview');
  const [isStatusModalOpen, setIsStatusModalOpen] = useState(false);
  const [isAssignmentModalOpen, setIsAssignmentModalOpen] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editTitle, setEditTitle] = useState('');
  const [editDescription, setEditDescription] = useState('');
  const [editPriority, setEditPriority] = useState<'Low' | 'Medium' | 'High' | 'Critical'>(
    'Medium'
  );

  const { data: ticket, isLoading: isLoadingTicket } = useTicket(id || '');
  const { data: history, isLoading: isLoadingHistory } = useTicketHistory(id || '');
  const { data: serviceUsersData, isLoading: isLoadingUsers } = useServiceUsers();
  const updateMutation = useUpdateTicket();
  const changeStatusMutation = useChangeTicketStatus();
  const assignMutation = useAssignTicket();

  const handleStatusChange = async (newStatus: TicketStatus, justification?: string) => {
    if (!id) return;

    try {
      await changeStatusMutation.mutateAsync({
        id,
        data: {
          newStatus: newStatus as string,
          justification,
        },
      });
      showToast('Status zmieniony', 'success');
      setIsStatusModalOpen(false);
    } catch {
      showToast('Nie udało się zmienić statusu', 'error');
    }
  };

  const handleAssignment = async (userId?: string) => {
    if (!id) return;

    try {
      await assignMutation.mutateAsync({
        id,
        data: {
          serviceUserId: userId,
        },
      });
      showToast(userId ? 'Zgłoszenie przypisane' : 'Przypisanie cofnięte', 'success');
      setIsAssignmentModalOpen(false);
    } catch {
      showToast('Nie udało się przypisać zgłoszenia', 'error');
    }
  };

  const handleStartEdit = () => {
    if (!ticket) return;
    setEditTitle(ticket.title);
    setEditDescription(ticket.description);
    setEditPriority(ticket.priority);
    setIsEditing(true);
  };

  const handleCancelEdit = () => {
    setIsEditing(false);
    setEditTitle('');
    setEditDescription('');
  };

  const handleSaveEdit = async () => {
    if (!id || !editTitle.trim() || !editDescription.trim()) {
      showToast('Tytuł i opis nie mogą być puste', 'error');
      return;
    }

    try {
      await updateMutation.mutateAsync({
        id,
        data: {
          title: editTitle,
          description: editDescription,
          priority: editPriority,
        },
      });
      showToast('Zgłoszenie zaktualizowane', 'success');
      setIsEditing(false);
    } catch {
      showToast('Nie udało się zaktualizować zgłoszenia', 'error');
    }
  };

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
        <Button onClick={() => navigate('/service/tickets')}>Powrót do listy zgłoszeń</Button>
      </div>
    );
  }

  return (
    <div className={styles.page}>
      {/* Header */}
      <div className={styles.header}>
        <div className={styles.headerTop}>
          <div className={styles.headerLeft}>
            <button onClick={() => navigate('/service/tickets')} className={styles.backButton}>
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

        <div className={styles.headerActions}>
          {!isEditing ? (
            <>
              <Button variant="secondary" onClick={handleStartEdit}>
                ✏️ Edytuj
              </Button>
              <Button variant="secondary" onClick={() => setIsStatusModalOpen(true)}>
                🔄 Zmień status
              </Button>
              <Button variant="primary" onClick={() => setIsAssignmentModalOpen(true)}>
                👤 Przypisz technika
              </Button>
            </>
          ) : (
            <>
              <Button variant="secondary" onClick={handleCancelEdit}>
                Anuluj
              </Button>
              <Button
                variant="primary"
                onClick={handleSaveEdit}
                disabled={updateMutation.isPending}
              >
                {updateMutation.isPending ? 'Zapisywanie...' : 'Zapisz zmiany'}
              </Button>
            </>
          )}
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
          className={`${styles.tab} ${activeTab === 'timeline' ? styles.activeTab : ''}`}
          onClick={() => setActiveTab('timeline')}
        >
          📅 Historia
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
                {isEditing ? (
                  <div className={styles.editForm}>
                    <div className={styles.formField}>
                      <label>Tytuł</label>
                      <input
                        type="text"
                        value={editTitle}
                        onChange={(e) => setEditTitle(e.target.value)}
                        className={styles.input}
                      />
                    </div>
                    <div className={styles.formField}>
                      <label>Opis</label>
                      <textarea
                        value={editDescription}
                        onChange={(e) => setEditDescription(e.target.value)}
                        rows={6}
                        className={styles.textarea}
                      />
                    </div>
                    <div className={styles.formField}>
                      <label>Priorytet</label>
                      <select
                        value={editPriority}
                        onChange={(e) => setEditPriority(e.target.value as typeof editPriority)}
                        className={styles.select}
                      >
                        <option value="Low">Niski</option>
                        <option value="Medium">Średni</option>
                        <option value="High">Wysoki</option>
                        <option value="Critical">Krytyczny</option>
                      </select>
                    </div>
                  </div>
                ) : (
                  <>
                    <div className={styles.infoRow}>
                      <span className={styles.label}>Opis:</span>
                      <span className={styles.value}>{ticket.description}</span>
                    </div>
                    <div className={styles.infoRow}>
                      <span className={styles.label}>Organizacja:</span>
                      <span className={styles.value}>
                        <Link
                          to={`/service/organizations/${ticket.organizationId}`}
                          className={styles.link}
                        >
                          {ticket.organizationName}
                        </Link>
                      </span>
                    </div>
                    <div className={styles.infoRow}>
                      <span className={styles.label}>Maszyna:</span>
                      <span className={styles.value}>
                        <Link to={`/service/machines/${ticket.machineId}`} className={styles.link}>
                          {ticket.machineModel} (S/N: {ticket.machineSerialNumber})
                        </Link>
                      </span>
                    </div>
                    <div className={styles.infoRow}>
                      <span className={styles.label}>Utworzone przez:</span>
                      <span className={styles.value}>{ticket.createdByName}</span>
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
                        <span className={styles.label}>Przypisane do:</span>
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
                  </>
                )}
              </Card>
            </div>
          </div>
        )}

        {activeTab === 'timeline' && (
          <Card>
            <h2 className={styles.sectionTitle}>Historia zmian</h2>
            <Timeline events={history || []} isLoading={isLoadingHistory} />
          </Card>
        )}

        {activeTab === 'comments' && (
          <Card>
            <h2 className={styles.sectionTitle}>Komentarze i notatki</h2>
            <CommentThread ticketId={ticket.id} isServiceUser={true} />
          </Card>
        )}

        {activeTab === 'attachments' && (
          <Card>
            <h2 className={styles.sectionTitle}>Załączniki</h2>
            <AttachmentGallery ticketId={ticket.id} />
          </Card>
        )}
      </div>

      {/* Modals */}
      <StatusChangeModal
        isOpen={isStatusModalOpen}
        onClose={() => setIsStatusModalOpen(false)}
        currentStatus={ticket.status}
        onConfirm={handleStatusChange}
        isLoading={changeStatusMutation.isPending}
      />

      <AssignmentDropdown
        isOpen={isAssignmentModalOpen}
        onClose={() => setIsAssignmentModalOpen(false)}
        currentAssigneeId={ticket.assignedToId}
        serviceUsers={serviceUsersData?.items || []}
        onConfirm={handleAssignment}
        isLoading={assignMutation.isPending}
        isFetchingUsers={isLoadingUsers}
      />
    </div>
  );
};
