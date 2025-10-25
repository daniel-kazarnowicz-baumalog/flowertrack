import { ReactNode } from 'react';

interface Stat {
  value: string;
  label: string;
}

interface HeroSectionProps {
  badge: string;
  title: string;
  gradientWord: string;
  subtitle: string;
  primaryCTA: {
    text: string;
    icon: string;
  };
  secondaryCTA: {
    text: string;
    icon: string;
  };
  stats: Stat[];
  floatingIcons: string[];
}

export const HeroSection = ({
  badge,
  title,
  gradientWord,
  subtitle,
  primaryCTA,
  secondaryCTA,
  stats,
  floatingIcons,
}: HeroSectionProps) => {
  // Split title by gradientWord to apply gradient styling
  const titleParts = title.split(gradientWord);

  return (
    <header className="hero">
      <div className="hero-content">
        <div className="hero-badge">
          <span className="badge-text">{badge}</span>
        </div>
        <h1 className="hero-title">
          {titleParts[0]}
          {gradientWord && <span className="gradient-text">{gradientWord}</span>}
          {titleParts[1]}
        </h1>
        <p className="hero-subtitle">{subtitle}</p>
        <div className="hero-cta">
          <button className="btn-primary">
            <span>{primaryCTA.text}</span>
            <span className="arrow">{primaryCTA.icon}</span>
          </button>
          <button className="btn-secondary">
            <span className="play-icon">{secondaryCTA.icon}</span>
            {secondaryCTA.text}
          </button>
        </div>
        <div className="hero-stats">
          {stats.map((stat, index) => (
            <div className="stat" key={index}>
              <div className="stat-value">{stat.value}</div>
              <div className="stat-label">{stat.label}</div>
            </div>
          ))}
        </div>
      </div>

      {/* Animated background elements */}
      <div className="hero-background">
        {floatingIcons.map((icon, index) => (
          <div key={index} className={`floating-icon icon-${index + 1}`}>
            {icon}
          </div>
        ))}
        <div className="grid-pattern"></div>
      </div>
    </header>
  );
};
