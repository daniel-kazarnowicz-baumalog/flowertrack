---
applyTo: "**/*.test.{ts,tsx}"
---

# Testing Guidelines

## Frontend Testing (React Testing Library)

### Test Structure
```tsx
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { ComponentName } from './ComponentName';

describe('ComponentName', () => {
  it('should render correctly', () => {
    render(<ComponentName title="Test" />);
    
    expect(screen.getByText('Test')).toBeInTheDocument();
  });
  
  it('should handle user interaction', async () => {
    const handleClick = vi.fn();
    render(<ComponentName onClick={handleClick} />);
    
    const button = screen.getByRole('button');
    fireEvent.click(button);
    
    expect(handleClick).toHaveBeenCalledTimes(1);
  });
  
  it('should fetch and display data', async () => {
    render(<ComponentName />);
    
    await waitFor(() => {
      expect(screen.getByText('Loaded Data')).toBeInTheDocument();
    });
  });
});
```

### Testing Best Practices
1. **Test user behavior, not implementation**
   - Query by role, label, or text
   - Avoid testing internal state
   
2. **Use appropriate queries**
   - `getByRole` - preferred for accessibility
   - `getByLabelText` - for form inputs
   - `getByText` - for text content
   - `getByTestId` - last resort

3. **Mock external dependencies**
   ```tsx
   vi.mock('./api/ticketService', () => ({
     fetchTickets: vi.fn(() => Promise.resolve([]))
   }));
   ```

4. **Test accessibility**
   ```tsx
   expect(screen.getByRole('button', { name: /submit/i })).toBeInTheDocument();
   ```

## Backend Testing (xUnit)

### Test Class Structure
```csharp
public class TicketServiceTests : IDisposable
{
    private readonly Mock<ITicketRepository> _mockRepository;
    private readonly TicketService _sut;
    
    public TicketServiceTests()
    {
        _mockRepository = new Mock<ITicketRepository>();
        _sut = new TicketService(_mockRepository.Object);
    }
    
    [Fact]
    public async Task MethodName_Scenario_ExpectedResult()
    {
        // Arrange
        var input = new TestData();
        _mockRepository.Setup(r => r.GetAsync(1))
            .ReturnsAsync(expectedValue);
        
        // Act
        var result = await _sut.MethodName(input);
        
        // Assert
        Assert.Equal(expectedValue, result);
    }
    
    [Theory]
    [InlineData(1, "Result1")]
    [InlineData(2, "Result2")]
    public async Task TestWithMultipleInputs(int input, string expected)
    {
        // Test implementation
    }
    
    public void Dispose()
    {
        // Cleanup if needed
    }
}
```

### Testing Principles

1. **Follow AAA Pattern**
   - Arrange: Setup test data and mocks
   - Act: Execute the method under test
   - Assert: Verify the outcome

2. **Test one thing per test**
   - Each test should verify a single behavior
   - Use descriptive test names

3. **Use appropriate assertions**
   - `Assert.Equal` for value comparison
   - `Assert.NotNull` for null checks
   - `Assert.Throws` for exceptions
   - `Assert.True/False` for boolean conditions

4. **Mock external dependencies**
   ```csharp
   _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
       .ReturnsAsync((Ticket?)null);
   ```

## Test Coverage Goals

- Aim for >70% code coverage on critical paths
- 100% coverage on business logic
- Test edge cases and error conditions
- Don't test framework code or trivial getters/setters

## What to Test

✅ **Do test:**
- Business logic
- Data transformations
- Error handling
- User interactions
- API integrations
- Edge cases

❌ **Don't test:**
- Framework features
- Third-party libraries
- Simple property getters/setters
- Private methods (test through public interface)
