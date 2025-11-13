import { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { useInviteTeamMember } from '../../hooks/useTeam';
import { useToast } from '../../hooks/useToast';
import type { InviteTeamMemberRequest, UserRole } from '../../types/api';
import { getApiErrorMessage } from '../../lib/apiClient';
import './InviteTeamMemberModal.css';

export interface InviteTeamMemberModalProps {
  isOpen: boolean;
  onClose: () => void;
  organizationId: string;
  onSuccess: () => void;
}

interface FormData {
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
}

interface FormErrors {
  email?: string;
  firstName?: string;
  lastName?: string;
  general?: string;
}

export function InviteTeamMemberModal({
  isOpen,
  onClose,
  organizationId,
  onSuccess,
}: InviteTeamMemberModalProps) {
  const { showToast } = useToast();
  const inviteMutation = useInviteTeamMember(organizationId);

  const [formData, setFormData] = useState<FormData>({
    email: '',
    firstName: '',
    lastName: '',
    role: 'User',
  });

  const [errors, setErrors] = useState<FormErrors>({});

  const resetForm = () => {
    setFormData({
      email: '',
      firstName: '',
      lastName: '',
      role: 'User',
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
      newErrors.email = 'Email is required';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Please enter a valid email address';
    }

    // First name validation
    if (!formData.firstName.trim()) {
      newErrors.firstName = 'First name is required';
    } else if (formData.firstName.length > 100) {
      newErrors.firstName = 'First name cannot exceed 100 characters';
    }

    // Last name validation
    if (!formData.lastName.trim()) {
      newErrors.lastName = 'Last name is required';
    } else if (formData.lastName.length > 100) {
      newErrors.lastName = 'Last name cannot exceed 100 characters';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    const request: InviteTeamMemberRequest = {
      email: formData.email.trim(),
      firstName: formData.firstName.trim(),
      lastName: formData.lastName.trim(),
      role: formData.role,
    };

    try {
      await inviteMutation.mutateAsync(request);
      showToast(`Invitation sent to ${request.email}`, 'success');
      resetForm();
      onSuccess();
      onClose();
    } catch (error) {
      const errorMessage = getApiErrorMessage(error);

      // Check for duplicate user error (409 Conflict)
      if ((error as { response?: { status?: number } }).response?.status === 409) {
        setErrors({ email: 'This user is already a member' });
      } else {
        setErrors({ general: errorMessage });
      }
    }
  };

  const handleInputChange = (field: keyof FormData, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
    // Clear error for this field when user starts typing
    if (errors[field as keyof FormErrors]) {
      setErrors((prev) => ({ ...prev, [field]: undefined }));
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={handleClose}
      title="Invite Team Member"
      size="md"
      footer={
        <div className="invite-modal__footer">
          <Button variant="ghost" onClick={handleClose} disabled={inviteMutation.isPending}>
            Cancel
          </Button>
          <Button
            variant="primary"
            onClick={handleSubmit}
            isLoading={inviteMutation.isPending}
            disabled={inviteMutation.isPending}
          >
            Send Invitation
          </Button>
        </div>
      }
    >
      <form onSubmit={handleSubmit} className="invite-modal__form">
        {errors.general && (
          <div className="invite-modal__error" role="alert">
            {errors.general}
          </div>
        )}

        <div className="invite-modal__field">
          <label htmlFor="email" className="invite-modal__label">
            Email <span className="invite-modal__required">*</span>
          </label>
          <Input
            id="email"
            type="email"
            value={formData.email}
            onChange={(e) => handleInputChange('email', e.target.value)}
            placeholder="member@example.com"
            error={errors.email}
            disabled={inviteMutation.isPending}
            autoComplete="email"
          />
        </div>

        <div className="invite-modal__field">
          <label htmlFor="firstName" className="invite-modal__label">
            First Name <span className="invite-modal__required">*</span>
          </label>
          <Input
            id="firstName"
            type="text"
            value={formData.firstName}
            onChange={(e) => handleInputChange('firstName', e.target.value)}
            placeholder="John"
            error={errors.firstName}
            disabled={inviteMutation.isPending}
            autoComplete="given-name"
            maxLength={100}
          />
        </div>

        <div className="invite-modal__field">
          <label htmlFor="lastName" className="invite-modal__label">
            Last Name <span className="invite-modal__required">*</span>
          </label>
          <Input
            id="lastName"
            type="text"
            value={formData.lastName}
            onChange={(e) => handleInputChange('lastName', e.target.value)}
            placeholder="Doe"
            error={errors.lastName}
            disabled={inviteMutation.isPending}
            autoComplete="family-name"
            maxLength={100}
          />
        </div>

        <div className="invite-modal__field">
          <label className="invite-modal__label">
            Role <span className="invite-modal__required">*</span>
          </label>
          <div className="invite-modal__role-options">
            <label className="invite-modal__role-option">
              <input
                type="radio"
                name="role"
                value="User"
                checked={formData.role === 'User'}
                onChange={(e) => handleInputChange('role', e.target.value)}
                disabled={inviteMutation.isPending}
              />
              <div className="invite-modal__role-content">
                <div className="invite-modal__role-title">User</div>
                <div className="invite-modal__role-description">
                  Can view and create tickets for the organization
                </div>
              </div>
            </label>

            <label className="invite-modal__role-option">
              <input
                type="radio"
                name="role"
                value="Admin"
                checked={formData.role === 'Admin'}
                onChange={(e) => handleInputChange('role', e.target.value)}
                disabled={inviteMutation.isPending}
              />
              <div className="invite-modal__role-content">
                <div className="invite-modal__role-title">
                  <span className="invite-modal__role-icon">👑</span> Admin
                </div>
                <div className="invite-modal__role-description">
                  Can invite members, manage team, and access all tickets
                </div>
              </div>
            </label>
          </div>
        </div>
      </form>
    </Modal>
  );
}
