import './ClientDashboard.css';

/**
 * ClientDashboard - Main dashboard for client portal
 * Shows machine status, ticket overview, and recent activity
 */
export const ClientDashboard = () => {
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
          <div className="clientDashboard__statusValue">--</div>
          <div className="clientDashboard__statusInfo">Coming soon</div>
        </div>

        <div className="clientDashboard__statusCard clientDashboard__statusCard--red">
          <div className="clientDashboard__statusLabel">Alarm Machines</div>
          <div className="clientDashboard__statusValue">--</div>
          <div className="clientDashboard__statusInfo">Coming soon</div>
        </div>

        <div className="clientDashboard__statusCard clientDashboard__statusCard--yellow">
          <div className="clientDashboard__statusLabel">Maintenance</div>
          <div className="clientDashboard__statusValue">--</div>
          <div className="clientDashboard__statusInfo">Coming soon</div>
        </div>

        <div className="clientDashboard__statusCard clientDashboard__statusCard--gray">
          <div className="clientDashboard__statusLabel">Inactive Machines</div>
          <div className="clientDashboard__statusValue">--</div>
          <div className="clientDashboard__statusInfo">Coming soon</div>
        </div>
      </div>

      {/* Tickets Overview */}
      <div className="clientDashboard__grid">
        <div className="clientDashboard__statusCard">
          <div className="clientDashboard__statusLabel">My Active Tickets</div>
          <div className="clientDashboard__statusValue">--</div>
          <div className="clientDashboard__statusInfo">Coming soon</div>
        </div>

        <div className="clientDashboard__statusCard">
          <div className="clientDashboard__statusLabel">All Organization Tickets</div>
          <div className="clientDashboard__statusValue">--</div>
          <div className="clientDashboard__statusInfo">Coming soon</div>
        </div>

        <div className="clientDashboard__statusCard">
          <div className="clientDashboard__statusLabel">Resolved This Week</div>
          <div className="clientDashboard__statusValue">--</div>
          <div className="clientDashboard__statusInfo">Coming soon</div>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="clientDashboard__placeholder">
        <h2 className="clientDashboard__placeholderTitle">Create Support Ticket</h2>
        <p className="clientDashboard__placeholderText">
          Need help with your equipment? Create a new support ticket and our team will assist you.
        </p>
        <button className="clientDashboard__actionButton" disabled>
          <svg width="20" height="20" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Create Ticket
        </button>
        <div className="clientDashboard__placeholderBadge">
          <span className="clientDashboard__comingSoon">Phase 2</span>
        </div>
      </div>

      {/* Recent Activity Placeholder */}
      <div className="clientDashboard__placeholder">
        <h2 className="clientDashboard__placeholderTitle">Recent Activity</h2>
        <p className="clientDashboard__placeholderText">
          Your recent tickets and updates will be displayed here.
        </p>
        <span className="clientDashboard__comingSoon">Phase 2</span>
      </div>
    </div>
  );
};
