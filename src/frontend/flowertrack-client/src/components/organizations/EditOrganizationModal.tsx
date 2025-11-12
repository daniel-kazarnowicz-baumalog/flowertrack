import { useState, useEffect } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import type { UpdateOrganizationRequest, OrganizationDto } from '../../types/api';
import './EditOrganizationModal.css';

interface EditOrganizationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: UpdateOrganizationRequest) => void;
  organization: OrganizationDto | null;
  isLoading?: boolean;
}

/**
 * EditOrganizationModal - Modal for editing organization information
 * Does not include admin user fields (only organization details)
 */
export const EditOrganizationModal = ({
  isOpen,
  onClose,
  onSubmit,
  organization,
  isLoading = false,
}: EditOrganizationModalProps) => {
  const [formData, setFormData] = useState<UpdateOrganizationRequest>({
    name: '',
    contactEmail: '',
    contactPhone: '',
    address: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  // Pre-populate form when organization changes
  useEffect(() => {
    if (organization) {
      setFormData({
        name: organization.name,
        contactEmail: organization.contactEmail || '',
        contactPhone: organization.contactPhone || '',
        address: organization.address || '',
      });
      setErrors({});
    }
  }, [organization]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    // Required fields
    if (!formData.name?.trim()) {
      newErrors.name = 'Organization name is required';
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

    // Remove empty optional fields, but keep name even if somehow empty
    const submitData: UpdateOrganizationRequest = {};

    if (formData.name?.trim()) {
      submitData.name = formData.name.trim();
    }
    if (formData.contactEmail?.trim()) {
      submitData.contactEmail = formData.contactEmail.trim();
    }
    if (formData.contactPhone?.trim()) {
      submitData.contactPhone = formData.contactPhone.trim();
    }
    if (formData.address?.trim()) {
      submitData.address = formData.address.trim();
    }

    onSubmit(submitData);
  };

  const handleClose = () => {
    setErrors({});
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Edit Organization" size="lg">
      <form onSubmit={handleSubmit} className="editOrganizationModal">
        <div className="editOrganizationModal__section">
          <h3 className="editOrganizationModal__sectionTitle">Organization Information</h3>

          <div className="editOrganizationModal__field">
            <label htmlFor="name" className="editOrganizationModal__label">
              Organization Name <span className="editOrganizationModal__required">*</span>
            </label>
            <Input
              id="name"
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              error={errors.name}
              disabled={isLoading}
              placeholder="e.g., Acme Manufacturing"
            />
          </div>

          <div className="editOrganizationModal__field">
            <label htmlFor="contactEmail" className="editOrganizationModal__label">
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
            <p className="editOrganizationModal__hint">
              General contact email for the organization
            </p>
          </div>

          <div className="editOrganizationModal__field">
            <label htmlFor="contactPhone" className="editOrganizationModal__label">
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

          <div className="editOrganizationModal__field">
            <label htmlFor="address" className="editOrganizationModal__label">
              Address
            </label>
            <textarea
              id="address"
              value={formData.address}
              onChange={(e) => setFormData({ ...formData, address: e.target.value })}
              className={`editOrganizationModal__textarea ${errors.address ? 'editOrganizationModal__textarea--error' : ''}`}
              disabled={isLoading}
              placeholder="Street, City, Postal Code, Country"
              rows={3}
            />
            {errors.address && (
              <span className="editOrganizationModal__error">{errors.address}</span>
            )}
          </div>
        </div>

        <div className="editOrganizationModal__footer">
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
