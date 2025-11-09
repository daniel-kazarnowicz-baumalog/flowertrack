import React from 'react';
import './HeroSection.css';

interface HeroStat {
  value: string;
  label: string;
}

interface HeroSectionProps {
  badge: string;
  title: string;
  highlightedWord: string;
  subtitle: string;
  primaryCTA: {
    text: string;
    icon: string;
    onClick?: () => void;
  };
  secondaryCTA: {
    text: string;
    icon: string;
    onClick?: () => void;
  };
  stats: HeroStat[];
  backgroundIcons: string[];
}

export const HeroSection: React.FC<HeroSectionProps> = ({
  badge,
  title,
  highlightedWord,
  subtitle,
  primaryCTA,
  secondaryCTA,
  stats,
  backgroundIcons,
}) => {
  return (
    <section className="hero-section">
      <div className="hero-background">
        {backgroundIcons.map((icon, index) => (
          <span
            key={index}
            className={`floating-icon floating-icon-${index + 1}`}
          >
            {icon}
          </span>
        ))}
      </div>

      <div className="hero-container">
        <div className="hero-badge">{badge}</div>

        <h1 className="hero-title">
          {title.split(highlightedWord)[0]}
          <span className="hero-highlight">{highlightedWord}</span>
          {title.split(highlightedWord)[1]}
        </h1>

        <p className="hero-subtitle">{subtitle}</p>

        <div className="hero-actions">
          <button
            className="hero-btn hero-btn-primary"
            onClick={primaryCTA.onClick}
          >
            {primaryCTA.text}
            <span className="hero-btn-icon">{primaryCTA.icon}</span>
          </button>
          <button
            className="hero-btn hero-btn-secondary"
            onClick={secondaryCTA.onClick}
          >
            <span className="hero-btn-icon">{secondaryCTA.icon}</span>
            {secondaryCTA.text}
          </button>
        </div>

        <div className="hero-stats">
          {stats.map((stat, index) => (
            <div key={index} className="hero-stat">
              <div className="hero-stat-value">{stat.value}</div>
              <div className="hero-stat-label">{stat.label}</div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};
