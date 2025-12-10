import { useTranslation } from 'react-i18next';
import { Outlet } from 'react-router-dom';
import { AppShell } from './AppShell';

export const ServiceLayout = () => {
  const { t } = useTranslation();

  const navSections = [
    {
      title: t('nav.main', 'Główne'),
      items: [
        {
          to: '/service/dashboard',
          icon: '📊',
          label: t('nav.dashboard', 'Dashboard'),
        },
        {
          to: '/service/tickets',
          icon: '🎫',
          label: t('nav.tickets', 'Zgłoszenia'),
        },
      ],
    },
    {
      title: t('nav.manage', 'Zarządzanie'),
      items: [
        {
          to: '/service/organizations',
          icon: '🏢',
          label: t('nav.clients', 'Klienci'),
        },
        {
          to: '/service/machines',
          icon: '🏭',
          label: t('nav.machines', 'Maszyny'),
        },
        {
          to: '/service/users',
          icon: '👥',
          label: t('nav.team', 'Zespół'),
        },
      ],
    },
  ];

  return (
    <AppShell navSections={navSections} logoLink="/service/dashboard" logoText="FLOWerTRACK">
      <Outlet />
    </AppShell>
  );
};
