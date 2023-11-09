using TDL.Constants;
using TDL.Services;
using Zenject;

namespace TDL.Commands
{
	public class ShowContent3DModelScreenCommand : ICommand
	{
		[Inject] private readonly IWindowService _windowService;

		public void Execute()
		{
			_windowService.ShowWindow(WindowConstants.Content3DModelScreen);
		}
	}
}