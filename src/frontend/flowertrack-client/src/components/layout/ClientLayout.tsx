import { useTranslation } from 'react-i18next';
import { Outlet } from 'react-router-dom';
import { AppShell } from './AppShell';

export const ClientLayout = () => {
  const { t } = useTranslation();

  const navSections = [
    {
      items: [
        {
          to: '/client/dashboard',
          icon: '📊',
          label: t('nav.dashboard', 'Pulpit'),
        },
        {
          to: '/client/tickets',
          icon: '🎫',
          label: t('nav.tickets', 'Zgłoszenia'),
        },
        {
          to: '/client/team',
          icon: '👥',
          label: t('nav.team', 'Mój Zespół'),
        },
      ],
    },
  ];

  return (
    <AppShell navSections={navSections} logoLink="/client/dashboard" logoText="FLOWerTRACK Client">
      <Outlet />
    </AppShell>
  );
};
