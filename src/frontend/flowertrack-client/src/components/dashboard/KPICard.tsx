import React from 'react';
import './KPICard.css';

export interface KPICardProps {
  title: string;
  value: string | number;
  trend?: string;
  trendDirection?: 'up' | 'down' | 'neutral';
  icon?: React.ReactNode;
  variant?: 'default' | 'primary' | 'success' | 'warning' | 'danger' | 'info';
  onClick?: () => void;
  loading?: boolean;
}

export const KPICard: React.FC<KPICardProps> = ({
  title,
  value,
  trend,
  trendDirection = 'neutral',
  icon,
  variant = 'default',
  onClick,
  loading = false,
}) => {
  return (
    <div
      className={`kpi-card kpi-card--${variant} ${onClick ? 'kpi-card--clickable' : ''}`}
      onClick={onClick}
    >
      {loading ? (
        <div className="kpi-card__loading">
          <div className="kpi-card__loading-bar"></div>
        </div>
      ) : (
        <>
          <div className="kpi-card__header">
            <span className="kpi-card__title">{title}</span>
            {icon && <div className="kpi-card__icon">{icon}</div>}
          </div>
          <div className="kpi-card__content">
            <div className="kpi-card__value">{value}</div>
            {trend && (
              <div className={`kpi-card__trend kpi-card__trend--${trendDirection}`}>
                {trendDirection === 'up' && '↑'}
                {trendDirection === 'down' && '↓'}
                {trend}
              </div>
            )}
          </div>
          <div className="kpi-card__bg-icon">{icon}</div>
        </>
      )}
    </div>
  );
};
