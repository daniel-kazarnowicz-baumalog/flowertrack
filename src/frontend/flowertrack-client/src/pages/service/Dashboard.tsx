import { useAuth } from '../../contexts/AuthContext';

const ServiceDashboard = () => {
  const { user, logout } = useAuth();

  return (
    <div style={{ padding: '2rem' }}>
      <h1>Service Dashboard</h1>
      <p>Welcome, {user?.email}</p>
      <p>Role: {user?.role}</p>
      <button
        onClick={logout}
        style={{ padding: '0.5rem 1rem', marginTop: '1rem', cursor: 'pointer' }}
      >
        Logout
      </button>
    </div>
  );
};

export default ServiceDashboard;
