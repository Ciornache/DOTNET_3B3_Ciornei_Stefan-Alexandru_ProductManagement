using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductManagement.Features.Products;
using ProductManagement.Features.Products.Mappers;
using ProductManagement.Mappers;
using ProductManagement.Persistence;
using ProductManagement.Validators;
using ProductManagement.Middleware;

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
            Description = "API for managing products with advanced validation and mapping.",
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

// Register both AutoMapper profiles
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ProductMappingProfile>();
    cfg.AddProfile<AdvancedProductMappingProfile>();
}, typeof(ProductMappingProfile), typeof(AdvancedProductMappingProfile));

builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<GetAllProductsHandler>();
builder.Services.AddScoped<DeleteProductHandler>();
builder.Services.AddScoped<GetProductByIdHandler>();
builder.Services.AddScoped<UpdateProductHandler>();

// Register CreateProductProfileValidator as scoped service
builder.Services.AddScoped<IValidator<CreateProductProfileCommand>, CreateProductProfileValidator>();

// Register all validators from assembly containing CreateProductProfileValidator
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductProfileValidator>();
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

// Add CorrelationMiddleware to pipeline
app.UseMiddleware<CorrelationMiddleware>();

app.UseHttpsRedirection();

// Update endpoint mapping to /products with product-specific documentation
app.MapPost("/products", async (CreateProductProfileCommand req, CreateProductHandler handler) =>
    await handler.Handle(req))
    .WithName("CreateProduct")
    .WithOpenApi(operation => new(operation)
    {
        Summary = "Create a new product",
        Description = "Creates a new product with validation and advanced mapping support"
    });

app.MapGet("/products", async (GetAllProductsHandler handler) =>
    await handler.Handle(new GetAllProductsQuery()))
    .WithName("GetAllProducts")
    .WithOpenApi(operation => new(operation)
    {
        Summary = "Get all products",
        Description = "Retrieves all products with their profiles"
    });

app.MapDelete("/products/{id:guid}", async (Guid id, DeleteProductHandler handler) =>
{
    await handler.Handle(new DeleteProductCommand(id));
})
.WithName("DeleteProduct")
.WithOpenApi(operation => new(operation)
{
    Summary = "Delete a product",
    Description = "Deletes a product by ID"
});

app.MapGet("/products/{id:guid}", async (Guid id, GetProductByIdHandler handler) =>
    await handler.Handle(new GetProductByIdQuery(id)))
    .WithName("GetProductById")
    .WithOpenApi(operation => new(operation)
    {
        Summary = "Get product by ID",
        Description = "Retrieves a single product by its unique identifier"
    });

app.MapPut("/products/{id:guid}",
    async (Guid id, UpdateProductCommand request, UpdateProductHandler handler) =>
    {
        var updatedRequest = request with { Id = id };
        var result = await handler.Handle(updatedRequest);
        return result;
    })
    .WithName("UpdateProduct")
    .WithOpenApi(operation => new(operation)
    {
        Summary = "Update a product",
        Description = "Updates an existing product by ID"
    });

app.Run();
