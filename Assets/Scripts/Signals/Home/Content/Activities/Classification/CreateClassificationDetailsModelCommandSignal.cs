
public class CreateClassificationDetailsModelCommandSignal : ISignal
{
    public string ClassificationDetailsResponse { get; private set; }

    public CreateClassificationDetailsModelCommandSignal(string classificationDetailsResponse)
    {
        ClassificationDetailsResponse = classificationDetailsResponse;
    }
}