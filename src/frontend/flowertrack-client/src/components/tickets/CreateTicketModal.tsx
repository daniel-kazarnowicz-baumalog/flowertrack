/**
 * CreateTicketModal - Modal for creating new tickets (Client Portal)
 */

import React, { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { Checkbox } from '../ui/Checkbox';
import { FileUploader } from '../ui/FileUploader';
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
  onSubmit: (data: CreateTicketRequest, files: File[]) => void;
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

  const [files, setFiles] = useState<File[]>([]);
  const [consent, setConsent] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.machineId) {
      newErrors.machineId = 'Wybierz maszynę';
    }
    if (!formData.title.trim()) {
      newErrors.title = 'Wpisz tytuł zgłoszenia';
    } else if (formData.title.length < 10) {
      newErrors.title = 'Tytuł musi mieć min. 10 znaków';
    }

    if (!formData.description.trim()) {
      newErrors.description = 'Wpisz opis problemu';
    } else if (formData.description.length < 20) {
      newErrors.description = 'Opis musi mieć min. 20 znaków';
    }

    if (!consent) {
      newErrors.consent = 'Musisz wyrazić zgodę na przetwarzanie danych';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) {
      return;
    }

    // Pass files along with the form data - note that parent component needs to handle this
    // We are extending the submit signature implicitly here, needs update in parent
    onSubmit(formData, files);
  };

  const handleClose = () => {
    setFormData({
      machineId: '',
      title: '',
      description: '',
      priority: 'Medium',
    });
    setFiles([]);
    setConsent(false);
    setErrors({});
    onClose();
  };

  const handleRemoveFile = (index: number) => {
    setFiles(prev => prev.filter((_, i) => i !== index));
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
            Tytuł (min. 10 znaków) <span className={styles.required}>*</span>
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
            Opis problemu (min. 20 znaków) <span className={styles.required}>*</span>
          </label>
          <textarea
            id="description"
            value={formData.description}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            placeholder="Szczegółowy opis problemu, objawów, okoliczności wystąpienia..."
            className={`${styles.select} ${errors.description ? styles.error : ''}`} // Reuse select styles for basic textarea
            style={{ minHeight: '120px', fontFamily: 'inherit' }}
            disabled={isLoading}
          />
          {errors.description && <span className={styles.errorMessage}>{errors.description}</span>}
        </div>

        {/* Attachments */}
        <div className={styles.field}>
          <label className={styles.label}>Załączniki (max 3 pliki, 10MB)</label>
          <FileUploader
            onFilesSelected={(newFiles) => setFiles([...files, ...newFiles].slice(0, 3))}
            maxFiles={3 - files.length}
            disabled={isLoading || files.length >= 3}
          />
          {files.length > 0 && (
            <div className={styles.fileList}>
              {files.map((file, idx) => (
                <div key={idx} className={styles.fileItem}>
                  <span className={styles.fileName}>{file.name} ({(file.size / 1024).toFixed(0)} KB)</span>
                  <button
                    type="button"
                    className={styles.removeFile}
                    onClick={() => handleRemoveFile(idx)}
                    disabled={isLoading}
                  >
                    ×
                  </button>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Consent */}
        <div className={styles.field}>
          <Checkbox
            label="Zgadzam się na przetwarzanie danych osobowych w celu realizacji zgłoszenia"
            checked={consent}
            onChange={(e) => setConsent(e.target.checked)}
            error={errors.consent}
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
