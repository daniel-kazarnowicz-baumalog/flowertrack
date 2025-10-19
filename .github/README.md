# GitHub Copilot Configuration - Quick Reference

## Files in `.github/` Directory

### Core Configuration
- **`copilot-instructions.md`** - Main instructions for Copilot agent, chat, and code review
- **`copilot-setup-steps.yml`** - Environment setup steps for coding agent

### Path-Specific Instructions (`instructions/`)
- **`react.instructions.md`** - React & TypeScript guidelines (applies to `**/*.{ts,tsx}`)
- **`csharp.instructions.md`** - C# & .NET guidelines (applies to `**/*.cs`)
- **`api.instructions.md`** - API Controller specific guidelines (applies to `**/Controllers/**/*.cs`)
- **`testing.instructions.md`** - Testing guidelines (applies to `**/*.test.{ts,tsx}`)

## How Copilot Uses These Files

### Coding Agent
When you assign a GitHub issue to Copilot, it will:
1. Read `copilot-instructions.md` for project context
2. Execute `copilot-setup-steps.yml` to prepare the environment
3. Apply path-specific instructions based on files being edited
4. Create a branch, make changes, and open a PR

### Copilot Chat
- Uses `copilot-instructions.md` and path-specific instructions to provide context-aware suggestions
- Follows coding standards and conventions defined in these files

### Copilot Code Review
- Reviews PRs against guidelines in these instruction files
- Checks for adherence to project standards and best practices

## Best Practices for Maintaining These Files

1. **Keep instructions up-to-date** as project evolves
2. **Be specific** - clear instructions yield better results
3. **Include examples** - show, don't just tell
4. **Document commands** that work (and those that don't)
5. **Update when adding new tools/frameworks** to the project

## Adding New Instructions

To add instructions for new file types, create a file in `instructions/` with:
- Front matter specifying `applyTo` glob pattern
- Clear, actionable guidelines
- Code examples demonstrating best practices

Example:
```markdown
---
applyTo: "**/*.sql"
---

# SQL Guidelines

## Formatting
- Use uppercase for SQL keywords
- Indent nested queries
...
```

## Getting the Most from Copilot

1. **Write clear issue descriptions** - treat issues as prompts
2. **Reference specific files** when asking for help
3. **Use inline comments** to guide Copilot during coding
4. **Review suggestions carefully** before accepting
5. **Iterate on PRs** using comments to refine changes

## Resources

- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [Best Practices for Copilot Coding Agent](https://docs.github.com/en/copilot/using-github-copilot/using-github-copilot-coding-agent/best-practices-for-using-github-copilot-to-work-on-tasks)
- Project README: `/README.md`
