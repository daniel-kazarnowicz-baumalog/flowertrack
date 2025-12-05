import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

interface ProtectedRouteProps {
    children: JSX.Element;
    requiredRole?: string;
}

const ProtectedRoute = ({ children, requiredRole }: ProtectedRouteProps) => {
    const { isAuthenticated, user, loading } = useAuth();

    if (loading) {
        return <div>Loading...</div>; // Or a proper spinner
    }

    if (!isAuthenticated) {
        return <Navigate to="/" replace />;
    }

    if (requiredRole && user?.role !== requiredRole) {
        // Allow admin to access service routes if token says 'service' or 'admin' 
        // but let's stick to the plan. 
        // If user role doesn't match required role, maybe redirect to their own dashboard?
        // Or 403.
        // For simplicity, if service tries to access client, redirect to service dashboard.
        if (user?.role === 'service') return <Navigate to="/service/dashboard" replace />;
        if (user?.role === 'client') return <Navigate to="/client/dashboard" replace />;
        return <Navigate to="/" replace />;
    }

    return children;
};

export default ProtectedRoute;
