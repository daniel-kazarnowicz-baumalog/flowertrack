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
    email: '',
    phone: '',
    address: '',
    city: '',
    postalCode: '',
    country: '',
    notes: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (organization && isOpen) {
      setFormData({
        name: organization.name,
        email: organization.contactEmail,
        phone: organization.contactPhone || '',
        address: organization.address || '',
        city: organization.city || '',
        postalCode: organization.postalCode || '',
        country: organization.country || '',
        notes: organization.notes || '',
      });
    }
  }, [organization, isOpen]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (formData.name && !formData.name.trim()) {
      newErrors.name = 'Organization name cannot be empty';
    }
    if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Invalid email format';
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
      size="lg"
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
        <div className="editOrg__section">
          <h3>Organization Details</h3>
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
        </div>

        <div className="editOrg__section">
          <h3>Contact Information</h3>
          <div className="editOrg__grid">
            <div className="editOrg__field">
              <Input
                label="Contact Email"
                type="email"
                value={formData.email || ''}
                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                error={errors.email}
                disabled={isLoading}
              />
            </div>
            <div className="editOrg__field">
              <Input
                label="Contact Phone"
                type="tel"
                value={formData.phone || ''}
                onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                disabled={isLoading}
              />
            </div>
          </div>
        </div>

        <div className="editOrg__section">
          <h3>Address</h3>
          <div className="editOrg__field">
            <Input
              label="Street Address"
              type="text"
              value={formData.address || ''}
              onChange={(e) => setFormData({ ...formData, address: e.target.value })}
              disabled={isLoading}
            />
          </div>
          <div className="editOrg__grid">
            <div className="editOrg__field">
              <Input
                label="City"
                type="text"
                value={formData.city || ''}
                onChange={(e) => setFormData({ ...formData, city: e.target.value })}
                disabled={isLoading}
              />
            </div>
            <div className="editOrg__field">
              <Input
                label="Postal Code"
                type="text"
                value={formData.postalCode || ''}
                onChange={(e) => setFormData({ ...formData, postalCode: e.target.value })}
                disabled={isLoading}
              />
            </div>
          </div>
          <div className="editOrg__field">
            <Input
              label="Country"
              type="text"
              value={formData.country || ''}
              onChange={(e) => setFormData({ ...formData, country: e.target.value })}
              disabled={isLoading}
            />
          </div>
        </div>

        <div className="editOrg__section">
          <h3>Additional Information</h3>
          <div className="editOrg__field">
            <label className="editOrg__label">Notes</label>
            <textarea
              className="editOrg__textarea"
              value={formData.notes || ''}
              onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
              disabled={isLoading}
              rows={4}
              placeholder="Additional notes about this organization..."
            />
          </div>
        </div>
      </form>
    </Modal>
  );
};
