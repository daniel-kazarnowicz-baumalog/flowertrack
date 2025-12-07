/**
 * RegisterMachineModal - Modal for registering new machines (Service Portal)
 */

import React, { useState, useEffect } from 'react';
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
  /** Pre-selected organization ID. When provided, the organization selector is disabled. */
  organizationId?: string;
}

export const RegisterMachineModal: React.FC<RegisterMachineModalProps> = ({
  isOpen,
  onClose,
  onSuccess,
  organizationId: preselectedOrgId,
}) => {
  const { organizations, isLoading: loadingOrgs } = useOrganizations();
  const { createMachine, isCreating } = useMachineMutations();

  const [formData, setFormData] = useState<CreateMachineRequest>({
    organizationId: preselectedOrgId || '',
    serialNumber: '',
    brand: '',
    model: '',
    location: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [formError, setFormError] = useState('');

  // Update organizationId when preselectedOrgId changes
  useEffect(() => {
    if (preselectedOrgId) {
      setFormData((prev) => ({ ...prev, organizationId: preselectedOrgId }));
    }
  }, [preselectedOrgId]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.serialNumber.trim()) {
      newErrors.serialNumber = 'Serial number is required';
    } else if (formData.serialNumber.length > 255) {
      newErrors.serialNumber = 'Serial number cannot exceed 255 characters';
    }

    if (!formData.organizationId) {
      newErrors.organizationId = 'Organization is required';
    }

    if (formData.brand && formData.brand.length > 100) {
      newErrors.brand = 'Brand cannot exceed 100 characters';
    }

    if (formData.model && formData.model.length > 100) {
      newErrors.model = 'Model cannot exceed 100 characters';
    }

    if (formData.location && formData.location.length > 255) {
      newErrors.location = 'Location cannot exceed 255 characters';
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
      organizationId: preselectedOrgId || '',
      serialNumber: '',
      brand: '',
      model: '',
      location: '',
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
              label="Brand"
              type="text"
              value={formData.brand || ''}
              onChange={(e) => setFormData({ ...formData, brand: e.target.value })}
              error={errors.brand}
              disabled={isCreating}
              placeholder="e.g., Baumalog"
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
            disabled={isCreating || loadingOrgs || !!preselectedOrgId}
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

        <div className="registerMachine__grid">
          <div className="registerMachine__field">
            <Input
              label="Model"
              type="text"
              value={formData.model || ''}
              onChange={(e) => setFormData({ ...formData, model: e.target.value })}
              error={errors.model}
              disabled={isCreating}
              placeholder="e.g., Model X-2000"
            />
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
        </div>
      </form>
    </Modal>
  );
};
