import { useState } from 'react';
import { Link } from 'react-router-dom';
import '../styles/login.css';

const ServiceLogin = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        console.log('Service Login Attempt:', { email, password });
        // TODO: Implement actual login logic
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

                    <button type="submit" className="login-btn">
                        Zaloguj się
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
