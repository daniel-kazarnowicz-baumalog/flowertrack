import type { Page, Locator } from '@playwright/test';
import { BasePage } from './BasePage';

/**
 * Landing Page Object
 * Represents the main landing page of the application
 */
export class LandingPage extends BasePage {
  // Selectors
  private readonly heroSection: Locator;
  private readonly logo: Locator;
  private readonly navigationBar: Locator;
  private readonly ctaButton: Locator;

  constructor(page: Page) {
    super(page);
    // Strona główna ma nagłówek h1 z nazwą aplikacji i karty portali
    this.heroSection = page.getByRole('heading', { name: /FLOW.*TRACK/i }).first();
    this.logo = page.locator('text="🌸"').first();
    // Linki do portalów zamiast nav
    this.navigationBar = page.getByRole('link', { name: /portal/i }).first();
    this.ctaButton = page.getByRole('link', { name: /portal serwisu|service/i }).first();
  }

  /**
   * Navigate to landing page (gateway)
   */
  async goto() {
    await super.goto('');
    await this.page.waitForLoadState('domcontentloaded');
  }

  /**
   * Check if hero section is visible
   */
  async isHeroVisible(): Promise<boolean> {
    return await this.heroSection.isVisible();
  }

  /**
   * Check if logo is visible
   */
  async isLogoVisible(): Promise<boolean> {
    return await this.logo.isVisible();
  }

  /**
   * Check if navigation bar is visible
   */
  async isNavigationVisible(): Promise<boolean> {
    return await this.navigationBar.isVisible();
  }

  /**
   * Click the CTA button
   */
  async clickCTA() {
    await this.ctaButton.click();
  }

  /**
   * Get all navigation links
   */
  async getNavigationLinks(): Promise<string[]> {
    const links = await this.navigationBar.locator('a').allTextContents();
    return links;
  }
}
