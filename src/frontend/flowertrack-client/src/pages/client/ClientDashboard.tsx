import { useClientDashboardStats, useTicketTrends } from '../../hooks/useDashboard';
import { useAuth } from '../../contexts/AuthContext';
import { Loader } from '../../components/ui/Loader';
import { TicketTrendChart, StatusDistributionChart } from '../../components/charts';
import './ClientDashboard.css';

/**
 * ClientDashboard - Main dashboard for client portal
 * Shows machine status, ticket overview, and recent activity
 */
export const ClientDashboard = () => {
  const { user } = useAuth();
  // Type guard to check if user is ClientAuthUser with organizationId
  const organizationId =
    user && 'organizationId' in user ? (user as { organizationId: string }).organizationId : '';

  const { data: stats, isLoading, error } = useClientDashboardStats(organizationId);
  const { data: trends, isLoading: trendsLoading } = useTicketTrends(organizationId);

  if (isLoading) {
    return (
      <div className="clientDashboard">
        <div className="clientDashboard__loading">
          <Loader size="lg" />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="clientDashboard">
        <div className="clientDashboard__error">
          <p>Failed to load dashboard data. Please try refreshing the page.</p>
        </div>
      </div>
    );
  }

  const getActivityIcon = (type: string): string => {
    switch (type) {
      case 'created':
        return '🆕';
      case 'updated':
        return '🔄';
      case 'assigned':
        return '👤';
      case 'resolved':
        return '✅';
      default:
        return '📋';
    }
  };

  const getActivityLabel = (type: string): string => {
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
  };

  const formatDistanceToNow = (dateString: string): string => {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays < 7) return `${diffDays}d ago`;
    return date.toLocaleDateString();
  };

  return (
    <div className="clientDashboard">
      {/* Header */}
      <div className="clientDashboard__header">
        <h1 className="clientDashboard__title">Dashboard</h1>
        <p className="clientDashboard__subtitle">Monitor your machines and support tickets</p>
      </div>

      {/* Machine Status Cards */}
      <div className="clientDashboard__grid">
        <div className="clientDashboard__statusCard clientDashboard__statusCard--green">
          <div className="clientDashboard__statusLabel">Active Machines</div>
          <div className="clientDashboard__statusValue">{stats?.activeMachinesCount || 0}</div>
          <div className="clientDashboard__statusInfo">Operating normally</div>
        </div>

        <div className="clientDashboard__statusCard clientDashboard__statusCard--red">
          <div className="clientDashboard__statusLabel">Alarm Machines</div>
          <div className="clientDashboard__statusValue">{stats?.machinesInAlarmCount || 0}</div>
          <div className="clientDashboard__statusInfo">Require attention</div>
        </div>

        <div className="clientDashboard__statusCard clientDashboard__statusCard--yellow">
          <div className="clientDashboard__statusLabel">Maintenance</div>
          <div className="clientDashboard__statusValue">
            {stats?.machinesInMaintenanceCount || 0}
          </div>
          <div className="clientDashboard__statusInfo">Under maintenance</div>
        </div>

        <div className="clientDashboard__statusCard clientDashboard__statusCard--gray">
          <div className="clientDashboard__statusLabel">Inactive Machines</div>
          <div className="clientDashboard__statusValue">{stats?.inactiveMachinesCount || 0}</div>
          <div className="clientDashboard__statusInfo">Not in use</div>
        </div>
      </div>

      {/* Tickets Overview */}
      <div className="clientDashboard__grid">
        <div className="clientDashboard__statusCard clientDashboard__statusCard--blue">
          <div className="clientDashboard__statusLabel">My Active Tickets</div>
          <div className="clientDashboard__statusValue">{stats?.myActiveTicketsCount || 0}</div>
          <div className="clientDashboard__statusInfo">Your open tickets</div>
        </div>

        <div className="clientDashboard__statusCard clientDashboard__statusCard--purple">
          <div className="clientDashboard__statusLabel">All Organization Tickets</div>
          <div className="clientDashboard__statusValue">{stats?.allOrgTicketsCount || 0}</div>
          <div className="clientDashboard__statusInfo">Team's total tickets</div>
        </div>

        <div className="clientDashboard__statusCard clientDashboard__statusCard--green">
          <div className="clientDashboard__statusLabel">Resolved This Week</div>
          <div className="clientDashboard__statusValue">{stats?.resolvedThisWeekCount || 0}</div>
          <div className="clientDashboard__statusInfo">Last 7 days</div>
        </div>
      </div>

      {/* Ticket Status and Trends Charts */}
      <div className="clientDashboard__chartsRow">
        <StatusDistributionChart data={stats?.ticketsByStatus || {}} />
      </div>

      {/* Ticket Trends Chart */}
      {!trendsLoading && trends && <TicketTrendChart data={trends} />}

      {/* Recent Activity */}
      <div className="clientDashboard__activityCard">
        <h3 className="clientDashboard__cardTitle">Recent Activity</h3>
        {stats?.recentActivity && stats.recentActivity.length > 0 ? (
          <div className="clientDashboard__activityList">
            {stats.recentActivity.map((activity) => (
              <div key={activity.id} className="clientDashboard__activityItem">
                <div className="clientDashboard__activityIcon">
                  {getActivityIcon(activity.type)}
                </div>
                <div className="clientDashboard__activityContent">
                  <div className="clientDashboard__activityTitle">
                    {getActivityLabel(activity.type)}: {activity.ticketTitle}
                  </div>
                  <div className="clientDashboard__activityMeta">
                    {activity.user} • {formatDistanceToNow(activity.timestamp)}
                  </div>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <div className="clientDashboard__empty">No recent activity</div>
        )}
      </div>
    </div>
  );
};
