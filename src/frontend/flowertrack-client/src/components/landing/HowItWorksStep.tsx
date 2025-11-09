import React from 'react';
import './HowItWorksStep.css';

interface HowItWorksStepProps {
  number: number;
  icon: string;
  title: string;
  description: string;
}

export const HowItWorksStep: React.FC<HowItWorksStepProps> = ({
  number,
  icon,
  title,
  description,
}) => {
  return (
    <div className="how-it-works-step">
      <div className="step-number">{number}</div>
      <div className="step-icon">{icon}</div>
      <h3 className="step-title">{title}</h3>
      <p className="step-description">{description}</p>
    </div>
  );
};
