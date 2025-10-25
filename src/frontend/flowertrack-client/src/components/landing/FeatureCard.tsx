interface FeatureCardProps {
  icon: string;
  title: string;
  description: string;
  features: string[];
  featured?: boolean;
  badge?: string;
}

export const FeatureCard = ({
  icon,
  title,
  description,
  features,
  featured = false,
  badge,
}: FeatureCardProps) => {
  return (
    <div className={`feature-card ${featured ? 'featured' : ''}`}>
      {badge && <div className="feature-badge">{badge}</div>}
      <div className="feature-icon-wrapper">
        <div className="feature-icon">{icon}</div>
      </div>
      <h3>{title}</h3>
      <p>{description}</p>
      <ul className="feature-list">
        {features.map((feature, index) => (
          <li key={index}>{feature}</li>
        ))}
      </ul>
    </div>
  );
};
