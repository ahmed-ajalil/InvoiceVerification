using System;
using System.Collections.Generic;
using CR_HRPortalAI_DataAcess.Models;
using Microsoft.EntityFrameworkCore;

namespace CR_HRPortalAI_DataAcess.CR_HRPortal;

public partial class HrportalAiContext : DbContext
{
    private readonly string Connectionstring;
    public HrportalAiContext(ConnectionModel configuration)
    {
        if (configuration.Connection != "" && configuration.Connection != null)

        {

            Connectionstring = configuration.Connection;

        }
    }

    public HrportalAiContext(DbContextOptions<HrportalAiContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

    {

        if (!optionsBuilder.IsConfigured)

        {

            optionsBuilder.UseSqlServer(Connectionstring);

        }

    }

    public virtual DbSet<AnnualRevenue> AnnualRevenues { get; set; }

    public virtual DbSet<AzureAiConsumptionAndService> AzureAiConsumptionAndServices { get; set; }

    public virtual DbSet<AzureCognitiveServicesAppliedAi> AzureCognitiveServicesAppliedAis { get; set; }

    public virtual DbSet<AzureMlCompute> AzureMlComputes { get; set; }

    public virtual DbSet<AzureServiceMappingTable> AzureServiceMappingTables { get; set; }

    public virtual DbSet<BlobFileDatum> BlobFileData { get; set; }

    public virtual DbSet<BusinessGrowth> BusinessGrowths { get; set; }

    public virtual DbSet<CustomConfiguration> CustomConfigurations { get; set; }

    public virtual DbSet<DefaultConfiguration> DefaultConfigurations { get; set; }

    public virtual DbSet<FinalSummary> FinalSummaries { get; set; }

    public virtual DbSet<ImplementationApplicationBuildingAndTraining> ImplementationApplicationBuildingAndTrainings { get; set; }

    public virtual DbSet<IndustryStandred> IndustryStandreds { get; set; }

    public virtual DbSet<ManagementAndQualityAssuranceLabor> ManagementAndQualityAssuranceLabors { get; set; }

    public virtual DbSet<ManualProcessAutomation> ManualProcessAutomations { get; set; }

    public virtual DbSet<MicrosoftAzureEstimate> MicrosoftAzureEstimates { get; set; }

    public virtual DbSet<ModelFineTuneDatum> ModelFineTuneData { get; set; }

    public virtual DbSet<OperationalEfficiency> OperationalEfficiencies { get; set; }

    public virtual DbSet<RetiredLagacyTechnology> RetiredLagacyTechnologies { get; set; }

    public virtual DbSet<SpendingOptimization> SpendingOptimizations { get; set; }

    public virtual DbSet<TotalBenefit> TotalBenefits { get; set; }

    public virtual DbSet<TotalCost> TotalCosts { get; set; }

    public virtual DbSet<UserInformation> UserInformations { get; set; }

    public virtual DbSet<ModelDetails> ModelDetails { get; set; }

    public virtual DbSet<SystemInstructionsDTO> SystemInstructions { get; set; }

    public virtual DbSet<SuggestionsDTO> Suggestions { get; set; }

    public virtual DbSet<TrainModelDTO> TrainModel { get; set; }
    public virtual DbSet<ModelNames> ModelNames { get; set; }

    public virtual DbSet<ModelSizes> Sizes { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnnualRevenue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AnnualRe__3214EC07D16D1C9A");

            entity.ToTable("AnnualRevenue");

            entity.Property(e => e.BusinessSize)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BusinessType)
                .HasMaxLength(25)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AzureAiConsumptionAndService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Azure_AI__3214EC07E619F185");

            entity.ToTable("Azure_AI_Consumption_And_Services");

            entity.Property(e => e.Initial).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Metric)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("METRIC");
            entity.Property(e => e.Year1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Year2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Year3).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<AzureCognitiveServicesAppliedAi>(entity =>
        {
            entity.ToTable("Azure_Cognitive_Services_Applied_AI");

            entity.Property(e => e.CustomerName).HasColumnName("Customer_name");
            entity.Property(e => e.EstimatedUpfrontCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Estimated_upfront_cost");
            entity.Property(e => e.EstimatedUpfrontCostCustomised)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Estimated_upfront_cost_customised");
            entity.Property(e => e.ServiceCategory).HasColumnName("Service_category");
            entity.Property(e => e.ServiceType).HasColumnName("Service_type");
        });

        modelBuilder.Entity<AzureMlCompute>(entity =>
        {
            entity.ToTable("Azure_ML_Compute");

            entity.Property(e => e.CustomerName).HasColumnName("Customer_name");
            entity.Property(e => e.EstimatedMonthlyCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Estimated_Monthly_cost");
            entity.Property(e => e.EstimatedMonthlyCostCustomised)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Estimated_Monthly_cost_customised");
            entity.Property(e => e.EstimatedUpfrontCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Estimated_upfront_cost");
            entity.Property(e => e.ServiceCategory).HasColumnName("Service_category");
            entity.Property(e => e.ServiceType).HasColumnName("Service_type");
        });

        modelBuilder.Entity<AzureServiceMappingTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AzureSer__3214EC079CC4D153");

            entity.ToTable("AzureServiceMapping_table");

            entity.Property(e => e.CapabilitiesName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IndustryType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ServiceName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ServiceType)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<BusinessGrowth>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Business_Growth");

            entity.Property(e => e.Metric)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("METRIC");
            entity.Property(e => e.Year1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR1");
            entity.Property(e => e.Year2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR2");
            entity.Property(e => e.Year3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR3");
        });

        modelBuilder.Entity<CustomConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CustomCo__3214EC072FF468F4");

            entity.ToTable("CustomConfiguration");

            entity.Property(e => e.AnnualRevenue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AnnualSaving).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CurrentErrorrateProcesses).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EstimateOperatingAzureMlinvestment)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("EstimateOperatingAzureMLInvestment");
            entity.Property(e => e.EstimatePotentialRevenueAzureAi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("EstimatePotentialRevenueAzureAI");
            entity.Property(e => e.EstimateSizeOfRevenueAzureProductandServices).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrganizationName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.OrganizationOperatingMargin).HasMaxLength(50);
            entity.Property(e => e.PlanToDevelopAzureAiservice).HasColumnName("PlanToDevelopAzureAIService");
            entity.Property(e => e.TotalPeopleAiMlmodeling).HasColumnName("TotalPeopleAI/MLModeling");
            entity.Property(e => e.TotalPeopleWorkAutomatedAzureAi)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("TotalPeopleWorkAutomatedAzureAI");
        });

        modelBuilder.Entity<DefaultConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DefaultC__3214EC07E3D2BA32");

            entity.ToTable("DefaultConfiguration");

            entity.Property(e => e.AnnualRevenue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AnnualSaving).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CurrentErrorrateProcesses).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EstimateOperatingAzureMlinvestment)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("EstimateOperatingAzureMLInvestment");
            entity.Property(e => e.EstimatePotentialRevenueAzureAi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("EstimatePotentialRevenueAzureAI");
            entity.Property(e => e.EstimateSizeOfRevenueAzureProductandServices).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrganizationName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.OrganizationOperatingMargin).HasMaxLength(50);
            entity.Property(e => e.PlanToDevelopAzureAiservice).HasColumnName("PlanToDevelopAzureAIService");
            entity.Property(e => e.TotalPeopleAiMlmodeling).HasColumnName("TotalPeopleAI/MLModeling");
            entity.Property(e => e.TotalPeopleWorkAutomatedAzureAi)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("TotalPeopleWorkAutomatedAzureAI");
        });

        modelBuilder.Entity<FinalSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Final_Summary");

            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CATEGORY");
            entity.Property(e => e.Initial)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("INITIAL");
            entity.Property(e => e.Presentvalue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PRESENTVALUE");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("TOTAL");
            entity.Property(e => e.Year1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR1");
            entity.Property(e => e.Year2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR2");
            entity.Property(e => e.Year3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR3");
        });

        modelBuilder.Entity<ImplementationApplicationBuildingAndTraining>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Implementation_Application_Building_And_Training");

            entity.Property(e => e.Initial)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("INITIAL");
            entity.Property(e => e.Metric)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("METRIC");
            entity.Property(e => e.Year1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR1");
            entity.Property(e => e.Year2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR2");
            entity.Property(e => e.Year3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR3");
        });

        modelBuilder.Entity<IndustryStandred>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_IndustryStandred1");

            entity.ToTable("IndustryStandred");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AiMlengineerAnnualSalary)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("AI/MLEngineerAnnualSalary");
            entity.Property(e => e.AnnualSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AzureCognitiveAppliedAiservices)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("AzureCognitiveAppliedAIServices");
            entity.Property(e => e.DataEngineerHourlySalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MicrosoftImplementationSupportServices).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MonthsImplementation).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NumberOfFteengineers).HasColumnName("NumberOfFTEEngineers");
            entity.Property(e => e.PercentageOfAzureAiapplicationRefining)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PercentageOfAzureAIApplicationRefining");
            entity.Property(e => e.QaemployeeAverageAnnualSalary)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("QAEmployeeAverageAnnualSalary");
            entity.Property(e => e.RecapturedEmpHours).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TimeSavingRate).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ManagementAndQualityAssuranceLabor>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Management_And_Quality_Assurance_Labor");

            entity.Property(e => e.Initial).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Metric)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("METRIC");
            entity.Property(e => e.Year1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Year2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Year3).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ManualProcessAutomation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Manual_Process_Automation");

            entity.Property(e => e.Metric)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("METRIC");
            entity.Property(e => e.Year1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR1");
            entity.Property(e => e.Year2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR2");
            entity.Property(e => e.Year3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR3");
        });

        modelBuilder.Entity<MicrosoftAzureEstimate>(entity =>
        {
            entity.ToTable("Microsoft_Azure_Estimate");

            entity.Property(e => e.CustomerName).HasColumnName("Customer_name");
            entity.Property(e => e.EstimatedMonthlyCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Estimated_monthly_cost");
            entity.Property(e => e.EstimatedUpfrontCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Estimated_upfront_cost");
            entity.Property(e => e.EstimatedUpfrontCostCustomised)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Estimated_upfront_cost_customised");
            entity.Property(e => e.ServiceCategory).HasColumnName("Service_category");
            entity.Property(e => e.ServiceType).HasColumnName("Service_type");
        });

        modelBuilder.Entity<ModelFineTuneDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ModelFin__3214EC078BB4F6DA");

            entity.Property(e => e.Datetime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsValid).HasMaxLength(50);
            entity.Property(e => e.LabelId).HasColumnName("LabelID");
            entity.Property(e => e.Url).HasColumnName("URL");
            entity.Property(e => e.Username).HasMaxLength(300);
        });

        modelBuilder.Entity<OperationalEfficiency>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Operational_Efficiency");

            entity.Property(e => e.Metric)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("METRIC");
            entity.Property(e => e.Year1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR1");
            entity.Property(e => e.Year2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR2");
            entity.Property(e => e.Year3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR3");
        });

        modelBuilder.Entity<RetiredLagacyTechnology>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Retired_Lagacy_Technology");

            entity.Property(e => e.Metric)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("METRIC");
            entity.Property(e => e.Year1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR1");
            entity.Property(e => e.Year2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR2");
            entity.Property(e => e.Year3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR3");
        });

        modelBuilder.Entity<SpendingOptimization>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Spending_Optimization");

            entity.Property(e => e.Metric)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("METRIC");
            entity.Property(e => e.Year1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR1");
            entity.Property(e => e.Year2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR2");
            entity.Property(e => e.Year3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR3");
        });

        modelBuilder.Entity<TotalBenefit>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Total_Benefits");

            entity.Property(e => e.Metric)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("METRIC");
            entity.Property(e => e.Presentvalue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PRESENTVALUE");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("TOTAL");
            entity.Property(e => e.Year1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR1");
            entity.Property(e => e.Year2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR2");
            entity.Property(e => e.Year3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR3");
        });

        modelBuilder.Entity<TotalCost>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Total_Costs");

            entity.Property(e => e.Cost)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("COST");
            entity.Property(e => e.Initial)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("INITIAL");
            entity.Property(e => e.Presentvalue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PRESENTVALUE");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("TOTAL");
            entity.Property(e => e.Year1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR1");
            entity.Property(e => e.Year2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR2");
            entity.Property(e => e.Year3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("YEAR3");
        });

        modelBuilder.Entity<UserInformation>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserInfo__1788CC4CF99E8A08");

            entity.ToTable("UserInformation");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LoginType).HasMaxLength(200);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
