/**
 * CreateTicketModal - Modal for creating new tickets (Client Portal)
 */

import React, { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import type { CreateTicketRequest, TicketPriority } from '../../types/api';
import styles from './CreateTicketModal.module.css';

interface Machine {
  id: string;
  model: string;
  serialNumber: string;
}

interface CreateTicketModalProps {
  isOpen: boolean;
  onClose: () => void;
  machines: Machine[];
  onSubmit: (data: CreateTicketRequest) => void;
  isLoading?: boolean;
  isFetchingMachines?: boolean;
}

export const CreateTicketModal: React.FC<CreateTicketModalProps> = ({
  isOpen,
  onClose,
  machines,
  onSubmit,
  isLoading = false,
  isFetchingMachines = false,
}) => {
  const [formData, setFormData] = useState<CreateTicketRequest>({
    machineId: '',
    title: '',
    description: '',
    priority: 'Medium',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.machineId) {
      newErrors.machineId = 'Wybierz maszynę';
    }
    if (!formData.title.trim()) {
      newErrors.title = 'Wpisz tytuł zgłoszenia';
    }
    if (!formData.description.trim()) {
      newErrors.description = 'Wpisz opis problemu';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) {
      return;
    }

    onSubmit(formData);
  };

  const handleClose = () => {
    setFormData({
      machineId: '',
      title: '',
      description: '',
      priority: 'Medium',
    });
    setErrors({});
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Nowe zgłoszenie">
      <form onSubmit={handleSubmit} className={styles.form}>
        {/* Machine Selection */}
        <div className={styles.field}>
          <label htmlFor="machine" className={styles.label}>
            Maszyna <span className={styles.required}>*</span>
          </label>
          {isFetchingMachines ? (
            <div className={styles.loading}>Ładowanie maszyn...</div>
          ) : (
            <>
              <select
                id="machine"
                value={formData.machineId}
                onChange={(e) => setFormData({ ...formData, machineId: e.target.value })}
                className={`${styles.select} ${errors.machineId ? styles.error : ''}`}
                disabled={isLoading}
              >
                <option value="">Wybierz maszynę...</option>
                {machines.map((machine) => (
                  <option key={machine.id} value={machine.id}>
                    {machine.model} - S/N: {machine.serialNumber}
                  </option>
                ))}
              </select>
              {errors.machineId && <span className={styles.errorMessage}>{errors.machineId}</span>}
            </>
          )}
        </div>

        {/* Priority */}
        <div className={styles.field}>
          <label htmlFor="priority" className={styles.label}>
            Priorytet
          </label>
          <select
            id="priority"
            value={formData.priority}
            onChange={(e) =>
              setFormData({
                ...formData,
                priority: e.target.value as TicketPriority,
              })
            }
            className={styles.select}
            disabled={isLoading}
          >
            <option value="Low">Niski</option>
            <option value="Medium">Średni</option>
            <option value="High">Wysoki</option>
            <option value="Critical">Krytyczny</option>
          </select>
        </div>

        {/* Title */}
        <div className={styles.field}>
          <label htmlFor="title" className={styles.label}>
            Tytuł <span className={styles.required}>*</span>
          </label>
          <Input
            id="title"
            type="text"
            value={formData.title}
            onChange={(e) => setFormData({ ...formData, title: e.target.value })}
            placeholder="Krótki opis problemu..."
            error={errors.title}
            disabled={isLoading}
          />
        </div>

        {/* Description */}
        <div className={styles.field}>
          <label htmlFor="description" className={styles.label}>
            Opis problemu <span className={styles.required}>*</span>
          </label>
          <Input
            id="description"
            value={formData.description}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            placeholder="Szczegółowy opis problemu, objawów, okoliczności wystąpienia..."
            error={errors.description}
            disabled={isLoading}
          />
        </div>

        {/* Actions */}
        <div className={styles.actions}>
          <Button type="button" variant="secondary" onClick={handleClose} disabled={isLoading}>
            Anuluj
          </Button>
          <Button type="submit" disabled={isLoading || isFetchingMachines}>
            {isLoading ? 'Tworzenie...' : 'Utwórz zgłoszenie'}
          </Button>
        </div>
      </form>
    </Modal>
  );
};
