# Dynamic validation
This project is for doing validation dynamically by loading validation rule from external source. Such as: Database, cache, ...

## 1. Dependencies:
- This project depends on:
  - Configuration micro service.
  - Multi-tenant service.
  - ICache implementation.

```cs
public static void AddApplicationServices(this IServiceCollection serviceCollection)
{
    serviceCollection.AddDynamicValidation();
}
```

# Validation Attribute
Add the validation attribute into target object that we need to validate
```cs
public class AddBrokerCommand
{
    [DynamicValidation("name.generic")]
    public string Name { get; set; }
}
```
Finally call the validation service to validate the object

```cs
public class BrokerService 
{

    private readonly IDynamicValidator _validator;
    
    public BrokerService(IValidator validator)
    {
        _validator = validator;
    }
    
    public async Task AddBrokerAsync(AddBrokerCommand command)
    {
        await _validator.ValidateAsync(command);
        // do other stuff
    }
}
````