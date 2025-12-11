import { useTranslation } from 'react-i18next';
import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip, Legend } from 'recharts';
import './StatusDistributionChart.css';

interface StatusDistributionChartProps {
  data: Record<string, number>;
  title?: string;
}

const STATUS_COLORS: Record<string, string> = {
  New: '#3b82f6', // Blue
  Accepted: '#8b5cf6', // Violet
  InProgress: '#f59e0b', // Amber
  Resolved: '#10b981', // Emerald
  Closed: '#64748b', // Slate
  Reopened: '#ef4444', // Rose
};

export function StatusDistributionChart({
  data,
  title = 'Tickets by Status',
}: StatusDistributionChartProps) {
  const { t } = useTranslation();

  // Helper to translate status
  const getStatusLabel = (status: string) => {
    switch (status) {
      case 'New':
        return t('tickets.statusNew', 'Nowy');
      case 'Accepted':
        return t('tickets.statusOpen', 'Otwarty'); // Assuming Accepted maps to Open or add statusAccepted
      case 'InProgress':
        return t('tickets.statusInProgress', 'W trakcie');
      case 'Resolved':
        return t('tickets.statusResolved', 'Rozwiązany');
      case 'Closed':
        return t('tickets.statusClosed', 'Zamknięty');
      case 'Reopened':
        return t('tickets.statusReopened', 'Ponownie otwarty'); // Add if needed
      default:
        return status;
    }
  };

  // Transform data for Recharts
  const chartData = Object.entries(data).map(([name, value]) => ({
    name: getStatusLabel(name),
    originalName: name, // Keep original for color lookup
    value,
  }));

  // Get color for status
  const getColor = (status: string) => STATUS_COLORS[status] || '#9ca3af';

  return (
    <div className="statusDistributionChart">
      <h3 className="statusDistributionChart__title">{title}</h3>
      <ResponsiveContainer width="100%" height={300}>
        <PieChart>
          <Pie
            data={chartData}
            cx="50%"
            cy="50%"
            innerRadius={60}
            outerRadius={80}
            paddingAngle={5}
            dataKey="value"
          >
            {chartData.map((entry, index) => (
              <Cell
                key={`cell-${index}`}
                fill={getColor(entry.originalName)}
                stroke="var(--color-bg-card)"
                strokeWidth={2}
                style={{ filter: 'drop-shadow(0px 2px 4px rgba(0,0,0,0.1))' }}
              />
            ))}
          </Pie>
          <Tooltip
            contentStyle={{
              backgroundColor: 'var(--color-bg-popover)',
              border: '1px solid var(--color-border-subtle)',
              borderRadius: '8px',
              padding: '8px 12px',
              boxShadow: 'var(--shadow-md)',
            }}
            itemStyle={{ color: 'var(--color-text-primary)' }}
          />
          <Legend verticalAlign="bottom" height={36} iconType="circle" />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
}
