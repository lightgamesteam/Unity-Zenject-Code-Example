
public class DynamicTooltipEvents : TooltipEvents
{
    protected override void Awake()
    {
        // for dynamic items override parent's Awake to prevent from finding panel components 
    }

    private void Start()
    {
        // start finding panel components when a dynamic item has been instantiated
        base.Awake();
    }
}