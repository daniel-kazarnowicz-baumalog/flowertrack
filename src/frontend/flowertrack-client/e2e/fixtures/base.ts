import { test as base, expect } from '@playwright/test';

/**
 * Base fixtures for E2E tests
 * Extend this file with custom fixtures as needed
 */

type BaseFixtures = {
  // Add custom fixtures here
};

/**
 * Extended test with custom fixtures
 */
export const test = base.extend<BaseFixtures>({
  // Add fixture implementations here
});

export { expect };
