# Test Data

## Install Core

```
Install-Package TestData.Interface
```

## Web Api Support

```
Install-Package TestData.Interface.Web
```

## MediatR Support

```
Install-Package TestData.Interface.MediatR
```

## Csv (FileSets) Support

```
Install-Package TestData.Interface.Files
```

## Angular front end

This is designed to work with the webapi `TestDataController` which uses the attribute route prefix `/api/testdata`. However you could use any backend provided it accepts and returns data in the expected format. The test data UI is served on the client route: `/testdata`.

```
bower install test-data --save
```

TestDataController returns a list of datasets in this format. It accepts a `get` request with no parameters.

``` csharp
descriptors.Select(d => new
  {
      Dependencies = d.Dependencies.Select(t => t.FullName),
      d.Name,
      d.Description,
      d.Type.FullName,
      TypeName = d.Type.Name,
      Properties = d.Properties.Select(p => new
      {
          FieldName = p.MemberInfo.Name,
          p.Property.Name,
          p.Property.Description,
          p.Property.DataType,
          p.Property.Required
      })
  }
```

DataSet request payloads should match below. It makes a `post` request.

``` csharp
public interface IDataSetRequest
{
    string DataSet { get; }
    IDictionary<string, IDictionary<string, string>> Properties { get; }
}
```

## Configuring your angular application

You will need to require the module `test-data` and register a constant called `apiBase`. Do not include a trailing forward slash. This is the base path used to make the request e.g. `/api` becomes `/api/testdata`.

``` js
angular.module('app', ['test-data'])
  .constant('apiBase', '/api');
```

## Configuring for AutoFac and MediatR

``` csharp
//Assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith("demo") || a.GetName().Name.StartsWith("TestData")).ToArray();

void RegisterTestData(ContainerBuilder builder)
{
    builder.RegisterApiControllers(typeof(TestDataController).Assembly);
    builder.RegisterTypes(Assemblies.SelectMany(a => a.GetTypes()).Where(x => !x.IsAbstract && typeof(IDataSet).IsAssignableFrom(x)).ToArray());

    builder
        .Register<Func<Type, IDataSet>>(x =>
        {
            var scope = x.Resolve<ILifetimeScope>();
            return (t) => scope.Resolve(t) as IDataSet;
        })
        .InstancePerRequest();

    builder
        .Register(x =>
        {
            var mediator = x.Resolve<IMediator>();
            var dispatcher = new MediatRDispatcher(mediator);
            return dispatcher;
        })
        .As<IDispatcher>()
        .InstancePerRequest();
}
```

## DataSets

The string returned from a dataset is returned to the client (to be used as feedback).

## Sample DataSet

This example is a bit contrived but demonstrates specifying a property and making use of it.

``` csharp
[DataSet("Cities", "A list of cities")]
public class CityDataSet : IDataSet
{
    private readonly IDbContext _dbContext;

    public  CityDataSet(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [DataSetProperty("Prefix", DataType.String, "Prefix the city name")]
    public string Prefix { get; set; }

    public async Task<string> Execute()
    {
        var city = new City
        {
            Name = Prefix + " Melbourne"
        };

        var cities = _dbContext.Set<City>();
        cities.Add(city);

        await _dbContext.SaveChangesAsync();

        return "Inserted 1 City";
    }
}
```

This example shows reading data from a CSV. The csv path is relative to the assembly containing the DataSet.

``` csharp
[DataSet("States", "A list of states")]
[DataSetDependency(typeof(CityDataSet))]
public class StateDataSet : IDataSet
{
    private readonly IDbSet<State> _states;

    public StateDataSet(IDbSet<State> states)
    {
        _states = states;
    }

    [DataSetProperty("Starts With", DataType.String, "Filter the states inserted", Required = true)]
    public string StartsWith { get; set; }

    [DataSetProperty("Created On", DataType.Date, "Date the states were created on")]
    public DateTime CreatedOn { get; set; }

    public async Task<string> Execute()
    {
        int count = 0;
        States.ForEach(stateRecord =>
        {
            if (StartsWith != null && !stateRecord.Name.StartsWith(StartsWith))
            {
                return;
            }

            var state = new State
            {
                Name = stateRecord.Name,
                CreatedOn = CreatedOn
            };
            count ++;

            states.Add(state);
        }, this);

        return String.Format("Inserted {0} States", count);
    }

    [FileDataSet("./data/states.csv")]
    public FileDataSetInstance<State> States { get; set; }
}
```
