/**
 * AssignmentDropdown - Dropdown for assigning tickets to service users
 */

import React, { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Loader } from '../ui/Loader';
import styles from './AssignmentDropdown.module.css';

interface ServiceUser {
  id: string;
  fullName: string;
  email: string;
  role: string;
}

interface AssignmentDropdownProps {
  isOpen: boolean;
  onClose: () => void;
  currentAssigneeId?: string;
  serviceUsers: ServiceUser[];
  onConfirm: (userId?: string) => void;
  isLoading?: boolean;
  isFetchingUsers?: boolean;
}

export const AssignmentDropdown: React.FC<AssignmentDropdownProps> = ({
  isOpen,
  onClose,
  currentAssigneeId,
  serviceUsers,
  onConfirm,
  isLoading = false,
  isFetchingUsers = false,
}) => {
  const [selectedUserId, setSelectedUserId] = useState<string | undefined>(currentAssigneeId);
  const [searchQuery, setSearchQuery] = useState('');

  const filteredUsers = serviceUsers.filter(
    (user) =>
      user.fullName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      user.email.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onConfirm(selectedUserId);
  };

  const handleUnassign = () => {
    setSelectedUserId(undefined);
    onConfirm(undefined);
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Przypisz technika">
      <form onSubmit={handleSubmit} className={styles.form}>
        {isFetchingUsers ? (
          <div className={styles.loadingContainer}>
            <Loader size="md" />
            <span>Ładowanie listy techników...</span>
          </div>
        ) : (
          <>
            <div className={styles.searchBox}>
              <input
                type="text"
                placeholder="Szukaj technika..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className={styles.searchInput}
                disabled={isLoading}
              />
            </div>

            <div className={styles.userList}>
              {filteredUsers.length === 0 ? (
                <div className={styles.empty}>
                  <p>Nie znaleziono techników</p>
                </div>
              ) : (
                filteredUsers.map((user) => (
                  <label key={user.id} className={styles.userItem}>
                    <input
                      type="radio"
                      name="assignee"
                      value={user.id}
                      checked={selectedUserId === user.id}
                      onChange={() => setSelectedUserId(user.id)}
                      disabled={isLoading}
                      className={styles.radio}
                    />
                    <div className={styles.userInfo}>
                      <div className={styles.userName}>{user.fullName}</div>
                      <div className={styles.userDetails}>
                        <span className={styles.userEmail}>{user.email}</span>
                        <span className={styles.userRole}>• {user.role}</span>
                      </div>
                    </div>
                  </label>
                ))
              )}
            </div>

            <div className={styles.actions}>
              {currentAssigneeId && (
                <Button
                  type="button"
                  variant="secondary"
                  onClick={handleUnassign}
                  disabled={isLoading}
                >
                  Cofnij przypisanie
                </Button>
              )}
              <Button type="button" variant="ghost" onClick={onClose} disabled={isLoading}>
                Anuluj
              </Button>
              <Button type="submit" disabled={isLoading || selectedUserId === currentAssigneeId}>
                {isLoading ? 'Zapisywanie...' : 'Przypisz'}
              </Button>
            </div>
          </>
        )}
      </form>
    </Modal>
  );
};
