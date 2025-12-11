/**
 * Timeline Component - Visual audit trail for ticket history
 * Displays chronological list of ticket events with icons and details
 */

import React from 'react';
import { formatDistanceToNow } from 'date-fns';
import { pl } from 'date-fns/locale';
import type { TicketHistoryEvent } from '../../types/api';
import styles from './Timeline.module.css';

interface TimelineProps {
  events: TicketHistoryEvent[];
  isLoading?: boolean;
}

const getEventIcon = (eventType: string): string => {
  const iconMap: Record<string, string> = {
    Created: '➕',
    StatusChanged: '🔄',
    PriorityChanged: '⚡',
    Assigned: '👤',
    Unassigned: '👥',
    Updated: '✏️',
    CommentAdded: '💬',
    NoteAdded: '📝',
    AttachmentAdded: '📎',
    AttachmentRemoved: '🗑️',
    Resolved: '✅',
    Closed: '🔒',
    Reopened: '🔓',
  };
  return iconMap[eventType] || '📋';
};

const getEventColor = (eventType: string): string => {
  if (eventType === 'Created') return styles.eventCreated;
  if (eventType === 'Resolved' || eventType === 'Closed') return styles.eventResolved;
  if (eventType === 'StatusChanged') return styles.eventStatusChange;
  if (eventType === 'PriorityChanged') return styles.eventPriorityChange;
  if (eventType === 'Assigned' || eventType === 'Unassigned') return styles.eventAssignment;
  if (eventType === 'CommentAdded' || eventType === 'NoteAdded') return styles.eventComment;
  if (eventType === 'AttachmentAdded' || eventType === 'AttachmentRemoved')
    return styles.eventAttachment;
  if (eventType === 'Reopened') return styles.eventReopened;
  return styles.eventDefault;
};

export const Timeline: React.FC<TimelineProps> = ({ events, isLoading = false }) => {
  if (isLoading) {
    return (
      <div className={styles.timeline}>
        <div className={styles.loading}>
          <div className={styles.skeleton}></div>
          <div className={styles.skeleton}></div>
          <div className={styles.skeleton}></div>
        </div>
      </div>
    );
  }

  if (!Array.isArray(events) || events.length === 0) {
    return (
      <div className={styles.timeline}>
        <div className={styles.empty}>
          <p>Brak historii zmian</p>
        </div>
      </div>
    );
  }

  const formatSafeDate = (dateString: string) => {
    if (!dateString) return '';
    try {
      const date = new Date(dateString);
      if (isNaN(date.getTime())) return '';
      return formatDistanceToNow(date, {
        addSuffix: true,
        locale: pl,
      });
    } catch {
      return '';
    }
  };

  return (
    <div className={styles.timeline}>
      <div className={styles.events}>
        {events.map((event, index) => (
          <div
            key={event.id || index}
            className={`${styles.event} ${getEventColor(event.eventType)}`}
          >
            <div className={styles.eventIcon}>
              <span>{getEventIcon(event.eventType)}</span>
            </div>
            <div className={styles.eventContent}>
              <div className={styles.eventHeader}>
                <span className={styles.eventType}>{event.description}</span>
                <span className={styles.eventTime}>
                  {formatSafeDate(event.createdAt)}
                </span>
              </div>
              <div className={styles.eventDetails}>
                <span className={styles.eventPerformer}>{event.performedByUserName}</span>
                {event.metadata && (
                  <div className={styles.eventMetadata}>
                    {Object.entries(event.metadata).map(([key, value]) => (
                      <div key={key} className={styles.metadataItem}>
                        <strong>{key}:</strong> {String(value)}
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
