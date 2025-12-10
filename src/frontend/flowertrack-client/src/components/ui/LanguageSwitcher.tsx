import { useTranslation } from 'react-i18next';
import './LanguageSwitcher.css';

export const LanguageSwitcher = () => {
  const { i18n } = useTranslation();

  const changeLanguage = (lng: string) => {
    i18n.changeLanguage(lng);
  };

  return (
    <div className="languageSwitcher">
      <button
        className={`languageSwitcher__btn ${i18n.language === 'pl' ? 'active' : ''}`}
        onClick={() => changeLanguage('pl')}
      >
        PL
      </button>
      <span className="languageSwitcher__separator">|</span>
      <button
        className={`languageSwitcher__btn ${i18n.language === 'en' ? 'active' : ''}`}
        onClick={() => changeLanguage('en')}
      >
        EN
      </button>
    </div>
  );
};
