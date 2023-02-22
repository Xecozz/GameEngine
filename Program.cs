using OpenTK.Windowing.Common;

namespace GameEngine;

internal static class Program
{
    private static void Main()
    {
        using var game = new Window(800, 600, "Xecozz Game Engine"); //create window
        game.Run(); //run game
    }
}