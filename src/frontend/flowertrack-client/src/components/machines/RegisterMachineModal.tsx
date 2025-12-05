/**
 * RegisterMachineModal - Modal for registering new machines (Service Portal)
 */

import React, { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { useOrganizations } from '../../hooks/useOrganizations';
import { useMachineMutations } from '../../hooks/useMachines';
import type { CreateMachineRequest, MachineDto } from '../../types/api';
import './RegisterMachineModal.css';

interface RegisterMachineModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: (machine: MachineDto) => void;
}

export const RegisterMachineModal: React.FC<RegisterMachineModalProps> = ({
  isOpen,
  onClose,
  onSuccess,
}) => {
  const { organizations, isLoading: loadingOrgs } = useOrganizations();
  const { createMachine, isCreating } = useMachineMutations();

  const [formData, setFormData] = useState<CreateMachineRequest>({
    organizationId: '',
    serialNumber: '',
    model: '',
    installationDate: '',
    location: '',
    notes: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [formError, setFormError] = useState('');

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.serialNumber.trim()) {
      newErrors.serialNumber = 'Serial number is required';
    } else if (formData.serialNumber.length > 100) {
      newErrors.serialNumber = 'Serial number cannot exceed 100 characters';
    }

    if (!formData.model.trim()) {
      newErrors.model = 'Model is required';
    } else if (formData.model.length > 100) {
      newErrors.model = 'Model cannot exceed 100 characters';
    }

    if (!formData.organizationId) {
      newErrors.organizationId = 'Organization is required';
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

    if (!validate()) {
      return;
    }

    try {
      const machine = await createMachine.mutateAsync(formData);
      handleClose();
      onSuccess(machine);
    } catch (error) {
      setFormError((error as Error).message || 'Failed to register machine');
    }
  };

  const handleClose = () => {
    setFormData({
      organizationId: '',
      serialNumber: '',
      model: '',
      installationDate: '',
      location: '',
      notes: '',
    });
    setErrors({});
    setFormError('');
    onClose();
  };

  const activeOrgs = organizations?.filter((org) => org.name) || [];

  return (
    <Modal
      isOpen={isOpen}
      onClose={handleClose}
      title="Register New Machine"
      size="lg"
      closeOnOverlayClick={false}
      footer={
        <div className="registerMachine__footer">
          <Button variant="ghost" onClick={handleClose} disabled={isCreating}>
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={isCreating || loadingOrgs}>
            {isCreating ? 'Registering...' : 'Register Machine'}
          </Button>
        </div>
      }
    >
      <form onSubmit={handleSubmit} className="registerMachine__form">
        {formError && <div className="registerMachine__error">{formError}</div>}

        <div className="registerMachine__grid">
          <div className="registerMachine__field">
            <Input
              label="Serial Number"
              type="text"
              value={formData.serialNumber}
              onChange={(e) => setFormData({ ...formData, serialNumber: e.target.value })}
              error={errors.serialNumber}
              required
              disabled={isCreating}
              placeholder="e.g., SN-123456"
            />
          </div>

          <div className="registerMachine__field">
            <Input
              label="Model"
              type="text"
              value={formData.model}
              onChange={(e) => setFormData({ ...formData, model: e.target.value })}
              error={errors.model}
              required
              disabled={isCreating}
              placeholder="e.g., Model X-2000"
            />
          </div>
        </div>

        <div className="registerMachine__field">
          <label className="registerMachine__label">
            Organization <span className="registerMachine__required">*</span>
          </label>
          <select
            value={formData.organizationId}
            onChange={(e) => setFormData({ ...formData, organizationId: e.target.value })}
            className={`registerMachine__select ${errors.organizationId ? 'registerMachine__select--error' : ''}`}
            disabled={isCreating || loadingOrgs}
            required
          >
            <option value="">Select an organization</option>
            {activeOrgs.map((org) => (
              <option key={org.id} value={org.id}>
                {org.name}
              </option>
            ))}
          </select>
          {errors.organizationId && (
            <span className="registerMachine__errorText">{errors.organizationId}</span>
          )}
        </div>

        <div className="registerMachine__field">
          <Input
            label="Location"
            type="text"
            value={formData.location || ''}
            onChange={(e) => setFormData({ ...formData, location: e.target.value })}
            error={errors.location}
            disabled={isCreating}
            placeholder="e.g., Building A, Floor 2"
          />
        </div>

        <div className="registerMachine__field">
          <Input
            label="Installation Date"
            type="date"
            value={formData.installationDate || ''}
            onChange={(e) => setFormData({ ...formData, installationDate: e.target.value })}
            error={errors.installationDate}
            disabled={isCreating}
            max={new Date().toISOString().split('T')[0]}
          />
        </div>

        <div className="registerMachine__field">
          <label className="registerMachine__label">Notes</label>
          <textarea
            value={formData.notes || ''}
            onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
            className={`registerMachine__textarea ${errors.notes ? 'registerMachine__textarea--error' : ''}`}
            disabled={isCreating}
            placeholder="Add any relevant specifications or notes..."
            rows={4}
            maxLength={1000}
          />
          {errors.notes && <span className="registerMachine__errorText">{errors.notes}</span>}
          <div className="registerMachine__charCount">
            {formData.notes?.length || 0} / 1000 characters
          </div>
        </div>
      </form>
    </Modal>
  );
};
