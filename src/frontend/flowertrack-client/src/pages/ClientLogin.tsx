import { useState } from 'react';
import { Link } from 'react-router-dom';
import '../styles/login.css';

const ClientLogin = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        console.log('Client Login Attempt:', { email, password });
        // TODO: Implement actual login logic
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

                    <button type="submit" className="login-btn">
                        Zaloguj się
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
