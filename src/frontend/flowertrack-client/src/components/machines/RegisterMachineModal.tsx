import { useState, useEffect } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { useOrganizations } from '../../hooks/useOrganizations';
import type { CreateMachineRequest } from '../../types/api';
import './RegisterMachineModal.css';

interface RegisterMachineModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: CreateMachineRequest) => void;
  isLoading?: boolean;
}

/**
 * RegisterMachineModal - Modal for registering a new machine
 * Includes organization selection, machine details, and installation info
 */
export const RegisterMachineModal = ({
  isOpen,
  onClose,
  onSubmit,
  isLoading = false,
}: RegisterMachineModalProps) => {
  const [formData, setFormData] = useState<CreateMachineRequest>({
    organizationId: '',
    model: '',
    serialNumber: '',
    installationDate: '',
    location: '',
    notes: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [orgSearch, setOrgSearch] = useState('');
  const [showOrgDropdown, setShowOrgDropdown] = useState(false);

  // Fetch organizations for dropdown
  const { organizations } = useOrganizations();

  // Filter organizations based on search
  const filteredOrganizations = organizations
    ? organizations.filter((org) =>
        org.name.toLowerCase().includes(orgSearch.toLowerCase())
      )
    : [];
  const selectedOrg = organizations
    ? organizations.find((org) => org.id === formData.organizationId)
    : undefined;

  // Reset form when modal closes
  useEffect(() => {
    if (!isOpen) {
      setFormData({
        organizationId: '',
        model: '',
        serialNumber: '',
        installationDate: '',
        location: '',
        notes: '',
      });
      setErrors({});
      setOrgSearch('');
      setShowOrgDropdown(false);
    }
  }, [isOpen]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    // Required fields
    if (!formData.serialNumber?.trim()) {
      newErrors.serialNumber = 'Serial number is required';
    }

    if (!formData.model?.trim()) {
      newErrors.model = 'Model is required';
    }

    if (!formData.organizationId) {
      newErrors.organizationId = 'Organization is required';
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

    // Remove empty optional fields
    const submitData: CreateMachineRequest = {
      organizationId: formData.organizationId,
      model: formData.model.trim(),
      serialNumber: formData.serialNumber.trim(),
    };

    if (formData.installationDate?.trim()) {
      submitData.installationDate = formData.installationDate.trim();
    }
    if (formData.location?.trim()) {
      submitData.location = formData.location.trim();
    }
    if (formData.notes?.trim()) {
      submitData.notes = formData.notes.trim();
    }

    onSubmit(submitData);
  };

  const handleClose = () => {
    setErrors({});
    setOrgSearch('');
    setShowOrgDropdown(false);
    onClose();
  };

  const handleOrgSelect = (orgId: string, orgName: string) => {
    setFormData({ ...formData, organizationId: orgId });
    setOrgSearch(orgName);
    setShowOrgDropdown(false);
    setErrors({ ...errors, organizationId: '' });
  };

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Register Machine" size="lg">
      <form onSubmit={handleSubmit} className="registerMachineModal">
        <div className="registerMachineModal__section">
          <h3 className="registerMachineModal__sectionTitle">Machine Information</h3>

          <div className="registerMachineModal__field">
            <label htmlFor="serialNumber" className="registerMachineModal__label">
              Serial Number <span className="registerMachineModal__required">*</span>
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
            <p className="registerMachineModal__hint">Unique identifier for the machine</p>
          </div>

          <div className="registerMachineModal__field">
            <label htmlFor="model" className="registerMachineModal__label">
              Model <span className="registerMachineModal__required">*</span>
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

          <div className="registerMachineModal__field">
            <label htmlFor="organization" className="registerMachineModal__label">
              Organization <span className="registerMachineModal__required">*</span>
            </label>
            <div className="registerMachineModal__orgSelect">
              <Input
                id="organization"
                type="text"
                value={selectedOrg ? selectedOrg.name : orgSearch}
                onChange={(e) => {
                  setOrgSearch(e.target.value);
                  setShowOrgDropdown(true);
                  if (selectedOrg) {
                    setFormData({ ...formData, organizationId: '' });
                  }
                }}
                onFocus={() => setShowOrgDropdown(true)}
                error={errors.organizationId}
                disabled={isLoading}
                placeholder="Search organizations..."
              />
              {showOrgDropdown && filteredOrganizations.length > 0 && (
                <div className="registerMachineModal__orgDropdown">
                  {filteredOrganizations.map((org) => (
                    <div
                      key={org.id}
                      className="registerMachineModal__orgOption"
                      onClick={() => handleOrgSelect(org.id, org.name)}
                    >
                      <div className="registerMachineModal__orgName">{org.name}</div>
                      <div className="registerMachineModal__orgMeta">
                        {org.contactEmail || 'No email'}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          <div className="registerMachineModal__field">
            <label htmlFor="location" className="registerMachineModal__label">
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

          <div className="registerMachineModal__field">
            <label htmlFor="installationDate" className="registerMachineModal__label">
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

          <div className="registerMachineModal__field">
            <label htmlFor="notes" className="registerMachineModal__label">
              Notes
            </label>
            <textarea
              id="notes"
              value={formData.notes}
              onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
              className={`registerMachineModal__textarea ${errors.notes ? 'registerMachineModal__textarea--error' : ''}`}
              disabled={isLoading}
              placeholder="Additional information about the machine..."
              rows={4}
            />
            {errors.notes && <span className="registerMachineModal__error">{errors.notes}</span>}
          </div>
        </div>

        <div className="registerMachineModal__footer">
          <Button type="button" variant="secondary" onClick={handleClose} disabled={isLoading}>
            Cancel
          </Button>
          <Button type="submit" variant="primary" disabled={isLoading}>
            {isLoading ? 'Registering...' : 'Register Machine'}
          </Button>
        </div>
      </form>
    </Modal>
  );
};
