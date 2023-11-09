using UnityEngine;

public class CloseAppCommand : ICommand
{
    public void Execute()
    {
        Application.Quit();
    }
}