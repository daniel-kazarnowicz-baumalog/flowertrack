import {
  LineChart,
  Line,
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
  // Format data for display
  const chartData = data.map((item) => ({
    ...item,
    date: new Date(item.date).toLocaleDateString('pl-PL', { month: 'short', day: 'numeric' }),
  }));

  return (
    <div className="ticketTrendChart">
      <h3 className="ticketTrendChart__title">{title}</h3>
      <ResponsiveContainer width="100%" height={300}>
        <LineChart data={chartData} margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
          <XAxis dataKey="date" stroke="#6b7280" style={{ fontSize: '0.875rem' }} />
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
          <Line
            type="monotone"
            dataKey="created"
            stroke="#3b82f6"
            strokeWidth={2}
            name="Created"
            dot={{ r: 3 }}
            activeDot={{ r: 5 }}
          />
          <Line
            type="monotone"
            dataKey="resolved"
            stroke="#10b981"
            strokeWidth={2}
            name="Resolved"
            dot={{ r: 3 }}
            activeDot={{ r: 5 }}
          />
          <Line
            type="monotone"
            dataKey="total"
            stroke="#8b5cf6"
            strokeWidth={2}
            name="Total Active"
            dot={{ r: 3 }}
            activeDot={{ r: 5 }}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}
