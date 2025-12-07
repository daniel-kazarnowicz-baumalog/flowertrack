import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { useServiceDashboardStats, useTicketTrends } from '../../hooks/useDashboard';
import { useAuth } from '../../contexts/AuthContext';
import { Loader } from '../../components/ui/Loader';
import { Card } from '../../components/ui/Card';
import { KPICard } from '../../components/dashboard/KPICard';
import { RecentActivityFeed } from '../../components/dashboard/RecentActivityFeed';
import { QuickActions } from '../../components/dashboard/QuickActions';
import { DashboardList } from '../../components/dashboard/DashboardList';
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
  const navigate = useNavigate();
  const { data: stats, isLoading, error } = useServiceDashboardStats(user?.id);
  const { data: trends } = useTicketTrends();

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
        <div>
          <h1 className="serviceDashboard__title">{t('dashboard.title')}</h1>
          <p className="serviceDashboard__subtitle">{t('dashboard.subtitle')}</p>
        </div>
      </div>

      {/* Quick Actions */}
      <QuickActions />

      {/* KPI Cards Grid */}
      <div className="serviceDashboard__grid">
        {/* Total Active Tickets */}
        <KPICard
          title={t('dashboard.activeTickets')}
          value={stats?.activeTicketsCount || 0}
          trend={t('dashboard.ticketsNotClosed')}
          variant="primary"
          icon={<span>🎫</span>}
          onClick={() => navigate('/service/tickets?status=active')}
        />

        {/* Critical Priority */}
        <KPICard
          title={t('dashboard.criticalPriority')}
          value={stats?.criticalTicketsCount || 0}
          trend={t('dashboard.requiresImmediate')}
          variant="danger"
          icon={<span>🔥</span>}
          onClick={() => navigate('/service/tickets?priority=Critical')}
        />

        {/* Active Alarms */}
        <KPICard
          title="Aktywne Alarmy"
          value={stats?.activeAlarmsCount || 0}
          trend="Maszyny wymagające uwagi"
          variant="danger"
          icon={<span>🚨</span>}
          onClick={() => navigate('/service/machines?status=alarm')}
        />

        {/* Resolved This Week */}
        <KPICard
          title="Rozwiązane (Tydzień)"
          value={stats?.resolvedThisWeekCount || 0}
          trend="Zamknięte w ostatnich 7 dniach"
          variant="success"
          icon={<span>✅</span>}
          onClick={() => navigate('/service/tickets?status=resolved')}
        />
      </div>

      {/* Charts Row */}
      <div className="serviceDashboard__row">
        <Card className="serviceDashboard__card">
          <StatusDistributionChart
            data={stats?.ticketsByStatus || {}}
            title={t('dashboard.ticketsByStatus')}
          />
        </Card>

        <Card className="serviceDashboard__card">
          <PriorityDistributionChart
            data={stats?.ticketsByPriority || {}}
            title={t('dashboard.ticketsByPriority')}
          />
        </Card>
      </div>

      {/* Trend Chart */}
      <div className="serviceDashboard__row">
        <Card className="serviceDashboard__card serviceDashboard__card--full">
          <TicketTrendChart data={trends || []} title={t('dashboard.ticketTrends')} />
        </Card>
      </div>

      {/* Bottom Section: Activity & Lists */}
      <div className="serviceDashboard__bottomGrid">
        {/* Recent Activity */}
        <div className="serviceDashboard__activitySection">
          <h2 className="serviceDashboard__sectionTitle">{t('dashboard.recentActivity')}</h2>
          <RecentActivityFeed activities={stats?.recentActivity || []} />
        </div>

        {/* Organizations with Alarms */}
        <div className="serviceDashboard__listSection">
          <DashboardList
            title="Organizacje z Alarmami"
            items={(stats?.organizationsWithAlarms || []).map((org) => ({
              id: org.id,
              title: org.name,
              badge: `${org.alarmCount} alarmów`,
              badgeType: 'danger',
              onClick: () => navigate(`/service/organizations/${org.id}`),
            }))}
            emptyMessage="Brak aktywnych alarmów"
          />
        </div>

        {/* Upcoming Maintenance */}
        <div className="serviceDashboard__listSection">
          <DashboardList
            title="Nadchodzące Przeglądy"
            items={(stats?.upcomingMaintenance || []).map((machine) => ({
              id: machine.id,
              title: machine.name,
              subtitle: `${machine.organizationName} - ${new Date(machine.date).toLocaleDateString()}`,
              badge: 'Wkrótce',
              badgeType: 'warning',
              onClick: () => navigate(`/service/machines/${machine.id}`),
            }))}
            emptyMessage="Brak nadchodzących przeglądów"
          />
        </div>
      </div>
    </div>
  );
};
