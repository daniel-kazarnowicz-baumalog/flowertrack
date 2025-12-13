import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Input } from '../ui';
import { useInviteServiceUser } from '../../hooks/useServiceUsers';
import './InviteTechnicianModal.css';

export interface InviteTechnicianModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

interface FormData {
  email: string;
  firstName: string;
  lastName: string;
  role: 'Admin' | 'Technician';
  password: string;
}

interface FormErrors {
  email?: string;
  firstName?: string;
  lastName?: string;
  password?: string;
  general?: string;
}

export const InviteTechnicianModal = ({
  isOpen,
  onClose,
  onSuccess,
}: InviteTechnicianModalProps) => {
  const { t } = useTranslation();
  const [formData, setFormData] = useState<FormData>({
    email: '',
    firstName: '',
    lastName: '',
    role: 'Technician',
    password: '',
  });
  const [errors, setErrors] = useState<FormErrors>({});
  const inviteMutation = useInviteServiceUser();

  const resetForm = () => {
    setFormData({
      email: '',
      firstName: '',
      lastName: '',
      role: 'Technician',
      password: '',
    });
    setErrors({});
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    // Email validation
    if (!formData.email.trim()) {
      newErrors.email = t('users.emailRequired');
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = t('users.invalidEmail');
    }

    // First name validation
    if (!formData.firstName.trim()) {
      newErrors.firstName = t('users.firstNameRequired');
    } else if (formData.firstName.length > 100) {
      newErrors.firstName = t('users.firstNameTooLong');
    }

    // Last name validation
    if (!formData.lastName.trim()) {
      newErrors.lastName = t('users.lastNameRequired');
    } else if (formData.lastName.length > 100) {
      newErrors.lastName = t('users.lastNameTooLong');
    }

    // Password validation (optional but if provided, must be valid)
    if (formData.password && formData.password.length < 8) {
      newErrors.password = t('users.passwordTooShort');
    } else if (formData.password && formData.password.length > 100) {
      newErrors.password = t('users.passwordTooLong');
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      await inviteMutation.mutateAsync(formData);
      resetForm();
      onSuccess();
      onClose();
    } catch (error) {
      // Handle specific error cases
      const axiosError = error as {
        response?: { status?: number; data?: { message?: string } };
      };

      if (axiosError.response?.status === 409) {
        setErrors({ email: t('users.emailExists') });
      } else if (axiosError.response?.status === 403) {
        setErrors({ general: t('users.noPermissionInvite') });
      } else {
        setErrors({
          general: axiosError.response?.data?.message || t('users.inviteFailed'),
        });
      }
    }
  };

  const handleInputChange = (field: keyof FormData, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
    // Clear error for this field when user starts typing
    if (field in errors) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[field as keyof FormErrors];
        return newErrors;
      });
    }
  };

  const handleRoleChange = (role: 'Admin' | 'Technician') => {
    setFormData((prev) => ({ ...prev, role }));
  };

  const footerContent = (
    <div className="inviteTechnicianForm__footer">
      <Button
        type="button"
        variant="ghost"
        onClick={handleClose}
        disabled={inviteMutation.isPending}
      >
        {t('common.cancel')}
      </Button>
      <Button
        type="submit"
        form="invite-technician-form"
        variant="primary"
        isLoading={inviteMutation.isPending}
      >
        {t('users.sendInvitation')}
      </Button>
    </div>
  );

  return (
    <Modal
      isOpen={isOpen}
      onClose={handleClose}
      title={t('users.inviteNewMember')}
      size="md"
      footer={footerContent}
    >
      <form id="invite-technician-form" onSubmit={handleSubmit} className="inviteTechnicianForm">
        {errors.general && <div className="inviteTechnicianForm__error">{errors.general}</div>}

        <div className="inviteTechnicianForm__field">
          <Input
            type="email"
            label={t('auth.email')}
            placeholder="technician@example.com"
            value={formData.email}
            onChange={(e) => handleInputChange('email', e.target.value)}
            error={errors.email}
            required
            disabled={inviteMutation.isPending}
          />
        </div>

        <div className="inviteTechnicianForm__field">
          <Input
            type="text"
            label={t('users.firstName')}
            placeholder="John"
            value={formData.firstName}
            onChange={(e) => handleInputChange('firstName', e.target.value)}
            error={errors.firstName}
            required
            disabled={inviteMutation.isPending}
          />
        </div>

        <div className="inviteTechnicianForm__field">
          <Input
            type="text"
            label={t('users.lastName')}
            placeholder="Doe"
            value={formData.lastName}
            onChange={(e) => handleInputChange('lastName', e.target.value)}
            error={errors.lastName}
            required
            disabled={inviteMutation.isPending}
          />
        </div>

        <div className="inviteTechnicianForm__field">
          <Input
            type="password"
            label={`${t('auth.password')} (${t('common.optional')})`}
            placeholder={t('users.leaveEmptyForEmail')}
            value={formData.password}
            onChange={(e) => handleInputChange('password', e.target.value)}
            error={errors.password}
            disabled={inviteMutation.isPending}
          />
          <span className="inviteTechnicianForm__hint">{t('users.passwordHint')}</span>
        </div>

        <div className="inviteTechnicianForm__field">
          <label className="inviteTechnicianForm__label">
            {t('users.role')} <span className="inviteTechnicianForm__required">*</span>
          </label>
          <div className="roleSelector">
            <label
              className={`roleCard ${formData.role === 'Technician' ? 'roleCard--selected' : ''}`}
            >
              <input
                type="radio"
                name="role"
                value="Technician"
                checked={formData.role === 'Technician'}
                onChange={() => handleRoleChange('Technician')}
                disabled={inviteMutation.isPending}
                className="roleCard__input"
              />
              <div className="roleCard__content">
                <div className="roleCard__icon">🔧</div>
                <div className="roleCard__name">{t('users.technician')}</div>
                <div className="roleCard__description">
                  {t('users.inviteDescriptionTechnician')}
                </div>
              </div>
            </label>

            <label className={`roleCard ${formData.role === 'Admin' ? 'roleCard--selected' : ''}`}>
              <input
                type="radio"
                name="role"
                value="Admin"
                checked={formData.role === 'Admin'}
                onChange={() => handleRoleChange('Admin')}
                disabled={inviteMutation.isPending}
                className="roleCard__input"
              />
              <div className="roleCard__content">
                <div className="roleCard__icon">👑</div>
                <div className="roleCard__name">{t('users.administrator')}</div>
                <div className="roleCard__description">{t('users.inviteDescriptionAdmin')}</div>
              </div>
            </label>
          </div>
        </div>
      </form>
    </Modal>
  );
};
