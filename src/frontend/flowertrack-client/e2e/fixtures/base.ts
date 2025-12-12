import { test as base, expect } from '@playwright/test';

/**
 * Base fixtures for E2E tests
 * Extend this file with custom fixtures as needed
 */

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
type BaseFixtures = {
  // Add custom fixtures here - empty for now, will be extended later
};

/**
 * Extended test with custom fixtures
 */
export const test = base.extend<BaseFixtures>({
  // Add fixture implementations here
});

export { expect };
