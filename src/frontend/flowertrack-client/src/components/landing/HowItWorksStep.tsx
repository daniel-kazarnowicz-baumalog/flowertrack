interface HowItWorksStepProps {
  number: number;
  icon: string;
  title: string;
  description: string;
}

export const HowItWorksStep = ({ number, icon, title, description }: HowItWorksStepProps) => {
  return (
    <div className="step">
      <div className="step-number">{number}</div>
      <div className="step-icon">{icon}</div>
      <h3>{title}</h3>
      <p>{description}</p>
    </div>
  );
};
