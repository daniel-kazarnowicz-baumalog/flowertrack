import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { useClientDashboardStats } from '../../hooks/useDashboard';
import { Loader } from '../../components/ui/Loader';
import { KPICard } from '../../components/dashboard/KPICard';
import { RecentActivityFeed } from '../../components/dashboard/RecentActivityFeed';
import { StatusDistributionChart } from '../../components/charts';
import { CreateTicketModal } from '../../components/tickets/CreateTicketModal';
import type { CreateTicketRequest } from '../../types/api';
import ticketService from '../../services/ticketService';
import './ClientDashboard.css';

const ClientDashboard = () => {
  const { t } = useTranslation();
  const { user } = useAuth();
  const navigate = useNavigate();
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

  // Use user.organizationId or a fallback if not present
  const { data: stats, isLoading } = useClientDashboardStats(user?.organizationId || '');

  const handleCreateTicket = async (data: CreateTicketRequest, files: File[]) => {
    try {
      const ticket = await ticketService.createTicket(data);
      if (files.length > 0) {
        await Promise.all(files.map((file) => ticketService.uploadAttachment(ticket.id, file)));
      }
      setIsCreateModalOpen(false);
      navigate('/client/tickets');
    } catch (error) {
      console.error('Failed to create ticket', error);
    }
  };

  if (isLoading) {
    return (
      <div className="clientDashboard__loading">
        <Loader size="lg" />
      </div>
    );
  }

  const currentDate = new Date().toLocaleDateString('pl-PL', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });

  return (
    <div className="clientDashboard">
      {/* Welcome Header */}
      <header className="clientDashboard__header">
        <div className="clientDashboard__welcome">
          <h1>
            {t('auth.welcome', 'Witaj')}, {user?.name?.split(' ')[0]}! 👋
          </h1>
          <span className="clientDashboard__date">{currentDate}</span>
        </div>
        <div className="clientDashboard__quickAction">
          <button
            className="clientDashboard__newTicketBtn"
            onClick={() => setIsCreateModalOpen(true)}
          >
            <span>+</span>
            {t('tickets.create', 'Nowe Zgłoszenie')}
          </button>
        </div>
      </header>

      {/* Machines Overview */}
      <section className="clientDashboard__section">
        <h2 className="clientDashboard__sectionTitle">Twoje Maszyny</h2>
        <div className="clientDashboard__stats">
          <KPICard
            title="Aktywne Maszyny"
            value={stats?.activeMachinesCount || 0}
            variant="success"
            icon={<span>🏭</span>}
          />
          <KPICard
            title="Wymaga Uwagi"
            value={stats?.machinesInAlarmCount || 0}
            variant={stats?.machinesInAlarmCount ? 'danger' : 'success'}
            trend={stats?.machinesInAlarmCount ? 'Maszyny w alarmie' : 'Wszystko w porządku'}
            icon={<span>🚨</span>}
          />
          <KPICard
            title="Przeglądy"
            value={stats?.machinesInMaintenanceCount || 0}
            variant="warning"
            trend="W trakcie konserwacji"
            icon={<span>🔧</span>}
          />
        </div>
      </section>

      {/* Tickets Overview */}
      <div className="clientDashboard__grid">
        <div className="clientDashboard__card">
          <h2 className="clientDashboard__sectionTitle">Ostatnia Aktywność</h2>
          <RecentActivityFeed
            activities={stats?.recentActivity || []}
            className="clientDashboard__activities"
          />
        </div>

        <div className="clientDashboard__card">
          <h2 className="clientDashboard__sectionTitle">Status Zgłoszeń</h2>
          <StatusDistributionChart data={stats?.ticketsByStatus || {}} title="" />
        </div>
      </div>

      <CreateTicketModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onSubmit={handleCreateTicket}
        machines={[]} // TODO: Fetch machines from API
      />
    </div>
  );
};

export default ClientDashboard;
