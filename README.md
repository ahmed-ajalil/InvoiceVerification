# Invoice Verification System

A comprehensive solution for invoice verification, document processing, and management using AI and cloud integration.

## Overview

This system enables efficient invoice processing and verification with smart document management capabilities, AI-powered analysis, and multi-cloud storage options.

## Projects in the Solution

- **CR_CoreBot**: Main web application built with Razor Pages
- **CR_CoreBot_DTO**: Data Transfer Objects project
- **CR_CoreBot_DataAccess**: Data access layer for database operations
- **CR_CoreBot_Service**: Business logic and services
- **CR_HRPortalAI_DataAcess**: Data access for HR Portal AI functionality

## Technology Stack

- ASP.NET Core 8.0
- Razor Pages
- Entity Framework Core
- Azure Services (Blob Storage)
- AWS S3 Integration
- GitHub/LibGit2Sharp Integration
- Azure AD Authentication

## Features

- Invoice verification and processing
- Document management with Azure Blob Storage
- GitHub repository integration
- Fine-tuning models
- User authentication and authorization

## Getting Started

1. Clone this repository
2. Copy `appsettings.example.json` to `appsettings.json` and update with your settings
3. Open the solution in Visual Studio
4. Build and run the application

## Configuration

For security reasons, sensitive configuration is not included in the repository. You must:

1. Create an `appsettings.json` file based on the `appsettings.example.json` template
2. Fill in your Azure Storage Account credentials and other connection strings

**IMPORTANT: Never commit sensitive data like connection strings or account keys to the repository!**

## Requirements

- .NET 8.0 SDK
- SQL Server
- Azure account (for Blob Storage)
- AWS account (for S3 functionality)

## Security Note

This repository uses GitHub's secret scanning to prevent accidental exposure of credentials. If you're a contributor and encounter push protection issues, check for any hardcoded credentials in your code.