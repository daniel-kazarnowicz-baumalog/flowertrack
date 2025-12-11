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
    this.heroSection = page
      .locator('[data-testid="hero-section"]')
      .or(page.locator('header'))
      .first();
    this.logo = page
      .locator('[data-testid="logo"]')
      .or(page.getByRole('img', { name: /logo/i }))
      .first();
    this.navigationBar = page.locator('[data-testid="navigation"]').or(page.locator('nav')).first();
    this.ctaButton = page
      .locator('[data-testid="cta-button"]')
      .or(page.getByRole('button', { name: /get started|start/i }))
      .first();
  }

  /**
   * Navigate to landing page
   */
  async goto() {
    await super.goto('/');
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
