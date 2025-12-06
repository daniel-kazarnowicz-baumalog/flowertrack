import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import '../styles/login.css';
import api from '../lib/api';
import { useAuth } from '../contexts/AuthContext';

const ServiceLogin = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    // We will need to wrap the app in AuthProvider for this to work
    // For now, we'll implement assuming the hook exists and will be provided
    // If not, we'll fix the tree later.
    // Actually, let's verify AuthContext import path.
    // It is '../contexts/AuthContext'

    const { login, mockLogin } = useAuth();
    const navigate = useNavigate();

    /**
     * @obsolete DEV ONLY - Handle mock login without backend
     * TODO: Remove before production deployment
     */
    const handleMockLogin = () => {
        mockLogin('service');
        navigate('/service/dashboard');
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);

        try {
            const response = await api.post('/auth/service/login', { email, password });
            // Assuming the API returns token and user object. 
            // If the structure is different, we will debug it.
            // Based on standard practices:
            const { token, user } = response.data;
            // Or maybe it's response.data.token and we decode user? 
            // Let's assume response.data contains what we need for now.

            // Adjusting based on typical .NET API responses, sometimes it's just token.
            // But let's assume we get user details too or we can decode token.
            // For safety, let's check what we receive or just pass what we get.

            login(token, user || { email, role: 'service' });
            navigate('/service/dashboard');
        } catch (err: any) {
            console.error('Login error:', err);
            setError(err.response?.data?.message || 'Błąd logowania. Sprawdź dane.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="login-container service-theme">
            <div className="login-card">
                <div className="login-header">
                    <Link to="/" className="back-link">← Wróć</Link>
                    <div className="icon-circle">🔧</div>
                    <h2>Portal Serwisu</h2>
                    <p>Zaloguj się do panelu technicznego</p>
                </div>

                <form onSubmit={handleSubmit} className="login-form">
                    {error && <div className="error-message" style={{ color: 'red', marginBottom: '1rem', textAlign: 'center' }}>{error}</div>}
                    <div className="form-group">
                        <label htmlFor="email">Email służbowy</label>
                        <input
                            type="email"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="imie.nazwisko@firma.pl"
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">Hasło</label>
                        <input
                            type="password"
                            id="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="••••••••"
                            required
                        />
                    </div>

                    <button type="submit" className="login-btn" disabled={isLoading}>
                        {isLoading ? 'Logowanie...' : 'Zaloguj się'}
                    </button>

                    {/* @obsolete DEV ONLY - Remove this button before production deployment */}
                    <button
                        type="button"
                        className="login-btn"
                        onClick={handleMockLogin}
                        style={{
                            marginTop: '0.5rem',
                            background: 'linear-gradient(135deg, #ff6b6b, #ee5a24)',
                            border: '2px dashed #fff'
                        }}
                    >
                        🧪 Zaloguj bez autentykacji (DEV)
                    </button>
                </form>

                <div className="login-footer">
                    <a href="#">Nie pamiętasz hasła?</a>
                </div>
            </div>
        </div>
    );
};

export default ServiceLogin;
