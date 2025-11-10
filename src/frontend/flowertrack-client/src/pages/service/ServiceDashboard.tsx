import './ServiceDashboard.css';

/**
 * ServiceDashboard - Main dashboard for service portal
 * Shows KPIs, charts, and recent activity
 */
export const ServiceDashboard = () => {
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
          <div className="serviceDashboard__kpiValue">--</div>
          <div className="serviceDashboard__kpiTrend">Coming soon</div>
        </div>

        {/* Critical Priority */}
        <div className="serviceDashboard__kpiCard">
          <div className="serviceDashboard__kpiLabel">Critical Priority</div>
          <div className="serviceDashboard__kpiValue">--</div>
          <div className="serviceDashboard__kpiTrend">Coming soon</div>
        </div>

        {/* My Assigned */}
        <div className="serviceDashboard__kpiCard">
          <div className="serviceDashboard__kpiLabel">My Assigned</div>
          <div className="serviceDashboard__kpiValue">--</div>
          <div className="serviceDashboard__kpiTrend">Coming soon</div>
        </div>

        {/* Unassigned */}
        <div className="serviceDashboard__kpiCard">
          <div className="serviceDashboard__kpiLabel">Unassigned</div>
          <div className="serviceDashboard__kpiValue">--</div>
          <div className="serviceDashboard__kpiTrend">Coming soon</div>
        </div>
      </div>

      {/* Charts Placeholder */}
      <div className="serviceDashboard__placeholder">
        <h2 className="serviceDashboard__placeholderTitle">Analytics & Charts</h2>
        <p className="serviceDashboard__placeholderText">
          Ticket trends, priority distribution, and organization insights will appear here.
        </p>
        <span className="serviceDashboard__comingSoon">Phase 3</span>
      </div>

      {/* Recent Activity Placeholder */}
      <div className="serviceDashboard__placeholder">
        <h2 className="serviceDashboard__placeholderTitle">Recent Activity</h2>
        <p className="serviceDashboard__placeholderText">
          Latest ticket updates and system events will be displayed here.
        </p>
        <span className="serviceDashboard__comingSoon">Phase 2</span>
      </div>
    </div>
  );
};
