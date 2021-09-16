using System;
using GarmoFamilyTree.Interfaces;
using GarmoFamilyTree.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.IO;
using GarmoFamilyTree.DataAccess;
using GarmoFamilyTree.DataAccess.DbRepositories;
using GarmoFamilyTree.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace GarmoFamilyTree
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public static IConfiguration Configuration { get; private set; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddScoped<IFamilyTreeService, FamilyTreeService>();
      services.AddScoped<IRandomNumberService, RandomNumberService>();
      services.AddScoped<FamilyTreeRepository>();
      services.AddScoped<DbFamilyTreeRepository>();
      services.AddDbContext<FamilyTreeContext>(opt =>
        opt.UseInMemoryDatabase("FamilyTree"));

      services.AddMemoryCache();
      services.AddControllers();

      //services.AddSwaggerGen();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Version = "v1",
          Title = "Garmo FamilyTree API",
          Description = "An API to register and maintain a family tree",
          Contact = new OpenApiContact
          {
            Name = "Sondre Garmo",
            Email = string.Empty
          }
        });

        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      // Enable middleware to serve generated Swagger as a JSON endpoint.
      app.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
      // specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Garmo FamilyTree API V1");
        c.RoutePrefix = string.Empty;
      });

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapGet("/",
          async context => { await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda"); });
      });
    }
  }
}
