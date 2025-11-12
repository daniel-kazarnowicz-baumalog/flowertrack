/**
 * AttachmentGallery Component - Manage ticket attachments
 * Supports file upload with drag-drop, preview, download, and delete
 */

import React, { useState, useRef } from 'react';
import { formatDistanceToNow } from 'date-fns';
import { pl } from 'date-fns/locale';
import {
  useTicketAttachments,
  useUploadAttachment,
  useDeleteAttachment,
} from '../../hooks/useAttachments';
import { useAuth } from '../../contexts/AuthContext';
import { Button } from '../ui/Button';
import { Modal } from '../ui/Modal';
import { useToast } from '../../hooks/useToast';
import type { AttachmentDto } from '../../types/api';
import styles from './AttachmentGallery.module.css';

interface AttachmentGalleryProps {
  ticketId: string;
}

const MAX_FILE_SIZE = 50 * 1024 * 1024; // 50MB

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 B';
  const k = 1024;
  const sizes = ['B', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(2))} ${sizes[i]}`;
};

const getFileIcon = (fileName: string): string => {
  const extension = fileName.split('.').pop()?.toLowerCase();
  const iconMap: Record<string, string> = {
    pdf: '📄',
    doc: '📝',
    docx: '📝',
    xls: '📊',
    xlsx: '📊',
    png: '🖼️',
    jpg: '🖼️',
    jpeg: '🖼️',
    gif: '🖼️',
    zip: '🗜️',
    rar: '🗜️',
    txt: '📃',
    csv: '📊',
  };
  return iconMap[extension || ''] || '📎';
};

export const AttachmentGallery: React.FC<AttachmentGalleryProps> = ({ ticketId }) => {
  const { user } = useAuth();
  const { showToast } = useToast();
  const { data: attachments, isLoading } = useTicketAttachments(ticketId);
  const uploadMutation = useUploadAttachment();
  const deleteMutation = useDeleteAttachment();

  const [isDragging, setIsDragging] = useState(false);
  const [deletingAttachment, setDeletingAttachment] = useState<AttachmentDto | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileSelect = (files: FileList | null) => {
    if (!files || files.length === 0) return;

    const file = files[0];

    // Validate file size
    if (file.size > MAX_FILE_SIZE) {
      showToast(`Plik jest za duży. Maksymalny rozmiar: ${formatFileSize(MAX_FILE_SIZE)}`, 'error');
      return;
    }

    // Upload file
    uploadMutation.mutate(
      { ticketId, file },
      {
        onSuccess: () => {
          showToast('Plik dodany', 'success');
          if (fileInputRef.current) {
            fileInputRef.current.value = '';
          }
        },
        onError: () => {
          showToast('Nie udało się dodać pliku', 'error');
        },
      }
    );
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(true);
  };

  const handleDragLeave = () => {
    setIsDragging(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
    handleFileSelect(e.dataTransfer.files);
  };

  const handleDeleteAttachment = async () => {
    if (!deletingAttachment) return;

    try {
      await deleteMutation.mutateAsync({
        ticketId,
        attachmentId: deletingAttachment.id,
      });
      showToast('Plik usunięty', 'success');
      setDeletingAttachment(null);
    } catch {
      showToast('Nie udało się usunąć pliku', 'error');
    }
  };

  const canDeleteAttachment = (attachment: AttachmentDto) => {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    return attachment.uploadedByUserId === user?.id || (user as any)?.role === 'Admin';
  };

  if (isLoading) {
    return (
      <div className={styles.attachmentGallery}>
        <div className={styles.loading}>
          <div className={styles.skeleton}></div>
          <div className={styles.skeleton}></div>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.attachmentGallery}>
      {/* Upload Area */}
      <div
        className={`${styles.uploadArea} ${isDragging ? styles.dragging : ''}`}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
      >
        <input
          ref={fileInputRef}
          type="file"
          onChange={(e) => handleFileSelect(e.target.files)}
          className={styles.fileInput}
          id="file-upload"
        />
        <label htmlFor="file-upload" className={styles.uploadLabel}>
          <span className={styles.uploadIcon}>📤</span>
          <span className={styles.uploadText}>Przeciągnij plik tutaj lub kliknij, aby wybrać</span>
          <span className={styles.uploadHint}>
            Maksymalny rozmiar: {formatFileSize(MAX_FILE_SIZE)}
          </span>
        </label>
        {uploadMutation.isPending && (
          <div className={styles.uploadProgress}>
            <div className={styles.spinner}></div>
            <span>Przesyłanie...</span>
          </div>
        )}
      </div>

      {/* Attachments List */}
      <div className={styles.attachmentsList}>
        {!attachments || attachments.length === 0 ? (
          <div className={styles.empty}>
            <p>Brak załączników</p>
          </div>
        ) : (
          attachments.map((attachment: AttachmentDto) => (
            <div key={attachment.id} className={styles.attachment}>
              <div className={styles.attachmentIcon}>{getFileIcon(attachment.fileName)}</div>
              <div className={styles.attachmentInfo}>
                <div className={styles.attachmentName}>{attachment.fileName}</div>
                <div className={styles.attachmentMeta}>
                  <span className={styles.attachmentSize}>
                    {formatFileSize(attachment.fileSize)}
                  </span>
                  <span className={styles.attachmentSeparator}>•</span>
                  <span className={styles.attachmentUploader}>{attachment.uploadedByUserName}</span>
                  <span className={styles.attachmentSeparator}>•</span>
                  <span className={styles.attachmentTime}>
                    {formatDistanceToNow(new Date(attachment.createdAt), {
                      addSuffix: true,
                      locale: pl,
                    })}
                  </span>
                </div>
              </div>
              <div className={styles.attachmentActions}>
                <a
                  href={`/api/tickets/${ticketId}/attachments/${attachment.id}/download`}
                  download={attachment.fileName}
                  className={styles.downloadButton}
                  title="Pobierz"
                >
                  ⬇️
                </a>
                {canDeleteAttachment(attachment) && (
                  <button
                    type="button"
                    onClick={() => setDeletingAttachment(attachment)}
                    className={styles.deleteButton}
                    title="Usuń"
                  >
                    🗑️
                  </button>
                )}
              </div>
            </div>
          ))
        )}
      </div>

      {/* Delete Confirmation Modal */}
      {deletingAttachment && (
        <Modal isOpen={true} onClose={() => setDeletingAttachment(null)} title="Usuń załącznik">
          <div className={styles.modalContent}>
            <p>Czy na pewno chcesz usunąć plik "{deletingAttachment.fileName}"?</p>
            <div className={styles.modalActions}>
              <Button variant="secondary" onClick={() => setDeletingAttachment(null)}>
                Anuluj
              </Button>
              <Button
                variant="danger"
                onClick={handleDeleteAttachment}
                disabled={deleteMutation.isPending}
              >
                {deleteMutation.isPending ? 'Usuwanie...' : 'Usuń'}
              </Button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
};
