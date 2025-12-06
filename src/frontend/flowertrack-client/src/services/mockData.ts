/**
 * @obsolete DEV ONLY - Mock data for frontend testing without backend
 * TODO: Remove this file before production deployment
 */

import type { ServiceDashboardStats, ClientDashboardStats, TicketTrend } from './dashboardService';

/**
 * Check if current session is using mock authentication
 */
export function isMockSession(): boolean {
    const token = localStorage.getItem('token');
    return token?.startsWith('mock-dev-token-') ?? false;
}

/**
 * Mock service dashboard statistics
 */
export function getMockServiceDashboardStats(): ServiceDashboardStats {
    return {
        activeTicketsCount: 24,
        criticalTicketsCount: 3,
        myAssignedTicketsCount: 8,
        unassignedTicketsCount: 5,
        ticketsByStatus: {
            'Nowy': 6,
            'W trakcie': 12,
            'Oczekuje': 4,
            'Rozwiązany': 15,
            'Zamknięty': 8,
        },
        ticketsByPriority: {
            'Krytyczny': 3,
            'Wysoki': 7,
            'Średni': 18,
            'Niski': 17,
        },
        recentActivity: [
            {
                id: 'act-1',
                type: 'created',
                ticketId: 'TKT-001',
                ticketTitle: 'Awaria przenośnika taśmowego - linia A',
                timestamp: new Date(Date.now() - 15 * 60000).toISOString(),
                user: 'Jan Kowalski',
            },
            {
                id: 'act-2',
                type: 'assigned',
                ticketId: 'TKT-002',
                ticketTitle: 'Przegląd okresowy - maszyna pakująca',
                timestamp: new Date(Date.now() - 45 * 60000).toISOString(),
                user: 'Anna Nowak',
            },
            {
                id: 'act-3',
                type: 'resolved',
                ticketId: 'TKT-003',
                ticketTitle: 'Wymiana filtrów - system wentylacji',
                timestamp: new Date(Date.now() - 2 * 3600000).toISOString(),
                user: 'Piotr Wiśniewski',
            },
            {
                id: 'act-4',
                type: 'updated',
                ticketId: 'TKT-004',
                ticketTitle: 'Kalibracja czujników temperatury',
                timestamp: new Date(Date.now() - 5 * 3600000).toISOString(),
                user: 'Maria Kamińska',
            },
            {
                id: 'act-5',
                type: 'created',
                ticketId: 'TKT-005',
                ticketTitle: 'Błąd sterownika PLC - stanowisko 3',
                timestamp: new Date(Date.now() - 24 * 3600000).toISOString(),
                user: 'Tomasz Zieliński',
            },
        ],
    };
}

/**
 * Mock client dashboard statistics
 */
export function getMockClientDashboardStats(): ClientDashboardStats {
    return {
        activeMachinesCount: 12,
        machinesInAlarmCount: 1,
        machinesInMaintenanceCount: 2,
        inactiveMachinesCount: 3,
        myActiveTicketsCount: 4,
        allOrgTicketsCount: 18,
        resolvedThisWeekCount: 5,
        ticketsByStatus: {
            'Nowy': 3,
            'W trakcie': 5,
            'Oczekuje': 2,
            'Rozwiązany': 6,
            'Zamknięty': 2,
        },
        recentActivity: [
            {
                id: 'act-c1',
                type: 'created',
                ticketId: 'TKT-C001',
                ticketTitle: 'Zgłoszenie awarii - linia produkcyjna',
                timestamp: new Date(Date.now() - 30 * 60000).toISOString(),
                user: 'Operator Jan',
            },
            {
                id: 'act-c2',
                type: 'updated',
                ticketId: 'TKT-C002',
                ticketTitle: 'Przegląd maszyny sortującej',
                timestamp: new Date(Date.now() - 3 * 3600000).toISOString(),
                user: 'Serwisant Anna',
            },
        ],
    };
}

/**
 * Mock ticket trends data (last 30 days)
 */
export function getMockTicketTrends(): TicketTrend[] {
    const trends: TicketTrend[] = [];
    const today = new Date();

    for (let i = 29; i >= 0; i--) {
        const date = new Date(today);
        date.setDate(date.getDate() - i);
        const dateStr = date.toISOString().split('T')[0];

        // Generate semi-random but realistic data
        const baseCreated = Math.floor(Math.random() * 4) + 1;
        const baseResolved = Math.floor(Math.random() * 3) + 1;

        trends.push({
            date: dateStr,
            created: baseCreated,
            resolved: baseResolved,
            total: 45 + Math.floor(Math.random() * 10) - 5,
        });
    }

    return trends;
}

/**
 * Mock tickets list
 */
export function getMockTickets() {
    return {
        items: [
            {
                id: 'tkt-001',
                ticketNumber: 'FT-2024-001',
                title: 'Awaria przenośnika taśmowego - linia A',
                description: 'Przenośnik zatrzymuje się po 10 minutach pracy. Konieczna diagnoza silnika.',
                status: 'W trakcie',
                priority: 'Wysoki',
                organizationId: 'org-1',
                organizationName: 'Fabryka Kwiatów Sp. z o.o.',
                machineId: 'mach-1',
                machineName: 'Przenośnik PT-100',
                assignedToId: 'user-1',
                assignedToName: 'Jan Serwisant',
                createdById: 'user-client-1',
                createdByName: 'Maria Operator',
                createdAt: new Date(Date.now() - 2 * 24 * 3600000).toISOString(),
                updatedAt: new Date(Date.now() - 1 * 3600000).toISOString(),
            },
            {
                id: 'tkt-002',
                ticketNumber: 'FT-2024-002',
                title: 'Przegląd okresowy - maszyna pakująca',
                description: 'Planowany przegląd kwartalny zgodnie z harmonogramem.',
                status: 'Nowy',
                priority: 'Średni',
                organizationId: 'org-1',
                organizationName: 'Fabryka Kwiatów Sp. z o.o.',
                machineId: 'mach-2',
                machineName: 'Pakowarka PK-200',
                assignedToId: null,
                assignedToName: null,
                createdById: 'user-client-1',
                createdByName: 'Maria Operator',
                createdAt: new Date(Date.now() - 1 * 24 * 3600000).toISOString(),
                updatedAt: new Date(Date.now() - 1 * 24 * 3600000).toISOString(),
            },
            {
                id: 'tkt-003',
                ticketNumber: 'FT-2024-003',
                title: 'Błąd sterownika PLC',
                description: 'Sterownik pokazuje błąd E-45. Maszyna nie startuje.',
                status: 'Krytyczny',
                priority: 'Krytyczny',
                organizationId: 'org-2',
                organizationName: 'Greentech Industries',
                machineId: 'mach-3',
                machineName: 'Robot spawalniczy RS-500',
                assignedToId: 'user-2',
                assignedToName: 'Anna Technik',
                createdById: 'user-client-2',
                createdByName: 'Piotr Kierownik',
                createdAt: new Date(Date.now() - 6 * 3600000).toISOString(),
                updatedAt: new Date(Date.now() - 30 * 60000).toISOString(),
            },
        ],
        totalCount: 3,
        pageNumber: 1,
        pageSize: 10,
    };
}

/**
 * Mock organizations list
 */
export function getMockOrganizations() {
    return {
        items: [
            {
                id: 'org-1',
                name: 'Fabryka Kwiatów Sp. z o.o.',
                address: 'ul. Kwiatowa 15, 00-001 Warszawa',
                contactEmail: 'kontakt@fabrykakwiatow.pl',
                contactPhone: '+48 22 123 45 67',
                isActive: true,
                machinesCount: 8,
                usersCount: 12,
                activeTicketsCount: 3,
                createdAt: '2023-01-15T10:00:00Z',
            },
            {
                id: 'org-2',
                name: 'Greentech Industries',
                address: 'ul. Przemysłowa 42, 30-200 Kraków',
                contactEmail: 'biuro@greentech.pl',
                contactPhone: '+48 12 987 65 43',
                isActive: true,
                machinesCount: 15,
                usersCount: 25,
                activeTicketsCount: 5,
                createdAt: '2022-06-01T08:30:00Z',
            },
            {
                id: 'org-3',
                name: 'EcoFlora Polska',
                address: 'ul. Ogrodowa 8, 50-100 Wrocław',
                contactEmail: 'info@ecoflora.pl',
                contactPhone: '+48 71 555 12 34',
                isActive: false,
                machinesCount: 5,
                usersCount: 8,
                activeTicketsCount: 0,
                createdAt: '2023-08-20T14:00:00Z',
            },
        ],
        totalCount: 3,
        pageNumber: 1,
        pageSize: 10,
    };
}

/**
 * Mock machines list
 */
export function getMockMachines() {
    return {
        items: [
            {
                id: 'mach-1',
                serialNumber: 'PT-100-2023-001',
                name: 'Przenośnik PT-100',
                model: 'ConveyorPro 100',
                manufacturer: 'TechBelt Inc.',
                organizationId: 'org-1',
                organizationName: 'Fabryka Kwiatów Sp. z o.o.',
                status: 'Active',
                lastMaintenanceDate: '2024-10-15',
                nextMaintenanceDate: '2025-01-15',
                installationDate: '2023-03-01',
            },
            {
                id: 'mach-2',
                serialNumber: 'PK-200-2022-015',
                name: 'Pakowarka PK-200',
                model: 'PackMaster 200X',
                manufacturer: 'PackSolutions',
                organizationId: 'org-1',
                organizationName: 'Fabryka Kwiatów Sp. z o.o.',
                status: 'Maintenance',
                lastMaintenanceDate: '2024-11-01',
                nextMaintenanceDate: '2025-02-01',
                installationDate: '2022-08-15',
            },
            {
                id: 'mach-3',
                serialNumber: 'RS-500-2021-008',
                name: 'Robot spawalniczy RS-500',
                model: 'WeldBot 500',
                manufacturer: 'RoboWeld GmbH',
                organizationId: 'org-2',
                organizationName: 'Greentech Industries',
                status: 'Alarm',
                lastMaintenanceDate: '2024-09-20',
                nextMaintenanceDate: '2024-12-20',
                installationDate: '2021-05-10',
            },
        ],
        totalCount: 3,
        pageNumber: 1,
        pageSize: 10,
    };
}
