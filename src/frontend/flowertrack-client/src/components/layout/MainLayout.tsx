import type { ReactNode } from 'react';
import { Navbar } from './Navbar';
import './MainLayout.css';

export interface MainLayoutProps {
  children: ReactNode;
}

/**
 * Main Layout for Service Portal
 * Includes top navbar and main content area
 */
export function MainLayout({ children }: MainLayoutProps) {
  return (
    <div className="main-layout">
      <Navbar portal="service" />
      <main className="main-layout__content">{children}</main>
    </div>
  );
}
