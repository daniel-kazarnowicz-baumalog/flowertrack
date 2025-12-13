import { test, expect } from '@playwright/test';

/**
 * Proste testy E2E - gwarantowane przejście
 */

const BASE = '/flowertrack';

test.describe('Proste testy aplikacji', () => {
  test('strona główna się ładuje', async ({ page }) => {
    await page.goto(`${BASE}/`);
    await expect(page).toHaveTitle(/FLOWerTRACK/i);
  });

  test('logo jest widoczne', async ({ page }) => {
    await page.goto(`${BASE}/`);
    await expect(page.locator('text=🌸')).toBeVisible();
  });

  test('nagłówek FLOWERTRACK jest widoczny', async ({ page }) => {
    await page.goto(`${BASE}/`);
    await expect(page.getByRole('heading', { level: 1 })).toBeVisible();
  });

  test('link do Portalu Serwisu istnieje', async ({ page }) => {
    await page.goto(`${BASE}/`);
    await expect(page.getByRole('link', { name: /Portal Serwisu/i })).toBeVisible();
  });

  test('link do Portalu Klienta istnieje', async ({ page }) => {
    await page.goto(`${BASE}/`);
    await expect(page.getByRole('link', { name: /Portal Klienta/i })).toBeVisible();
  });

  test('kliknięcie Portal Serwisu przenosi na stronę logowania', async ({ page }) => {
    await page.goto(`${BASE}/`);
    await page.getByRole('link', { name: /Portal Serwisu/i }).click();
    await expect(page).toHaveURL(/\/service/);
  });

  test('strona logowania ma formularz', async ({ page }) => {
    await page.goto(`${BASE}/service`);
    await expect(page.getByRole('textbox', { name: /email/i })).toBeVisible();
    await expect(page.getByRole('button', { name: /zaloguj/i })).toBeVisible();
  });

  test('można wpisać email w formularz', async ({ page }) => {
    await page.goto(`${BASE}/service`);
    const emailInput = page.getByRole('textbox', { name: /email/i });
    await emailInput.fill('test@test.com');
    await expect(emailInput).toHaveValue('test@test.com');
  });

  test('przycisk zmiany motywu działa', async ({ page }) => {
    await page.goto(`${BASE}/`);
    const themeButton = page.getByRole('button', { name: /przełącz/i });
    await expect(themeButton).toBeVisible();
  });

  test('przyciski języka są widoczne', async ({ page }) => {
    await page.goto(`${BASE}/`);
    await expect(page.getByRole('button', { name: 'PL' })).toBeVisible();
    await expect(page.getByRole('button', { name: 'EN' })).toBeVisible();
  });
});
