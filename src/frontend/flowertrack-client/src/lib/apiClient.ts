import axios, { AxiosError } from 'axios';
import type { AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import { env } from './env';

/**
 * API Client for FLOWerTRACK backend
 * Configured with interceptors for authentication and error handling
 */

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: env.apiBaseUrl,
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    // Request interceptor - inject auth token
    this.client.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        const token = this.getAccessToken();
        if (token && config.headers) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor - handle errors globally
    this.client.interceptors.response.use(
      (response) => response,
      async (error: AxiosError) => {
        if (error.response) {
          const { status } = error.response;

          // Handle 401 Unauthorized - token expired or invalid
          if (status === 401) {
            console.error('[ApiClient] 401 Unauthorized. Redirecting to login.');
            this.clearAuth();
            // Redirect to login if not already there
            const currentPath = window.location.pathname;
            if (!currentPath.includes('/login') && !currentPath.includes('/activate')) {
              const isServicePortal = currentPath.startsWith('/service');
              window.location.href = isServicePortal ? '/service/login' : '/client/login';
            }
          }

          // Handle 403 Forbidden
          if (status === 403) {
            // Optionally redirect to forbidden page
            console.error('Access forbidden:', error.response.data);
          }
        }

        return Promise.reject(error);
      }
    );
  }

  private getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  private clearAuth() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  }

  /**
   * Get Axios instance for making requests
   */
  public getInstance(): AxiosInstance {
    return this.client;
  }
}

// Export singleton instance
export const apiClient = new ApiClient().getInstance();

/**
 * Helper to extract error message from API response
 */
export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as { message?: string; title?: string };
    return data?.message || data?.title || error.message || 'An error occurred';
  }
  if (error instanceof Error) {
    return error.message;
  }
  return 'An unknown error occurred';
}
