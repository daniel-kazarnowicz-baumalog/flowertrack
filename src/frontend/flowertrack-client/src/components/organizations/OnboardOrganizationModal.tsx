import { useState } from 'react';
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

/**
 * OnboardOrganizationModal - Modal for onboarding new organizations
 * Creates organization and admin user in a single transaction
 */
export const OnboardOrganizationModal = ({
  isOpen,
  onClose,
  onSubmit,
  isLoading = false,
}: OnboardOrganizationModalProps) => {
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

    // Required fields
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

    // Optional email validation
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

    // Remove empty optional fields
    const submitData: OnboardOrganizationRequest = {
      organizationName: formData.organizationName,
      adminEmail: formData.adminEmail,
      adminFirstName: formData.adminFirstName,
      adminLastName: formData.adminLastName,
    };

    if (formData.contactEmail?.trim()) {
      submitData.contactEmail = formData.contactEmail;
    }
    if (formData.contactPhone?.trim()) {
      submitData.contactPhone = formData.contactPhone;
    }
    if (formData.address?.trim()) {
      submitData.address = formData.address;
    }

    onSubmit(submitData);
  };

  const handleClose = () => {
    // Reset form
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
    <Modal isOpen={isOpen} onClose={handleClose} title="Onboard New Organization" size="lg">
      <form onSubmit={handleSubmit} className="onboardOrganizationModal">
        <div className="onboardOrganizationModal__section">
          <h3 className="onboardOrganizationModal__sectionTitle">Organization Information</h3>

          <div className="onboardOrganizationModal__field">
            <label htmlFor="organizationName" className="onboardOrganizationModal__label">
              Organization Name <span className="onboardOrganizationModal__required">*</span>
            </label>
            <Input
              id="organizationName"
              type="text"
              value={formData.organizationName}
              onChange={(e) => setFormData({ ...formData, organizationName: e.target.value })}
              error={errors.organizationName}
              disabled={isLoading}
              placeholder="e.g., Acme Manufacturing"
            />
          </div>

          <div className="onboardOrganizationModal__field">
            <label htmlFor="contactEmail" className="onboardOrganizationModal__label">
              Contact Email
            </label>
            <Input
              id="contactEmail"
              type="email"
              value={formData.contactEmail}
              onChange={(e) => setFormData({ ...formData, contactEmail: e.target.value })}
              error={errors.contactEmail}
              disabled={isLoading}
              placeholder="contact@organization.com"
            />
            <p className="onboardOrganizationModal__hint">
              General contact email for the organization
            </p>
          </div>

          <div className="onboardOrganizationModal__field">
            <label htmlFor="contactPhone" className="onboardOrganizationModal__label">
              Contact Phone
            </label>
            <Input
              id="contactPhone"
              type="tel"
              value={formData.contactPhone}
              onChange={(e) => setFormData({ ...formData, contactPhone: e.target.value })}
              error={errors.contactPhone}
              disabled={isLoading}
              placeholder="+48 123 456 789"
            />
          </div>

          <div className="onboardOrganizationModal__field">
            <label htmlFor="address" className="onboardOrganizationModal__label">
              Address
            </label>
            <textarea
              id="address"
              value={formData.address}
              onChange={(e) => setFormData({ ...formData, address: e.target.value })}
              className={`onboardOrganizationModal__textarea ${errors.address ? 'onboardOrganizationModal__textarea--error' : ''}`}
              disabled={isLoading}
              placeholder="Street, City, Postal Code, Country"
              rows={3}
            />
            {errors.address && (
              <span className="onboardOrganizationModal__error">{errors.address}</span>
            )}
          </div>
        </div>

        <div className="onboardOrganizationModal__section">
          <h3 className="onboardOrganizationModal__sectionTitle">Admin User Information</h3>
          <p className="onboardOrganizationModal__sectionDescription">
            The admin user will receive an activation email to set their password.
          </p>

          <div className="onboardOrganizationModal__field">
            <label htmlFor="adminEmail" className="onboardOrganizationModal__label">
              Admin Email <span className="onboardOrganizationModal__required">*</span>
            </label>
            <Input
              id="adminEmail"
              type="email"
              value={formData.adminEmail}
              onChange={(e) => setFormData({ ...formData, adminEmail: e.target.value })}
              error={errors.adminEmail}
              disabled={isLoading}
              placeholder="admin@organization.com"
            />
          </div>

          <div className="onboardOrganizationModal__row">
            <div className="onboardOrganizationModal__field">
              <label htmlFor="adminFirstName" className="onboardOrganizationModal__label">
                First Name <span className="onboardOrganizationModal__required">*</span>
              </label>
              <Input
                id="adminFirstName"
                type="text"
                value={formData.adminFirstName}
                onChange={(e) => setFormData({ ...formData, adminFirstName: e.target.value })}
                error={errors.adminFirstName}
                disabled={isLoading}
                placeholder="John"
              />
            </div>

            <div className="onboardOrganizationModal__field">
              <label htmlFor="adminLastName" className="onboardOrganizationModal__label">
                Last Name <span className="onboardOrganizationModal__required">*</span>
              </label>
              <Input
                id="adminLastName"
                type="text"
                value={formData.adminLastName}
                onChange={(e) => setFormData({ ...formData, adminLastName: e.target.value })}
                error={errors.adminLastName}
                disabled={isLoading}
                placeholder="Doe"
              />
            </div>
          </div>
        </div>

        <div className="onboardOrganizationModal__footer">
          <Button type="button" variant="secondary" onClick={handleClose} disabled={isLoading}>
            Cancel
          </Button>
          <Button type="submit" variant="primary" disabled={isLoading}>
            {isLoading ? 'Onboarding...' : 'Onboard Organization'}
          </Button>
        </div>
      </form>
    </Modal>
  );
};
