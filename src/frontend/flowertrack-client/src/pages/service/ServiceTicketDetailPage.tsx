/**
 * ServiceTicketDetailPage - Modern Redesign
 * A premium, grid-based layout with glassmorphism headers and contextual sidebars.
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

  // Edit state
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

  // Smart Action Logic
  const getSmartAction = (status: TicketStatus) => {
    switch (status) {
      case 'New':
        return { label: 'Przyjmij zgłoszenie', nextStatus: 'Accepted', icon: '📥' };
      case 'Accepted':
        return { label: 'Rozpocznij pracę', nextStatus: 'InProgress', icon: '▶️' };
      case 'InProgress':
        return { label: 'Rozwiąż zgłoszenie', nextStatus: 'Resolved', icon: '✅' };
      case 'Resolved':
        return { label: 'Zamknij zgłoszenie', nextStatus: 'Closed', icon: '🔒' };
      case 'Closed':
        return { label: 'Otwórz ponownie', nextStatus: 'Reopened', icon: '🔓' };
      default:
        return { label: 'Zmień status', nextStatus: null, icon: '🔄' };
    }
  };

  const handleSmartAction = () => {
    if (!ticket) return;
    const action = getSmartAction(ticket.status);

    // If it requires justification (Resolved/Closed) or manual selection, open modal
    if (action.nextStatus === 'Resolved' || action.nextStatus === 'Closed' || !action.nextStatus) {
      setIsStatusModalOpen(true);
    } else {
      // Direct update for simple transitions
      handleStatusChange(action.nextStatus as TicketStatus);
    }
  };

  const handleStatusChange = async (newStatus: TicketStatus, justification?: string) => {
    if (!id) return;
    try {
      await changeStatusMutation.mutateAsync({
        id,
        data: { newStatus, justification },
      });
      showToast(`Status zmieniony na ${newStatus}`, 'success');
      setIsStatusModalOpen(false);
    } catch {
      showToast('Nie udało się zmienić statusu', 'error');
    }
  };

  const handleAssignment = async (userId?: string) => {
    if (!id || !userId) return;
    try {
      await assignMutation.mutateAsync({
        id,
        data: { assignedToUserId: userId },
      });
      showToast('Technik przypisany', 'success');
      setIsAssignmentModalOpen(false);
    } catch {
      showToast('Błąd przypisywania', 'error');
    }
  };

  const handleSaveEdit = async () => {
    if (!id || !editTitle.trim() || !editDescription.trim()) return;
    try {
      await updateMutation.mutateAsync({
        id,
        data: { title: editTitle, description: editDescription, priority: editPriority },
      });
      showToast('Zgłoszenie zaktualizowane', 'success');
      setIsEditing(false);
    } catch {
      showToast('Błąd aktualizacji', 'error');
    }
  };

  const handleStartEdit = () => {
    if (!ticket) return;
    setEditTitle(ticket.title);
    setEditDescription(ticket.description);
    setEditPriority(ticket.priority);
    setIsEditing(true);
  };

  const formatSafeDate = (dateString?: string) => {
    if (!dateString) return 'Nieznana data';
    try {
      return formatDistanceToNow(new Date(dateString), { addSuffix: true, locale: pl });
    } catch {
      return 'Błąd daty';
    }
  };

  if (isLoadingTicket) {
    return (
      <div className={styles.loadingContainer}>
        <Loader size="lg" />
        <p>Szukam zgłoszenia...</p>
      </div>
    );
  }

  if (!ticket) {
    return (
      <div className={styles.errorContainer}>
        <h2>Nie znaleziono zgłoszenia</h2>
        <Button onClick={() => navigate('/service/tickets')}>Wróć do listy</Button>
      </div>
    );
  }

  const smartAction = getSmartAction(ticket.status);

  return (
    <div className={styles.page}>
      {/* --- HEADER --- */}
      <div className={styles.header}>
        <div className={styles.headerTop}>
          <div className={styles.headerLeft}>
            <button onClick={() => navigate('/service/tickets')} className={styles.backButton}>
              ← Powrót do listy
            </button>
            <div className={styles.titleWrapper}>
              <span className={styles.ticketNumber}>{ticket.ticketNumber}</span>
              {isEditing ? (
                <input
                  className={styles.input}
                  value={editTitle}
                  onChange={(e) => setEditTitle(e.target.value)}
                  style={{ fontSize: '1.5rem', fontWeight: 700, width: '100%' }}
                />
              ) : (
                <h1 className={styles.title}>{ticket.title}</h1>
              )}
            </div>
            <div className={styles.headerMeta}>
              <Badge variant={getStatusColor(ticket.status)}>{ticket.status}</Badge>
              <Badge variant={getPriorityColor(ticket.priority)}>{ticket.priority}</Badge>
              <span style={{ color: 'var(--color-text-tertiary)', fontSize: '0.9rem' }}>
                Utworzono {formatSafeDate(ticket.createdAt)} przez {ticket.createdByName}
              </span>
            </div>
          </div>
        </div>

        <div className={styles.actionBar}>
          <div className={styles.tabs}>
            <button
              className={`${styles.tab} ${activeTab === 'overview' ? styles.activeTab : ''}`}
              onClick={() => setActiveTab('overview')}
            >
              Przegląd
            </button>
            <button
              className={`${styles.tab} ${activeTab === 'timeline' ? styles.activeTab : ''}`}
              onClick={() => setActiveTab('timeline')}
            >
              Historia
            </button>
            <button
              className={`${styles.tab} ${activeTab === 'comments' ? styles.activeTab : ''}`}
              onClick={() => setActiveTab('comments')}
            >
              Komentarze
            </button>
            <button
              className={`${styles.tab} ${activeTab === 'attachments' ? styles.activeTab : ''}`}
              onClick={() => setActiveTab('attachments')}
            >
              Załączniki
            </button>
          </div>

          <div className={styles.headerActions}>
            {!isEditing ? (
              <>
                <Button variant="ghost" onClick={handleStartEdit}>
                  Edytuj
                </Button>
                <Button
                  variant="primary"
                  onClick={handleSmartAction}
                  className={styles.statusButton}
                >
                  {smartAction.icon} {smartAction.label}
                </Button>
              </>
            ) : (
              <>
                <Button variant="ghost" onClick={() => setIsEditing(false)}>
                  Anuluj
                </Button>
                <Button variant="primary" onClick={handleSaveEdit}>
                  Zapisz zmiany
                </Button>
              </>
            )}
          </div>
        </div>
      </div>

      {/* --- GRID LAYOUT --- */}
      <div className={styles.gridContainer}>
        {/* LEFT COLUMN: Main Content */}
        <div className={styles.contentArea}>
          {activeTab === 'overview' && (
            <div className={styles.sidebarCard} style={{ padding: 'var(--space-6)' }}>
              <h3 className={styles.infoLabel} style={{ marginBottom: 'var(--space-4)' }}>
                Opis Zgłoszenia
              </h3>
              {isEditing ? (
                <textarea
                  className={styles.textarea}
                  rows={8}
                  value={editDescription}
                  onChange={(e) => setEditDescription(e.target.value)}
                />
              ) : (
                <p style={{ lineHeight: 1.6, color: 'var(--color-text-primary)' }}>
                  {ticket.description}
                </p>
              )}

              <div
                style={{
                  marginTop: 'var(--space-6)',
                  paddingTop: 'var(--space-6)',
                  borderTop: '1px solid var(--color-border-subtle)',
                }}
              >
                <h3 className={styles.infoLabel} style={{ marginBottom: 'var(--space-4)' }}>
                  Najnowsza Aktywność
                </h3>
                {/* Show mini timeline here */}
                <Timeline
                  events={Array.isArray(history) ? history.slice(0, 3) : []}
                  isLoading={isLoadingHistory}
                />
                <div style={{ marginTop: 'var(--space-4)', textAlign: 'center' }}>
                  <Button variant="ghost" onClick={() => setActiveTab('timeline')}>
                    Zobacz pełną historię
                  </Button>
                </div>
              </div>
            </div>
          )}

          {activeTab === 'timeline' && (
            <div className={styles.sidebarCard} style={{ padding: 'var(--space-0)' }}>
              <div style={{ padding: 'var(--space-6)' }}>
                <Timeline events={history || []} isLoading={isLoadingHistory} />
              </div>
            </div>
          )}

          {activeTab === 'comments' && (
            <div className={styles.sidebarCard} style={{ padding: 'var(--space-6)' }}>
              <CommentThread ticketId={ticket.id} isServiceUser={true} />
            </div>
          )}

          {activeTab === 'attachments' && (
            <div className={styles.sidebarCard} style={{ padding: 'var(--space-6)' }}>
              <AttachmentGallery ticketId={ticket.id} />
            </div>
          )}
        </div>

        {/* RIGHT COLUMN: Context Sidebar */}
        <div className={styles.sidebar}>
          {/* Assignment Card */}
          <div className={styles.sidebarCard}>
            <div className={styles.sidebarHeader}>
              <span>👤 Przypisanie</span>
            </div>
            <div className={styles.sidebarContent}>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Technik</span>
                <div className={styles.infoValue} style={{ justifyContent: 'space-between' }}>
                  <span>{ticket.assignedToName || 'Nieprzypisane'}</span>
                  <Button size="sm" variant="ghost" onClick={() => setIsAssignmentModalOpen(true)}>
                    {ticket.assignedToName ? 'Zmień' : 'Przypisz'}
                  </Button>
                </div>
              </div>
            </div>
          </div>

          {/* Machine Card */}
          <div className={styles.sidebarCard}>
            <div className={styles.sidebarHeader}>
              <span>🏭 Maszyna</span>
            </div>
            <div className={styles.sidebarContent}>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Model</span>
                <span className={styles.infoValue}>
                  <Link to={`/service/machines/${ticket.machineId}`} className={styles.link}>
                    {ticket.machineModel}
                  </Link>
                </span>
              </div>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Numer Seryjny</span>
                <span className={styles.infoValue}>{ticket.machineSerialNumber}</span>
              </div>
            </div>
          </div>

          {/* Organization Card */}
          <div className={styles.sidebarCard}>
            <div className={styles.sidebarHeader}>
              <span>🏢 Klient</span>
            </div>
            <div className={styles.sidebarContent}>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Organizacja</span>
                <span className={styles.infoValue}>
                  <Link
                    to={`/service/organizations/${ticket.organizationId}`}
                    className={styles.link}
                  >
                    {ticket.organizationName}
                  </Link>
                </span>
              </div>
            </div>
          </div>

          {/* Dates Card */}
          <div className={styles.sidebarCard}>
            <div className={styles.sidebarHeader}>
              <span>📅 Daty</span>
            </div>
            <div className={styles.sidebarContent}>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Utworzono</span>
                <span className={styles.infoValue}>{formatSafeDate(ticket.createdAt)}</span>
              </div>
              {ticket.updatedAt && (
                <div className={styles.infoItem}>
                  <span className={styles.infoLabel}>Ostatnia zmiana</span>
                  <span className={styles.infoValue}>{formatSafeDate(ticket.updatedAt)}</span>
                </div>
              )}
              {ticket.resolvedAt && (
                <div className={styles.infoItem}>
                  <span className={styles.infoLabel}>Rozwiązano</span>
                  <span className={styles.infoValue}>{formatSafeDate(ticket.resolvedAt)}</span>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* --- MODALS --- */}
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
        serviceUsers={serviceUsersData || []}
        onConfirm={handleAssignment}
        isLoading={assignMutation.isPending}
        isFetchingUsers={isLoadingUsers}
      />
    </div>
  );
};
