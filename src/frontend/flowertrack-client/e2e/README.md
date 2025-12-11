# Playwright E2E Tests for FLOWerTRACK

This directory contains end-to-end tests for the FLOWerTRACK application using Playwright.

## Structure

```
e2e/
├── fixtures/          # Test fixtures and custom test extensions
│   └── base.ts        # Base test fixture
├── pages/             # Page Object Models (POM)
│   ├── BasePage.ts    # Base page class
│   ├── LandingPage.ts # Landing page model
│   └── LoginPage.ts   # Login page model
├── utils/             # Helper utilities and test data
│   ├── helpers.ts     # Common test helpers
│   └── testData.ts    # Test data and fixtures
├── landing.spec.ts    # Landing page tests
├── login.spec.ts      # Login/auth tests
└── navigation.spec.ts # Navigation and routing tests
```

## Running Tests

### All Tests
```bash
npm run test:e2e
```

### Interactive UI Mode
```bash
npm run test:e2e:ui
```

### Headed Mode (see browser)
```bash
npm run test:e2e:headed
```

### Debug Mode
```bash
npm run test:e2e:debug
```

### View Last Report
```bash
npm run test:e2e:report
```

### Code Generator
```bash
npm run test:e2e:codegen
```

## Writing Tests

### Page Object Model Pattern

All tests use the Page Object Model (POM) pattern. Each page has its own class:

```typescript
import { LoginPage } from '../pages/LoginPage';

test('should login successfully', async ({ page }) => {
  const loginPage = new LoginPage(page);
  await loginPage.goto();
  await loginPage.login('user@example.com', 'password');
  
  const isLoggedIn = await loginPage.isLoggedIn();
  expect(isLoggedIn).toBeTruthy();
});
```

### Flexible Selectors

Tests use flexible selectors that work with or without `data-testid` attributes:

```typescript
// Prefers data-testid but falls back to semantic selectors
this.emailInput = page
  .locator('[data-testid="email-input"]')
  .or(page.getByLabel(/email/i))
  .first();
```

### Test Data

Use test data from `utils/testData.ts`:

```typescript
import { testUsers, generateTestData } from '../utils/testData';

// Use predefined test users
await loginPage.login(
  testUsers.serviceTechnician.email,
  testUsers.serviceTechnician.password
);

// Generate random data
const email = generateTestData.email();
```

## Best Practices

1. **Use Page Objects**: Keep selectors and page interactions in page classes
2. **Flexible Selectors**: Use semantic selectors with `data-testid` fallbacks
3. **Wait for Stability**: Use `waitForLoadState('networkidle')` after navigation
4. **Descriptive Names**: Test names should clearly describe what they test
5. **Skip When Needed**: Use `test.skip()` for tests requiring backend setup
6. **Clean Up**: Reset state between tests using `beforeEach` hooks
7. **Assertions**: Always include meaningful assertions with clear expectations

## Configuration

Configuration is in `playwright.config.ts`. Key settings:

- **Base URL**: `http://localhost:5173` (configurable via `PLAYWRIGHT_BASE_URL`)
- **Browsers**: Chromium, Firefox, WebKit
- **Mobile**: Pixel 7, iPhone 14
- **Retries**: 2 on CI, 0 locally
- **Timeout**: 30s per test
- **Screenshots**: On failure
- **Video**: Retained on failure
- **Trace**: On first retry

## CI/CD

Tests are optimized for CI:
- Automatic dev server startup
- Parallel execution (disabled on CI for stability)
- HTML and JSON reports
- Artifacts saved on failure

## Debugging

### Visual Debugging
```bash
npm run test:e2e:ui
```

### Step-by-step Debugging
```bash
npm run test:e2e:debug
```

### Screenshots
Screenshots are automatically taken on test failures in the `screenshots/` directory.

### Traces
Traces are recorded on first retry and can be viewed in the Playwright Trace Viewer.

## Adding New Tests

1. Create a new `.spec.ts` file in `e2e/`
2. Create corresponding page objects in `e2e/pages/` if needed
3. Use flexible selectors that work without `data-testid`
4. Add test data to `e2e/utils/testData.ts` if needed
5. Follow existing test structure and patterns
6. Run tests to verify they work

## Environment Variables

- `PLAYWRIGHT_BASE_URL`: Override base URL (default: `http://localhost:5173`)
- `CI`: Set to enable CI-specific behavior

## Troubleshooting

### Tests timing out
- Increase timeout in `playwright.config.ts`
- Check if dev server is running
- Verify network conditions

### Selector not found
- Add `data-testid` attributes to components
- Update page objects with better selectors
- Check if element is actually rendered

### Flaky tests
- Add explicit waits: `waitForLoadState('networkidle')`
- Use `waitFor()` on elements
- Avoid hard-coded timeouts
- Check for race conditions

## Resources

- [Playwright Documentation](https://playwright.dev)
- [Best Practices](https://playwright.dev/docs/best-practices)
- [Page Object Model](https://playwright.dev/docs/pom)
- [Debugging Guide](https://playwright.dev/docs/debug)
