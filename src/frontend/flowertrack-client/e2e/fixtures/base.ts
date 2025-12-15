/* eslint-disable react-hooks/rules-of-hooks */
import { test as base, expect, type Page } from '@playwright/test';
import { testUsers } from '../utils/testData';

/**
 * Authentication helper - stores auth state in localStorage
 */
async function authenticateAs(
  page: Page,
  role: 'service' | 'client',
  userKey: keyof typeof testUsers = 'adminUser'
) {
  const user = testUsers[userKey];

  // Create mock user object
  const userData = {
    id: 'test-user-id-' + Date.now(),
    email: user.email,
    role,
    name: 'Test User',
    fullName: 'Test User',
    firstName: 'Test',
    lastName: 'User',
    isAdmin: user.role === 'admin' || user.role === 'service_admin',
  };

  // Create mock token (for testing purposes)
  const token = 'test-token-' + Date.now();

  // Navigate to base URL first to set localStorage in correct origin
  await page.goto('');

  // Set auth state in localStorage
  await page.evaluate(
    ({ token, userData }) => {
      localStorage.setItem('accessToken', token);
      localStorage.setItem('user', JSON.stringify(userData));
    },
    { token, userData }
  );
}

/**
 * Extended fixtures for E2E tests
 */
type BaseFixtures = {
  /** Authenticate as service user before test */
  authenticatedServicePage: Page;
  /** Authenticate as client user before test */
  authenticatedClientPage: Page;
};

/**
 * Extended test with custom fixtures
 */
export const test = base.extend<BaseFixtures>({
  authenticatedServicePage: async ({ page }, use) => {
    await authenticateAs(page, 'service', 'adminUser');
    await use(page);
  },

  authenticatedClientPage: async ({ page }, use) => {
    await authenticateAs(page, 'client', 'clientUser');
    await use(page);
  },
});

export { expect };

/**
 * Helper to clear authentication
 */
export async function clearAuth(page: Page) {
  await page.evaluate(() => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('user');
  });
}
