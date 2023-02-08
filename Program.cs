using System;
using System.Numerics;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameEngine;

internal static class Program
{
    private static void Main(string[] args)
    {
        using (var game = new Window(800, 600, "Xecozz Game Engine")) //create game window
        {
            game.VSync = VSyncMode.Adaptive; //set vsync mode 60fps
            game.Run(); //run game
        }
    }
}