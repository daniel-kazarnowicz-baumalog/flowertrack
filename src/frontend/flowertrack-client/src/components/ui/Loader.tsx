import './Loader.css';

export interface LoaderProps {
  size?: 'sm' | 'md' | 'lg';
  fullScreen?: boolean;
  text?: string;
}

export function Loader({ size = 'md', fullScreen = false, text }: LoaderProps) {
  const sizeClass = `loader--${size}`;

  if (fullScreen) {
    return (
      <div className="loader-fullscreen">
        <div className={`loader ${sizeClass}`} />
        {text && <p className="loader-text">{text}</p>}
      </div>
    );
  }

  return (
    <div className="loader-wrapper">
      <div className={`loader ${sizeClass}`} />
      {text && <p className="loader-text">{text}</p>}
    </div>
  );
}
