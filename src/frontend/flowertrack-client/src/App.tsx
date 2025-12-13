import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Gateway from './pages/Gateway';
import ServiceLogin from './pages/service/ServiceLoginPage';
import ClientLogin from './pages/client/ClientLoginPage';
import ProtectedRoute from './components/ProtectedRoute';
import { ServiceDashboard } from './pages/service/ServiceDashboard';
import ClientDashboard from './pages/client/Dashboard';
import NotFound from './pages/NotFound';
import { ServiceLayout } from './components/layout/ServiceLayout';
import { ClientLayout } from './components/layout/ClientLayout';
import { PublicLayout } from './components/layout/PublicLayout';

// Service Pages
import { ServiceTicketsPage } from './pages/service/ServiceTicketsPage';
import { ServiceTicketDetailPage } from './pages/service/ServiceTicketDetailPage';
import { OrganizationsListPage } from './pages/service/OrganizationsListPage';
import { OrganizationDetailPage } from './pages/service/OrganizationDetailPage';
import { MachinesListPage } from './pages/service/MachinesListPage';
import { MachineDetailPage } from './pages/service/MachineDetailPage';
import { ServiceUsersListPage } from './pages/service/ServiceUsersListPage';
import { ServiceForgotPasswordPage } from './pages/service/ServiceForgotPasswordPage';
import { ServiceResetPasswordPage } from './pages/service/ServiceResetPasswordPage';

// Client Pages
import { ClientTicketsPage } from './pages/client/ClientTicketsPage';
import { ClientTicketDetailPage } from './pages/client/ClientTicketDetailPage';
import { OrganizationTeamPage } from './pages/client/OrganizationTeamPage';
import { ClientActivatePage } from './pages/client/ClientActivatePage';

function App() {
  return (
    <BrowserRouter basename="/flowertrack">
      <Routes>
        {/* Public Routes with Persistent Layout */}
        <Route element={<PublicLayout />}>
          <Route path="/" element={<Gateway />} />
          <Route path="/service" element={<ServiceLogin />} />
          <Route path="/client" element={<ClientLogin />} />
          <Route path="/client/activate" element={<ClientActivatePage />} />
          <Route path="/service/forgot-password" element={<ServiceForgotPasswordPage />} />
          <Route path="/service/reset-password" element={<ServiceResetPasswordPage />} />
        </Route>

        {/* Service - Protected Routes */}
        {/* Service - Protected Routes */}
        <Route
          path="/service/*"
          element={
            <ProtectedRoute requiredRole="service">
              <ServiceLayout />
            </ProtectedRoute>
          }
        >
          <Route path="dashboard" element={<ServiceDashboard />} />
          <Route path="tickets" element={<ServiceTicketsPage />} />
          <Route path="tickets/:id" element={<ServiceTicketDetailPage />} />
          <Route path="organizations" element={<OrganizationsListPage />} />
          <Route path="organizations/:id" element={<OrganizationDetailPage />} />
          <Route path="machines" element={<MachinesListPage />} />
          <Route path="machines/:id" element={<MachineDetailPage />} />
          <Route path="users" element={<ServiceUsersListPage />} />
          <Route path="*" element={<NotFound />} />
        </Route>

        {/* Client - Protected Routes CLONE TEST */}
        <Route
          path="/client/*"
          element={
            <ProtectedRoute requiredRole="client">
              <ClientLayout />
            </ProtectedRoute>
          }
        >
          <Route path="dashboard" element={<ClientDashboard />} />
          <Route path="tickets" element={<ClientTicketsPage />} />
          <Route path="tickets/:id" element={<ClientTicketDetailPage />} />
          <Route path="team" element={<OrganizationTeamPage />} />
          <Route path="*" element={<NotFound />} />
        </Route>

        {/* Fallback */}
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
