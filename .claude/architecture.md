# System Architecture

## Overview
This is an enterprise-grade Microsoft Bot Framework application designed for AI-powered conversational interfaces across multiple business domains. The system implements a modular, multi-layered architecture with strong separation of concerns and extensive Azure cloud integration.

## Architecture Layers

### 1. Presentation Layer
**Location**: `CR_CoreBot/`
- **Controllers**: Handle HTTP requests and coordinate responses
  - `AdminController` - Administrative functions and user management
  - `HomeController` - Main application entry and navigation
  - `LoginController` - Authentication and session management  
  - `SecurityController` - AI safety and content filtering management
  - `ModelDetailsController` - AI model configuration and training
  - `HRadminController` - HR-specific administrative functions

- **Area Controllers**: Domain-specific bot interfaces
  - `AiAssistant/AiAssistantBotController` - General AI assistance
  - `Banking/BankBotController` - Banking domain chatbot
  - `PBIChatbot/PBIBotController` - Power BI analytics chatbot

- **Views**: Razor MVC views for web interface
- **Bot Framework Integration**: Bot controllers and adapters

### 2. Business Logic Layer  
**Location**: `CR_CoreBot_Service/`
- **Service Adapters**:
  - `AdminService` - Business logic for administrative operations
  - `HrAdminService` - HR-specific business operations
  - `HolHealthCareService` - Healthcare domain services
  - `ServiceMethods` - Common service utilities
  - `AzureOpenAI` - AI service integration wrapper

### 3. Data Transfer Layer
**Location**: `CR_CoreBot_DTO/`
- **Configuration DTOs**: `AppSettingsDTO`, `ChatBotKeyConfigurationDTO`
- **Business DTOs**: Various domain-specific data transfer objects
- **Helper Classes**: `AppSettingHelper` for configuration management

### 4. Data Access Layer
**Location**: `CR_CoreBot_DataAccess/` and `CR_HRPortalAI_DataAcess/`

#### Primary Data Context (`CR_OpenAI/`)
- `CropenAiContext` - Main Entity Framework context
- `CrautomationContext` - Automation-specific context  
- `CropenAiProcContext` - Stored procedure context

#### Core Entities
- `CustomerInformation` - Customer data management
- `CustomerConfiguration` - Customer-specific settings
- `ChatBotKeyConfiguration` - API key management
- `BotConfiguration` - Bot behavior configuration
- `UserSessionTracker` - Session management
- `Feedback` - User feedback collection

#### HR Portal Data Context
- `HrportalAiContext` - HR-specific database context
- ROI calculation entities (`TotalCost`, `TotalBenefit`)
- Model training entities (`ModelFineTuneDatum`, `TrainModelDTO`)

## Key Architectural Patterns

### 1. Bot Framework Architecture
- **Adapter Pattern**: `AdapterWithErrorHandler` provides centralized error handling
- **Dialog System**: Conversational flow management with `DialogBot` base class
- **State Management**: User and conversation state persistence
- **Channel Abstraction**: Multi-channel bot deployment support

### 2. Dependency Injection
- All services registered in `Startup.ConfigureServices()`
- Singleton pattern for bot framework components
- Transient registration for business services
- Configuration binding for external service settings

### 3. Repository Pattern (Implicit)
- Entity Framework contexts act as repositories
- Multiple contexts for different business domains
- Database-first approach with generated entities

### 4. Authentication & Authorization
- Azure AD integration for enterprise authentication
- Session-based authentication with configurable timeout
- Role-based access control through custom filters
- JWT Bearer token support for API access

## External Integrations

### Azure Services
- **Azure OpenAI**: GPT-4/GPT-4o model integration
- **Azure Cognitive Services**: Text analytics and language processing
- **Azure Content Safety**: Responsible AI content filtering
- **Azure Blob Storage**: File and document management  
- **Azure Search**: Document indexing and retrieval
- **Azure Translator**: Multi-language support

### Security & Compliance
- **Content Safety Filters**: Hate, self-harm, sexual, violence detection
- **Prompt Injection Protection**: Security against adversarial inputs  
- **Regional Compliance**: Region-specific feature availability
- **Toxicity Detection**: Automated content moderation
- **Grounding Validation**: Fact-checking and accuracy verification

## Data Flow

### 1. User Request Flow
```
User Request → Controller → Service Layer → Data Access → Database
                     ↓
           Bot Framework → AI Services → Response Processing → User
```

### 2. AI Processing Flow
```
User Input → Content Safety → OpenAI Processing → Response Validation → Output
```

### 3. Configuration Flow
```
appsettings.json → AppSettingsDTO → Service Registration → Runtime Usage
```

## Scalability Considerations

### 1. Multi-Tenant Architecture
- Customer-specific configurations
- Isolated data contexts per customer
- Configurable AI model deployments

### 2. State Management
- Memory-based state for development
- Scalable to Azure Storage for production
- Session persistence across requests

### 3. Caching Strategy
- Distributed memory cache for session data
- Configuration caching through dependency injection
- Static file caching for assets

## Security Architecture

### 1. Authentication Layers
- Azure AD for enterprise users  
- Bot Framework authentication for channels
- Session-based web authentication
- API key management for external services

### 2. Data Protection
- Encrypted database connections
- Secure configuration management
- Content filtering before processing
- Audit logging through NLog

### 3. AI Safety Implementation
- Multi-layered content validation
- Regional compliance enforcement  
- Configurable safety thresholds
- Comprehensive logging and monitoring

## Deployment Architecture

### 1. Azure App Service
- Web application hosting on Azure
- Slot-based deployment (Dev/Staging/Production)
- Auto-scaling capabilities
- Integrated monitoring and logging

### 2. Database Layer
- Azure SQL Database
- Multiple connection strings for different environments
- Entity Framework migrations for schema management

### 3. CI/CD Pipeline
- Azure DevOps integration
- Automated build and deployment
- Environment-specific configurations
- Infrastructure as Code support