---
applyTo: "**/*.{ts,tsx}"
---

# React & TypeScript Guidelines

## Component Structure

### Functional Components Only
- Use functional components exclusively - **NO class components**
- Keep components focused and under 200 lines
- Extract reusable logic into custom hooks

### Example Structure
```tsx
import React, { useState, useEffect } from 'react';
import './ComponentName.css';

interface ComponentNameProps {
  title: string;
  onAction?: () => void;
  isActive?: boolean;
}

export const ComponentName: React.FC<ComponentNameProps> = ({ 
  title, 
  onAction, 
  isActive = false 
}) => {
  const [state, setState] = useState<StateType>(initialValue);

  useEffect(() => {
    // Side effects here
    return () => {
      // Cleanup
    };
  }, [dependencies]);

  return (
    <div className="component-name">
      <h2>{title}</h2>
      {/* Component content */}
    </div>
  );
};
```

## React Hooks Usage

### State Management
- **`useState`**: For local component state
  ```tsx
  const [count, setCount] = useState<number>(0);
  const [user, setUser] = useState<User | null>(null);
  ```

### Side Effects
- **`useEffect`**: For side effects, always include cleanup when needed
  ```tsx
  useEffect(() => {
    const subscription = subscribeToData();
    return () => subscription.unsubscribe();
  }, [dependency]);
  ```

### Performance Optimization
- **`useCallback`**: Memoize callbacks to prevent unnecessary re-renders
  ```tsx
  const handleClick = useCallback(() => {
    doSomething(value);
  }, [value]);
  ```

- **`useMemo`**: Memoize expensive computations
  ```tsx
  const expensiveValue = useMemo(() => 
    computeExpensiveValue(data), 
    [data]
  );
  ```

- **`React.memo`**: Wrap components that render often with same props
  ```tsx
  export const ExpensiveComponent = React.memo(({ data }) => {
    // Component logic
  });
  ```

### Custom Hooks
Extract reusable logic into custom hooks:
```tsx
function useLocalStorage<T>(key: string, initialValue: T) {
  const [value, setValue] = useState<T>(() => {
    const stored = localStorage.getItem(key);
    return stored ? JSON.parse(stored) : initialValue;
  });

  useEffect(() => {
    localStorage.setItem(key, JSON.stringify(value));
  }, [key, value]);

  return [value, setValue] as const;
}
```

## TypeScript Best Practices

### Type Everything
- Always define prop types using interfaces or types
- Avoid `any` - use `unknown` if type is truly unknown
- Use strict null checks

### Props Interface
```tsx
interface ButtonProps {
  label: string;
  onClick: () => void;
  variant?: 'primary' | 'secondary';
  isDisabled?: boolean;
  children?: React.ReactNode;
}
```

### Event Handlers
```tsx
const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
  setValue(e.target.value);
};

const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
  e.preventDefault();
  // Handle form submission
};
```

### Generic Components
```tsx
interface ListProps<T> {
  items: T[];
  renderItem: (item: T) => React.ReactNode;
}

function List<T>({ items, renderItem }: ListProps<T>) {
  return <ul>{items.map(renderItem)}</ul>;
}
```

## Naming Conventions

### Props
- Event handlers: `onEventName` (e.g., `onClick`, `onSubmit`)
- Boolean props: `isCondition` or `hasProperty` (e.g., `isActive`, `hasError`)
- Callback props: `onAction` (e.g., `onSave`, `onCancel`)

### Components
- PascalCase for component names
- Match filename to component name
- Use descriptive names: `UserProfileCard`, not `Card`

### Files
- Component files: `ComponentName.tsx`
- Hook files: `useHookName.ts`
- Type files: `types.ts` or `ComponentName.types.ts`
- Style files: `ComponentName.css` or `ComponentName.module.css`

## Styling

### CSS Organization
- Co-locate styles with components
- Use CSS Modules for scoped styles
- Follow BEM naming convention for classes

```css
/* ComponentName.css */
.component-name {
  /* Container styles */
}

.component-name__element {
  /* Element styles */
}

.component-name__element--modifier {
  /* Modified element styles */
}
```

### Conditional Classes
```tsx
const className = `button ${isActive ? 'button--active' : ''} ${size}`;
// Or use classnames library
```

## Accessibility (A11Y)

### Semantic HTML
- Use appropriate HTML elements: `<button>`, `<nav>`, `<main>`, etc.
- Don't use `<div>` for clickable elements

### ARIA Attributes
```tsx
<button
  aria-label="Close dialog"
  aria-pressed={isActive}
  aria-disabled={isDisabled}
  onClick={handleClick}
>
  Close
</button>
```

### Keyboard Navigation
- Ensure all interactive elements are keyboard accessible
- Use `tabIndex` appropriately
- Handle `onKeyDown` for custom interactions

## Performance

### Avoid Unnecessary Re-renders
1. Use `React.memo` for pure components
2. Memoize callbacks with `useCallback`
3. Memoize expensive computations with `useMemo`
4. Keep state as local as possible

### Code Splitting
```tsx
const LazyComponent = React.lazy(() => import('./LazyComponent'));

function App() {
  return (
    <Suspense fallback={<Loading />}>
      <LazyComponent />
    </Suspense>
  );
}
```

### Lists and Keys
- Always provide stable `key` props for list items
- Don't use array index as key for dynamic lists

```tsx
{items.map(item => (
  <ListItem key={item.id} data={item} />
))}
```

## Form Handling

### Controlled Components
```tsx
const [formData, setFormData] = useState({ name: '', email: '' });

const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
  const { name, value } = e.target;
  setFormData(prev => ({ ...prev, [name]: value }));
};

<input
  type="text"
  name="name"
  value={formData.name}
  onChange={handleChange}
/>
```

### Form Validation
- Validate on submit, optionally on blur
- Show clear error messages
- Disable submit button during submission

## Error Handling

### Error Boundaries
Create error boundary components for graceful error handling:
```tsx
class ErrorBoundary extends React.Component<Props, State> {
  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    // Log error
  }
  render() {
    if (this.state.hasError) {
      return <ErrorFallback />;
    }
    return this.props.children;
  }
}
```

### Async Error Handling
```tsx
const [error, setError] = useState<Error | null>(null);

const fetchData = async () => {
  try {
    setError(null);
    const data = await api.getData();
    setData(data);
  } catch (err) {
    setError(err as Error);
  }
};
```

## Testing Considerations

- Write component tests using React Testing Library
- Test user interactions, not implementation details
- Mock external dependencies (API calls, etc.)
- Ensure components are accessible

## Common Pitfalls to Avoid

❌ **Don't:**
- Use class components
- Mutate state directly
- Put hooks inside conditions/loops
- Forget cleanup in `useEffect`
- Use `any` type
- Ignore accessibility
- Create deeply nested components

✅ **Do:**
- Use functional components with hooks
- Update state immutably
- Follow Rules of Hooks
- Clean up subscriptions/timers
- Type everything properly
- Include ARIA labels
- Keep components flat and composable
