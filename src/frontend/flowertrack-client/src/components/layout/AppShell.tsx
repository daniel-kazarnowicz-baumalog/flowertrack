import { useState, type ReactNode } from 'react';
import { Sidebar } from './Sidebar';
import { ThemeSwitcher } from '../ui/ThemeSwitcher';
import { LanguageSwitcher } from '../ui/LanguageSwitcher';
import { ParticlesBackground } from '../ui/ParticlesBackground';
import './AppShell.css';

interface SidebarItem {
    to: string;
    icon: ReactNode;
    label: string;
}

interface SidebarSection {
    title?: string;
    items: SidebarItem[];
}

interface AppShellProps {
    children: ReactNode;
    navSections: SidebarSection[];
    logoLink?: string;
    logoText?: string;
}

export const AppShell = ({
    children,
    navSections,
    logoLink = '/',
    logoText,
}: AppShellProps) => {
    const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
    const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(() => {
        const saved = localStorage.getItem('sidebarCollapsed');
        return saved === 'true';
    });

    const toggleSidebar = () => {
        setIsSidebarCollapsed((prev) => {
            const newState = !prev;
            localStorage.setItem('sidebarCollapsed', String(newState));
            return newState;
        });
    };

    return (
        <div className={`appShell ${isSidebarCollapsed ? 'appShell--collapsed' : ''}`}>
            <ParticlesBackground particleCount={30} className="appShell__particles" />

            <Sidebar
                sections={navSections}
                isOpen={isMobileMenuOpen}
                onCloseMobile={() => setIsMobileMenuOpen(false)}
                logoLink={logoLink}
                logoText={logoText}
                isCollapsed={isSidebarCollapsed}
                onToggleCollapse={toggleSidebar}
            />

            <div className="appShell__main">
                <header className="appShell__header">
                    <button
                        className="appShell__menuBtn"
                        onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                        aria-label="Toggle menu"
                    >
                        ☰
                    </button>

                    <div className="appShell__headerRight">
                        <LanguageSwitcher />
                        <ThemeSwitcher variant="icon" />
                    </div>
                </header>

                <div className="appShell__content">
                    <div className="appShell__container">
                        {children}
                    </div>
                </div>
            </div>
        </div>
    );
};
