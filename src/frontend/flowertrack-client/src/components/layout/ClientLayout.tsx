import type { ReactNode } from 'react';
import { Navbar } from './Navbar';
import './ClientLayout.css';

export interface ClientLayoutProps {
  children: ReactNode;
}

/**
 * Client Layout for Client Portal
 * Includes top navbar and main content area
 */
export function ClientLayout({ children }: ClientLayoutProps) {
  return (
    <div className="client-layout">
      <Navbar portal="client" />
      <main className="client-layout__content">{children}</main>
    </div>
  );
}
