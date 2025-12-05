/**
 * EditOrganizationModal - Modal for editing organization details (Service Portal)
 */

import React, { useState, useEffect } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import type { OrganizationDto, UpdateOrganizationRequest } from '../../types/api';
import './EditOrganizationModal.css';

interface EditOrganizationModalProps {
  isOpen: boolean;
  onClose: () => void;
  organization: OrganizationDto | null;
  onSubmit: (data: UpdateOrganizationRequest) => void;
  isLoading?: boolean;
}

export const EditOrganizationModal: React.FC<EditOrganizationModalProps> = ({
  isOpen,
  onClose,
  organization,
  onSubmit,
  isLoading = false,
}) => {
  const [formData, setFormData] = useState<UpdateOrganizationRequest>({
    name: '',
    contactEmail: '',
    contactPhone: '',
    address: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (organization && isOpen) {
      setFormData({
        name: organization.name,
        contactEmail: organization.contactEmail,
        contactPhone: organization.contactPhone || '',
        address: organization.address || '',
      });
    }
  }, [organization, isOpen]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (formData.name && !formData.name.trim()) {
      newErrors.name = 'Organization name cannot be empty';
    }
    if (formData.contactEmail && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.contactEmail)) {
      newErrors.contactEmail = 'Invalid email format';
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
    setErrors({});
    onClose();
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={handleClose}
      title={`Edit Organization - ${organization?.name || ''}`}
      size="md"
      footer={
        <div className="editOrg__footer">
          <Button variant="ghost" onClick={handleClose} disabled={isLoading}>
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={isLoading}>
            {isLoading ? 'Saving...' : 'Save Changes'}
          </Button>
        </div>
      }
    >
      <form onSubmit={handleSubmit} className="editOrg__form">
        <div className="editOrg__field">
          <Input
            label="Organization Name"
            type="text"
            value={formData.name || ''}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            error={errors.name}
            disabled={isLoading}
          />
        </div>
        <div className="editOrg__field">
          <Input
            label="Contact Email"
            type="email"
            value={formData.contactEmail || ''}
            onChange={(e) => setFormData({ ...formData, contactEmail: e.target.value })}
            error={errors.contactEmail}
            disabled={isLoading}
          />
        </div>
        <div className="editOrg__field">
          <Input
            label="Contact Phone"
            type="tel"
            value={formData.contactPhone || ''}
            onChange={(e) => setFormData({ ...formData, contactPhone: e.target.value })}
            disabled={isLoading}
          />
        </div>
        <div className="editOrg__field">
          <Input
            label="Address"
            type="text"
            value={formData.address || ''}
            onChange={(e) => setFormData({ ...formData, address: e.target.value })}
            disabled={isLoading}
          />
        </div>
      </form>
    </Modal>
  );
};
