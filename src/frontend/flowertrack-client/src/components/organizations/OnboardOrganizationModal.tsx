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
  onSubmit: (data: OnboardOrganizationRequest) => void;
  isLoading?: boolean;
}

export const OnboardOrganizationModal: React.FC<OnboardOrganizationModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  isLoading = false,
}) => {
  const [formData, setFormData] = useState<OnboardOrganizationRequest>({
    organizationName: '',
    adminEmail: '',
    adminFirstName: '',
    adminLastName: '',
    contactEmail: '',
    contactPhone: '',
    address: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.organizationName.trim()) {
      newErrors.organizationName = 'Organization name is required';
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

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) {
      return;
    }

    onSubmit(formData);
  };

  const handleClose = () => {
    setFormData({
      organizationName: '',
      adminEmail: '',
      adminFirstName: '',
      adminLastName: '',
      contactEmail: '',
      contactPhone: '',
      address: '',
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
              value={formData.organizationName}
              onChange={(e) => setFormData({ ...formData, organizationName: e.target.value })}
              error={errors.organizationName}
              required
              disabled={isLoading}
            />
          </div>
        </div>

        <div className="onboardOrg__section">
          <h3>Admin User</h3>
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
              label="Email"
              type="email"
              value={formData.adminEmail}
              onChange={(e) => setFormData({ ...formData, adminEmail: e.target.value })}
              error={errors.adminEmail}
              required
              disabled={isLoading}
            />
          </div>
        </div>

        <div className="onboardOrg__section">
          <h3>Contact Information (Optional)</h3>
          <div className="onboardOrg__field">
            <Input
              label="Contact Email"
              type="email"
              value={formData.contactEmail || ''}
              onChange={(e) => setFormData({ ...formData, contactEmail: e.target.value })}
              disabled={isLoading}
            />
          </div>
          <div className="onboardOrg__field">
            <Input
              label="Contact Phone"
              type="tel"
              value={formData.contactPhone || ''}
              onChange={(e) => setFormData({ ...formData, contactPhone: e.target.value })}
              disabled={isLoading}
            />
          </div>
          <div className="onboardOrg__field">
            <Input
              label="Address"
              type="text"
              value={formData.address || ''}
              onChange={(e) => setFormData({ ...formData, address: e.target.value })}
              disabled={isLoading}
            />
          </div>
        </div>
      </form>
    </Modal>
  );
};
