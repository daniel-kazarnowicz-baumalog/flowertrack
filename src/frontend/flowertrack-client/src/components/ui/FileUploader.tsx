import React, { useRef, useState } from 'react';
import type { DragEvent, ChangeEvent } from 'react';
import './FileUploader.css';

interface FileUploaderProps {
    onFilesSelected: (files: File[]) => void;
    maxFiles?: number;
    maxSizeInBytes?: number;
    accept?: string;
    disabled?: boolean;
}

export const FileUploader: React.FC<FileUploaderProps> = ({
    onFilesSelected,
    maxFiles = 3,
    maxSizeInBytes = 10 * 1024 * 1024, // 10MB
    accept = '*',
    disabled = false,
}) => {
    const [isDragging, setIsDragging] = useState(false);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const handleDragOver = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        if (disabled) return;
        setIsDragging(true);
    };

    const handleDragLeave = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(false);
    };

    const handleDrop = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(false);
        if (disabled) return;

        if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
            validateAndPassFiles(Array.from(e.dataTransfer.files));
        }
    };

    const handleFileInputChange = (e: ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files.length > 0) {
            validateAndPassFiles(Array.from(e.target.files));
        }
    };

    const validateAndPassFiles = (files: File[]) => {
        const validFiles: File[] = [];

        // In a real app we might want to show errors for invalid files.
        // For now, we just filter them or take the first N.

        for (const file of files) {
            if (file.size > maxSizeInBytes) {
                // Skip too large
                continue;
            }
            validFiles.push(file);
        }

        // Limit count
        const limitedFiles = validFiles.slice(0, maxFiles);
        onFilesSelected(limitedFiles);

        // Reset input
        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
    };

    const handleButtonClick = () => {
        fileInputRef.current?.click();
    };

    return (
        <div
            className={`file-uploader ${isDragging ? 'dragging' : ''} ${disabled ? 'disabled' : ''}`}
            onDragOver={handleDragOver}
            onDragLeave={handleDragLeave}
            onDrop={handleDrop}
        >
            <input
                type="file"
                ref={fileInputRef}
                onChange={handleFileInputChange}
                multiple
                accept={accept}
                className="file-input"
                disabled={disabled}
            />

            <div className="uploader-content">
                <div className="upload-icon">
                    <svg width="40" height="40" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M12 16L12 8" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                        <path d="M9 11L12 8L15 11" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                        <path d="M8 16H16" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                </div>
                <p className="upload-text">
                    <span className="link-text" onClick={disabled ? undefined : handleButtonClick}>
                        Kliknij
                    </span>
                    {' '}lub upuść pliki tutaj
                </p>
                <p className="upload-hint">
                    Max {maxFiles} pliki, każdy do {maxSizeInBytes / 1024 / 1024}MB
                </p>
            </div>
        </div>
    );
};
