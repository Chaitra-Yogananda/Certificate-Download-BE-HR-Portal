# CI/CD Pipeline Quick Start Guide

This guide will help you understand and use the GitHub Actions CI/CD pipeline for the HR Certificate Portal Backend.

## Table of Contents

1. [Overview](#overview)
2. [Quick Start](#quick-start)
3. [Workflows Explained](#workflows-explained)
4. [Common Scenarios](#common-scenarios)
5. [Configuration](#configuration)
6. [Troubleshooting](#troubleshooting)

## Overview

The CI/CD pipeline automates:
- ✅ Building and testing code
- ✅ Code quality checks
- ✅ Security scanning
- ✅ Dependency vulnerability detection
- ✅ Deployment to staging/production
- ✅ Release management

## Quick Start

### For Developers

1. **Create a feature branch:**
   ```bash
   git checkout -b feature/my-awesome-feature
   ```

2. **Make your changes and commit:**
   ```bash
   git add .
   git commit -m "Add awesome feature"
   ```

3. **Push your branch:**
   ```bash
   git push origin feature/my-awesome-feature
   ```

4. **Create a Pull Request:**
   - Go to GitHub repository
   - Click "New Pull Request"
   - Select your branch
   - The following checks will run automatically:
     - ✓ CI Build
     - ✓ Code Quality
     - ✓ CodeQL Security Scan
     - ✓ Dependency Review

5. **Review and merge:**
   - Wait for all checks to pass (green ✓)
   - Request code review
   - Merge when approved

### For Maintainers

#### Deploying to Staging/Production

1. Navigate to **Actions** tab in GitHub
2. Select **CD - Deploy** workflow
3. Click **Run workflow**
4. Choose environment:
   - `staging` - for testing
   - `production` - for live deployment
5. Click **Run workflow** button

#### Creating a Release

**Option 1: Using Git Tags**
```bash
git tag v1.0.0
git push origin v1.0.0
```

**Option 2: Manual Trigger**
1. Go to **Actions** → **Release**
2. Click **Run workflow**
3. Enter version (e.g., v1.0.0)
4. Click **Run workflow**

## Workflows Explained

### 🔨 CI - Build and Test

**When it runs:** Every push and PR to `main`/`develop`

**What it does:**
- Builds your code in Release mode
- Runs all tests
- Creates artifacts you can download

**How to view:**
- Go to Actions → CI - Build and Test
- Click on a run to see details
- Download artifacts from the run page

### 📊 Code Quality

**When it runs:** Every push and PR to `main`/`develop`

**What it does:**
- Checks code formatting
- Runs static analysis
- Catches common mistakes

**How to fix issues:**
```bash
# Format your code
dotnet format

# Check locally
dotnet build /p:TreatWarningsAsErrors=true
```

### 🔒 CodeQL Security Scan

**When it runs:** 
- Every push and PR to `main`/`develop`
- Weekly on Mondays
- Manual trigger

**What it does:**
- Scans for security vulnerabilities
- Detects potential bugs
- Checks for code quality issues

**How to view results:**
- Go to **Security** tab → **Code scanning**
- Review alerts and fix critical/high issues

### 📦 Dependency Review

**When it runs:** On every Pull Request

**What it does:**
- Checks for vulnerable dependencies
- Posts results in PR comments
- Fails PR if critical vulnerabilities found

**How to fix:**
- Update vulnerable packages:
  ```bash
  dotnet add package <PackageName> --version <SafeVersion>
  ```

### 🚀 CD - Deploy

**When it runs:** Manual trigger only

**What it does:**
- Builds production-ready application
- Creates deployment package
- Deploys to Azure Web App (if configured)

**Prerequisites:**
- Azure Web App created
- Secrets configured (see Configuration section)

### 📋 Release

**When it runs:** When you push a version tag (v*.*.*)

**What it does:**
- Builds for Linux and Windows
- Creates release archives
- Generates changelog
- Creates GitHub Release

## Common Scenarios

### Scenario 1: I want to add a new feature

```bash
# 1. Create branch
git checkout -b feature/new-login-method

# 2. Make changes
# ... edit files ...

# 3. Test locally
dotnet build
dotnet test

# 4. Commit and push
git add .
git commit -m "Add new login method"
git push origin feature/new-login-method

# 5. Create PR on GitHub
# 6. Wait for CI checks
# 7. Merge when approved
```

### Scenario 2: CI build is failing

1. **Check the error:**
   - Go to Actions tab
   - Click on the failed run
   - Look at the error message

2. **Common fixes:**
   - Build error: Fix compilation issues locally
   - Test failure: Fix failing tests
   - Format error: Run `dotnet format`

3. **Test locally:**
   ```bash
   dotnet restore
   dotnet build --configuration Release
   dotnet test
   ```

4. **Push fix:**
   ```bash
   git add .
   git commit -m "Fix CI build"
   git push
   ```

### Scenario 3: Security vulnerability detected

1. **Check the alert:**
   - Go to Security → Code scanning
   - Or check PR comments for dependency alerts

2. **Fix the vulnerability:**
   - For code issues: Follow CodeQL suggestions
   - For dependencies: Update packages
     ```bash
     dotnet list package --vulnerable
     dotnet add package <PackageName> --version <SafeVersion>
     ```

3. **Verify fix:**
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Commit and push:**
   ```bash
   git add .
   git commit -m "Fix security vulnerability"
   git push
   ```

### Scenario 4: Deploying to production

1. **Verify staging works:**
   - Deploy to staging first
   - Test thoroughly

2. **Deploy to production:**
   - Go to Actions → CD - Deploy
   - Run workflow
   - Select "production"
   - Monitor deployment

3. **Verify deployment:**
   - Check application logs
   - Test critical endpoints
   - Monitor for errors

### Scenario 5: Creating a new release

1. **Prepare for release:**
   - Ensure all tests pass
   - Update version numbers if needed
   - Write changelog notes

2. **Create release:**
   ```bash
   git checkout main
   git pull
   git tag -a v1.2.0 -m "Release version 1.2.0"
   git push origin v1.2.0
   ```

3. **Workflow automatically:**
   - Builds release artifacts
   - Creates GitHub Release
   - Attaches binaries

4. **Announce release:**
   - Share release notes
   - Update documentation

## Configuration

### Setting up Azure Deployment

1. **Create Azure Web App:**
   - Go to Azure Portal
   - Create new Web App
   - Choose .NET 8 runtime

2. **Get publish profile:**
   - Open your Web App
   - Click "Get publish profile"
   - Download the file

3. **Add GitHub Secrets:**
   - Go to GitHub repo → Settings → Secrets
   - Add `AZURE_WEBAPP_NAME` = your app name
   - Add `AZURE_WEBAPP_PUBLISH_PROFILE` = contents of publish profile

4. **Enable deployment:**
   - Edit `.github/workflows/cd.yml`
   - Uncomment Azure deployment step
   - Commit and push

### Configuring Environments

1. **Create environments:**
   - Go to Settings → Environments
   - Create `staging` and `production`

2. **Add protection rules:**
   - Require reviewers for production
   - Set deployment branch rules

3. **Add environment secrets:**
   - Add environment-specific configs
   - Connection strings
   - API keys

## Troubleshooting

### Build Fails with "Package not found"

**Solution:**
```bash
dotnet restore
dotnet clean
dotnet build
```

### CodeQL Timeout

**Solution:**
- CodeQL might timeout on large projects
- Increase timeout in workflow file
- Or exclude test projects from scanning

### Deployment Fails

**Checklist:**
- ✓ Secrets are configured correctly
- ✓ Azure Web App is running
- ✓ Publish profile is up to date
- ✓ Application builds successfully

### Release Workflow Not Triggering

**Checklist:**
- ✓ Tag format is correct (v1.0.0)
- ✓ Tag is pushed to GitHub
- ✓ Workflow file is in main/default branch

## Getting Help

1. **Check workflow logs:**
   - Actions tab → Select workflow → View logs

2. **Review documentation:**
   - `.github/workflows/README.md`
   - GitHub Actions docs

3. **Common resources:**
   - [GitHub Actions Documentation](https://docs.github.com/en/actions)
   - [.NET CLI Reference](https://docs.microsoft.com/en-us/dotnet/core/tools/)
   - [Azure Deployment Guide](https://docs.microsoft.com/en-us/azure/app-service/)

## Best Practices

1. ✅ **Always create feature branches** - Never commit directly to main
2. ✅ **Write meaningful commit messages** - Help others understand changes
3. ✅ **Keep PRs small** - Easier to review and less likely to break
4. ✅ **Test locally first** - Catch issues before pushing
5. ✅ **Monitor CI results** - Don't ignore failing checks
6. ✅ **Update dependencies regularly** - Keep security patches current
7. ✅ **Use staging environment** - Test before production
8. ✅ **Follow semantic versioning** - MAJOR.MINOR.PATCH

## Workflow Status Badges

Add these to your README to show workflow status:

```markdown
![CI](https://github.com/Chaitra-Yogananda/Certificate-Download-BE-HR-Portal/actions/workflows/ci.yml/badge.svg)
![CodeQL](https://github.com/Chaitra-Yogananda/Certificate-Download-BE-HR-Portal/actions/workflows/codeql.yml/badge.svg)
![Code Quality](https://github.com/Chaitra-Yogananda/Certificate-Download-BE-HR-Portal/actions/workflows/code-quality.yml/badge.svg)
```

---

**Questions?** Open an issue or check the workflow logs in the Actions tab!
