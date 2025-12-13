import type { Page, Locator } from '@playwright/test';
import { BasePage } from './BasePage';

/**
 * Client Login Page Object
 * Represents the client portal login/authentication page
 */
export class ClientLoginPage extends BasePage {
  // Selectors
  readonly emailInput: Locator;
  readonly passwordInput: Locator;
  readonly submitButton: Locator;
  readonly errorMessage: Locator;
  readonly backLink: Locator;
  readonly activateLink: Locator;

  constructor(page: Page) {
    super(page);
    this.emailInput = page.getByRole('textbox', { name: /email/i });
    this.passwordInput = page.getByRole('textbox', { name: /hasło|password/i });
    this.submitButton = page.getByRole('button', { name: /zaloguj/i });
    this.errorMessage = page
      .locator('.input-message--error, [role="alert"], .toast--error')
      .first();
    this.backLink = page.getByRole('link', { name: /wróć|back/i });
    this.activateLink = page.getByRole('link', { name: /aktyw|activate/i });
  }

  /**
   * Navigate to client login page
   */
  async goto() {
    await super.goto('client');
    await this.page.waitForLoadState('domcontentloaded');
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
   * Check if logged in successfully (redirected to client dashboard)
   */
  async isLoggedIn(): Promise<boolean> {
    try {
      await this.page.waitForURL(/\/client\/(dashboard|tickets)/, { timeout: 5000 });
      return true;
    } catch {
      return false;
    }
  }
}
