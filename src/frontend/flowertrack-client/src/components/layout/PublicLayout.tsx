import { Outlet, useLocation } from 'react-router-dom';
import { ParticlesBackground } from '../ui/ParticlesBackground';
import { LanguageSwitcher } from '../ui/LanguageSwitcher';
import { ThemeSwitcher } from '../ui/ThemeSwitcher';
import './PublicLayout.css';

export const PublicLayout = () => {
    const location = useLocation();

    return (
        <div className="public-layout">
            {/* Persistent Background */}
            <ParticlesBackground particleCount={80} />

            {/* Persistent Controls */}
            <div className="public-controls">
                <LanguageSwitcher />
                <ThemeSwitcher variant="toggle" />
            </div>

            {/* Render Page Content with Transition Key */}
            <div className="public-content">
                <div key={location.pathname} className="page-enter-container page-enter">
                    <Outlet />
                </div>
            </div>
        </div>
    );
};
