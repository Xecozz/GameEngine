using OpenTK.Windowing.Common;

namespace GameEngine;

internal static class Program
{
    private static void Main(string[] args)
    {
        using var game = new Window(800, 600, "Xecozz Game Engine"); //create window
        game.VSync = VSyncMode.Adaptive; //set vsync mode 60fps
        game.Run(); //run game
    }
}