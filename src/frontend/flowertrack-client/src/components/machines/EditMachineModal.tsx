import { useState, useEffect } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import type { UpdateMachineRequest, MachineDto } from '../../types/api';
import './EditMachineModal.css';

interface EditMachineModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: UpdateMachineRequest) => void;
  machine: MachineDto | null;
  isLoading?: boolean;
}

/**
 * EditMachineModal - Modal for editing machine details
 * Pre-populates form with current machine data
 */
export const EditMachineModal = ({
  isOpen,
  onClose,
  onSubmit,
  machine,
  isLoading = false,
}: EditMachineModalProps) => {
  const [formData, setFormData] = useState<UpdateMachineRequest>({
    model: '',
    serialNumber: '',
    location: '',
    installationDate: '',
    notes: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  // Pre-populate form when machine changes
  useEffect(() => {
    if (machine) {
      setFormData({
        model: machine.model,
        serialNumber: machine.serialNumber,
        location: machine.location || '',
        installationDate: machine.installationDate ? machine.installationDate.split('T')[0] : '',
        notes: machine.notes || '',
      });
      setErrors({});
    }
  }, [machine]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    // Required fields
    if (!formData.model?.trim()) {
      newErrors.model = 'Model is required';
    }

    if (!formData.serialNumber?.trim()) {
      newErrors.serialNumber = 'Serial number is required';
    }

    // Date validation
    if (formData.installationDate) {
      const date = new Date(formData.installationDate);
      const today = new Date();
      if (date > today) {
        newErrors.installationDate = 'Installation date cannot be in the future';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) {
      return;
    }

    // Remove empty optional fields, but keep required fields
    const submitData: UpdateMachineRequest = {};

    if (formData.model?.trim()) {
      submitData.model = formData.model.trim();
    }
    if (formData.serialNumber?.trim()) {
      submitData.serialNumber = formData.serialNumber.trim();
    }
    if (formData.location?.trim()) {
      submitData.location = formData.location.trim();
    }
    if (formData.installationDate?.trim()) {
      submitData.installationDate = formData.installationDate.trim();
    }
    if (formData.notes?.trim()) {
      submitData.notes = formData.notes.trim();
    }

    onSubmit(submitData);
  };

  const handleClose = () => {
    setErrors({});
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Edit Machine" size="lg">
      <form onSubmit={handleSubmit} className="editMachineModal">
        <div className="editMachineModal__section">
          <h3 className="editMachineModal__sectionTitle">Machine Information</h3>

          <div className="editMachineModal__field">
            <label htmlFor="serialNumber" className="editMachineModal__label">
              Serial Number <span className="editMachineModal__required">*</span>
            </label>
            <Input
              id="serialNumber"
              type="text"
              value={formData.serialNumber}
              onChange={(e) =>
                setFormData({ ...formData, serialNumber: e.target.value.toUpperCase() })
              }
              error={errors.serialNumber}
              disabled={isLoading}
              placeholder="e.g., BML-2024-001"
            />
          </div>

          <div className="editMachineModal__field">
            <label htmlFor="model" className="editMachineModal__label">
              Model <span className="editMachineModal__required">*</span>
            </label>
            <Input
              id="model"
              type="text"
              value={formData.model}
              onChange={(e) => setFormData({ ...formData, model: e.target.value })}
              error={errors.model}
              disabled={isLoading}
              placeholder="e.g., Baumalog X-500"
            />
          </div>

          <div className="editMachineModal__field">
            <label htmlFor="location" className="editMachineModal__label">
              Location
            </label>
            <Input
              id="location"
              type="text"
              value={formData.location}
              onChange={(e) => setFormData({ ...formData, location: e.target.value })}
              error={errors.location}
              disabled={isLoading}
              placeholder="e.g., Building A, Floor 2"
            />
          </div>

          <div className="editMachineModal__field">
            <label htmlFor="installationDate" className="editMachineModal__label">
              Installation Date
            </label>
            <Input
              id="installationDate"
              type="date"
              value={formData.installationDate}
              onChange={(e) => setFormData({ ...formData, installationDate: e.target.value })}
              error={errors.installationDate}
              disabled={isLoading}
            />
          </div>

          <div className="editMachineModal__field">
            <label htmlFor="notes" className="editMachineModal__label">
              Notes
            </label>
            <textarea
              id="notes"
              value={formData.notes}
              onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
              className={`editMachineModal__textarea ${errors.notes ? 'editMachineModal__textarea--error' : ''}`}
              disabled={isLoading}
              placeholder="Additional information about the machine..."
              rows={4}
            />
            {errors.notes && <span className="editMachineModal__error">{errors.notes}</span>}
          </div>
        </div>

        <div className="editMachineModal__footer">
          <Button type="button" variant="secondary" onClick={handleClose} disabled={isLoading}>
            Cancel
          </Button>
          <Button type="submit" variant="primary" disabled={isLoading}>
            {isLoading ? 'Saving...' : 'Save Changes'}
          </Button>
        </div>
      </form>
    </Modal>
  );
};
