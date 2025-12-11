import { useTranslation } from 'react-i18next';
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts';
import type { TicketTrend } from '../../services/dashboardService';
import './TicketTrendChart.css';

interface TicketTrendChartProps {
  data: TicketTrend[];
  title?: string;
}

export function TicketTrendChart({
  data,
  title = 'Ticket Trends (Last 30 Days)',
}: TicketTrendChartProps) {
  const { t } = useTranslation();

  // Format data for display
  const chartData = data.map((item) => ({
    ...item,
    date: new Date(item.date).toLocaleDateString(navigator.language, {
      month: 'short',
      day: 'numeric',
    }),
  }));

  return (
    <div className="ticketTrendChart">
      <h3 className="ticketTrendChart__title">{title}</h3>
      <ResponsiveContainer width="100%" height={300}>
        <AreaChart data={chartData} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
          <defs>
            <linearGradient id="colorTotal" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#6366f1" stopOpacity={0.3} />
              <stop offset="95%" stopColor="#6366f1" stopOpacity={0} />
            </linearGradient>
            <linearGradient id="colorCreated" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#3b82f6" stopOpacity={0.3} />
              <stop offset="95%" stopColor="#3b82f6" stopOpacity={0} />
            </linearGradient>
            <linearGradient id="colorResolved" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#10b981" stopOpacity={0.3} />
              <stop offset="95%" stopColor="#10b981" stopOpacity={0} />
            </linearGradient>
          </defs>
          <CartesianGrid
            strokeDasharray="3 3"
            stroke="var(--color-border-subtle)"
            vertical={false}
          />
          <XAxis
            dataKey="date"
            stroke="var(--color-text-tertiary)"
            style={{ fontSize: '0.75rem' }}
            tick={{ fill: 'var(--color-text-secondary)' }}
            tickLine={false}
            axisLine={false}
            dy={10}
          />
          <YAxis
            stroke="var(--color-text-tertiary)"
            style={{ fontSize: '0.75rem' }}
            tick={{ fill: 'var(--color-text-secondary)' }}
            tickLine={false}
            axisLine={false}
            dx={-10}
          />
          <Tooltip
            contentStyle={{
              backgroundColor: 'var(--color-bg-popover)',
              border: '1px solid var(--color-border-subtle)',
              borderRadius: '8px',
              padding: '8px 12px',
              boxShadow: 'var(--shadow-md)',
            }}
            itemStyle={{ color: 'var(--color-text-primary)' }}
            labelStyle={{ color: 'var(--color-text-secondary)', marginBottom: '0.5rem' }}
          />
          <Legend wrapperStyle={{ paddingTop: '20px' }} iconType="circle" />
          <Area
            type="monotone"
            dataKey="created"
            stroke="#3b82f6"
            strokeWidth={2}
            fillOpacity={1}
            fill="url(#colorCreated)"
            name={t('tickets.statusNew', 'Nowe').replace('Nowy', 'Nowe')} // Ad-hoc pluralization fix since we are in tight loop
            activeDot={{ r: 6, strokeWidth: 0 }}
          />
          <Area
            type="monotone"
            dataKey="resolved"
            stroke="#10b981"
            strokeWidth={2}
            fillOpacity={1}
            fill="url(#colorResolved)"
            name={t('tickets.statusResolved', 'Rozwiązane').replace('Rozwiązany', 'Rozwiązane')}
            activeDot={{ r: 6, strokeWidth: 0 }}
          />
          <Area
            type="monotone"
            dataKey="total"
            stroke="#6366f1"
            strokeWidth={2}
            fillOpacity={1}
            fill="url(#colorTotal)"
            name={t('dashboard.activeTickets', 'Aktywne')}
            activeDot={{ r: 6, strokeWidth: 0 }}
          />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
}
