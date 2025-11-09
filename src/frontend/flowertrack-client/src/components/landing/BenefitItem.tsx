import React from 'react';
import './BenefitItem.css';

interface BenefitItemProps {
  icon: string;
  title: string;
  description: string;
}

export const BenefitItem: React.FC<BenefitItemProps> = ({ icon, title, description }) => {
  return (
    <div className="benefit-item">
      <div className="benefit-icon">{icon}</div>
      <div className="benefit-content">
        <h4 className="benefit-title">{title}</h4>
        <p className="benefit-description">{description}</p>
      </div>
    </div>
  );
};
