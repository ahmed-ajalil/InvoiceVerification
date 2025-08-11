using CR_CoreBot_DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public class CropenAiProcContext : DbContext
{
    public readonly IConfiguration _configuration;
    public readonly string Connectionstring;
    public CropenAiProcContext()
    {
        var serviceProvider = GetServiceProvider();
        _configuration = serviceProvider.GetRequiredService<IConfiguration>();
        Connectionstring = _configuration.GetConnectionString("RestoreDefaultConnection");
    }
    private IServiceProvider GetServiceProvider()
    {
        var httpContextAccessor = new HttpContextAccessor();
        return httpContextAccessor.HttpContext?.RequestServices;
    }

    public DbSet<ContentSafetyModel> contentSafety { get; set; }
    public DbSet<assessModel> assessDbModel { get; set; }
    public DbSet<evaluationtypeall> evaluationsDb { get; set; }
    public DbSet<promptInjectionDatasetDetail> promptInjectionDatasetDb { get; set; }
    public DbSet<promptInjectionDModelDetail> promptInjectionDModelDb { get; set; }
    public DbSet<Toxicityprompt> toxicityPromptDb { get; set; }
    public DbSet<ImageToxicity> imgageToxicityPromptDb { get; set; }
    public DbSet<prompt> promptDb { get; set; }
    public DbSet<PromptAssessments> promptAssessmentsDb { get; set; }
    public DbSet<LLMAssessmentData> llmAssessmentDb { get; set; }
    public DbSet<ChatbotFileName>? ChatbotFileNameDb { get; set; }
    public DbSet<assessmentScore>? assessmentScoreDb { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(Connectionstring);
   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<assessmentScore>().HasNoKey();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PromptAssessments>().HasNoKey();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LLMAssessmentData>().HasNoKey();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChatbotFileName>().HasNoKey();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<prompt>().HasNoKey();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ImageToxicity>().HasNoKey();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Toxicityprompt>().HasNoKey();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<promptInjectionDatasetDetail>().HasNoKey();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<promptInjectionDModelDetail>().HasNoKey();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<evaluationtypeall>().HasNoKey();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ContentSafetyModel>().HasNoKey();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<assessModel>().HasNoKey();
        base.OnModelCreating(modelBuilder);
    }
}
