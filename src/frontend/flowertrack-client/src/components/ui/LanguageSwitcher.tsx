import { useTranslation } from 'react-i18next';
import './LanguageSwitcher.css';

const languages = [
    { code: 'pl', label: 'PL', flag: '🇵🇱' },
    { code: 'en', label: 'EN', flag: '🇬🇧' },
];

export const LanguageSwitcher = () => {
    const { i18n } = useTranslation();
    const currentLang = i18n.language;

    const handleLanguageChange = (langCode: string) => {
        i18n.changeLanguage(langCode);
    };

    return (
        <div className="languageSwitcher">
            {languages.map((lang) => (
                <button
                    key={lang.code}
                    className={`languageSwitcher__btn ${currentLang === lang.code ? 'languageSwitcher__btn--active' : ''}`}
                    onClick={() => handleLanguageChange(lang.code)}
                    title={lang.label}
                >
                    <span className="languageSwitcher__flag">{lang.flag}</span>
                    <span className="languageSwitcher__label">{lang.label}</span>
                </button>
            ))}
        </div>
    );
};

export default LanguageSwitcher;
