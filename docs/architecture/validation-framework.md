# ✅ Validation Framework Architecture

## Overview

Zentient.Abstractions provides a comprehensive validation framework that goes beyond simple input validation. It offers declarative validation rules, contextual validation scenarios, automatic error handling, and deep integration with the envelope pattern for consistent error reporting across your application.

## 🎯 Core Concepts

### **Validators as Architectural Components**

Validators are not just rule containers - they are architectural components with rich metadata and behaviors:

```csharp
[ValidatorDefinition("UserManagement.CreateUser", Version = "2.1.0")]
public class CreateUserValidator : 
    IValidator<CreateUserCommand, UserCode, UserError>
{
    public string Id => "CreateUserValidator.v2.1";
    public string Name => "Create User Command Validator";
    public string Description => "Validates user creation requests with business rules";
    
    private readonly IUserRepository _userRepository;
    private readonly IConfigurationService _config;
    
    public CreateUserValidator(
        IUserRepository userRepository,
        IConfigurationService config)
    {
        _userRepository = userRepository;
        _config = config;
    }
    
    public IMetadata Metadata => new MetadataCollection
    {
        ["Domain"] = "UserManagement",
        ["ValidationType"] = "Command",
        ["BusinessRules"] = new[]
        {
            "EmailUniqueness",
            "PasswordComplexity", 
            "AgeRestriction",
            "PhoneNumberFormat"
        },
        ["ValidationLevel"] = "Strict",
        ["PerformanceTarget"] = "< 50ms",
        ["Dependencies"] = new[] { "IUserRepository", "IConfigurationService" }
    };
    
    public async Task<IValidationResult<UserCode, UserError>> ValidateAsync(
        CreateUserCommand command,
        ValidationContext context = default,
        CancellationToken cancellationToken = default)
    {
        var result = new ValidationResultBuilder<UserCode, UserError>();
        
        // Email validation
        await ValidateEmailAsync(command.Email, result, cancellationToken);
        
        // Name validation
        ValidateName(command.FirstName, command.LastName, result);
        
        // Age validation
        ValidateAge(command.DateOfBirth, result);
        
        // Phone validation
        ValidatePhoneNumber(command.PhoneNumber, result);
        
        // Business rule validation
        await ValidateBusinessRulesAsync(command, result, cancellationToken);
        
        return result.Build();
    }
    
    private async Task ValidateEmailAsync(
        string email, 
        ValidationResultBuilder<UserCode, UserError> result,
        CancellationToken cancellationToken)
    {
        // Format validation
        if (string.IsNullOrWhiteSpace(email))
        {
            result.AddError(UserError.EmailRequired(), "Email");
            return;
        }
        
        if (!IsValidEmailFormat(email))
        {
            result.AddError(UserError.InvalidEmailFormat(email), "Email");
            return;
        }
        
        // Length validation
        if (email.Length > 255)
        {
            result.AddError(UserError.EmailTooLong(email.Length, 255), "Email");
            return;
        }
        
        // Uniqueness validation
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser.IsSuccess)
        {
            result.AddError(UserError.EmailAlreadyExists(email), "Email");
        }
        
        // Domain whitelist validation
        var allowedDomains = await _config.GetAllowedEmailDomainsAsync();
        if (allowedDomains.Any() && !allowedDomains.Contains(GetEmailDomain(email)))
        {
            result.AddError(UserError.EmailDomainNotAllowed(email), "Email");
        }
    }
    
    private void ValidateName(
        string firstName,
        string lastName,
        ValidationResultBuilder<UserCode, UserError> result)
    {
        // First name validation
        if (string.IsNullOrWhiteSpace(firstName))
        {
            result.AddError(UserError.FirstNameRequired(), "FirstName");
        }
        else if (firstName.Length > 50)
        {
            result.AddError(UserError.FirstNameTooLong(firstName.Length, 50), "FirstName");
        }
        else if (!IsValidNameFormat(firstName))
        {
            result.AddError(UserError.InvalidFirstNameFormat(firstName), "FirstName");
        }
        
        // Last name validation
        if (string.IsNullOrWhiteSpace(lastName))
        {
            result.AddError(UserError.LastNameRequired(), "LastName");
        }
        else if (lastName.Length > 50)
        {
            result.AddError(UserError.LastNameTooLong(lastName.Length, 50), "LastName");
        }
        else if (!IsValidNameFormat(lastName))
        {
            result.AddError(UserError.InvalidLastNameFormat(lastName), "LastName");
        }
    }
    
    private void ValidateAge(
        DateTime dateOfBirth,
        ValidationResultBuilder<UserCode, UserError> result)
    {
        var age = CalculateAge(dateOfBirth);
        var minAge = _config.GetMinimumAge();
        var maxAge = _config.GetMaximumAge();
        
        if (age < minAge)
        {
            result.AddError(UserError.AgeTooYoung(age, minAge), "DateOfBirth");
        }
        else if (age > maxAge)
        {
            result.AddError(UserError.AgeTooOld(age, maxAge), "DateOfBirth");
        }
        
        if (dateOfBirth > DateTime.UtcNow)
        {
            result.AddError(UserError.FutureDateOfBirth(dateOfBirth), "DateOfBirth");
        }
    }
    
    private async Task ValidateBusinessRulesAsync(
        CreateUserCommand command,
        ValidationResultBuilder<UserCode, UserError> result,
        CancellationToken cancellationToken)
    {
        // Rate limiting validation
        var recentRegistrations = await _userRepository.GetRecentRegistrationCountAsync(
            TimeSpan.FromHours(1), cancellationToken);
            
        var maxRegistrationsPerHour = _config.GetMaxRegistrationsPerHour();
        if (recentRegistrations >= maxRegistrationsPerHour)
        {
            result.AddError(UserError.RegistrationRateLimitExceeded(
                recentRegistrations, maxRegistrationsPerHour), "General");
        }
        
        // Geographic restrictions
        var clientIP = GetClientIP();
        var isAllowedRegion = await _config.IsAllowedRegionAsync(clientIP);
        if (!isAllowedRegion)
        {
            result.AddError(UserError.RegistrationNotAllowedInRegion(clientIP), "General");
        }
    }
}
```

## 🔧 Validation Rules & Patterns

### **Declarative Validation Rules**

Define validation rules using attributes and fluent syntax:

```csharp
// Attribute-based validation
public record CreateProductCommand : ICommand<CreateProductCommand, ProductCode, ProductError>
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-\.]+$", ErrorMessage = "Name contains invalid characters")]
    public string Name { get; init; } = string.Empty;
    
    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; init; } = string.Empty;
    
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between $0.01 and $999,999.99")]
    [DataType(DataType.Currency)]
    public decimal Price { get; init; }
    
    [Required(ErrorMessage = "Category is required")]
    [ValidCategory] // Custom validation attribute
    public string Category { get; init; } = string.Empty;
    
    [ValidSKU] // Custom validation attribute
    public string SKU { get; init; } = string.Empty;
    
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; init; }
    
    [Url(ErrorMessage = "Image URL must be a valid URL")]
    public string? ImageUrl { get; init; }
    
    [ValidDimensions] // Custom validation for complex object
    public ProductDimensions? Dimensions { get; init; }
}

// Custom validation attributes
public class ValidCategoryAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string category)
            return new ValidationResult("Category must be a string");
            
        var validCategories = new[] { "Electronics", "Clothing", "Books", "Home", "Sports" };
        
        if (!validCategories.Contains(category, StringComparer.OrdinalIgnoreCase))
        {
            return new ValidationResult($"Category must be one of: {string.Join(", ", validCategories)}");
        }
        
        return ValidationResult.Success;
    }
}

public class ValidSKUAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string sku)
            return ValidationResult.Success; // Allow null/empty for optional field
            
        // SKU format: ABC-12345-XYZ
        var skuPattern = @"^[A-Z]{3}-\d{5}-[A-Z]{3}$";
        
        if (!Regex.IsMatch(sku, skuPattern))
        {
            return new ValidationResult("SKU must follow format: ABC-12345-XYZ (3 letters, 5 digits, 3 letters)");
        }
        
        return ValidationResult.Success;
    }
}
```

### **Fluent Validation Rules**

For complex validation scenarios requiring dependency injection:

```csharp
[ValidatorRegistration(ServiceLifetime.Scoped)]
public class CreateProductFluentValidator : FluentValidator<CreateProductCommand, ProductCode, ProductError>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryService _categoryService;
    private readonly IConfigurationService _config;
    
    public CreateProductFluentValidator(
        IProductRepository productRepository,
        ICategoryService categoryService,
        IConfigurationService config)
    {
        _productRepository = productRepository;
        _categoryService = categoryService;
        _config = config;
        
        ConfigureRules();
    }
    
    private void ConfigureRules()
    {
        // Basic field validation
        RuleFor(x => x.Name)
            .NotEmpty().WithError(ProductError.NameRequired())
            .Length(2, 100).WithError(cmd => ProductError.NameLength(cmd.Name.Length, 2, 100))
            .Matches(@"^[a-zA-Z0-9\s\-\.]+$").WithError(cmd => ProductError.NameInvalidFormat(cmd.Name))
            .MustAsync(BeUniqueNameAsync).WithError(cmd => ProductError.NameAlreadyExists(cmd.Name));
            
        RuleFor(x => x.Description)
            .NotEmpty().WithError(ProductError.DescriptionRequired())
            .MaximumLength(1000).WithError(cmd => ProductError.DescriptionTooLong(cmd.Description.Length, 1000));
            
        RuleFor(x => x.Price)
            .GreaterThan(0).WithError(cmd => ProductError.PriceInvalid(cmd.Price))
            .LessThanOrEqualTo(999999.99m).WithError(cmd => ProductError.PriceTooHigh(cmd.Price, 999999.99m))
            .Must(BeValidPriceIncrement).WithError(cmd => ProductError.PriceInvalidIncrement(cmd.Price));
            
        RuleFor(x => x.Category)
            .NotEmpty().WithError(ProductError.CategoryRequired())
            .MustAsync(BeValidCategoryAsync).WithError(cmd => ProductError.CategoryInvalid(cmd.Category));
            
        RuleFor(x => x.SKU)
            .NotEmpty().When(x => _config.GetRequireSKU()).WithError(ProductError.SKURequired())
            .Matches(@"^[A-Z]{3}-\d{5}-[A-Z]{3}$").When(x => !string.IsNullOrEmpty(x.SKU))
                .WithError(cmd => ProductError.SKUInvalidFormat(cmd.SKU))
            .MustAsync(BeUniqueSKUAsync).When(x => !string.IsNullOrEmpty(x.SKU))
                .WithError(cmd => ProductError.SKUAlreadyExists(cmd.SKU));
                
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithError(cmd => ProductError.StockQuantityNegative(cmd.StockQuantity));
            
        RuleFor(x => x.ImageUrl)
            .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.ImageUrl))
                .WithError(cmd => ProductError.ImageUrlInvalid(cmd.ImageUrl!));
                
        // Complex validation rules
        RuleFor(x => x)
            .MustAsync(PassBusinessRulesAsync).WithError(cmd => ProductError.BusinessRulesViolation())
            .Must(BeValidForCategory).WithError(cmd => ProductError.InvalidForCategory(cmd.Category));
            
        // Conditional validation
        When(x => x.Category == "Electronics", () =>
        {
            RuleFor(x => x.Dimensions)
                .NotNull().WithError(ProductError.DimensionsRequiredForElectronics());
                
            RuleFor(x => x.Price)
                .GreaterThan(10).WithError(cmd => ProductError.ElectronicsMinimumPrice(cmd.Price, 10));
        });
        
        When(x => x.Category == "Clothing", () =>
        {
            RuleFor(x => x.Dimensions)
                .Must(HaveValidClothingDimensions).WithError(cmd => ProductError.InvalidClothingDimensions());
        });
    }
    
    private async Task<bool> BeUniqueNameAsync(string name, CancellationToken cancellationToken)
    {
        var existing = await _productRepository.GetByNameAsync(name);
        return !existing.IsSuccess;
    }
    
    private async Task<bool> BeValidCategoryAsync(string category, CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetValidCategoriesAsync();
        return categories.Contains(category, StringComparer.OrdinalIgnoreCase);
    }
    
    private async Task<bool> BeUniqueSKUAsync(string sku, CancellationToken cancellationToken)
    {
        var existing = await _productRepository.GetBySKUAsync(sku);
        return !existing.IsSuccess;
    }
    
    private bool BeValidPriceIncrement(decimal price)
    {
        var increment = _config.GetPriceIncrement();
        return price % increment == 0;
    }
    
    private async Task<bool> PassBusinessRulesAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // Complex business rules that span multiple properties
        var rules = await _config.GetBusinessRulesAsync();
        
        foreach (var rule in rules)
        {
            if (!await rule.ValidateAsync(command, cancellationToken))
                return false;
        }
        
        return true;
    }
}
```

## 🌐 Contextual Validation

### **Validation Contexts & Scenarios**

Different validation rules for different scenarios:

```csharp
// Validation context definition
public record ValidationContext
{
    public string Scenario { get; init; } = "Default";
    public string? UserId { get; init; }
    public string? TenantId { get; init; }
    public Dictionary<string, object> Properties { get; init; } = new();
    public ValidationLevel Level { get; init; } = ValidationLevel.Strict;
}

public enum ValidationLevel
{
    Permissive,  // Basic validation only
    Standard,    // Standard business rules
    Strict,      // All validation rules
    Paranoid     // Maximum validation + additional security checks
}

// Context-aware validator
[ValidatorRegistration(ServiceLifetime.Scoped)]
public class UpdateUserValidator : IValidator<UpdateUserCommand, UserCode, UserError>
{
    public async Task<IValidationResult<UserCode, UserError>> ValidateAsync(
        UpdateUserCommand command,
        ValidationContext context = default,
        CancellationToken cancellationToken = default)
    {
        var result = new ValidationResultBuilder<UserCode, UserError>();
        
        // Scenario-based validation
        switch (context.Scenario)
        {
            case "SelfUpdate":
                await ValidateSelfUpdateAsync(command, result, context, cancellationToken);
                break;
                
            case "AdminUpdate":
                await ValidateAdminUpdateAsync(command, result, context, cancellationToken);
                break;
                
            case "SystemUpdate":
                await ValidateSystemUpdateAsync(command, result, context, cancellationToken);
                break;
                
            case "BulkUpdate":
                await ValidateBulkUpdateAsync(command, result, context, cancellationToken);
                break;
                
            default:
                await ValidateDefaultUpdateAsync(command, result, context, cancellationToken);
                break;
        }
        
        // Level-based validation
        if (context.Level >= ValidationLevel.Standard)
        {
            await ValidateBusinessRulesAsync(command, result, context, cancellationToken);
        }
        
        if (context.Level >= ValidationLevel.Strict)
        {
            await ValidateSecurityRulesAsync(command, result, context, cancellationToken);
        }
        
        if (context.Level >= ValidationLevel.Paranoid)
        {
            await ValidateParanoidRulesAsync(command, result, context, cancellationToken);
        }
        
        return result.Build();
    }
    
    private async Task ValidateSelfUpdateAsync(
        UpdateUserCommand command,
        ValidationResultBuilder<UserCode, UserError> result,
        ValidationContext context,
        CancellationToken cancellationToken)
    {
        // Users can only update their own profiles
        if (command.UserId != context.UserId)
        {
            result.AddError(UserError.UnauthorizedUpdate(command.UserId, context.UserId!), "UserId");
            return;
        }
        
        // Restricted fields for self-update
        var restrictedFields = new[] { "Role", "Status", "Permissions" };
        var changedFields = GetChangedFields(command);
        var restrictedChanges = changedFields.Intersect(restrictedFields).ToList();
        
        if (restrictedChanges.Any())
        {
            result.AddError(UserError.RestrictedFieldsInSelfUpdate(restrictedChanges), "General");
        }
    }
    
    private async Task ValidateAdminUpdateAsync(
        UpdateUserCommand command,
        ValidationResultBuilder<UserCode, UserError> result,
        ValidationContext context,
        CancellationToken cancellationToken)
    {
        // Verify admin permissions
        var hasAdminPermission = await _permissionService.HasPermissionAsync(
            context.UserId!, "Users.Admin", cancellationToken);
            
        if (!hasAdminPermission)
        {
            result.AddError(UserError.InsufficientPermissions("Users.Admin"), "General");
            return;
        }
        
        // Admins cannot modify super admin accounts
        var targetUser = await _userRepository.GetByIdAsync(command.UserId);
        if (targetUser.IsSuccess && targetUser.Value.Role == "SuperAdmin")
        {
            result.AddError(UserError.CannotModifySuperAdmin(command.UserId), "UserId");
        }
    }
}

// Usage with context
public class UserController : ControllerBase
{
    private readonly IValidator<UpdateUserCommand, UserCode, UserError> _validator;
    
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserCommand command)
    {
        var context = new ValidationContext
        {
            Scenario = "SelfUpdate",
            UserId = GetCurrentUserId(),
            Level = ValidationLevel.Standard
        };
        
        var validationResult = await _validator.ValidateAsync(command, context);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        // Process update...
    }
    
    [HttpPut("admin/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminUpdateUser(string userId, [FromBody] UpdateUserCommand command)
    {
        var context = new ValidationContext
        {
            Scenario = "AdminUpdate",
            UserId = GetCurrentUserId(),
            TenantId = GetCurrentTenantId(),
            Level = ValidationLevel.Strict
        };
        
        command = command with { UserId = userId };
        var validationResult = await _validator.ValidateAsync(command, context);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        // Process admin update...
    }
}
```

## 🔗 Validation Pipeline Integration

### **Automatic Validation in Message Pipeline**

Seamless integration with CQRS pipeline:

```csharp
// Validation behavior that runs automatically
public class ValidationBehavior<TRequest, TResponse> : 
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IValidationContextFactory _contextFactory;
    
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (!_validators.Any())
        {
            return await next();
        }
        
        var context = await _contextFactory.CreateContextAsync(request);
        var validationTasks = _validators.Select(v => v.ValidateAsync(request, context, cancellationToken));
        var validationResults = await Task.WhenAll(validationTasks);
        
        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();
            
        if (failures.Any())
        {
            return CreateValidationErrorResponse<TResponse>(failures);
        }
        
        return await next();
    }
    
    private static TResponse CreateValidationErrorResponse<TResponse>(List<ValidationError> failures)
    {
        // Convert validation failures to appropriate response type
        if (typeof(TResponse).IsAssignableTo(typeof(IEnvelope)))
        {
            return CreateEnvelopeValidationResponse<TResponse>(failures);
        }
        
        throw new ValidationException("Validation failed", failures);
    }
}

// Context factory for automatic context creation
[ServiceRegistration(ServiceLifetime.Scoped)]
public class ValidationContextFactory : IValidationContextFactory
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly ITenantService _tenantService;
    
    public async Task<ValidationContext> CreateContextAsync<T>(T request)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var scenario = DetermineScenario(request, httpContext);
        var level = DetermineValidationLevel(request, httpContext);
        
        var context = new ValidationContext
        {
            Scenario = scenario,
            Level = level,
            UserId = GetCurrentUserId(httpContext),
            TenantId = GetCurrentTenantId(httpContext)
        };
        
        // Add request-specific properties
        if (request is IHasMetadata metadataRequest)
        {
            foreach (var item in metadataRequest.Metadata)
            {
                context.Properties[item.Key] = item.Value;
            }
        }
        
        return context;
    }
    
    private string DetermineScenario<T>(T request, HttpContext? httpContext)
    {
        // Determine scenario from request type, route, headers, etc.
        if (httpContext?.Request.Path.StartsWithSegments("/api/admin") == true)
            return "AdminUpdate";
            
        if (httpContext?.Request.Path.StartsWithSegments("/api/bulk") == true)
            return "BulkUpdate";
            
        if (httpContext?.Request.Path.StartsWithSegments("/api/system") == true)
            return "SystemUpdate";
            
        return "Default";
    }
}
```

### **Conditional Validation Rules**

Apply validation rules based on runtime conditions:

```csharp
[ValidatorRegistration(ServiceLifetime.Scoped)]
public class ConditionalOrderValidator : FluentValidator<CreateOrderCommand, OrderCode, OrderError>
{
    private readonly IFeatureToggleService _features;
    private readonly IConfigurationService _config;
    
    public ConditionalOrderValidator(
        IFeatureToggleService features,
        IConfigurationService config)
    {
        _features = features;
        _config = config;
        
        ConfigureRules();
    }
    
    private void ConfigureRules()
    {
        // Standard validation
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithError(OrderError.CustomerIdRequired());
            
        RuleFor(x => x.Items)
            .NotEmpty().WithError(OrderError.ItemsRequired())
            .Must(HaveValidItems).WithError(cmd => OrderError.InvalidItems());
            
        // Feature-toggled validation
        When(x => _features.IsEnabled("StrictInventoryValidation"), () =>
        {
            RuleFor(x => x.Items)
                .MustAsync(AllItemsInStockAsync).WithError(cmd => OrderError.ItemsNotInStock());
        });
        
        When(x => _features.IsEnabled("PremiumCustomerValidation"), () =>
        {
            RuleFor(x => x)
                .MustAsync(ValidatePremiumCustomerRulesAsync)
                .WithError(cmd => OrderError.PremiumCustomerRulesViolation());
        });
        
        // Configuration-based validation
        When(x => _config.GetValue<bool>("EnableFraudDetection"), () =>
        {
            RuleFor(x => x)
                .MustAsync(PassFraudDetectionAsync)
                .WithError(cmd => OrderError.FraudDetected());
        });
        
        // Time-based validation
        When(x => IsBusinessHours(), () =>
        {
            RuleFor(x => x.TotalAmount)
                .LessThanOrEqualTo(_config.GetValue<decimal>("BusinessHours.MaxOrderAmount"))
                .WithError(cmd => OrderError.AmountExceedsBusinessHoursLimit());
        });
        
        When(x => !IsBusinessHours(), () =>
        {
            RuleFor(x => x.TotalAmount)
                .LessThanOrEqualTo(_config.GetValue<decimal>("AfterHours.MaxOrderAmount"))
                .WithError(cmd => OrderError.AmountExceedsAfterHoursLimit());
        });
        
        // User role-based validation
        When(x => IsInternalUser(), () =>
        {
            RuleFor(x => x.DiscountPercentage)
                .LessThanOrEqualTo(50).WithError(cmd => OrderError.DiscountTooHigh(cmd.DiscountPercentage, 50));
        });
        
        When(x => !IsInternalUser(), () =>
        {
            RuleFor(x => x.DiscountPercentage)
                .LessThanOrEqualTo(20).WithError(cmd => OrderError.DiscountTooHigh(cmd.DiscountPercentage, 20));
        });
    }
}
```

## 📊 Validation Performance & Optimization

### **Parallel Validation Execution**

Optimize validation performance for complex scenarios:

```csharp
[ValidatorRegistration(ServiceLifetime.Scoped)]
public class HighPerformanceUserValidator : IValidator<CreateUserCommand, UserCode, UserError>
{
    public async Task<IValidationResult<UserCode, UserError>> ValidateAsync(
        CreateUserCommand command,
        ValidationContext context = default,
        CancellationToken cancellationToken = default)
    {
        var result = new ValidationResultBuilder<UserCode, UserError>();
        
        // Create validation tasks that can run in parallel
        var validationTasks = new[]
        {
            ValidateEmailAsync(command.Email, result, cancellationToken),
            ValidatePhoneAsync(command.PhoneNumber, result, cancellationToken),
            ValidateBusinessRulesAsync(command, result, cancellationToken),
            ValidateSecurityRulesAsync(command, result, cancellationToken),
            ValidateComplianceRulesAsync(command, result, cancellationToken)
        };
        
        // Execute all validation tasks in parallel
        await Task.WhenAll(validationTasks);
        
        // Perform sequential validation that depends on parallel results
        if (result.HasErrors)
        {
            await ValidateDependentRulesAsync(command, result, cancellationToken);
        }
        
        return result.Build();
    }
    
    private async Task ValidateEmailAsync(
        string email,
        ValidationResultBuilder<UserCode, UserError> result,
        CancellationToken cancellationToken)
    {
        // Fast local validation
        if (!IsValidEmailFormat(email))
        {
            result.AddError(UserError.InvalidEmailFormat(email), "Email");
            return;
        }
        
        // Parallel external validations
        var tasks = new[]
        {
            CheckEmailUniquenessAsync(email, cancellationToken),
            CheckEmailDomainAsync(email, cancellationToken),
            CheckEmailReputationAsync(email, cancellationToken)
        };
        
        var results = await Task.WhenAll(tasks);
        
        // Combine results
        foreach (var validationResult in results.Where(r => !r.IsValid))
        {
            result.AddErrors(validationResult.Errors);
        }
    }
}
```

### **Validation Caching**

Cache validation results for improved performance:

```csharp
[ValidatorRegistration(ServiceLifetime.Scoped)]
public class CachedEmailValidator : IValidator<string, UserCode, UserError>
{
    private readonly ICache<ValidationResult<UserCode, UserError>> _cache;
    private readonly IEmailValidationService _emailService;
    
    public async Task<IValidationResult<UserCode, UserError>> ValidateAsync(
        string email,
        ValidationContext context = default,
        CancellationToken cancellationToken = default)
    {
        // Try cache first
        var cacheKey = $"EmailValidation:{email.ToLowerInvariant()}";
        var cached = await _cache.GetAsync(cacheKey);
        
        if (cached.IsSuccess && !cached.Value.IsExpired)
        {
            return cached.Value;
        }
        
        // Perform validation
        var result = await _emailService.ValidateAsync(email, cancellationToken);
        
        // Cache successful results for longer, failures for shorter duration
        var cacheDuration = result.IsValid 
            ? TimeSpan.FromHours(24)   // Valid emails cached for 24 hours
            : TimeSpan.FromMinutes(5);  // Invalid emails cached for 5 minutes
            
        await _cache.SetAsync(cacheKey, result, cacheDuration);
        
        return result;
    }
}
```

---

**Zentient's validation framework provides comprehensive, contextual validation with rich error handling, performance optimization, and seamless integration with the envelope pattern for consistent error reporting across your application.**
