/**
 * EditMachineModal - Modal for editing machine details (Service Portal)
 */

import React, { useState, useEffect } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { useMachineMutations } from '../../hooks/useMachines';
import type { MachineDto, UpdateMachineRequest, MachineStatus } from '../../types/api';
import './EditMachineModal.css';

interface EditMachineModalProps {
  isOpen: boolean;
  machine: MachineDto | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const EditMachineModal: React.FC<EditMachineModalProps> = ({
  isOpen,
  machine,
  onClose,
  onSuccess,
}) => {
  const { updateMachine, isUpdating } = useMachineMutations();

  const [formData, setFormData] = useState<UpdateMachineRequest & { status?: MachineStatus }>({
    model: '',
    serialNumber: '',
    location: '',
    installationDate: '',
    notes: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [formError, setFormError] = useState('');

  useEffect(() => {
    if (machine && isOpen) {
      setFormData({
        model: machine.model,
        serialNumber: machine.serialNumber,
        location: machine.location || '',
        installationDate: machine.installationDate || '',
        notes: machine.notes || '',
      });
      setErrors({});
      setFormError('');
    }
  }, [machine, isOpen]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (formData.model && !formData.model.trim()) {
      newErrors.model = 'Model cannot be empty';
    } else if (formData.model && formData.model.length > 100) {
      newErrors.model = 'Model cannot exceed 100 characters';
    }

    if (formData.location && formData.location.length > 200) {
      newErrors.location = 'Location cannot exceed 200 characters';
    }

    if (formData.notes && formData.notes.length > 1000) {
      newErrors.notes = 'Notes cannot exceed 1000 characters';
    }

    if (formData.installationDate) {
      const date = new Date(formData.installationDate);
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      if (date > today) {
        newErrors.installationDate = 'Installation date cannot be in the future';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError('');

    if (!validate() || !machine) {
      return;
    }

    try {
      await updateMachine.mutateAsync({
        id: machine.id,
        data: {
          model: formData.model,
          location: formData.location,
          installationDate: formData.installationDate,
          notes: formData.notes,
        },
      });
      onClose();
      onSuccess();
    } catch (error) {
      setFormError((error as Error).message || 'Failed to update machine');
    }
  };

  const handleClose = () => {
    setErrors({});
    setFormError('');
    onClose();
  };

  if (!machine) return null;

  return (
    <Modal
      isOpen={isOpen}
      onClose={handleClose}
      title={`Edit Machine - ${machine.serialNumber}`}
      size="lg"
      closeOnOverlayClick={false}
      footer={
        <div className="editMachine__footer">
          <Button variant="ghost" onClick={handleClose} disabled={isUpdating}>
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={isUpdating}>
            {isUpdating ? 'Saving...' : 'Save Changes'}
          </Button>
        </div>
      }
    >
      <form onSubmit={handleSubmit} className="editMachine__form">
        {formError && <div className="editMachine__error">{formError}</div>}

        <div className="editMachine__field">
          <Input label="Serial Number" type="text" value={machine.serialNumber} disabled readOnly />
          <span className="editMachine__helpText">Serial number cannot be changed</span>
        </div>

        <div className="editMachine__field">
          <Input
            label="Model"
            type="text"
            value={formData.model || ''}
            onChange={(e) => setFormData({ ...formData, model: e.target.value })}
            error={errors.model}
            disabled={isUpdating}
            placeholder="e.g., Model X-2000"
          />
        </div>

        <div className="editMachine__field">
          <Input
            label="Location"
            type="text"
            value={formData.location || ''}
            onChange={(e) => setFormData({ ...formData, location: e.target.value })}
            error={errors.location}
            disabled={isUpdating}
            placeholder="e.g., Building A, Floor 2"
          />
        </div>

        <div className="editMachine__field">
          <Input
            label="Installation Date"
            type="date"
            value={formData.installationDate || ''}
            onChange={(e) => setFormData({ ...formData, installationDate: e.target.value })}
            error={errors.installationDate}
            disabled={isUpdating}
            max={new Date().toISOString().split('T')[0]}
          />
        </div>

        <div className="editMachine__field">
          <label className="editMachine__label">Notes</label>
          <textarea
            value={formData.notes || ''}
            onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
            className={`editMachine__textarea ${errors.notes ? 'editMachine__textarea--error' : ''}`}
            disabled={isUpdating}
            placeholder="Add any relevant specifications or notes..."
            rows={4}
            maxLength={1000}
          />
          {errors.notes && <span className="editMachine__errorText">{errors.notes}</span>}
          <div className="editMachine__charCount">
            {formData.notes?.length || 0} / 1000 characters
          </div>
        </div>
      </form>
    </Modal>
  );
};
