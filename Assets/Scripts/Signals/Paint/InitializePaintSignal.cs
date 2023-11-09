public class InitializePaintSignal : ISignal
{
    public PaintView PaintView { get; private set; }
    
    public InitializePaintSignal(PaintView paintView)
    {
        PaintView = paintView;
    }
}
