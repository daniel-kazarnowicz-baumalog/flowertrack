import React, { useState } from 'react';
import { Modal, Button } from '../ui';
import { serviceUserService } from '../../services/serviceUserService';
import './ServiceResetPasswordModal.css';

interface ServiceResetPasswordModalProps {
  isOpen: boolean;
  onClose: () => void;
  userId: string;
  userName: string;
}

export const ServiceResetPasswordModal: React.FC<ServiceResetPasswordModalProps> = ({
  isOpen,
  onClose,
  userId,
  userName,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [newPassword, setNewPassword] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleReset = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await serviceUserService.resetUserPassword(userId);
      setNewPassword(response.temporaryPassword);
    } catch (err) {
      setError('Failed to reset password. Please try again.');
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleClose = () => {
    setNewPassword(null);
    setError(null);
    onClose();
  };

  const copyToClipboard = () => {
    if (newPassword) {
      navigator.clipboard.writeText(newPassword);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Reset User Password">
      <div className="resetExampleModal">
        {!newPassword ? (
          <>
            <p className="resetWarning">
              Are you sure you want to reset the password for <strong>{userName}</strong>?
            </p>
            <p className="resetInfo">
              This will invalidate their current password and generate a temporary one.
            </p>
            {error && <div className="resetError">{error}</div>}
            <div className="resetActions">
              <Button variant="ghost" onClick={handleClose} disabled={isLoading}>
                Cancel
              </Button>
              <Button variant="danger" onClick={handleReset} disabled={isLoading}>
                {isLoading ? 'Resetting...' : 'Reset Password'}
              </Button>
            </div>
          </>
        ) : (
          <div className="resetSuccess">
            <div className="successIcon">✅</div>
            <h3>Password Reset Successful</h3>
            <p>Please share this temporary password with the user:</p>
            <div className="passwordDisplay">
              <code>{newPassword}</code>
              <Button size="sm" variant="ghost" onClick={copyToClipboard}>
                Copy
              </Button>
            </div>
            <p className="securityNote">
              This password will expire in 24 hours. The user will be asked to change it upon login.
            </p>
            <div className="resetActions">
              <Button variant="primary" onClick={handleClose}>
                Done
              </Button>
            </div>
          </div>
        )}
      </div>
    </Modal>
  );
};
