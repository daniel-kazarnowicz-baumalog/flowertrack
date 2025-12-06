import { useTranslation } from 'react-i18next';
import { useServiceDashboardStats, useTicketTrends } from '../../hooks/useDashboard';
import { useAuth } from '../../contexts/AuthContext';
import { Loader } from '../../components/ui/Loader';
import { Card } from '../../components/ui/Card';
import { KPICard } from '../../components/dashboard/KPICard';
import { RecentActivityFeed } from '../../components/dashboard/RecentActivityFeed';
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
  const { t } = useTranslation();
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
          <h2>{t('errors.dashboardLoadError')}</h2>
          <p>{t('errors.tryRefresh')}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="serviceDashboard">
      {/* Header */}
      <div className="serviceDashboard__header">
        <h1 className="serviceDashboard__title">{t('dashboard.title')}</h1>
        <p className="serviceDashboard__subtitle">{t('dashboard.subtitle')}</p>
      </div>

      {/* KPI Cards Grid */}
      <div className="serviceDashboard__grid">
        {/* Total Active Tickets */}
        <KPICard
          title={t('dashboard.activeTickets')}
          value={stats?.activeTicketsCount || 0}
          trend={t('dashboard.ticketsNotClosed')}
          variant="primary"
          icon={<span>🎫</span>} // TODO: Use real icons
        />

        {/* Critical Priority */}
        <KPICard
          title={t('dashboard.criticalPriority')}
          value={stats?.criticalTicketsCount || 0}
          trend={t('dashboard.requiresImmediate')}
          variant="danger"
          icon={<span>🔥</span>}
        />

        {/* My Assigned */}
        <KPICard
          title={t('dashboard.myAssigned')}
          value={stats?.myAssignedTicketsCount || 0}
          trend={t('dashboard.ticketsAssignedToYou')}
          variant="info"
          icon={<span>👤</span>}
        />

        {/* Unassigned */}
        <KPICard
          title={t('dashboard.unassigned')}
          value={stats?.unassignedTicketsCount || 0}
          trend={t('dashboard.awaitingAssignment')}
          variant="warning"
          icon={<span>⚠️</span>}
        />
      </div>

      {/* Status Distribution */}
      <div className="serviceDashboard__row">
        <Card className="serviceDashboard__card">
          <StatusDistributionChart data={stats?.ticketsByStatus || {}} title={t('dashboard.ticketsByStatus')} />
        </Card>

        <Card className="serviceDashboard__card">
          <PriorityDistributionChart data={stats?.ticketsByPriority || {}} title={t('dashboard.ticketsByPriority')} />
        </Card>
      </div>

      {/* Ticket Trends Chart */}
      <div className="serviceDashboard__fullRow">
        <Card className="serviceDashboard__card">
          {!trendsLoading && trends && <TicketTrendChart data={trends} />}
        </Card>
      </div>

      {/* Recent Activity */}
      <div className="serviceDashboard__fullRow">
        <Card className="serviceDashboard__activityCard">
          <h2 className="serviceDashboard__cardTitle">{t('dashboard.recentActivity')}</h2>
          <RecentActivityFeed
            activities={stats?.recentActivity || []}
            className="serviceDashboard__activityList"
          />
        </Card>
      </div>
    </div>
  );
};
