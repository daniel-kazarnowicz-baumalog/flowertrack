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

// Helper functions for mapping status/priority to badge variants
export function getStatusColor(status: string): BadgeVariant {
  const variantMap: Record<string, BadgeVariant> = {
    New: 'info',
    Accepted: 'primary',
    InProgress: 'warning',
    Resolved: 'success',
    Closed: 'default',
    Reopened: 'danger',
  };
  return variantMap[status] || 'default';
}

export function getPriorityColor(priority: string): BadgeVariant {
  const variantMap: Record<string, BadgeVariant> = {
    Low: 'default',
    Medium: 'info',
    High: 'warning',
    Critical: 'danger',
  };
  return variantMap[priority] || 'default';
}

// Helper functions for ticket-specific badges
export function TicketStatusBadge({ status }: { status: string }) {
  return <Badge variant={getStatusColor(status)}>{status}</Badge>;
}

export function TicketPriorityBadge({ priority }: { priority: string }) {
  return <Badge variant={getPriorityColor(priority)}>{priority}</Badge>;
}
