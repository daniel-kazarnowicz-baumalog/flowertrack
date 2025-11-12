import { useServiceDashboardStats, useTicketTrends } from '../../hooks/useDashboard';
import { useAuth } from '../../contexts/AuthContext';
import { Loader } from '../../components/ui/Loader';
import { Card } from '../../components/ui/Card';
import {
  TicketTrendChart,
  StatusDistributionChart,
  PriorityDistributionChart,
} from '../../components/charts';
import './ServiceDashboard.css';

/**
 * ServiceDashboard - Main dashboard for service portal
 * Shows KPIs, charts, and recent activity
 */
export const ServiceDashboard = () => {
  const { user } = useAuth();
  const { data: stats, isLoading, error } = useServiceDashboardStats(user?.id);
  const { data: trends, isLoading: trendsLoading } = useTicketTrends();

  if (isLoading) {
    return (
      <div className="serviceDashboard">
        <div className="serviceDashboard__loading">
          <Loader size="lg" />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="serviceDashboard">
        <div className="serviceDashboard__error">
          <h2>Błąd ładowania dashboardu</h2>
          <p>Nie udało się pobrać statystyk. Spróbuj odświeżyć stronę.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="serviceDashboard">
      {/* Header */}
      <div className="serviceDashboard__header">
        <h1 className="serviceDashboard__title">Dashboard</h1>
        <p className="serviceDashboard__subtitle">Overview of tickets and system status</p>
      </div>

      {/* KPI Cards Grid */}
      <div className="serviceDashboard__grid">
        {/* Total Active Tickets */}
        <div className="serviceDashboard__kpiCard">
          <div className="serviceDashboard__kpiLabel">Active Tickets</div>
          <div className="serviceDashboard__kpiValue">{stats?.activeTicketsCount || 0}</div>
          <div className="serviceDashboard__kpiTrend">Tickets not closed or resolved</div>
        </div>

        {/* Critical Priority */}
        <div className="serviceDashboard__kpiCard serviceDashboard__kpiCard--critical">
          <div className="serviceDashboard__kpiLabel">Critical Priority</div>
          <div className="serviceDashboard__kpiValue">{stats?.criticalTicketsCount || 0}</div>
          <div className="serviceDashboard__kpiTrend">Requires immediate attention</div>
        </div>

        {/* My Assigned */}
        <div className="serviceDashboard__kpiCard serviceDashboard__kpiCard--assigned">
          <div className="serviceDashboard__kpiLabel">My Assigned</div>
          <div className="serviceDashboard__kpiValue">{stats?.myAssignedTicketsCount || 0}</div>
          <div className="serviceDashboard__kpiTrend">Tickets assigned to you</div>
        </div>

        {/* Unassigned */}
        <div className="serviceDashboard__kpiCard serviceDashboard__kpiCard--unassigned">
          <div className="serviceDashboard__kpiLabel">Unassigned</div>
          <div className="serviceDashboard__kpiValue">{stats?.unassignedTicketsCount || 0}</div>
          <div className="serviceDashboard__kpiTrend">Awaiting assignment</div>
        </div>
      </div>

      {/* Status Distribution */}
      <div className="serviceDashboard__row">
        <Card className="serviceDashboard__card">
          <h2 className="serviceDashboard__cardTitle">Tickets by Status</h2>
          <div className="serviceDashboard__statusGrid">
            {stats?.ticketsByStatus &&
              Object.entries(stats.ticketsByStatus).map(([status, count]) => (
                <div key={status} className="serviceDashboard__statusItem">
                  <div className="serviceDashboard__statusLabel">{status}</div>
                  <div className="serviceDashboard__statusValue">{count}</div>
                </div>
              ))}
          </div>
        </Card>

        <Card className="serviceDashboard__card">
          <h2 className="serviceDashboard__cardTitle">Tickets by Priority</h2>
          <div className="serviceDashboard__statusGrid">
            {stats?.ticketsByPriority &&
              Object.entries(stats.ticketsByPriority).map(([priority, count]) => (
                <div key={priority} className="serviceDashboard__statusItem">
                  <div className="serviceDashboard__statusLabel">{priority}</div>
                  <div className="serviceDashboard__statusValue">{count}</div>
                </div>
              ))}
          </div>
        </Card>
      </div>

      {/* Status and Priority Distribution - Grid */}
      <div className="serviceDashboard__row serviceDashboard__chartsRow">
        <StatusDistributionChart data={stats?.ticketsByStatus || {}} />
        <PriorityDistributionChart data={stats?.ticketsByPriority || {}} />
      </div>

      {/* Ticket Trends Chart */}
      {!trendsLoading && trends && <TicketTrendChart data={trends} />}

      {/* Recent Activity */}
      <Card className="serviceDashboard__activityCard">
        <h2 className="serviceDashboard__cardTitle">Recent Activity</h2>
        <div className="serviceDashboard__activityList">
          {stats?.recentActivity && stats.recentActivity.length > 0 ? (
            stats.recentActivity.map((activity) => (
              <div key={activity.id} className="serviceDashboard__activityItem">
                <div className="serviceDashboard__activityIcon">
                  {getActivityIcon(activity.type)}
                </div>
                <div className="serviceDashboard__activityContent">
                  <div className="serviceDashboard__activityTitle">{activity.ticketTitle}</div>
                  <div className="serviceDashboard__activityMeta">
                    {activity.user} • {getActivityLabel(activity.type)} •{' '}
                    {formatDistanceToNow(new Date(activity.timestamp))}
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className="serviceDashboard__empty">
              <p>No recent activity</p>
            </div>
          )}
        </div>
      </Card>

      {/* Charts Placeholder */}
      <div className="serviceDashboard__placeholder">
        <h2 className="serviceDashboard__placeholderTitle">Analytics & Charts</h2>
        <p className="serviceDashboard__placeholderText">
          Ticket trends, priority distribution, and organization insights will appear here.
        </p>
        <span className="serviceDashboard__comingSoon">Coming Soon</span>
      </div>
    </div>
  );
};

// Helper functions
function getActivityIcon(type: string): string {
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
}

function getActivityLabel(type: string): string {
  switch (type) {
    case 'created':
      return 'Created';
    case 'updated':
      return 'Updated';
    case 'assigned':
      return 'Assigned';
    case 'resolved':
      return 'Resolved';
    default:
      return 'Activity';
  }
}

function formatDistanceToNow(date: Date): string {
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffMins = Math.floor(diffMs / 60000);

  if (diffMins < 1) return 'just now';
  if (diffMins < 60) return `${diffMins}m ago`;
  const diffHours = Math.floor(diffMins / 60);
  if (diffHours < 24) return `${diffHours}h ago`;
  const diffDays = Math.floor(diffHours / 24);
  return `${diffDays}d ago`;
}
