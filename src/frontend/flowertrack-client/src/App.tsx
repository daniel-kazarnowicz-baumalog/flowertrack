import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ProtectedRoute } from './components/auth';
import { ServiceLayout } from './components/layout';
import {
  ServiceLoginPage,
  ServiceDashboard,
  ServiceForgotPasswordPage,
  ServiceResetPasswordPage,
  ServiceTicketsPage,
  ServiceTicketDetailPage,
  OrganizationsListPage,
  OrganizationDetailPage,
  ServiceUsersListPage,
  MachinesListPage,
  MachineDetailPage,
} from './pages/service';
import {
  ClientLoginPage,
  ClientDashboard,
  ClientActivatePage,
  ClientTicketsPage,
  ClientTicketDetailPage,
} from './pages/client';
import { ClientLayout } from './components/layout';
import NotFound from './pages/NotFound';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Root redirects to service login */}
        <Route path="/" element={<Navigate to="/service/login" replace />} />

        {/* Service Portal Routes */}
        <Route path="/service" element={<Navigate to="/service/login" replace />} />
        <Route path="/service/login" element={<ServiceLoginPage />} />
        <Route path="/service/forgot-password" element={<ServiceForgotPasswordPage />} />
        <Route path="/service/reset-password/:token" element={<ServiceResetPasswordPage />} />
        <Route
          path="/service/dashboard"
          element={
            <ProtectedRoute requiredRole="service">
              <ServiceLayout>
                <ServiceDashboard />
              </ServiceLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/service/tickets"
          element={
            <ProtectedRoute requiredRole="service">
              <ServiceLayout>
                <ServiceTicketsPage />
              </ServiceLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/service/tickets/:id"
          element={
            <ProtectedRoute requiredRole="service">
              <ServiceLayout>
                <ServiceTicketDetailPage />
              </ServiceLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/service/organizations"
          element={
            <ProtectedRoute requiredRole="service">
              <ServiceLayout>
                <OrganizationsListPage />
              </ServiceLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/service/organizations/:id"
          element={
            <ProtectedRoute requiredRole="service">
              <ServiceLayout>
                <OrganizationDetailPage />
              </ServiceLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/service/users"
          element={
            <ProtectedRoute requiredRole="service" requireAdmin={true}>
              <ServiceLayout>
                <ServiceUsersListPage />
          path="/service/machines"
          element={
            <ProtectedRoute requiredRole="service">
              <ServiceLayout>
                <MachinesListPage />
              </ServiceLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/service/machines/:id"
          element={
            <ProtectedRoute requiredRole="service">
              <ServiceLayout>
                <MachineDetailPage />
              </ServiceLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/service/admin"
          element={
            <ProtectedRoute requiredRole="service" requireAdmin={true}>
              <ServiceLayout>
                <div>Admin page - Phase 3</div>
              </ServiceLayout>
            </ProtectedRoute>
          }
        />

        {/* Client Portal Routes */}
        <Route path="/client" element={<Navigate to="/client/login" replace />} />
        <Route path="/client/login" element={<ClientLoginPage />} />
        <Route path="/client/activate/:token" element={<ClientActivatePage />} />
        <Route
          path="/client/dashboard"
          element={
            <ProtectedRoute requiredRole="client">
              <ClientLayout>
                <ClientDashboard />
              </ClientLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/client/tickets"
          element={
            <ProtectedRoute requiredRole="client">
              <ClientLayout>
                <ClientTicketsPage />
              </ClientLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/client/tickets/:id"
          element={
            <ProtectedRoute requiredRole="client">
              <ClientLayout>
                <ClientTicketDetailPage />
              </ClientLayout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/client/team"
          element={
            <ProtectedRoute requiredRole="client">
              <ClientLayout>
                <div>Team management - Phase 3</div>
              </ClientLayout>
            </ProtectedRoute>
          }
        />

        {/* 404 Not Found */}
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
