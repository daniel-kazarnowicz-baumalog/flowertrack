import React from 'react';
import { useTranslation } from 'react-i18next';
import './RecentActivityFeed.css';

interface ActivityItem {
  id: string;
  type: 'created' | 'updated' | 'assigned' | 'resolved';
  ticketId: string;
  ticketTitle: string;
  timestamp: string;
  user: string;
}

interface RecentActivityFeedProps {
  activities: ActivityItem[];
  className?: string;
  emptyMessage?: string;
}

export const RecentActivityFeed: React.FC<RecentActivityFeedProps> = ({
  activities,
  className = '',
  emptyMessage,
}) => {
  const { t } = useTranslation();

  const formatTimeAgo = (dateStr: string) => {
    const date = new Date(dateStr);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);

    if (diffMins < 1) return t('activity.justNow');
    if (diffMins < 60) return t('activity.minutesAgo', { count: diffMins });
    const diffHours = Math.floor(diffMins / 60);
    if (diffHours < 24) return t('activity.hoursAgo', { count: diffHours });
    const diffDays = Math.floor(diffHours / 24);
    return t('activity.daysAgo', { count: diffDays });
  };

  const getActivityIcon = (type: string) => {
    switch (type) {
      case 'created':
        return '📝';
      case 'updated':
        return '🔄';
      case 'assigned':
        return '👤';
      case 'resolved':
        return '✅';
      default:
        return '•';
    }
  };

  const getActivityColor = (type: string) => {
    switch (type) {
      case 'created':
        return 'activity-icon-wrapper--created';
      case 'updated':
        return 'activity-icon-wrapper--updated';
      case 'assigned':
        return 'activity-icon-wrapper--assigned';
      case 'resolved':
        return 'activity-icon-wrapper--resolved';
      default:
        return 'activity-icon-wrapper--default';
    }
  };

  if (!activities || activities.length === 0) {
    return (
      <div className={`activity-feed-empty ${className}`}>
        {emptyMessage || t('dashboard.noRecentActivity')}
      </div>
    );
  }

  return (
    <div className={`activity-feed ${className}`}>
      {activities.map((activity, index) => (
        <div key={activity.id} className="activity-item">
          <div className="activity-timeline">
            <div className={`activity-icon-wrapper ${getActivityColor(activity.type)}`}>
              <span className="activity-icon">{getActivityIcon(activity.type)}</span>
            </div>
            {index < activities.length - 1 && <div className="activity-line"></div>}
          </div>

          <div className="activity-content-wrapper">
            <div className="activity-header">
              <span className="activity-title">{activity.ticketTitle}</span>
              <span className="activity-time">{formatTimeAgo(activity.timestamp)}</span>
            </div>
            <div className="activity-details">
              <span className="activity-user">{activity.user}</span>
              <span className="activity-action"> • {t(`activity.${activity.type}`)}</span>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};
