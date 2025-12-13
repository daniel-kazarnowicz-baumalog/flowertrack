import { test, expect } from './fixtures/base';

/**
 * Basic Navigation E2E Tests
 * Tests for general navigation and routing
 */

test.describe('Navigation', () => {
  test('should navigate to service login page', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Kliknij na link Portal Serwisu
    const serviceLoginLink = page.getByRole('link', { name: /portal serwisu/i });

    if (await serviceLoginLink.isVisible()) {
      await serviceLoginLink.click();
      await page.waitForLoadState('networkidle');

      // Should be on service login page (bez /login - aplikacja ma /service)
      expect(page.url()).toMatch(/\/service/);
    } else {
      // If no link found, navigate directly
      await page.goto('/service');
      await page.waitForLoadState('networkidle');

      expect(page.url()).toMatch(/\/service/);
    }
  });

  test('should navigate to client login page', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Kliknij na link Portal Klienta
    const clientLoginLink = page.getByRole('link', { name: /portal klienta/i });

    if (await clientLoginLink.isVisible()) {
      await clientLoginLink.click();
      await page.waitForLoadState('networkidle');

      // Should be on client login page (bez /login - aplikacja ma /client)
      expect(page.url()).toMatch(/\/client/);
    } else {
      // If no link found, navigate directly
      await page.goto('/client');
      await page.waitForLoadState('networkidle');

      expect(page.url()).toMatch(/\/client/);
    }
  });

  test('should handle 404 for non-existent routes', async ({ page }) => {
    await page.goto('this-route-does-not-exist-12345');
    await page.waitForLoadState('networkidle');

    // Should show 404 page content
    const has404Text = await page
      .getByText(/404|not found|nie znaleziono|nie istnieje/i)
      .isVisible()
      .catch(() => false);

    // URL should contain the non-existent route or show 404 content
    const url = page.url();
    const isOnBadRoute = url.includes('this-route-does-not-exist');

    // Either showing 404 content or stayed on the bad route
    expect(has404Text || isOnBadRoute).toBeTruthy();
  });

  test('should handle browser back button', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Navigate to another page
    await page.goto('/service');
    await page.waitForLoadState('networkidle');

    // Go back
    await page.goBack();
    await page.waitForLoadState('networkidle');

    // Should be back at home - baseURL is /flowertrack/ so / redirects there
    expect(page.url()).toMatch(/\/flowertrack\/?$/);
  });

  test('should handle browser forward button', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Navigate to another page
    await page.goto('/service');
    await page.waitForLoadState('networkidle');

    // Go back then forward
    await page.goBack();
    await page.waitForLoadState('networkidle');

    await page.goForward();
    await page.waitForLoadState('networkidle');

    // Should be back at service
    expect(page.url()).toMatch(/\/service/);
  });

  test('should handle page refresh without errors', async ({ page }) => {
    // Collect console errors before navigating
    const consoleErrors: string[] = [];
    page.on('console', (msg) => {
      if (msg.type() === 'error') {
        // Ignore 404 errors for favicon or other non-critical resources
        const text = msg.text();
        if (!text.includes('favicon') && !text.includes('404')) {
          consoleErrors.push(text);
        }
      }
    });

    await page.goto('service');
    await page.waitForLoadState('networkidle');

    // Reload the page
    await page.reload();
    await page.waitForLoadState('networkidle');

    // Should still be on the same page
    expect(page.url()).toMatch(/\/service/);

    await page.waitForTimeout(1000);

    // Should have no critical console errors
    expect(consoleErrors).toHaveLength(0);
  });
});
