
namespace TDL.Signals
{
	public class DescriptionClickViewSignal : ISignal
	{
		public int Id { get; }

		public DescriptionClickViewSignal(int id)
		{
			Id = id;
		}
	}	
}