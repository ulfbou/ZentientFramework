# Zentient.Repository API Documentation
Zentient.Repository is a generic repository and unit of work pattern implementation for Entity Framework Core, part of the Zentient Framework. It provides a set of methods for managing the overall data access and transactional behavior of an application.

## Getting Started
To use Zentient.Repository, first install the NuGet package in your project:

```csharp
Install-Package Zentient.Repository
``` 

Then, create a new class that inherits from `Repository<T>` and `IRepository<T>`:

```csharp
public class MyService
{
	private readonly IRepository<MyEntity> _repository;

	public MyService(IRepository<MyEntity> repository)
	{
		_repository = repository;
	}

	public async Task<IEnumerable<MyEntity>> GetAllAsync()
	{
		return await _repository.GetAllAsync();
	}
}
```

Finally, inject the repository into your service or controller:

```csharp
services.AddScoped<IRepository<MyEntity>, Repository<MyEntity>>();
```

In this example, `MyEntity` is a class that represents a database entity. The `MyService` class contains a method that retrieves all entities from the database using the repository. The repository is injected into the service using dependency injection.

## Features
- **Generic Repository**: Provides a generic repository implementation for Entity Framework Core.
- **Unit of Work**: Implements the unit of work pattern to manage transactions and data access.
- **Asynchronous Methods**: Supports asynchronous methods for improved performance and scalability.
- **Dependency Injection**: Integrates with the ASP.NET Core dependency injection system for easy configuration and usage.
- **Exception Handling Delegate**: Allows custom exception handling logic to be defined for database operations.

## Usage
The following example demonstrates how to use the repository to use the units of work pattern:
```csharp
public class MyService
{
	private readonly IUnitOfWork _unitOfWork;

	public MyService(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	// generate some interesting sample code that makes use of the unit of work pattern with multiple repositories
	public async Task DoSomethingAsync()
	{
		using (var transaction = _unitOfWork.BeginTransaction())
		{
			try
			{
				var entity1 = new MyEntity { Name = "Entity 1" };
				var entity2 = new MyEntity { Name = "Entity 2" };
				_unitOfWork.Repository<MyEntity>().AddRangeAsync(new[] { entity1, entity2 });
				var entity3 = new MyOtherEntity { Name = "Entity 3", Description = "Description of Entity 3" };
				_unitOfWork.Repository<MyOtherEntity>().AddAsync(entity3);
				transaction.Commit();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				throw;
			}
		}
	}
}
```
In this example, the `MyService` class contains a method that demonstrates how to use the unit of work pattern with multiple repositories. The `DoSomethingAsync` method creates new entities and adds them to the repositories using the unit of work. If an exception occurs during the operation, the transaction is rolled back to maintain data consistency.

## License
Zentient.Repository is licensed under the MIT License. See the [LICENSE](https://github.com/ulfbou/ZentientFramework/wiki/License) file for more information.

## Contributing
We welcome contributions to Zentient.Repository! Please see our [contributing guide](https://github.com/ulfbou/ZentientFramework/wiki/Contributing) for more information.
