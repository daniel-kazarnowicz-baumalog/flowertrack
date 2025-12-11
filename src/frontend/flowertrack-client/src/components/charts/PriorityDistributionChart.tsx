import { useTranslation } from 'react-i18next';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  Cell,
} from 'recharts';
import './PriorityDistributionChart.css';

interface PriorityDistributionChartProps {
  data: Record<string, number>;
  title?: string;
}

const PRIORITY_COLORS: Record<string, string> = {
  Low: '#10b981', // Emerald 500
  Medium: '#3b82f6', // Blue 500
  High: '#f59e0b', // Amber 500
  Critical: '#ef4444', // Rose 500
};

export function PriorityDistributionChart({
  data,
  title = 'Tickets by Priority',
}: PriorityDistributionChartProps) {
  const { t } = useTranslation();

  // Helper to translate priority
  const getPriorityLabel = (priority: string) => {
    switch (priority) {
      case 'Low':
        return t('tickets.priorityLow', 'Niski');
      case 'Medium':
        return t('tickets.priorityMedium', 'Średni');
      case 'High':
        return t('tickets.priorityHigh', 'Wysoki');
      case 'Critical':
        return t('tickets.priorityCritical', 'Krytyczny');
      default:
        return priority;
    }
  };

  // Transform and sort data by priority order
  const priorityOrder = ['Low', 'Medium', 'High', 'Critical'];
  const chartData = priorityOrder
    .filter((priority) => data[priority] !== undefined)
    .map((priority) => ({
      name: getPriorityLabel(priority),
      originalName: priority, // Keep for color lookup
      value: data[priority],
    }));

  // Get color for priority
  const getColor = (priority: string) => PRIORITY_COLORS[priority] || '#9ca3af';

  return (
    <div className="priorityDistributionChart">
      <h3 className="priorityDistributionChart__title">{title}</h3>
      <ResponsiveContainer width="100%" height={300}>
        <BarChart data={chartData} margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" vertical={false} />
          <XAxis
            dataKey="name"
            stroke="#94a3b8"
            style={{ fontSize: '0.75rem' }}
            tick={{ fill: '#94a3b8' }}
            tickLine={false}
            axisLine={false}
          />
          <YAxis
            stroke="#94a3b8"
            style={{ fontSize: '0.75rem' }}
            tick={{ fill: '#94a3b8' }}
            tickLine={false}
            axisLine={false}
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
            cursor={{ fill: 'var(--color-bg-subtle)' }}
          />
          <Legend wrapperStyle={{ paddingTop: '10px' }} />
          <Bar dataKey="value" name={t('tickets.totalCount', 'Zgłoszenia')} radius={[4, 4, 0, 0]}>
            {chartData.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={getColor(entry.originalName)} />
            ))}
          </Bar>
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}
