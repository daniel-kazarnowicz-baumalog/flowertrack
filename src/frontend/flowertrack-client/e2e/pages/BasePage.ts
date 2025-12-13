import type { Page } from '@playwright/test';

/**
 * Base Page Object Model class
 * All page objects should extend this class
 */
export class BasePage {
  constructor(protected readonly page: Page) {}

  /**
   * Navigate to a specific path
   * Path should be relative without leading slash (e.g., 'service', 'client')
   * baseURL in playwright.config already includes /flowertrack/
   */
  async goto(path: string = '') {
    // Remove leading slash if present to avoid double slashes with baseURL
    const cleanPath = path.startsWith('/') ? path.slice(1) : path;
    await this.page.goto(cleanPath);
  }

  /**
   * Get page title
   */
  async getTitle(): Promise<string> {
    return await this.page.title();
  }

  /**
   * Wait for page to be fully loaded
   */
  async waitForPageLoad() {
    await this.page.waitForLoadState('networkidle');
  }

  /**
   * Take a screenshot
   */
  async screenshot(name: string) {
    await this.page.screenshot({ path: `screenshots/${name}.png`, fullPage: true });
  }
}
