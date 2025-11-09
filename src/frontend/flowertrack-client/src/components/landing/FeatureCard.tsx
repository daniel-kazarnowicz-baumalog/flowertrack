import React from 'react';
import './FeatureCard.css';

interface FeatureCardProps {
  icon: string;
  title: string;
  description: string;
  features: string[];
}

export const FeatureCard: React.FC<FeatureCardProps> = ({
  icon,
  title,
  description,
  features,
}) => {
  return (
    <div className="feature-card">
      <div className="feature-card-icon">{icon}</div>
      <h3 className="feature-card-title">{title}</h3>
      <p className="feature-card-description">{description}</p>
      <ul className="feature-card-list">
        {features.map((feature, index) => (
          <li key={index} className="feature-card-item">
            <span className="feature-card-check">✓</span>
            {feature}
          </li>
        ))}
      </ul>
    </div>
  );
};
