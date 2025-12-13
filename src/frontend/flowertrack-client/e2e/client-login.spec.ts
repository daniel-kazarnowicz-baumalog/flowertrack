import { test, expect } from './fixtures/base';
import { ClientLoginPage } from './pages/ClientLoginPage';
import { testData } from './utils/testData';

/**
 * Client Login Page E2E Tests
 * Comprehensive tests for client portal authentication
 */

test.describe('Client Login Page', () => {
  let clientLoginPage: ClientLoginPage;

  test.beforeEach(async ({ page }) => {
    clientLoginPage = new ClientLoginPage(page);
    await clientLoginPage.goto();
    await page.waitForLoadState('networkidle');
  });

  test.describe('Page Load & Structure', () => {
    test('should load client login page with correct URL', async ({ page }) => {
      await expect(page).toHaveURL(/\/client/);
      await expect(page).toHaveTitle(/FLOWerTRACK/i);
    });

    test('should display client portal heading', async ({ page }) => {
      // Check for client-specific heading - "Witaj ponownie"
      await expect(page.getByRole('heading', { name: /witaj|welcome/i })).toBeVisible();
    });

    test('should display all login form elements', async () => {
      await expect(clientLoginPage.emailInput).toBeVisible();
      await expect(clientLoginPage.passwordInput).toBeVisible();
      await expect(clientLoginPage.submitButton).toBeVisible();
      await expect(clientLoginPage.backLink).toBeVisible();
    });
  });

  test.describe('Form Validation', () => {
    test('should validate empty email field', async ({ page }) => {
      await clientLoginPage.fillPassword('SomePassword123!');
      await clientLoginPage.submit();

      await page.waitForTimeout(500);

      const emailInput = clientLoginPage.emailInput;
      const isInvalid = await emailInput.evaluate((el: HTMLInputElement) => {
        return el.validity.valueMissing || el.hasAttribute('aria-invalid');
      });

      expect(isInvalid).toBeTruthy();
    });

    test('should validate invalid email format', async ({ page }) => {
      await clientLoginPage.fillEmail(testData.invalidEmail);
      await clientLoginPage.fillPassword('SomePassword123!');
      await clientLoginPage.submit();

      await page.waitForTimeout(500);

      const emailInput = clientLoginPage.emailInput;
      const isInvalid = await emailInput.evaluate((el: HTMLInputElement) => {
        return !el.validity.valid || el.hasAttribute('aria-invalid');
      });

      expect(isInvalid).toBeTruthy();
    });
  });

  test.describe('Input Fields', () => {
    test('should accept email input', async () => {
      const testEmail = 'client@example.com';
      await clientLoginPage.fillEmail(testEmail);
      await expect(clientLoginPage.emailInput).toHaveValue(testEmail);
    });

    test('should accept password input', async () => {
      const testPassword = 'ClientPassword123!';
      await clientLoginPage.fillPassword(testPassword);
      await expect(clientLoginPage.passwordInput).toHaveValue(testPassword);
    });
  });

  test.describe('Navigation', () => {
    test('should navigate back to gateway', async ({ page }) => {
      await clientLoginPage.backLink.click();
      await expect(page).toHaveURL(/\/flowertrack\/?$/);
    });

    test('should navigate to service portal', async ({ page }) => {
      // Link text is "Przejdź tutaj" (Go here)
      const serviceLink = page.getByRole('link', { name: /przejdź tutaj|go here/i });
      await serviceLink.click();
      await expect(page).toHaveURL(/\/service/);
    });
  });

  test.describe('Responsiveness', () => {
    test('should be usable on mobile viewport', async ({ page }) => {
      await page.setViewportSize({ width: 375, height: 667 });
      await page.waitForLoadState('networkidle');

      await expect(clientLoginPage.emailInput).toBeVisible();
      await expect(clientLoginPage.passwordInput).toBeVisible();
      await expect(clientLoginPage.submitButton).toBeVisible();
    });

    test('should be usable on tablet viewport', async ({ page }) => {
      await page.setViewportSize({ width: 768, height: 1024 });
      await page.waitForLoadState('networkidle');

      await expect(clientLoginPage.emailInput).toBeVisible();
      await expect(clientLoginPage.submitButton).toBeVisible();
    });
  });

  test.describe('Theme & Language', () => {
    test('should have theme toggle button', async ({ page }) => {
      const themeButton = page.getByRole('button', { name: /przełącz|toggle|motyw|theme/i });
      await expect(themeButton).toBeVisible();
    });

    test('should have language toggle buttons', async ({ page }) => {
      const plButton = page.getByRole('button', { name: 'PL' });
      const enButton = page.getByRole('button', { name: 'EN' });

      await expect(plButton).toBeVisible();
      await expect(enButton).toBeVisible();
    });
  });
});
