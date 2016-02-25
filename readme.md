# Test Data

## Install Core

```
Install-Package TestData
```

## Web Api Support

```
Install-Package TestData.Web
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
