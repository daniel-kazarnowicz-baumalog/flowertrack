import React from 'react';
import type { InputHTMLAttributes } from 'react';
import './Checkbox.css';

interface CheckboxProps extends InputHTMLAttributes<HTMLInputElement> {
    label: string;
    error?: string;
}

export const Checkbox: React.FC<CheckboxProps> = ({ label, error, className, ...props }) => {
    return (
        <div className={`checkbox-wrapper ${className || ''}`}>
            <label className="checkbox-label">
                <input type="checkbox" className="checkbox-input" {...props} />
                <span className="checkbox-text">{label}</span>
            </label>
            {error && <span className="checkbox-error">{error}</span>}
        </div>
    );
};
