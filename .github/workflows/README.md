# GitHub Actions Workflows

This directory contains all CI/CD workflows for the HR Certificate Portal Backend.

## Workflow Files

### 1. `ci.yml` - Continuous Integration
**Purpose:** Automated build and test on code changes

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches
- Manual dispatch

**Steps:**
1. Checkout code
2. Setup .NET 8
3. Restore dependencies
4. Build project
5. Run tests
6. Publish test results
7. Upload build artifacts

**Outputs:**
- Build artifacts (retained for 7 days)
- Test results
- Build summary

---

### 2. `code-quality.yml` - Code Quality Checks
**Purpose:** Ensure code quality standards

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches
- Manual dispatch

**Steps:**
1. Checkout code
2. Setup .NET 8
3. Restore dependencies
4. Check code formatting (`dotnet format`)
5. Run static code analysis

---

### 3. `codeql.yml` - Security Scanning
**Purpose:** Identify security vulnerabilities

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches
- Weekly schedule (Mondays at 10:00 UTC)
- Manual dispatch

**Steps:**
1. Checkout code
2. Setup .NET 8
3. Initialize CodeQL
4. Build project
5. Perform security analysis

**Outputs:**
- Security alerts in GitHub Security tab
- Code scanning results

---

### 4. `dependency-review.yml` - Dependency Security
**Purpose:** Review dependencies in pull requests

**Triggers:**
- Pull requests to `main` or `develop` branches

**Steps:**
1. Checkout code
2. Run dependency review
3. Check for vulnerable dependencies
4. Comment on PR with results

**Configuration:**
- Fails on moderate or higher severity vulnerabilities
- Posts summary in PR comments

---

### 5. `cd.yml` - Continuous Deployment
**Purpose:** Deploy to staging or production environments

**Triggers:**
- Manual dispatch only (for controlled deployments)

**Inputs:**
- `environment`: Choose between `staging` or `production`

**Steps:**
1. Checkout code
2. Setup .NET 8
3. Restore dependencies
4. Build project
5. Publish application
6. Upload deployment package
7. Deploy to Azure (optional, needs configuration)

**Configuration Required for Azure:**
Add these secrets in GitHub repository settings:
- `AZURE_WEBAPP_NAME`: Your Azure Web App name
- `AZURE_WEBAPP_PUBLISH_PROFILE`: Download from Azure Portal

---

### 6. `release.yml` - Release Management
**Purpose:** Create versioned releases with artifacts

**Triggers:**
- Push tags matching `v*.*.*` (e.g., v1.0.0)
- Manual dispatch with version input

**Steps:**
1. Checkout code
2. Setup .NET 8
3. Restore dependencies
4. Build project
5. Publish for Linux x64
6. Publish for Windows x64
7. Create release archives
8. Generate changelog
9. Create GitHub Release

**Outputs:**
- Linux x64 build (.tar.gz)
- Windows x64 build (.zip)
- GitHub Release with changelog

---

## Workflow Execution Order

```
Code Push/PR
├── ci.yml (Runs automatically)
├── code-quality.yml (Runs automatically)
├── codeql.yml (Runs automatically)
└── dependency-review.yml (PRs only)

Manual Actions
├── cd.yml (Manual deploy)
└── release.yml (Tag push or manual)
```

## Environment Variables

Common environment variables used across workflows:

```yaml
DOTNET_VERSION: '8.0.x'
CONFIGURATION: Release
```

## Permissions

Workflows use minimal required permissions:

- **Read:** `contents`, `actions`
- **Write:** `security-events` (CodeQL), `pull-requests` (Dependency Review), `contents` (Release)

## Best Practices

1. **Always run CI checks** before merging PRs
2. **Review security alerts** from CodeQL and dependency scans
3. **Test in staging** before deploying to production
4. **Use semantic versioning** for releases (MAJOR.MINOR.PATCH)
5. **Keep workflows updated** with latest action versions

## Troubleshooting

### CI Build Fails
- Check .NET version compatibility
- Verify all NuGet packages can be restored
- Review build logs in Actions tab

### CodeQL Scan Fails
- Ensure project builds successfully
- Check for syntax errors in code
- Review CodeQL documentation for C# specific issues

### Deployment Fails
- Verify Azure credentials are correct
- Check Azure Web App is running
- Ensure publish profile is up to date

## Monitoring

Monitor workflow runs in:
- **GitHub Actions tab** of repository
- **Security tab** for CodeQL and dependency alerts
- **Pull request checks** for automated feedback

## Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [CodeQL Documentation](https://codeql.github.com/docs/)
- [Azure Web Apps Deployment](https://docs.microsoft.com/en-us/azure/app-service/)
