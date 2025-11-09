import type { ReactNode } from 'react';
import './Badge.css';

export type BadgeVariant = 'default' | 'success' | 'warning' | 'danger' | 'info' | 'primary';
export type BadgeSize = 'sm' | 'md' | 'lg';

export interface BadgeProps {
  variant?: BadgeVariant;
  size?: BadgeSize;
  children: ReactNode;
  className?: string;
}

export function Badge({ variant = 'default', size = 'md', children, className = '' }: BadgeProps) {
  const baseClass = 'badge';
  const variantClass = `badge--${variant}`;
  const sizeClass = `badge--${size}`;

  const classes = [baseClass, variantClass, sizeClass, className].filter(Boolean).join(' ');

  return <span className={classes}>{children}</span>;
}

// Helper functions for ticket-specific badges
export function TicketStatusBadge({ status }: { status: string }) {
  const variantMap: Record<string, BadgeVariant> = {
    New: 'info',
    Accepted: 'primary',
    InProgress: 'warning',
    Resolved: 'success',
    Closed: 'default',
    Reopened: 'danger',
  };

  return <Badge variant={variantMap[status] || 'default'}>{status}</Badge>;
}

export function TicketPriorityBadge({ priority }: { priority: string }) {
  const variantMap: Record<string, BadgeVariant> = {
    Low: 'default',
    Medium: 'info',
    High: 'warning',
    Critical: 'danger',
  };

  return <Badge variant={variantMap[priority] || 'default'}>{priority}</Badge>;
}
