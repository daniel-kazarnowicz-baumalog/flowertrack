/**
 * StatusChangeModal - Modal for changing ticket status
 * Requires justification for Resolved/Closed statuses
 */

import React, { useState, useEffect } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import type { TicketStatus } from '../../types/api';
import styles from './StatusChangeModal.module.css';

interface StatusChangeModalProps {
  isOpen: boolean;
  onClose: () => void;
  currentStatus: TicketStatus;
  onConfirm: (newStatus: TicketStatus, justification?: string) => void;
  isLoading?: boolean;
}

const STATUS_OPTIONS: TicketStatus[] = [
  'New',
  'Accepted',
  'InProgress',
  'Resolved',
  'Closed',
  'Reopened',
];

const STATUS_LABELS: Record<TicketStatus, string> = {
  New: 'Nowe',
  Accepted: 'Zaakceptowane',
  InProgress: 'W trakcie',
  Resolved: 'Rozwiązane',
  Closed: 'Zamknięte',
  Reopened: 'Ponownie otwarte',
};

const STATUS_ICONS: Record<TicketStatus, string> = {
  New: '📋',
  Accepted: '📥',
  InProgress: '🔧',
  Resolved: '✅',
  Closed: '🔒',
  Reopened: '🔓',
};

const STATUS_COLORS: Record<TicketStatus, string> = {
  New: 'new',
  Accepted: 'accepted',
  InProgress: 'inprogress',
  Resolved: 'resolved',
  Closed: 'closed',
  Reopened: 'reopened',
};

export const StatusChangeModal: React.FC<StatusChangeModalProps> = ({
  isOpen,
  onClose,
  currentStatus,
  onConfirm,
  isLoading = false,
}) => {
  const [newStatus, setNewStatus] = useState<TicketStatus>(currentStatus);
  const [justification, setJustification] = useState('');

  // Reset state when modal opens
  useEffect(() => {
    if (isOpen) {
      // Set to first available status that's not current
      const firstAvailable = STATUS_OPTIONS.find((s) => s !== currentStatus);
      setNewStatus(firstAvailable || currentStatus);
      setJustification('');
    }
  }, [isOpen, currentStatus]);

  const requiresJustification = newStatus === 'Resolved' || newStatus === 'Closed';

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (requiresJustification && !justification.trim()) {
      return;
    }

    onConfirm(newStatus, justification.trim() || undefined);
  };

  const handleClose = () => {
    setNewStatus(currentStatus);
    setJustification('');
    onClose();
  };

  const availableStatuses = STATUS_OPTIONS.filter((status) => status !== currentStatus);

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Zmień status zgłoszenia">
      <form onSubmit={handleSubmit} className={styles.form}>
        <div className={styles.field}>
          <label className={styles.label}>Nowy status</label>
          <div className={styles.statusGrid}>
            {availableStatuses.map((status) => (
              <label
                key={status}
                className={`${styles.statusOption} ${styles[`status--${STATUS_COLORS[status]}`]} ${
                  newStatus === status ? styles.statusOptionSelected : ''
                } ${isLoading ? styles.statusOptionDisabled : ''}`}
              >
                <input
                  type="radio"
                  name="status"
                  value={status}
                  checked={newStatus === status}
                  onChange={() => setNewStatus(status)}
                  disabled={isLoading}
                  className={styles.radioInput}
                />
                <span className={styles.statusIcon}>{STATUS_ICONS[status]}</span>
                <span className={styles.statusLabel}>{STATUS_LABELS[status]}</span>
              </label>
            ))}
          </div>
        </div>

        {requiresJustification && (
          <div className={styles.field}>
            <label htmlFor="justification" className={styles.label}>
              Uzasadnienie <span className={styles.required}>*</span>
            </label>
            <textarea
              id="justification"
              value={justification}
              onChange={(e) => setJustification(e.target.value)}
              placeholder="Opisz powód zmiany statusu..."
              disabled={isLoading}
              className={styles.textarea}
              rows={3}
            />
            {!justification.trim() && (
              <span className={styles.hint}>
                Uzasadnienie jest wymagane dla statusu „Rozwiązane" i „Zamknięte"
              </span>
            )}
          </div>
        )}

        <div className={styles.actions}>
          <Button type="button" variant="secondary" onClick={handleClose} disabled={isLoading}>
            Anuluj
          </Button>
          <Button
            type="submit"
            disabled={
              isLoading ||
              newStatus === currentStatus ||
              (requiresJustification && !justification.trim())
            }
          >
            {isLoading ? 'Zapisywanie...' : 'Zmień status'}
          </Button>
        </div>
      </form>
    </Modal>
  );
};
