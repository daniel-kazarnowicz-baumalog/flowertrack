import React from 'react';
import './CTASection.css';

interface CTAButton {
  text: string;
  variant: 'primary' | 'outline';
  onClick?: () => void;
}

interface CTASectionProps {
  headline: string;
  subheadline: string;
  buttons: CTAButton[];
}

export const CTASection: React.FC<CTASectionProps> = ({ headline, subheadline, buttons }) => {
  return (
    <section className="cta-section" id="contact">
      <div className="cta-container">
        <h2 className="cta-headline">{headline}</h2>
        <p className="cta-subheadline">{subheadline}</p>
        <div className="cta-buttons">
          {buttons.map((button, index) => (
            <button
              key={index}
              className={`cta-btn cta-btn-${button.variant}`}
              onClick={button.onClick}
            >
              {button.text}
            </button>
          ))}
        </div>
      </div>
    </section>
  );
};
