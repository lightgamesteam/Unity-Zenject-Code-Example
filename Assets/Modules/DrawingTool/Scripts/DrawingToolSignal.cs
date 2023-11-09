namespace DrawingTool.Core {
    public class DrawingToolSignal : ISignal {
        public bool HasResponse { get; private set; }
        public string Name { get; private set; }
        public string Metadata { get; private set; }

        public DrawingToolSignal(bool hasResponse, string name, string metadata) {
            HasResponse = hasResponse;
            Name = name;
            Metadata = metadata;
        }
    }
}