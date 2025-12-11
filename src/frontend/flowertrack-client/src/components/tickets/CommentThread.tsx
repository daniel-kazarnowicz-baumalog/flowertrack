/**
 * CommentThread Component - Display and manage ticket comments
 * Supports both public comments and internal notes (service users only)
 */

import { useState } from 'react';
import { formatDistanceToNow, isValid } from 'date-fns';
import { pl } from 'date-fns/locale';
import {
  useTicketComments,
  useAddComment,
  useAddNote,
  useUpdateComment,
  useDeleteComment,
} from '../../hooks/useComments';
import { useAuth } from '../../contexts/AuthContext';
import { Button } from '../ui/Button';

import { Modal } from '../ui/Modal';
import { useToast } from '../../hooks/useToast';
import type { CommentDto } from '../../types/api';
import styles from './CommentThread.module.css';

interface CommentThreadProps {
  ticketId: string;
  isServiceUser?: boolean;
}

export const CommentThread: React.FC<CommentThreadProps> = ({
  ticketId,
  isServiceUser = false,
}) => {
  const { user } = useAuth();
  const { showToast } = useToast();
  const { data: comments, isLoading, isError } = useTicketComments(ticketId);
  const addCommentMutation = useAddComment();
  const addNoteMutation = useAddNote();
  const updateCommentMutation = useUpdateComment();
  const deleteCommentMutation = useDeleteComment();

  const [newCommentText, setNewCommentText] = useState('');
  const [isInternalNote, setIsInternalNote] = useState(false);
  const [editingComment, setEditingComment] = useState<CommentDto | null>(null);
  const [editText, setEditText] = useState('');
  const [deletingComment, setDeletingComment] = useState<CommentDto | null>(null);

  const handleSubmitComment = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newCommentText.trim()) return;

    try {
      if (isInternalNote && isServiceUser) {
        await addNoteMutation.mutateAsync({
          ticketId,
          data: { content: newCommentText },
        });
        showToast('Notatka wewnętrzna dodana', 'success');
      } else {
        await addCommentMutation.mutateAsync({
          ticketId,
          data: { content: newCommentText },
        });
        showToast('Komentarz dodany', 'success');
      }
      setNewCommentText('');
      setIsInternalNote(false);
    } catch {
      showToast('Nie udało się dodać komentarza', 'error');
    }
  };

  const handleEditComment = (comment: CommentDto) => {
    setEditingComment(comment);
    setEditText(comment.content);
  };

  const handleSaveEdit = async () => {
    if (!editingComment || !editText.trim()) return;

    try {
      await updateCommentMutation.mutateAsync({
        ticketId,
        commentId: editingComment.id,
        data: { content: editText },
      });
      showToast('Komentarz zaktualizowany', 'success');
      setEditingComment(null);
      setEditText('');
    } catch {
      showToast('Nie udało się zaktualizować komentarza', 'error');
    }
  };

  const handleDeleteComment = async () => {
    if (!deletingComment) return;

    try {
      await deleteCommentMutation.mutateAsync({
        ticketId,
        commentId: deletingComment.id,
      });
      showToast('Komentarz usunięty', 'success');
      setDeletingComment(null);
    } catch {
      showToast('Nie udało się usunąć komentarza', 'error');
    }
  };

  const canEditComment = (comment: CommentDto) => {
    return comment.authorId === user?.id;
  };

  const canDeleteComment = (comment: CommentDto) => {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    return comment.authorId === user?.id || (user as any)?.role === 'Admin';
  };

  const formatDateDistance = (dateString: string) => {
    const date = new Date(dateString);
    if (!isValid(date)) return 'nieznana data';
    try {
      return formatDistanceToNow(date, {
        addSuffix: true,
        locale: pl,
      });
    } catch {
      return 'nieznana data';
    }
  };

  if (isLoading) {
    return (
      <div className={styles.commentThread}>
        <div className={styles.loading}>
          <div className={styles.skeleton}></div>
          <div className={styles.skeleton}></div>
        </div>
      </div>
    );
  }

  if (isError) {
    return (
      <div className={styles.commentThread}>
        <div className={styles.error}>
          <p>Nie udało się załadować komentarzy.</p>
        </div>
      </div>
    );
  }

  const visibleComments = Array.isArray(comments)
    ? comments.filter((comment) => !comment.isInternal || isServiceUser)
    : [];

  return (
    <div className={styles.commentThread}>
      {/* Comment Form */}
      <form onSubmit={handleSubmitComment} className={styles.commentForm}>
        <textarea
          value={newCommentText}
          onChange={(e) => setNewCommentText(e.target.value)}
          placeholder={isInternalNote ? 'Dodaj notatkę wewnętrzną...' : 'Dodaj komentarz...'}
          rows={3}
          className={styles.textarea}
        />
        <div className={styles.formActions}>
          {isServiceUser && (
            <label className={styles.internalCheckbox}>
              <input
                type="checkbox"
                checked={isInternalNote}
                onChange={(e) => setIsInternalNote(e.target.checked)}
              />
              <span>Notatka wewnętrzna (tylko dla serwisu)</span>
            </label>
          )}
          <Button
            type="submit"
            disabled={
              !newCommentText.trim() || addCommentMutation.isPending || addNoteMutation.isPending
            }
          >
            {addCommentMutation.isPending || addNoteMutation.isPending ? 'Dodawanie...' : 'Dodaj'}
          </Button>
        </div>
      </form>

      {/* Comments List */}
      <div className={styles.commentsList}>
        {visibleComments.length === 0 ? (
          <div className={styles.empty}>
            <p>Brak komentarzy</p>
          </div>
        ) : (
          visibleComments.map((comment: CommentDto) => (
            <div
              key={comment.id}
              className={`${styles.comment} ${comment.isInternal ? styles.internalNote : ''}`}
            >
              <div className={styles.commentHeader}>
                <div className={styles.commentAuthor}>
                  <span className={styles.authorName}>{comment.authorName}</span>
                  {comment.isInternal && <span className={styles.internalBadge}>Wewnętrzne</span>}
                </div>
                <div className={styles.commentMeta}>
                  <span className={styles.commentTime}>
                    {formatDateDistance(comment.createdAt)}
                  </span>
                  {canEditComment(comment) && (
                    <button
                      type="button"
                      onClick={() => handleEditComment(comment)}
                      className={styles.editButton}
                      title="Edytuj"
                    >
                      ✏️
                    </button>
                  )}
                  {canDeleteComment(comment) && (
                    <button
                      type="button"
                      onClick={() => setDeletingComment(comment)}
                      className={styles.deleteButton}
                      title="Usuń"
                    >
                      🗑️
                    </button>
                  )}
                </div>
              </div>
              <div className={styles.commentContent}>{comment.content}</div>
              {comment.updatedAt && comment.updatedAt !== comment.createdAt && (
                <div className={styles.commentEdited}>
                  (edytowano {formatDateDistance(comment.updatedAt)})
                </div>
              )}
            </div>
          ))
        )}
      </div>

      {/* Edit Modal */}
      {editingComment && (
        <Modal isOpen={true} onClose={() => setEditingComment(null)} title="Edytuj komentarz">
          <div className={styles.modalContent}>
            <textarea
              value={editText}
              onChange={(e) => setEditText(e.target.value)}
              rows={4}
              className={styles.textarea}
            />
            <div className={styles.modalActions}>
              <Button variant="secondary" onClick={() => setEditingComment(null)}>
                Anuluj
              </Button>
              <Button
                onClick={handleSaveEdit}
                disabled={!editText.trim() || updateCommentMutation.isPending}
              >
                {updateCommentMutation.isPending ? 'Zapisywanie...' : 'Zapisz'}
              </Button>
            </div>
          </div>
        </Modal>
      )}

      {/* Delete Confirmation Modal */}
      {deletingComment && (
        <Modal isOpen={true} onClose={() => setDeletingComment(null)} title="Usuń komentarz">
          <div className={styles.modalContent}>
            <p>Czy na pewno chcesz usunąć ten komentarz?</p>
            <div className={styles.modalActions}>
              <Button variant="secondary" onClick={() => setDeletingComment(null)}>
                Anuluj
              </Button>
              <Button
                variant="danger"
                onClick={handleDeleteComment}
                disabled={deleteCommentMutation.isPending}
              >
                {deleteCommentMutation.isPending ? 'Usuwanie...' : 'Usuń'}
              </Button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
};

