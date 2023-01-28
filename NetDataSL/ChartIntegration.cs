namespace NetDataSL;

public class ChartIntegration
{
    private readonly ChartIntegration? _singleton;
    internal ChartIntegration()
    {
        if(_singleton != null)
            return;
        _singleton = this;
        
    }
    
}