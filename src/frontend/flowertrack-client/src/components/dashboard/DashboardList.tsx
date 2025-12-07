import './DashboardList.css';

export interface DashboardListItem {
  id: string;
  title: string;
  subtitle?: string;
  badge?: string | number;
  badgeType?: 'danger' | 'warning' | 'info';
  onClick?: () => void;
}

interface DashboardListProps {
  title: string;
  items: DashboardListItem[];
  emptyMessage?: string;
}

export const DashboardList = ({
  title,
  items,
  emptyMessage = 'Brak danych',
}: DashboardListProps) => {
  return (
    <div className="dashboardList">
      <div className="dashboardList__header">
        <h3 className="dashboardList__title">{title}</h3>
      </div>
      <div className="dashboardList__content">
        {items.length === 0 ? (
          <div className="dashboardList__empty">{emptyMessage}</div>
        ) : (
          items.map((item) => (
            <div key={item.id} className="dashboardList__item" onClick={item.onClick}>
              <div className="dashboardList__itemMain">
                <span className="dashboardList__itemName">{item.title}</span>
                {item.subtitle && <span className="dashboardList__itemSub">{item.subtitle}</span>}
              </div>
              {item.badge && (
                <span
                  className={`dashboardList__badge dashboardList__badge--${item.badgeType || 'info'}`}
                >
                  {item.badge}
                </span>
              )}
            </div>
          ))
        )}
      </div>
    </div>
  );
};
