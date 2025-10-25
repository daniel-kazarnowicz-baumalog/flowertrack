interface CTASectionProps {
  headline: string;
  subheadline: string;
  primaryButtonText: string;
  secondaryButtonText: string;
}

export const CTASection = ({
  headline,
  subheadline,
  primaryButtonText,
  secondaryButtonText,
}: CTASectionProps) => {
  return (
    <section id="contact" className="cta-section">
      <div className="cta-content">
        <h2>{headline}</h2>
        <p>{subheadline}</p>
        <div className="cta-buttons">
          <button className="btn-primary-large">{primaryButtonText}</button>
          <button className="btn-outline">{secondaryButtonText}</button>
        </div>
      </div>
    </section>
  );
};
