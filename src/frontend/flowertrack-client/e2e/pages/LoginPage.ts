import type { Page, Locator } from '@playwright/test';
import { BasePage } from './BasePage';

/**
 * Login Page Object
 * Represents the login/authentication page
 */
export class LoginPage extends BasePage {
  // Selectors
  private readonly emailInput: Locator;
  private readonly passwordInput: Locator;
  private readonly submitButton: Locator;
  private readonly errorMessage: Locator;
  private readonly successMessage: Locator;

  constructor(page: Page) {
    super(page);
    this.emailInput = page
      .locator('[data-testid="email-input"]')
      .or(page.getByLabel(/email/i))
      .first();
    this.passwordInput = page
      .locator('[data-testid="password-input"]')
      .or(page.getByLabel(/password|hasło/i))
      .first();
    this.submitButton = page
      .locator('[data-testid="submit-button"]')
      .or(page.getByRole('button', { name: /login|sign in|zaloguj/i }))
      .first();
    this.errorMessage = page
      .locator('[data-testid="error-message"]')
      .or(page.locator('.error, .alert-error'))
      .first();
    this.successMessage = page
      .locator('[data-testid="success-message"]')
      .or(page.locator('.success, .alert-success'))
      .first();
  }

  /**
   * Navigate to login page
   */
  async goto() {
    await super.goto('/service');
  }

  /**
   * Fill in email field
   */
  async fillEmail(email: string) {
    await this.emailInput.fill(email);
  }

  /**
   * Fill in password field
   */
  async fillPassword(password: string) {
    await this.passwordInput.fill(password);
  }

  /**
   * Click submit/login button
   */
  async submit() {
    await this.submitButton.click();
  }

  /**
   * Perform complete login action
   */
  async login(email: string, password: string) {
    await this.fillEmail(email);
    await this.fillPassword(password);
    await this.submit();
  }

  /**
   * Check if error message is visible
   */
  async hasError(): Promise<boolean> {
    try {
      return await this.errorMessage.isVisible({ timeout: 3000 });
    } catch {
      return false;
    }
  }

  /**
   * Get error message text
   */
  async getErrorMessage(): Promise<string> {
    return (await this.errorMessage.textContent()) || '';
  }

  /**
   * Check if logged in successfully (redirected away from login page)
   */
  async isLoggedIn(): Promise<boolean> {
    await this.page.waitForURL(/\/(dashboard|home|tickets)/, { timeout: 5000 });
    return !this.page.url().includes('/login');
  }
}
