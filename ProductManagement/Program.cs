using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductManagement.Features.Products;
using ProductManagement.Features.Products.Mappers;
using ProductManagement.Persistence;
using ProductManagement.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc
    (
        "v1",
        new OpenApiInfo
        {
            Title = "Product Management API",
            Version = "v1",
            Description = "API for managing products.",
            Contact = new OpenApiContact
            {
                Name = "API Support",
                Email = "support@example.com",
            }
        });
});


builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ProductManagementContext>(options =>
    options.UseSqlite("Data Source=productmanagement.db"));

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AdvancedProductMappingProfile>(), typeof(AdvancedProductMappingProfile));

builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<GetAllProductsHandler>();
builder.Services.AddScoped<DeleteProductHandler>();
builder.Services.AddScoped<GetProductByIdHandler>();
builder.Services.AddScoped<UpdateProductHandler>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();
builder.Services.AddFluentValidationAutoValidation();


builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductManagementContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI
        (
            c=>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API V1");
                c.RoutePrefix = string.Empty;
                c.DisplayRequestDuration();
            }
        );
    
    app.MapOpenApi();
}

app.UseCors("DevCors");

app.UseHttpsRedirection();

app.MapPost("/products", async (CreateProductCommand req, CreateProductHandler handler) =>
    await handler.Handle(req));
app.MapGet("/products", async (GetAllProductsHandler handler) =>
    await handler.Handle(new GetAllProductsQuery()));
app.MapDelete("/products/{id:guid}", async (Guid id, DeleteProductHandler handler) =>
{
    await handler.Handle(new DeleteProductCommand(id));
});
app.MapGet("/products/{id:guid}", async (Guid id, GetProductByIdHandler handler) =>
    await handler.Handle(new GetProductByIdQuery(id)));
app.MapPut("/products/{id:guid}",
    async (Guid id, UpdateProductCommand request, UpdateProductHandler handler) =>
    {
        var updatedRequest = request with { Id = id };
        var result = await handler.Handle(updatedRequest);
        return result;
    });
app.Run();
