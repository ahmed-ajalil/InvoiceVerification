# Data Model Documentation

## Overview
This document provides comprehensive documentation of the database models and entities used in the Invoice Verification Bot Framework application. The system uses a multi-context Entity Framework approach with two primary database contexts serving different functional domains.

## Database Contexts

### 1. Core AI Context (`CropenAiContext`)
**Location**: `CR_CoreBot_DataAccess/CR_OpenAI/CropenAiContext.cs`
**Connection**: `RestoreDefaultConnection`
**Purpose**: Main application data including bot configurations, customer management, user sessions, and content safety.

### 2. HR Portal AI Context (`HrportalAiContext`) 
**Location**: `CR_HRPortalAI_DataAcess/CR_HRPortal/HrportalAiContext.cs`
**Connection**: Dynamic connection string via `ConnectionModel`
**Purpose**: HR-specific functionality including ROI calculations, model training, and business analytics.

---

## Core AI Entities (`CropenAiContext`)

### Customer Management

#### `CustomerInformation`
Core entity for managing customer/organization data.
- **Primary Key**: `CustomerId` (int)
- **Key Fields**:
  - `UserName` (string) - Login username
  - `Password` (string) - User password
  - `Name` (string) - User display name
  - `OrganizationName` (string) - Company/org name
  - `OrganizationLogo` (string) - Logo file path
  - `Category` (string) - Customer classification
  - `RoleId` (int) - Foreign key to role management
  - `LoginWith` (string, max 100) - Authentication method
  - `OpenAiindexName` (string) - Azure Search index reference
  - `Streaming` (int) - Streaming configuration flag

#### `CustomerConfiguration`
Customer-specific application settings and preferences.
- **Primary Key**: `ConfigurationId` (int)
- **Key Fields**:
  - `Username` (string, max 200) - Associated username
  - `LoginType` (string, max 200) - Authentication type
  - `FileFormat` (string, max 500) - Supported file formats
  - `CreateDateTime` (datetime) - Configuration timestamp

#### `CustomerModel`
Maps customers to AI model configurations.
- **Primary Key**: `Id` (int)  
- **Key Fields**:
  - `ServiceName` (string, max 100) - Associated service name
  - Additional customer-model mapping fields

### Bot Configuration

#### `BotConfiguration`
Stores OpenAI and Azure configuration per customer.
- **Primary Key**: `CustomerId` (int)
- **Key Fields**:
  - `OpenaiApiVersion` (string, max 50, default: "2023-03-15-preview")
  - `Temperature` (decimal(10,2), default: 0.7) - AI creativity setting
  - `MaxTokens` (int, default: 2000) - Response length limit
  - `AzureEndpoint` (string, max 255, default: "azure.com/")
  - `OpenaiApiKey` (string, max 255) - API authentication key
  - `OpenaiApiType` (string, max 50, default: "azure")
  - `DeploymentName` (string, max 255, default: "sqlintegration")

#### `ChatBotKeyConfiguration`
API key management for chatbot services.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `DateTime` (datetime, default: getdate()) - Key creation timestamp
  - Additional key configuration fields

### User Management & Sessions

#### `TblUserInformationMapping`
User account management and authentication.
- **Primary Key**: `UserId` (int)
- **Key Fields**:
  - `UserName` (string, max 100) - Login username
  - `Password` (string, max 50) - Encrypted password
  - `FirstName` (string, max 100) - User first name
  - `LastName` (string, max 100) - User last name
  - `LoginType` (string, max 200) - Authentication method
  - `CreatedDate` (datetime) - Account creation timestamp

#### `UserSessionTracker`
Session monitoring and analytics.
- **Primary Key**: `SessionTrackerId` (int)
- **Key Fields**:
  - `LoginTime` (datetime) - Session start time
  - `LoginWith` (string, max 200) - Login method used
  - `RoleName` (string, max 200) - User role during session
  - `CreatedDateTime` (datetime) - Record creation time

#### `RoleMaster`
Role-based access control definitions.
- **Primary Key**: `RoleId` (int)
- **Key Fields**:
  - `RoleName` (string, max 50) - Role identifier/name

### Content Safety & Compliance

#### `ContentSafetySettings`
Configuration for AI safety and content filtering.
- **Table**: `ContentSafetySettings`
- **Purpose**: Stores safety thresholds and filtering configurations
- **Custom Method**: `GetContentSafetySettings()` - Retrieves active safety settings

### File Management

#### `BlobConnection`
Azure Blob Storage connection management.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `DateTime` (datetime, default: getdate()) - Connection timestamp

#### `BlobFileDatum`
File metadata and blob storage references.
- **No Primary Key** (keyless entity)
- **Key Fields**:
  - `Id` (auto-generated) - Record identifier
  - File metadata and blob references

#### `TblCustomerFileDetails`
Customer-specific file tracking.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `FileName` (string, max 250) - Original filename
  - `FilePath` (string) - Storage path reference
  - `UserId` (int) - File owner reference
  - `CreatedDate` (datetime) - Upload timestamp

### AI Model Configuration

#### `TblModelConfiguration`
AI model settings and industry-specific configurations.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `Industry` (string) - Target industry/domain
  - `ConfigurationText` (string) - Model configuration JSON/text
  - `CreateDate` (datetime) - Configuration timestamp

### Analytics & Feedback

#### `PerformanceMatrixChecker`
System performance monitoring and metrics.
- **Primary Key**: `PeformanceId` (int)
- **Key Fields**:
  - `Username` (string, max 100) - User identifier
  - `Role` (string, max 200) - User role
  - `LoginType` (string, max 200) - Authentication method
  - `ResponseTime` (decimal(18,2)) - Response latency
  - `Cost` (decimal(18,9)) - Operation cost
  - `IsValid` (string, max 50) - Validity status
  - `CurrentDateTime` (datetime) - Measurement timestamp

#### `Feedback`
User feedback and satisfaction tracking.
- **Primary Key**: `FeedbackId` (int)
- **Key Fields**:
  - `Satisfactionwithbot` (string, max 60) - Satisfaction rating
  - `EmailFeedback` (string, max 20) - Feedback delivery method
  - `FeedbackDate` (datetime) - Submission timestamp

### Data Processing

#### `EntityExtractrion`
Entity extraction results from document processing.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `FileId` (int) - Source file reference
  - `FileName` (string, max 500) - Source filename
  - `Entity` (string, max 500) - Extracted entity
  - `Icdcode` (string, max 20) - Medical/industry code
  - `ImportantNotes` (string, max 50) - Processing notes
  - `CreatedDate` (datetime) - Processing timestamp

#### `TblSummaryExtractrion`
Document summarization results.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `FileId` (int) - Source file reference  
  - `FileName` (string, max 500) - Source filename
  - `SummaryDescription` (string) - Generated summary
  - `CreatedDate` (datetime) - Processing timestamp

### Domain-Specific Data

#### `PersonalDetail` (Banking Domain)
Customer personal information for banking chatbot.
- **Keyless Entity**
- **Key Fields**:
  - `CustId` (string, max 255) - Customer identifier
  - `CustomerName` (string, max 255) - Full name
  - `Email`, `Mobile` (string, max 255) - Contact information
  - `PermanentAddress`, `CommunicationAddress` (string, max 255)
  - `BankCode`, `BranchCode` (int) - Banking identifiers
  - `BranchName` (string, max 255) - Branch information
  - `Isfccode` (string, max 255) - Banking routing code

#### `Transaction` (Banking Domain)
Banking transaction history.
- **Keyless Entity**
- **Key Fields**:
  - `CustId` (string, max 255) - Customer reference
  - `TransectionId` (int) - Transaction identifier
  - `Date` (string, max 255) - Transaction date
  - `TransectionType` (string, max 255) - Transaction category
  - `DepositAmt`, `WithdrawalAmt` (decimal) - Amount fields
  - `ClosingBalance` (decimal) - Account balance

#### `CustomerOffer` (Banking Domain)
Banking product offerings.
- **Keyless Entity**
- **Key Fields**:
  - `CustId` (string, max 255) - Customer reference
  - `OfferId` (int) - Offer identifier
  - `OffersName` (string, max 255) - Offer description
  - `RateOfIntrest` (decimal) - Interest rate

---

## HR Portal AI Entities (`HrportalAiContext`)

### ROI Calculation Framework

#### `TotalCost` & `TotalBenefit`
Financial modeling for AI implementation ROI.

**TotalCost Entity:**
- **Keyless Entity**
- **Key Fields**:
  - `Cost` (string, max 255) - Cost category description
  - `Initial`, `Year1`, `Year2`, `Year3` (decimal(18,2)) - Multi-year projections
  - `Presentvalue` (decimal(18,2)) - Net present value
  - `Total` (decimal(18,2)) - Total cost calculation

**TotalBenefit Entity:**
- **Keyless Entity**  
- **Key Fields**:
  - `Metric` (string, max 255) - Benefit metric description
  - `Year1`, `Year2`, `Year3` (decimal(18,2)) - Annual benefit projections
  - `Presentvalue` (decimal(18,2)) - Net present value
  - `Total` (decimal(18,2)) - Total benefit calculation

#### `FinalSummary`
Consolidated ROI analysis results.
- **Keyless Entity**
- **Key Fields**:
  - `Category` (string, max 255) - Analysis category
  - `Initial`, `Year1`, `Year2`, `Year3` (decimal(18,2)) - Financial projections
  - `Presentvalue`, `Total` (decimal(18,2)) - Summary calculations

### Business Configuration

#### `CustomConfiguration` & `DefaultConfiguration`
Organizational parameters for ROI modeling.

**Shared Fields:**
- `OrganizationName` (string, max 255) - Company name
- `AnnualRevenue` (decimal(18,2)) - Company annual revenue
- `AnnualSaving` (decimal(18,2)) - Target savings
- `CurrentErrorrateProcesses` (decimal(18,2)) - Current error rate
- `OrganizationOperatingMargin` (string, max 50) - Operating margin
- `EstimatePotentialRevenueAzureAI` (decimal(18,2)) - AI revenue potential
- `EstimateOperatingAzureMLInvestment` (decimal(18,2)) - ML investment
- `EstimateSizeOfRevenueAzureProductandServices` (decimal(18,2)) - Product revenue
- `PlanToDevelopAzureAIService` (bool) - Development plans
- `TotalPeopleAI/MLModeling` (int) - AI/ML team size
- `TotalPeopleWorkAutomatedAzureAI` (string, max 10) - Automation impact

#### `AnnualRevenue`
Business size and revenue categorization.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `BusinessType` (string, max 25) - Business category
  - `BusinessSize` (string, max 255) - Organization size classification

### Azure Service Costing

#### `MicrosoftAzureEstimate`
Azure service cost estimations.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `CustomerName` (string) - Customer reference
  - `ServiceCategory` (string) - Azure service type
  - `ServiceType` (string) - Specific service
  - `EstimatedUpfrontCost` (decimal(18,2)) - Initial cost
  - `EstimatedMonthlyCost` (decimal(18,2)) - Recurring cost
  - `EstimatedUpfrontCostCustomised` (decimal(18,2)) - Custom pricing

#### `AzureCognitiveServicesAppliedAi`
Cognitive Services cost modeling.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `CustomerName` (string) - Customer reference
  - `ServiceCategory` (string) - Cognitive service type
  - `ServiceType` (string) - Specific AI service
  - `EstimatedUpfrontCost` (decimal(18,2)) - Initial investment
  - `EstimatedUpfrontCostCustomised` (decimal(18,2)) - Custom pricing

#### `AzureMlCompute`
Machine Learning compute cost analysis.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `CustomerName` (string) - Customer reference
  - `ServiceCategory` (string) - ML service category
  - `ServiceType` (string) - Compute type
  - `EstimatedUpfrontCost` (decimal(18,2)) - Initial cost
  - `EstimatedMonthlyCost` (decimal(18,2)) - Monthly recurring
  - `EstimatedMonthlyCostCustomised` (decimal(18,2)) - Custom monthly cost

### Business Impact Analysis

#### `BusinessGrowth`
Growth projections from AI implementation.
- **Keyless Entity**
- **Key Fields**:
  - `Metric` (string, max 255) - Growth metric
  - `Year1`, `Year2`, `Year3` (decimal(18,2)) - Annual growth projections

#### `OperationalEfficiency`
Efficiency gains measurement.
- **Keyless Entity**
- **Key Fields**:
  - `Metric` (string, max 255) - Efficiency metric
  - `Year1`, `Year2`, `Year3` (decimal(18,2)) - Annual improvements

#### `ManualProcessAutomation`
Process automation benefits tracking.
- **Keyless Entity**
- **Key Fields**:
  - `Metric` (string, max 255) - Automation metric
  - `Year1`, `Year2`, `Year3` (decimal(18,2)) - Automation benefits

#### `SpendingOptimization`
Cost optimization tracking.
- **Keyless Entity**
- **Key Fields**:
  - `Metric` (string, max 255) - Optimization area
  - `Year1`, `Year2`, `Year3` (decimal(18,2)) - Cost savings

#### `RetiredLagacyTechnology`
Legacy system retirement benefits.
- **Keyless Entity**
- **Key Fields**:
  - `Metric` (string, max 255) - Legacy system metric
  - `Year1`, `Year2`, `Year3` (decimal(18,2)) - Retirement benefits

### AI Model Management

#### `ModelDetails`
AI model configuration and metadata.
- **Primary Key**: `Id` (int)
- **Purpose**: Store model definitions and parameters

#### `ModelFineTuneDatum`
Model training and fine-tuning data.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `Username` (string, max 300) - Training initiator
  - `LabelId` (int) - Label reference
  - `URL` (string) - Training data source
  - `IsValid` (string, max 50) - Training validation status
  - `Datetime` (datetime, default: getdate()) - Training timestamp

#### `ModelNames` & `ModelSizes`
AI model catalog and specifications.
- **Primary Key**: `Id` (int)
- **Purpose**: Model inventory and sizing information

#### Training Configuration DTOs

**`SystemInstructionsDTO`:**
- System prompts and instruction templates

**`SuggestionsDTO`:**
- Auto-suggestion configurations

**`TrainModelDTO`:**
- Model training parameters and datasets

### Industry Standards

#### `IndustryStandred`
Industry benchmarks and standard metrics.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `AI/MLEngineerAnnualSalary` (decimal(18,2)) - Salary benchmarks
  - `DataEngineerHourlySalary` (decimal(18,2)) - Hourly rates
  - `QAEmployeeAverageAnnualSalary` (decimal(18,2)) - QA salaries
  - `AnnualSalary` (decimal(18,2)) - General salary benchmark
  - `AzureCognitiveAppliedAIServices` (decimal(18,2)) - Service costs
  - `MicrosoftImplementationSupportServices` (decimal(18,2)) - Support costs
  - `NumberOfFTEEngineers` (int) - Required headcount
  - `MonthsImplementation` (decimal(18,2)) - Implementation timeline
  - `PercentageOfAzureAIApplicationRefining` (decimal(18,2)) - Refinement percentage
  - `RecapturedEmpHours` (decimal(18,2)) - Hour recapture rate
  - `TimeSavingRate` (decimal(18,2)) - Time saving percentage

### Service Mapping

#### `AzureServiceMappingTable`
Maps business capabilities to Azure services.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `IndustryType` (string, max 255) - Target industry
  - `ServiceName` (string, max 255) - Azure service name
  - `ServiceType` (string, max 255) - Service category
  - `CapabilitiesName` (string, max 255) - Business capability

#### `AzureAiConsumptionAndService`
AI service consumption tracking.
- **Primary Key**: `Id` (int)
- **Key Fields**:
  - `Metric` (string, max 255) - Consumption metric
  - `Initial`, `Year1`, `Year2`, `Year3` (decimal(18,2)) - Usage projections

### User Management

#### `UserInformation`
HR portal user accounts.
- **Primary Key**: `UserId` (int)
- **Key Fields**:
  - `UserName` (string, max 100) - Login username
  - `Password` (string, max 50) - Encrypted password  
  - `FirstName` (string, max 100) - User first name
  - `LastName` (string, max 100) - User last name
  - `LoginType` (string, max 200) - Authentication method
  - `CreatedDate` (datetime) - Account creation

---

## Content Safety Models

### `ContentSafetyModel` (DTO)
**Location**: `CR_CoreBot_DataAccess/Models/ContentSafetyModel.cs`

Comprehensive content safety analytics and assessment framework.

#### Prompt Injection Analysis
- `totPromptInj` (int) - Total injection attempts
- `totPromptInjPass` (int) - Passed injection tests
- `totPromptInjFail` (int) - Failed injection tests  
- `totPromptInjScore` (decimal?) - Injection risk score

#### Hallucination Detection  
- `totPromptHal` (int) - Total hallucination checks
- `totPromptHalPass` (int) - Passed checks
- `totPromptHalFail` (int) - Failed checks
- `totPromptHalScore` (decimal?) - Hallucination score

#### Plagiarism Detection
- `totPromptPlg` (int) - Total plagiarism checks  
- `totPromptPlgPass` (int) - Passed checks
- `totPromptPlgFail` (int) - Failed checks
- `totPromptPlgScore` (decimal?) - Plagiarism risk score

#### Toxicity Analysis (Text)
- `totPromptTx` (int) - Total toxicity assessments
- `totPromptTxHtSc` (decimal?) - Hate speech score
- `totPromptTxShSc` (decimal?) - Self-harm score
- `totPromptTxSsSc` (decimal?) - Sexual content score  
- `totPromptTxVlSC` (decimal?) - Violence score

#### Image Toxicity Analysis
- `totPromptImg` (int) - Total image assessments
- `totPromptImgHtSc` (decimal?) - Image hate score
- `totPromptImgShSc` (decimal?) - Image self-harm score
- `totPromptImgSsSc` (decimal?) - Image sexual score
- `totPromptImgVlSC` (decimal?) - Image violence score

#### Assessment Collections
- `promptinjectiondataset` (List<promptInjectionDatasetDetail>) - Dataset details
- `promptinjectiondmodel` (List<promptInjectionDModelDetail>) - Model details
- `promptModel` (List<prompt>) - Prompt assessments
- `ToxicityPromptModel` (List<Toxicityprompt>) - Toxicity results
- `ImageToxicityModel` (List<ImageToxicity>) - Image analysis
- `evaluation` (List<evaluationtypeall>) - Evaluation types
- `LLMAssessmentDetail` (List<LLMAssessmentDetailData>) - Detailed LLM assessments
- `LLMAssessment` (List<LLMAssessmentData>) - LLM assessment summary
- `ChatbotFileName` (List<ChatbotFileName>?) - Associated files
- `promptAssessments` (List<PromptAssessments>?) - Assessment configurations

---

## Relationships & Foreign Keys

### Core Relationships
1. **Customer → Configuration**: `CustomerInformation.CustomerId` ↔ `BotConfiguration.CustomerId`
2. **Customer → Role**: `CustomerInformation.RoleId` ↔ `RoleMaster.RoleId`
3. **User → Files**: `TblUserInformationMapping.UserId` ↔ `TblCustomerFileDetails.UserId`
4. **Files → Processing**: `TblCustomerFileDetails.Id` ↔ `EntityExtractrion.FileId`

### HR Portal Relationships
1. **Configuration Inheritance**: `DefaultConfiguration` → `CustomConfiguration` (template pattern)
2. **Service Mapping**: `AzureServiceMappingTable` ↔ Cost estimation entities
3. **Model Training**: `ModelDetails` ↔ `ModelFineTuneDatum` ↔ Training DTOs

## Data Access Patterns

### Connection String Management
- **Core Context**: Static connection via `RestoreDefaultConnection`
- **HR Context**: Dynamic connection via `ConnectionModel.Connection`
- **Fallback**: Context options pattern for dependency injection

### Entity Framework Features Used
- **Code-First**: Model configuration via `OnModelCreating`
- **Keyless Entities**: Financial projection tables
- **Default Values**: Timestamps and configuration defaults
- **Custom Methods**: `GetContentSafetySettings()` business logic
- **Partial Classes**: Extensibility for custom business logic

### Performance Considerations
- **Indexing**: Primary keys and foreign key relationships
- **Decimal Precision**: Financial calculations use `decimal(18,2)`
- **String Constraints**: Appropriate length limits for performance
- **Timestamp Defaults**: Automatic record timestamping