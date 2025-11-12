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
  Low: '#10b981',
  Medium: '#3b82f6',
  High: '#f59e0b',
  Critical: '#ef4444',
};

export function PriorityDistributionChart({
  data,
  title = 'Tickets by Priority',
}: PriorityDistributionChartProps) {
  // Transform and sort data by priority order
  const priorityOrder = ['Low', 'Medium', 'High', 'Critical'];
  const chartData = priorityOrder
    .filter((priority) => data[priority] !== undefined)
    .map((priority) => ({
      name: priority,
      value: data[priority],
    }));

  // Get color for priority
  const getColor = (priority: string) => PRIORITY_COLORS[priority] || '#9ca3af';

  return (
    <div className="priorityDistributionChart">
      <h3 className="priorityDistributionChart__title">{title}</h3>
      <ResponsiveContainer width="100%" height={300}>
        <BarChart data={chartData} margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
          <XAxis dataKey="name" stroke="#6b7280" style={{ fontSize: '0.875rem' }} />
          <YAxis stroke="#6b7280" style={{ fontSize: '0.875rem' }} />
          <Tooltip
            contentStyle={{
              backgroundColor: 'white',
              border: '1px solid #e5e7eb',
              borderRadius: '6px',
              padding: '8px 12px',
            }}
          />
          <Legend wrapperStyle={{ fontSize: '0.875rem' }} />
          <Bar dataKey="value" name="Tickets" radius={[8, 8, 0, 0]}>
            {chartData.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={getColor(entry.name)} />
            ))}
          </Bar>
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}
