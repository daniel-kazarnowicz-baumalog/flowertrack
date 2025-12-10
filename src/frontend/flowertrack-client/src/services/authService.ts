import axios from 'axios';
import { apiClient } from '../lib/apiClient';

export interface LoginResponse {
  accessToken: string;
  refreshToken?: string;
  user: {
    id: string;
    email: string;
    fullName?: string;
    role?: string;
    organizationId?: string;
    organizationName?: string;
  };
}

export interface AuthServiceType {
  loginService: (email: string, password: string) => Promise<LoginResponse>;
  loginClient: (email: string, password: string) => Promise<LoginResponse>;
  logout: () => Promise<void>;
  refreshToken: () => Promise<string>;
  forgotPassword: (email: string) => Promise<void>;
  resetPassword: (token: string, newPassword: string) => Promise<void>;
}

export const authService: AuthServiceType = {
  /**
   * Login as service user (technician/admin)
   */
  loginService: async (email: string, password: string): Promise<LoginResponse> => {
    try {
      const response = await apiClient.post<LoginResponse>('/auth/service/login', {
        email,
        password,
      });
      console.log('[AuthService] Login response:', response.data);
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw new Error(error.response?.data?.message || 'Nieprawidłowy email lub hasło');
      }
      throw error;
    }
  },

  /**
   * Login as client user
   */
  loginClient: async (email: string, password: string): Promise<LoginResponse> => {
    try {
      const response = await apiClient.post<LoginResponse>('/auth/client/login', {
        email,
        password,
      });
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw new Error(error.response?.data?.message || 'Nieprawidłowy email lub hasło');
      }
      throw error;
    }
  },

  /**
   * Logout and invalidate token
   */
  logout: async (): Promise<void> => {
    try {
      await apiClient.post('/auth/logout');
    } catch {
      // Ignore logout errors
    }
  },

  /**
   * Refresh authentication token
   */
  refreshToken: async (): Promise<string> => {
    const response = await apiClient.post<{ accessToken: string }>('/auth/refresh');
    return response.data.accessToken;
  },

  /**
   * Request password reset email
   */
  forgotPassword: async (email: string): Promise<void> => {
    await apiClient.post('/auth/forgot-password', { email });
  },

  /**
   * Reset password with token
   */
  resetPassword: async (token: string, newPassword: string): Promise<void> => {
    await apiClient.post('/auth/reset-password', {
      token,
      newPassword,
    });
  },
};

export default authService;
