import { test, expect } from '../fixtures/base';
import { LandingPage } from '../pages/LandingPage';

/**
 * Landing Page E2E Tests
 * Tests for the main landing page functionality
 */

test.describe('Landing Page', () => {
  let landingPage: LandingPage;

  test.beforeEach(async ({ page }) => {
    landingPage = new LandingPage(page);
    await landingPage.goto();
  });

  test('should load successfully', async ({ page }) => {
    await expect(page).toHaveTitle(/FLOWerTRACK|flowertrack/i);
  });

  test('should display main elements', async () => {
    // Check if main page elements are visible
    // These tests use flexible selectors that work with or without data-testid attributes
    const isHeroVisible = await landingPage.isHeroVisible();
    expect(isHeroVisible).toBeTruthy();
  });

  test('should have working navigation', async () => {
    const isNavVisible = await landingPage.isNavigationVisible();
    expect(isNavVisible).toBeTruthy();
  });

  test('should be responsive', async ({ page }) => {
    // Test mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });
    await page.waitForLoadState('networkidle');

    const isNavVisible = await landingPage.isNavigationVisible();
    expect(isNavVisible).toBeTruthy();

    // Test tablet viewport
    await page.setViewportSize({ width: 768, height: 1024 });
    await page.waitForLoadState('networkidle');

    // Test desktop viewport
    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.waitForLoadState('networkidle');
  });

  test('should have no console errors', async ({ page }) => {
    const consoleErrors: string[] = [];

    page.on('console', (msg) => {
      if (msg.type() === 'error') {
        consoleErrors.push(msg.text());
      }
    });

    await landingPage.goto();
    await page.waitForLoadState('networkidle');

    // Allow some time for any lazy-loaded errors
    await page.waitForTimeout(2000);

    expect(consoleErrors).toHaveLength(0);
  });

  test('should load within acceptable time', async ({ page }) => {
    const startTime = Date.now();
    await landingPage.goto();
    await page.waitForLoadState('networkidle');
    const loadTime = Date.now() - startTime;

    // Page should load within 5 seconds
    expect(loadTime).toBeLessThan(5000);
  });
});
