import { useState } from 'react';
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

    // Password validation (optional but if provided, must be valid)
    if (formData.password && formData.password.length < 8) {
      newErrors.password = 'Password must be at least 8 characters';
    } else if (formData.password && formData.password.length > 100) {
      newErrors.password = 'Password cannot exceed 100 characters';
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
        setErrors({ email: 'A user with this email already exists' });
      } else if (axiosError.response?.status === 403) {
        setErrors({ general: "You don't have permission to invite users" });
      } else {
        setErrors({
          general: axiosError.response?.data?.message || 'Failed to send invitation',
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

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Invite New Team Member" size="md">
      <form onSubmit={handleSubmit} className="inviteTechnicianForm">
        {errors.general && <div className="inviteTechnicianForm__error">{errors.general}</div>}

        <div className="inviteTechnicianForm__field">
          <Input
            type="email"
            label="Email"
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
            label="First Name"
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
            label="Last Name"
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
            label="Password (optional)"
            placeholder="Leave empty to send invitation email"
            value={formData.password}
            onChange={(e) => handleInputChange('password', e.target.value)}
            error={errors.password}
            disabled={inviteMutation.isPending}
          />
          <span className="inviteTechnicianForm__hint">
            If set, user can login immediately. Otherwise, invitation email will be sent.
          </span>
        </div>

        <div className="inviteTechnicianForm__field">
          <label className="inviteTechnicianForm__label">
            Role <span className="inviteTechnicianForm__required">*</span>
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
                <div className="roleCard__name">Technician</div>
                <div className="roleCard__description">
                  Can manage tickets, machines, and organizations
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
                <div className="roleCard__name">Admin</div>
                <div className="roleCard__description">
                  Full system access including user management
                </div>
              </div>
            </label>
          </div>
        </div>

        <div className="inviteTechnicianForm__footer">
          <Button
            type="button"
            variant="ghost"
            onClick={handleClose}
            disabled={inviteMutation.isPending}
          >
            Cancel
          </Button>
          <Button type="submit" variant="primary" isLoading={inviteMutation.isPending}>
            Send Invitation
          </Button>
        </div>
      </form>
    </Modal>
  );
};
