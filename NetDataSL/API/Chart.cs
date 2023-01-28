namespace NetDataSL.API;


abstract class Chart
{
    internal void InitChart()
    {
        var content = $"CHART {Process(TypeId)} " +
                      $"{Process(Name)} " +
                      $"{Process(Title)} " +
                      $"{Process(Units)} " +
                      $"{Process(Family)} [" +
                      $"{Process(Context)} [" +
                      $"{Process(ChartType.ToString().ToLower())} [" +
                      $"{Process(Priority.ToString())} [" +
                      $"{Process(UpdateEvery.ToString("0.0"))} [" +
                      (Obsolete ? " \"obsolete\"" : "") + 
                      (Detail ? " \"detail\"" : "") + 
                      (StoreFirst ? " \"store_first\"" : "") + 
                      (Hidden ? " \"hidden\"" : "") + 
                      $" [" +
                      $"{Process(Plugin)} [" +
                      $"{Process(Module)}]" +
                      $"]" +
                      $"]" +
                      $"]" +
                      $"]" +
                      $"]" +
                      $"]";
        Line(content);
    }

    private string TypeId => Type + "." + Id;
    /// <summary>
    /// The Category of the Chart (Cpu or Memory)
    /// </summary>
    protected virtual string Type => "scpsl";
    /// <summary>
    /// The unique id of the chart for the category (utilization, interrupts)
    /// </summary>
    protected virtual string Id => "stat";
    /// <summary>
    /// The name that people will see instead of the Id.
    /// </summary>
    protected virtual string Name => Id;
    /// <summary>
    /// The text above the chart
    /// </summary>
    protected virtual string Title => "Scp SL Stats";
    /// <summary>
    /// The unit type that will be labeled. ex percentage
    /// </summary>
    protected virtual string Units => "percentage";
    /// <summary>
    /// used to group charts together. IE: Net1
    /// </summary>
    protected virtual string Family => Id;
    /// <summary>
    /// Gives the template of the chart. EX: multiple charts present same info for a different family.
    /// Ie. Context = Players, Families = Net 1, Net 2, Test Net etc... 
    /// </summary>
    private string Context => "Stat";

    private ChartType ChartType => ChartType.Line;
    /// <summary>
    /// Lower Number makes the chart appear higher on the page.
    /// </summary>
    protected virtual int Priority => 1000;

    /// <summary>
    /// How often it should be updated.
    /// </summary>
    protected virtual float UpdateEvery => 5f;
    protected virtual bool Obsolete => false;
    protected virtual bool Detail => false;
    protected virtual bool StoreFirst => false;
    protected virtual bool Hidden => false;

    private const string Plugin = "SCP-SL Integration";
    private const string Module = "Stats";
    private void Line(string x) => Console.WriteLine(x);
    private string Process(string str) => str.Replace($" ", "_");
}