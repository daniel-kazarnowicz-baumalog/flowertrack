/**
 * Test data helpers and utilities
 */

export const testUsers = {
  serviceAdmin: {
    email: 'service.admin@flowertrack.test',
    password: 'TestPassword123!',
    role: 'service_admin',
  },
  serviceTechnician: {
    email: 'technician@flowertrack.test',
    password: 'TestPassword123!',
    role: 'service_technician',
  },
  clientAdmin: {
    email: 'client.admin@flowertrack.test',
    password: 'TestPassword123!',
    role: 'client_admin',
  },
  clientUser: {
    email: 'client.user@flowertrack.test',
    password: 'TestPassword123!',
    role: 'client_user',
  },
  adminUser: {
    email: 'admin@flowertrack.dev',
    password: 'Admin123!',
    role: 'admin',
  },
};

export const testData = {
  invalidEmail: 'not-an-email',
  invalidPassword: '123',
  nonExistentUser: {
    email: 'nonexistent@example.com',
    password: 'WrongPassword123!',
  },
};

/**
 * Generate random test data
 */
export const generateTestData = {
  email: () => `test.${Date.now()}@example.com`,
  password: () => `TestPass${Date.now()}!`,
  username: () => `user_${Date.now()}`,
  ticketTitle: () => `Test Ticket ${Date.now()}`,
  ticketDescription: () => `Test description created at ${new Date().toISOString()}`,
};
