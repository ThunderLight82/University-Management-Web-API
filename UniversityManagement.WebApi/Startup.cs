using UniversityManagement.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UniversityManagement.Application.Services;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Application.Validations;
using UniversityManagement.WebApi.AutoMapper;

namespace UniversityManagement.WebApi;

public class Startup
{
    private IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextPool<UniversityDbContext>(options => options
            .UseSqlServer(Configuration.GetConnectionString("SQLServer")));

        RegisterServices(services);

        services.AddMvc().AddNewtonsoftJson(options => 
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "UniversityManagement.WebApi", Version = "v1" });
        });
        
        services.AddAutoMapper(typeof(EntitiesMapper));
    }
    
    private void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IStudentService, StudentService>();
        
        services.AddTransient<ICourseServiceValidation, CourseServiceValidation>();
        services.AddTransient<IGroupServiceValidation, GroupServiceValidation>();
        services.AddTransient<IStudentServiceValidation, StudentServiceValidation>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        
        // Endpoints for HomePage
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=GetCourses}/{id?}"
            );

            endpoints.MapControllerRoute(
                name: "Groups",
                pattern: "Home/GetGroups/{courseId}",
                defaults: new { controller = "Home", action = "GetGroups" }
            );

            endpoints.MapControllerRoute(
                name: "Students",
                pattern: "Home/GetStudents/{groupId}",
                defaults: new { controller = "Home", action = "GetStudents" }
            );
        });
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UniversityManagement.WebApi");
        });
    }
}