import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import '../styles/login.css';
import api from '../lib/api';
import { useAuth } from '../contexts/AuthContext';

const ClientLogin = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const { login } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);

        try {
            const response = await api.post('/auth/client/login', { email, password });
            // Assuming response data structure
            const { token, user } = response.data;
            login(token, user || { email, role: 'client' });
            navigate('/client/dashboard');
        } catch (err: any) {
            console.error('Login error:', err);
            setError(err.response?.data?.message || 'Błąd logowania. Sprawdź dane.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="login-container client-theme">
            <div className="login-card">
                <div className="login-header">
                    <Link to="/" className="back-link">← Wróć</Link>
                    <div className="icon-circle">🏢</div>
                    <h2>Portal Klienta</h2>
                    <p>Zaloguj się do panelu organizacji</p>
                </div>

                <form onSubmit={handleSubmit} className="login-form">
                    {error && <div className="error-message" style={{ color: 'red', marginBottom: '1rem', textAlign: 'center' }}>{error}</div>}
                    <div className="form-group">
                        <label htmlFor="email">Email</label>
                        <input
                            type="email"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="twoj@email.pl"
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
                </form>

                <div className="login-footer">
                    <a href="#">Pierwsze logowanie? Aktywuj konto</a>
                </div>
            </div>
        </div>
    );
};

export default ClientLogin;
