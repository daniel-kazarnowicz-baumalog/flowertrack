import { type ReactNode, useEffect, useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { Loader } from '../ui';
import './ProtectedRoute.css';

interface ProtectedRouteProps {
  children: ReactNode;
  requiredRole?: 'service' | 'client';
  requireAdmin?: boolean;
}

/**
 * ProtectedRoute component
 * Protects routes that require authentication
 * Redirects to appropriate login page if not authenticated
 * Supports role-based access control
 */
export const ProtectedRoute = ({
  children,
  requiredRole,
  requireAdmin = false,
}: ProtectedRouteProps) => {
  const { user, isAuthenticated, isLoading } = useAuth();
  const location = useLocation();
  const [isChecking, setIsChecking] = useState(true);

  useEffect(() => {
    // Give a brief moment for auth state to stabilize
    const timer = setTimeout(() => {
      setIsChecking(false);
    }, 100);

    return () => clearTimeout(timer);
  }, [isAuthenticated]);

  // Show loader while checking authentication
  if (isLoading || isChecking) {
    return (
      <div className="protectedRouteLoader">
        <Loader size="lg" />
      </div>
    );
  }

  // Not authenticated - redirect to login
  if (!isAuthenticated || !user) {
    const redirectTo = requiredRole === 'client' ? '/client/login' : '/service/login';
    return <Navigate to={redirectTo} state={{ from: location }} replace />;
  }

  // Check role if specified
  if (requiredRole && user.role !== requiredRole) {
    // Wrong portal - redirect to correct login
    const redirectTo = requiredRole === 'client' ? '/client/login' : '/service/login';
    return <Navigate to={redirectTo} replace />;
  }

  // Check admin requirement (only for service users)
  if (requireAdmin && user.role === 'service' && !user.isAdmin) {
    // Not admin - redirect to service dashboard
    return <Navigate to="/service/dashboard" replace />;
  }

  // All checks passed - render children
  return <>{children}</>;
};
