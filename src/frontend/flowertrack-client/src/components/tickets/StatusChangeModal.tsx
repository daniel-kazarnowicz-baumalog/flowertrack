/**
 * StatusChangeModal - Modal for changing ticket status
 * Requires justification for Resolved/Closed statuses
 */

import React, { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
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

export const StatusChangeModal: React.FC<StatusChangeModalProps> = ({
  isOpen,
  onClose,
  currentStatus,
  onConfirm,
  isLoading = false,
}) => {
  const [newStatus, setNewStatus] = useState<TicketStatus>(currentStatus);
  const [justification, setJustification] = useState('');

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

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Zmień status zgłoszenia">
      <form onSubmit={handleSubmit} className={styles.form}>
        <div className={styles.field}>
          <label htmlFor="status" className={styles.label}>
            Nowy status
          </label>
          <select
            id="status"
            value={newStatus}
            onChange={(e) => setNewStatus(e.target.value as TicketStatus)}
            className={styles.select}
            disabled={isLoading}
          >
            {STATUS_OPTIONS.filter((status) => status !== currentStatus).map((status) => (
              <option key={status} value={status}>
                {STATUS_LABELS[status]}
              </option>
            ))}
          </select>
        </div>

        {requiresJustification && (
          <div className={styles.field}>
            <label htmlFor="justification" className={styles.label}>
              Uzasadnienie <span className={styles.required}>*</span>
            </label>
            <Input
              id="justification"
              value={justification}
              onChange={(e) => setJustification(e.target.value)}
              placeholder="Uzasadnienie zmiany statusu (wymagane dla Rozwiązane/Zamknięte)..."
              disabled={isLoading}
            />
            {!justification.trim() && (
              <span className={styles.hint}>
                Uzasadnienie jest wymagane dla statusu "Rozwiązane" i "Zamknięte"
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
