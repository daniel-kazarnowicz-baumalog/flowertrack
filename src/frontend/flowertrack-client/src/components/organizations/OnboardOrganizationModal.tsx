/**
 * OnboardOrganizationModal - Modal for onboarding new organizations (Service Portal)
 */

import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
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
  const { t } = useTranslation();
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
      newErrors.name = t('organizations.nameRequired');
    } else if (formData.name.length < 3) {
      newErrors.name = t('organizations.nameTooShort');
    }
    if (!formData.adminEmail.trim()) {
      newErrors.adminEmail = t('organizations.adminEmailRequired');
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.adminEmail)) {
      newErrors.adminEmail = t('organizations.invalidEmail');
    }
    if (!formData.adminFirstName.trim()) {
      newErrors.adminFirstName = t('organizations.adminFirstNameRequired');
    }
    if (!formData.adminLastName.trim()) {
      newErrors.adminLastName = t('organizations.adminLastNameRequired');
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
      title={t('organizations.onboardNewOrganization')}
      size="lg"
      closeOnOverlayClick={false}
      footer={
        <div className="onboardOrg__footer">
          <Button variant="ghost" onClick={handleClose} disabled={isLoading}>
            {t('common.cancel')}
          </Button>
          <Button onClick={handleSubmit} disabled={isLoading}>
            {isLoading ? t('organizations.creating') : t('organizations.onboardOrganization')}
          </Button>
        </div>
      }
    >
      <form onSubmit={handleSubmit} className="onboardOrg__form">
        <div className="onboardOrg__section">
          <h3>{t('organizations.organizationDetails')}</h3>
          <div className="onboardOrg__field">
            <Input
              label={t('organizations.organizationName')}
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              error={errors.name}
              required
              disabled={isLoading}
              placeholder={t('organizations.namePlaceholder', 'e.g., Acme Corporation')}
            />
          </div>
        </div>

        <div className="onboardOrg__section">
          <h3>{t('organizations.adminUser')}</h3>
          <p className="onboardOrg__sectionHint">{t('organizations.adminSectionHint')}</p>
          <div className="onboardOrg__grid">
            <div className="onboardOrg__field">
              <Input
                label={t('users.firstName')}
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
                label={t('users.lastName')}
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
              label={t('organizations.adminEmail')}
              type="email"
              value={formData.adminEmail}
              onChange={(e) => setFormData({ ...formData, adminEmail: e.target.value })}
              error={errors.adminEmail}
              required
              disabled={isLoading}
              placeholder={t('auth.emailPlaceholder')}
            />
          </div>
        </div>

        <div className="onboardOrg__section">
          <h3>{t('organizations.contactInformation')}</h3>
          <div className="onboardOrg__field">
            <Input
              label={t('organizations.phone')}
              type="tel"
              value={formData.phone || ''}
              onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
              disabled={isLoading}
              placeholder={t('organizations.phonePlaceholder')}
            />
          </div>
        </div>

        <div className="onboardOrg__section">
          <h3>{t('organizations.address')}</h3>
          <div className="onboardOrg__field">
            <Input
              label={t('organizations.streetAddress')}
              type="text"
              value={formData.address || ''}
              onChange={(e) => setFormData({ ...formData, address: e.target.value })}
              disabled={isLoading}
              placeholder={t('organizations.addressPlaceholder')}
            />
          </div>
          <div className="onboardOrg__grid">
            <div className="onboardOrg__field">
              <Input
                label={t('common.city')}
                type="text"
                value={formData.city || ''}
                onChange={(e) => setFormData({ ...formData, city: e.target.value })}
                disabled={isLoading}
              />
            </div>
            <div className="onboardOrg__field">
              <Input
                label={t('organizations.zipCode')}
                type="text"
                value={formData.postalCode || ''}
                onChange={(e) => setFormData({ ...formData, postalCode: e.target.value })}
                disabled={isLoading}
              />
            </div>
          </div>
          <div className="onboardOrg__field">
            <Input
              label={t('common.country')}
              type="text"
              value={formData.country || ''}
              onChange={(e) => setFormData({ ...formData, country: e.target.value })}
              disabled={isLoading}
              placeholder={t('organizations.countryPlaceholder')}
            />
          </div>
        </div>

        <div className="onboardOrg__section">
          <h3>{t('common.notes')}</h3>
          <div className="onboardOrg__field">
            <textarea
              className="onboardOrg__textarea"
              value={formData.notes || ''}
              onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
              disabled={isLoading}
              rows={3}
              placeholder={t('organizations.notesPlaceholder')}
            />
          </div>
        </div>
      </form>
    </Modal>
  );
};
