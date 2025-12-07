/**
 * OnboardOrganizationModal - Modal for onboarding new organizations (Service Portal)
 */

import React, { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import type { OnboardOrganizationRequest } from '../../types/api';
import './OnboardOrganizationModal.css';

interface OnboardOrganizationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: OnboardOrganizationRequest) => Promise<void> | void;
  isLoading?: boolean;
}

export const OnboardOrganizationModal: React.FC<OnboardOrganizationModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  isLoading = false,
}) => {
  const [formData, setFormData] = useState<OnboardOrganizationRequest>({
    name: '',
    adminEmail: '',
    adminFirstName: '',
    adminLastName: '',
    phone: '',
    address: '',
    city: '',
    postalCode: '',
    country: '',
    notes: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Organization name is required';
    } else if (formData.name.length < 3) {
      newErrors.name = 'Organization name must be at least 3 characters';
    }
    if (!formData.adminEmail.trim()) {
      newErrors.adminEmail = 'Admin email is required';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.adminEmail)) {
      newErrors.adminEmail = 'Invalid email format';
    }
    if (!formData.adminFirstName.trim()) {
      newErrors.adminFirstName = 'Admin first name is required';
    }
    if (!formData.adminLastName.trim()) {
      newErrors.adminLastName = 'Admin last name is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) {
      return;
    }

    await onSubmit(formData);
  };

  const handleClose = () => {
    setFormData({
      name: '',
      adminEmail: '',
      adminFirstName: '',
      adminLastName: '',
      phone: '',
      address: '',
      city: '',
      postalCode: '',
      country: '',
      notes: '',
    });
    setErrors({});
    onClose();
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={handleClose}
      title="Onboard New Organization"
      size="lg"
      closeOnOverlayClick={false}
      footer={
        <div className="onboardOrg__footer">
          <Button variant="ghost" onClick={handleClose} disabled={isLoading}>
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={isLoading}>
            {isLoading ? 'Creating...' : 'Onboard Organization'}
          </Button>
        </div>
      }
    >
      <form onSubmit={handleSubmit} className="onboardOrg__form">
        <div className="onboardOrg__section">
          <h3>Organization Details</h3>
          <div className="onboardOrg__field">
            <Input
              label="Organization Name"
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              error={errors.name}
              required
              disabled={isLoading}
              placeholder="e.g., Acme Corporation"
            />
          </div>
        </div>

        <div className="onboardOrg__section">
          <h3>Admin User</h3>
          <p className="onboardOrg__sectionHint">
            This person will be the organization administrator and receive an activation email.
          </p>
          <div className="onboardOrg__grid">
            <div className="onboardOrg__field">
              <Input
                label="First Name"
                type="text"
                value={formData.adminFirstName}
                onChange={(e) => setFormData({ ...formData, adminFirstName: e.target.value })}
                error={errors.adminFirstName}
                required
                disabled={isLoading}
              />
            </div>
            <div className="onboardOrg__field">
              <Input
                label="Last Name"
                type="text"
                value={formData.adminLastName}
                onChange={(e) => setFormData({ ...formData, adminLastName: e.target.value })}
                error={errors.adminLastName}
                required
                disabled={isLoading}
              />
            </div>
          </div>
          <div className="onboardOrg__field">
            <Input
              label="Admin Email"
              type="email"
              value={formData.adminEmail}
              onChange={(e) => setFormData({ ...formData, adminEmail: e.target.value })}
              error={errors.adminEmail}
              required
              disabled={isLoading}
              placeholder="admin@example.com"
            />
          </div>
        </div>

        <div className="onboardOrg__section">
          <h3>Contact Information</h3>
          <div className="onboardOrg__field">
            <Input
              label="Phone"
              type="tel"
              value={formData.phone || ''}
              onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
              disabled={isLoading}
              placeholder="+48 123 456 789"
            />
          </div>
        </div>

        <div className="onboardOrg__section">
          <h3>Address</h3>
          <div className="onboardOrg__field">
            <Input
              label="Street Address"
              type="text"
              value={formData.address || ''}
              onChange={(e) => setFormData({ ...formData, address: e.target.value })}
              disabled={isLoading}
              placeholder="123 Main Street"
            />
          </div>
          <div className="onboardOrg__grid">
            <div className="onboardOrg__field">
              <Input
                label="City"
                type="text"
                value={formData.city || ''}
                onChange={(e) => setFormData({ ...formData, city: e.target.value })}
                disabled={isLoading}
              />
            </div>
            <div className="onboardOrg__field">
              <Input
                label="Postal Code"
                type="text"
                value={formData.postalCode || ''}
                onChange={(e) => setFormData({ ...formData, postalCode: e.target.value })}
                disabled={isLoading}
              />
            </div>
          </div>
          <div className="onboardOrg__field">
            <Input
              label="Country"
              type="text"
              value={formData.country || ''}
              onChange={(e) => setFormData({ ...formData, country: e.target.value })}
              disabled={isLoading}
              placeholder="Poland"
            />
          </div>
        </div>

        <div className="onboardOrg__section">
          <h3>Notes</h3>
          <div className="onboardOrg__field">
            <textarea
              className="onboardOrg__textarea"
              value={formData.notes || ''}
              onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
              disabled={isLoading}
              rows={3}
              placeholder="Additional notes about this organization..."
            />
          </div>
        </div>
      </form>
    </Modal>
  );
};
