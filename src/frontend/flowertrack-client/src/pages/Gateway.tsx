import { Link } from 'react-router-dom';
import '../styles/gateway.css'; // We will create this

const Gateway = () => {
    return (
        <div className="gateway-container">
            <div className="gateway-content">
                <div className="gateway-header">
                    <span className="logo-icon">⚙️</span>
                    <h1>FLOW<span className="highlight">er</span>TRACK</h1>
                    <p className="gateway-subtitle">Wybierz portal, aby kontynuować</p>
                </div>

                <div className="gateway-options">
                    <Link to="/service" className="gateway-card service-card">
                        <div className="card-icon">🔧</div>
                        <h2>Portal Serwisu</h2>
                        <p>Dla techników i administratorów serwisu</p>
                        <div className="card-arrow">→</div>
                    </Link>

                    <Link to="/client" className="gateway-card client-card">
                        <div className="card-icon">🏢</div>
                        <h2>Portal Klienta</h2>
                        <p>Dla organizacji i operatorów maszyn</p>
                        <div className="card-arrow">→</div>
                    </Link>
                </div>
            </div>
        </div>
    );
};

export default Gateway;
