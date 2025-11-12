import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip, Legend } from 'recharts';
import './StatusDistributionChart.css';

interface StatusDistributionChartProps {
  data: Record<string, number>;
  title?: string;
}

const STATUS_COLORS: Record<string, string> = {
  New: '#3b82f6',
  Accepted: '#8b5cf6',
  InProgress: '#f59e0b',
  Resolved: '#10b981',
  Closed: '#6b7280',
  Reopened: '#ef4444',
};

export function StatusDistributionChart({
  data,
  title = 'Tickets by Status',
}: StatusDistributionChartProps) {
  // Transform data for Recharts
  const chartData = Object.entries(data).map(([name, value]) => ({
    name,
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
            labelLine={false}
            label={({ name, percent }) => `${name} ${percent ? (percent * 100).toFixed(0) : 0}%`}
            outerRadius={80}
            fill="#8884d8"
            dataKey="value"
          >
            {chartData.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={getColor(entry.name)} />
            ))}
          </Pie>
          <Tooltip
            contentStyle={{
              backgroundColor: 'white',
              border: '1px solid #e5e7eb',
              borderRadius: '6px',
              padding: '8px 12px',
            }}
          />
          <Legend wrapperStyle={{ fontSize: '0.875rem' }} />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
}
