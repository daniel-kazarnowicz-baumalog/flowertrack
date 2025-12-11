import { test, expect } from './fixtures/base';
import { LoginPage } from './pages/LoginPage';
import { testUsers, testData } from './utils/testData';

/**
 * Login Page E2E Tests
 * Tests for authentication and login functionality
 */

test.describe('Login Page', () => {
  let loginPage: LoginPage;

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page);
    await loginPage.goto();
  });

  test('should load login page successfully', async ({ page }) => {
    await expect(page).toHaveURL(/\/service/);
  });

  test('should display login form elements', async ({ page }) => {
    // Verify all login form elements are present
    await expect(
      page.getByLabel(/email/i).or(page.locator('[data-testid="email-input"]')).first()
    ).toBeVisible();

    await expect(
      page
        .getByLabel(/password|hasło/i)
        .or(page.locator('[data-testid="password-input"]'))
        .first()
    ).toBeVisible();

    await expect(
      page
        .getByRole('button', { name: /login|sign in|zaloguj/i })
        .or(page.locator('[data-testid="submit-button"]'))
        .first()
    ).toBeVisible();
  });

  test('should show validation error for empty fields', async ({ page }) => {
    await loginPage.submit();

    // Wait for validation errors to appear
    await page.waitForTimeout(500);

    // Check if form has validation (HTML5 or custom)
    const emailInput = page
      .getByLabel(/email/i)
      .or(page.locator('[data-testid="email-input"]'))
      .first();
    const isInvalid = await emailInput.evaluate((el: HTMLInputElement) => {
      return el.validity.valueMissing || el.hasAttribute('aria-invalid');
    });

    expect(isInvalid).toBeTruthy();
  });

  test('should show error for invalid email format', async () => {
    await loginPage.fillEmail(testData.invalidEmail);
    await loginPage.fillPassword('SomePassword123!');
    await loginPage.submit();

    // Wait for potential error message
    await loginPage.page.waitForTimeout(1000);

    const hasError = await loginPage.hasError();
    // Either client-side validation or server error should occur
    expect(hasError).toBeTruthy();
  });

  test.skip('should handle non-existent user credentials', async () => {
    // Skip this test until backend is ready and test users are set up
    await loginPage.login(testData.nonExistentUser.email, testData.nonExistentUser.password);

    await loginPage.page.waitForTimeout(2000);

    const hasError = await loginPage.hasError();
    expect(hasError).toBeTruthy();
  });

  test.skip('should successfully login with valid credentials', async () => {
    // Skip this test until backend is ready and test users are set up
    await loginPage.login(testUsers.serviceTechnician.email, testUsers.serviceTechnician.password);

    // Wait for redirect
    await loginPage.page.waitForTimeout(2000);

    const isLoggedIn = await loginPage.isLoggedIn();
    expect(isLoggedIn).toBeTruthy();
  });

  test('should have password field type as password', async ({ page }) => {
    const passwordInput = page
      .getByLabel(/password|hasło/i)
      .or(page.locator('[data-testid="password-input"]'))
      .first();

    const inputType = await passwordInput.getAttribute('type');
    expect(inputType).toBe('password');
  });

  test('should be responsive on mobile devices', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 667 });
    await page.waitForLoadState('networkidle');

    // Verify form is still visible and usable on mobile
    await expect(
      page.getByLabel(/email/i).or(page.locator('[data-testid="email-input"]')).first()
    ).toBeVisible();
  });

  test('should have no console errors on load', async ({ page }) => {
    const consoleErrors: string[] = [];

    page.on('console', (msg) => {
      if (msg.type() === 'error') {
        consoleErrors.push(msg.text());
      }
    });

    await loginPage.goto();
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(2000);

    expect(consoleErrors).toHaveLength(0);
  });

  test('should successfully login with admin credentials', async ({ page }) => {
    // Test logowania dla użytkownika admin@flowertrack.dev
    await loginPage.goto();

    // Wypełnij pole email
    await loginPage.fillEmail(testUsers.adminUser.email);

    // Wypełnij pole hasła
    await loginPage.fillPassword(testUsers.adminUser.password);

    // Kliknij przycisk zalogowania
    await loginPage.submit();

    // Poczekaj na nawigację po zalogowaniu
    await page.waitForTimeout(2000);

    // Sprawdź czy użytkownik został zalogowany
    // Możliwe sprawdzenia:
    // 1. URL zmienił się (np. na /dashboard lub /home)
    // 2. Widoczne jest menu użytkownika
    // 3. Nie ma komunikatu o błędzie

    const currentUrl = page.url();
    const hasError = await loginPage.hasError();

    // Oczekujemy że nie ma błędu i URL zmienił się z /service (login page)
    expect(hasError).toBeFalsy();
    expect(currentUrl).toContain('/service/dashboard');
  });
});
