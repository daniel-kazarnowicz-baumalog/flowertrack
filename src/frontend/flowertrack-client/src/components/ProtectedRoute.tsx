import { type ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { Loader } from './ui';

interface ProtectedRouteProps {
  children: ReactNode;
  requiredRole?: 'service' | 'client';
}

const ProtectedRoute = ({ children, requiredRole }: ProtectedRouteProps) => {
  const { isAuthenticated, user, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div
        style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}
      >
        <Loader size="lg" />
      </div>
    );
  }

  // Development-only logging for route protection debugging
  if (import.meta.env.DEV) {
    // eslint-disable-next-line no-console
    console.log('[ProtectedRoute] Check:', {
      path: window.location.pathname,
      isAuthenticated,
      userRole: user?.role,
      requiredRole,
    });
  }

  if (!isAuthenticated || !user) {
    console.warn('[ProtectedRoute] Not authenticated. Redirecting to Gateway.');
    return <Navigate to="/" replace />;
  }

  if (requiredRole && user.role !== requiredRole) {
    console.warn(`[ProtectedRoute] Role mismatch. User: ${user.role}, Required: ${requiredRole}`);
    // Redirect to appropriate dashboard
    if (user.role === 'service') return <Navigate to="/service/dashboard" replace />;
    if (user.role === 'client') return <Navigate to="/client/dashboard" replace />;
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
