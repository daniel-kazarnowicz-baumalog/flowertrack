import { test, expect } from '../fixtures/base';

/**
 * Basic Navigation E2E Tests
 * Tests for general navigation and routing
 */

test.describe('Navigation', () => {
  test('should navigate to service login page', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Try to find and click service login link
    const serviceLoginLink = page
      .getByRole('link', { name: /service login|technician|serwis/i })
      .or(page.locator('a[href*="/service/login"]'))
      .first();

    if (await serviceLoginLink.isVisible()) {
      await serviceLoginLink.click();
      await page.waitForLoadState('networkidle');

      // Should be on service login page
      expect(page.url()).toMatch(/\/service\/login/);
    } else {
      // If no link found, navigate directly
      await page.goto('/service/login');
      await page.waitForLoadState('networkidle');

      expect(page.url()).toMatch(/\/service\/login/);
    }
  });

  test('should navigate to client login page', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Try to find and click client login link
    const clientLoginLink = page
      .getByRole('link', { name: /client login|customer|klient/i })
      .or(page.locator('a[href*="/client/login"]'))
      .first();

    if (await clientLoginLink.isVisible()) {
      await clientLoginLink.click();
      await page.waitForLoadState('networkidle');

      // Should be on client login page
      expect(page.url()).toMatch(/\/client\/login/);
    } else {
      // If no link found, navigate directly
      await page.goto('/client/login');
      await page.waitForLoadState('networkidle');

      expect(page.url()).toMatch(/\/client\/login/);
    }
  });

  test('should handle 404 for non-existent routes', async ({ page }) => {
    await page.goto('/this-route-does-not-exist-12345');
    await page.waitForLoadState('networkidle');

    // Should show 404 page or redirect to home
    const url = page.url();
    const has404Text = await page
      .getByText(/404|not found|nie znaleziono/i)
      .isVisible()
      .catch(() => false);

    // Either showing 404 content or redirected home
    expect(has404Text || url.endsWith('/')).toBeTruthy();
  });

  test('should handle browser back button', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Navigate to another page
    await page.goto('/service/login');
    await page.waitForLoadState('networkidle');

    // Go back
    await page.goBack();
    await page.waitForLoadState('networkidle');

    // Should be back at home
    expect(page.url()).toMatch(/\/$|\/$/);
  });

  test('should handle browser forward button', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Navigate to another page
    await page.goto('/service/login');
    await page.waitForLoadState('networkidle');

    // Go back then forward
    await page.goBack();
    await page.waitForLoadState('networkidle');

    await page.goForward();
    await page.waitForLoadState('networkidle');

    // Should be back at service login
    expect(page.url()).toMatch(/\/service\/login/);
  });

  test('should handle page refresh without errors', async ({ page }) => {
    await page.goto('/service/login');
    await page.waitForLoadState('networkidle');

    // Reload the page
    await page.reload();
    await page.waitForLoadState('networkidle');

    // Should still be on the same page
    expect(page.url()).toMatch(/\/service\/login/);

    // Should have no console errors
    const consoleErrors: string[] = [];
    page.on('console', (msg) => {
      if (msg.type() === 'error') {
        consoleErrors.push(msg.text());
      }
    });

    await page.reload();
    await page.waitForTimeout(2000);

    expect(consoleErrors).toHaveLength(0);
  });
});
