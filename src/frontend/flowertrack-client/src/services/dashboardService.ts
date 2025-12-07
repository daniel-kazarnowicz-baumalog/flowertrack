/**
 * Service for aggregating dashboard statistics from various endpoints.
 * Since no dedicated dashboard API exists, this service combines data from
 * tickets, machines, and organizations endpoints to generate statistics.
 */

import { apiClient } from '../lib/apiClient';
import type { TicketDto } from '../types/api';

export interface ServiceDashboardStats {
  activeTicketsCount: number;
  criticalTicketsCount: number;
  myAssignedTicketsCount: number;
  unassignedTicketsCount: number;
  activeAlarmsCount: number;
  resolvedThisWeekCount: number;
  ticketsByStatus: Record<string, number>;
  ticketsByPriority: Record<string, number>;
  recentActivity: Array<{
    id: string;
    type: 'created' | 'updated' | 'assigned' | 'resolved';
    ticketId: string;
    ticketTitle: string;
    timestamp: string;
    user: string;
  }>;
  organizationsWithAlarms: Array<{
    id: string;
    name: string;
    alarmCount: number;
  }>;
  upcomingMaintenance: Array<{
    id: string;
    name: string;
    organizationName: string;
    date: string;
  }>;
}

export interface ClientDashboardStats {
  activeMachinesCount: number;
  machinesInAlarmCount: number;
  machinesInMaintenanceCount: number;
  inactiveMachinesCount: number;
  myActiveTicketsCount: number;
  allOrgTicketsCount: number;
  resolvedThisWeekCount: number;
  ticketsByStatus: Record<string, number>;
  recentActivity: Array<{
    id: string;
    type: 'created' | 'updated' | 'assigned' | 'resolved';
    ticketId: string;
    ticketTitle: string;
    timestamp: string;
    user: string;
  }>;
}

export interface TicketTrend {
  date: string;
  created: number;
  resolved: number;
  total: number;
}

/**
 * Get service dashboard statistics
 */
export async function getServiceDashboardStats(userId?: string): Promise<ServiceDashboardStats> {
  // Fetch all tickets to aggregate stats
  const response = await apiClient.get<{
    items: TicketDto[];
    totalCount: number;
  }>('/tickets', {
    params: {
      pageSize: 100, // Max allowed by backend
      page: 1,
    },
  });

  const tickets = response.data.items.map(mapTicketEnums);

  // Calculate stats
  const activeTickets = tickets.filter((t) => t.status !== 'Closed' && t.status !== 'Resolved');
  const criticalTickets = tickets.filter((t) => t.priority === 'Critical');
  const myAssignedTickets = userId ? tickets.filter((t) => t.assignedToId === userId) : [];
  const unassignedTickets = tickets.filter((t) => !t.assignedToId);

  // Group tickets by status
  const ticketsByStatus = tickets.reduce<Record<string, number>>(
    (acc, ticket) => {
      acc[ticket.status] = (acc[ticket.status] || 0) + 1;
      return acc;
    },
    {} as Record<string, number>
  );

  // Group tickets by priority
  const ticketsByPriority = tickets.reduce<Record<string, number>>(
    (acc, ticket) => {
      acc[ticket.priority] = (acc[ticket.priority] || 0) + 1;
      return acc;
    },
    {} as Record<string, number>
  );

  // Recent activity (last 10 tickets sorted by updatedAt)
  const recentTickets = [...tickets]
    .sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime())
    .slice(0, 10);

  const recentActivity = recentTickets.map((ticket) => ({
    id: ticket.id,
    type: determineActivityType(ticket),
    ticketId: ticket.id,
    ticketTitle: ticket.title,
    timestamp: ticket.updatedAt,
    user: ticket.assignedToName || 'Nieprzypisany',
  }));

  // Calculate resolved this week
  const oneWeekAgo = new Date();
  oneWeekAgo.setDate(oneWeekAgo.getDate() - 7);
  const resolvedThisWeekCount = tickets.filter(
    (t) => (t.status === 'Resolved' || t.status === 'Closed') && new Date(t.updatedAt) >= oneWeekAgo
  ).length;

  return {
    activeTicketsCount: activeTickets.length,
    criticalTicketsCount: criticalTickets.length,
    myAssignedTicketsCount: myAssignedTickets.length,
    unassignedTicketsCount: unassignedTickets.length,
    activeAlarmsCount: 0, // TODO: Integrate with machine alarms API when available
    resolvedThisWeekCount,
    ticketsByStatus,
    ticketsByPriority,
    recentActivity,
    organizationsWithAlarms: [], // TODO: Integrate with machine alarms API when available
    upcomingMaintenance: [], // TODO: Integrate with maintenance schedule API when available
  };
}

/**
 * Get client dashboard statistics
 */
export async function getClientDashboardStats(
  organizationId: string
): Promise<ClientDashboardStats> {
  // Fetch tickets for organization
  const ticketsResponse = await apiClient.get<{
    items: TicketDto[];
    totalCount: number;
  }>('/tickets', {
    params: {
      organizationId,
      pageSize: 100,
      page: 1,
    },
  });

  const tickets = ticketsResponse.data.items.map(mapTicketEnums);

  // Fetch machines for organization
  const machinesResponse = await apiClient.get<Array<{ id: string; status: string }>>('/machines', {
    params: {
      organizationId,
    },
  });

  const machines = machinesResponse.data;

  // Calculate machine stats
  const activeMachines = machines.filter((m) => m.status === 'Active');
  const machinesInAlarm = machines.filter((m) => m.status === 'Alarm');
  const machinesInMaintenance = machines.filter((m) => m.status === 'Maintenance');
  const inactiveMachines = machines.filter((m) => m.status === 'Inactive');

  // Calculate ticket stats
  const myActiveTickets = tickets.filter((t) => t.status !== 'Closed' && t.status !== 'Resolved');
  const allOrgTickets = tickets;

  // Resolved this week
  const oneWeekAgo = new Date();
  oneWeekAgo.setDate(oneWeekAgo.getDate() - 7);
  const resolvedThisWeek = tickets.filter(
    (t) => t.status === 'Resolved' && new Date(t.updatedAt) >= oneWeekAgo
  );

  // Group by status
  const ticketsByStatus = tickets.reduce<Record<string, number>>(
    (acc, ticket) => {
      acc[ticket.status] = (acc[ticket.status] || 0) + 1;
      return acc;
    },
    {} as Record<string, number>
  );

  // Recent activity (last 10 tickets sorted by updatedAt)
  const recentTickets = [...tickets]
    .sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime())
    .slice(0, 10);

  const recentActivity = recentTickets.map((ticket) => ({
    id: ticket.id,
    type: determineActivityType(ticket),
    ticketId: ticket.id,
    ticketTitle: ticket.title,
    timestamp: ticket.updatedAt,
    user: ticket.assignedToName || 'Nieprzypisany',
  }));

  return {
    activeMachinesCount: activeMachines.length,
    machinesInAlarmCount: machinesInAlarm.length,
    machinesInMaintenanceCount: machinesInMaintenance.length,
    inactiveMachinesCount: inactiveMachines.length,
    myActiveTicketsCount: myActiveTickets.length,
    allOrgTicketsCount: allOrgTickets.length,
    resolvedThisWeekCount: resolvedThisWeek.length,
    ticketsByStatus,
    recentActivity,
  };
}

/**
 * Get ticket trends over time (last 30 days)
 */
export async function getTicketTrends(organizationId?: string): Promise<TicketTrend[]> {
  const response = await apiClient.get<{
    items: TicketDto[];
    totalCount: number;
  }>('/tickets', {
    params: {
      organizationId,
      pageSize: 100,
      page: 1,
    },
  });

  const tickets = response.data.items.map(mapTicketEnums);

  // Generate last 30 days
  const trends: TicketTrend[] = [];
  const today = new Date();

  for (let i = 29; i >= 0; i--) {
    const date = new Date(today);
    date.setDate(date.getDate() - i);
    const dateStr = date.toISOString().split('T')[0];

    const created = tickets.filter((t) => t.createdAt.split('T')[0] === dateStr).length;
    const resolved = tickets.filter(
      (t) => t.status === 'Resolved' && t.updatedAt.split('T')[0] === dateStr
    ).length;

    // Running total up to this date
    const totalUpToDate = tickets.filter((t) => new Date(t.createdAt) <= date).length;

    trends.push({
      date: dateStr,
      created,
      resolved,
      total: totalUpToDate,
    });
  }

  return trends;
}

/**
 * Map backend integer enums to frontend string unions
 */
function mapTicketEnums(ticket: any): TicketDto {
  const statusMap: Record<number, string> = {
    0: 'New',
    1: 'Accepted',
    2: 'InProgress',
    3: 'Resolved',
    4: 'Closed',
    5: 'Reopened',
  };
  const priorityMap: Record<number, string> = {
    0: 'Low',
    1: 'Medium',
    2: 'High',
    3: 'Critical',
  };

  return {
    ...ticket,
    status: typeof ticket.status === 'number' ? statusMap[ticket.status] || 'New' : ticket.status,
    priority:
      typeof ticket.priority === 'number'
        ? priorityMap[ticket.priority] || 'Medium'
        : ticket.priority,
  };
}

/**
 * Determine activity type from ticket state
 */
function determineActivityType(ticket: TicketDto): 'created' | 'updated' | 'assigned' | 'resolved' {
  if (ticket.status === 'Resolved') return 'resolved';
  if (ticket.assignedToId) return 'assigned';
  if (new Date(ticket.updatedAt).getTime() - new Date(ticket.createdAt).getTime() > 60000)
    return 'updated';
  return 'created';
}
