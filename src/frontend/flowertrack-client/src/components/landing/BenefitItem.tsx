interface BenefitItemProps {
  icon: string;
  title: string;
  description: string;
}

export const BenefitItem = ({ icon, title, description }: BenefitItemProps) => {
  return (
    <div className="benefit-item">
      <div className="benefit-icon">{icon}</div>
      <div>
        <h4>{title}</h4>
        <p>{description}</p>
      </div>
    </div>
  );
};
