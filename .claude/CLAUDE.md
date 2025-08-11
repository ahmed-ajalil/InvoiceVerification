# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Build Commands
- `dotnet build` - Build the solution
- `dotnet build --configuration Release` - Build in Release configuration
- `dotnet restore` - Restore NuGet packages for all projects

### Run Commands
- `dotnet run --project CR_CoreBot` - Run the main web application
- Application runs on https://localhost:3978/ by default

### Publish Commands
- `dotnet publish --configuration Release` - Publish the application for deployment

## Architecture Overview

This is a Microsoft Bot Framework application built on .NET 8.0 with a multi-project solution structure:

### Project Structure
- **CR_CoreBot** - Main ASP.NET Core web application containing bot controllers, dialogs, and views
- **CR_CoreBot_DataAccess** - Entity Framework data access layer with database contexts and models
- **CR_CoreBot_DTO** - Data Transfer Objects and configuration models
- **CR_CoreBot_Service** - Business logic and service layer with adapters
- **CR_HRPortalAI_DataAcess** - Separate data access layer for HR portal functionality

### Key Technologies
- Microsoft Bot Framework v4.18.1
- ASP.NET Core MVC with Razor views
- Entity Framework Core 7.0.5 with SQL Server
- Azure OpenAI integration (GPT-4/GPT-4o models)
- Azure Cognitive Services for text analytics and translation
- Azure Blob Storage for file management
- Azure Content Safety for responsible AI features

### Bot Functionality
The application provides multiple bot interfaces for different domains:
- **AI Assistant Bot** (`/AiAssistant/AiAssistantBot`) - General AI assistance
- **Banking Bot** (`/Banking/BankBot`) - Banking-specific interactions  
- **PBI Chatbot** (`/PBIChatbot/PBIBot`) - Power BI related queries

### Configuration Requirements
The application requires extensive configuration in `appsettings.json`:
- Azure AD authentication settings
- Azure OpenAI credentials and endpoints
- Content Safety API configuration
- Database connection strings
- Azure Blob Storage settings
- Azure Search configuration

### Content Safety & Responsible AI
The application implements comprehensive content safety features:
- Content filtering for hate, self-harm, sexual content, and violence
- Regional availability configuration for different AI safety features
- Prompt injection and toxicity detection
- Configurable severity response messages

### Data Architecture
- Uses Entity Framework with multiple database contexts
- Supports customer configuration and model fine-tuning data
- Session tracking and performance monitoring capabilities
- File upload and blob storage integration

### Deployment
- Configured for Azure Web App deployment via Azure DevOps pipelines
- Uses Azure Resource Manager for infrastructure deployment
- Supports slot-based deployments (Dev/Staging/Production)

## Important Notes

- The application targets .NET 8.0 framework
- No automated testing framework is currently configured
- Uses NLog for logging with Azure Blob Storage integration
- Extensive use of Azure services requires proper authentication and configuration
- Multiple Areas (AiAssistant, Banking, PBIChatbot) provide domain-specific functionality