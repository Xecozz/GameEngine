namespace GameEngine;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class Window : GameWindow
{
    public Window(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings() { Size = (width, height), Title = title })
    {
    } //create constructor


    //create vertex array
    private readonly float[] _vertices =
    {
        -0.5f, -0.5f, 0.0f, //Bottom-left vertex
        0.5f, -0.5f, 0.0f, //Bottom-right vertex
        0.0f, 0.5f, 0.0f //Top vertex
    };

    //create buffer
    private int _VertexBufferObject;

    //create vertex array object
    private int _VertexArrayObject;

    private Shader _shader; //create shader


    //when update frame 60/sec
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e); //update frame

        if (KeyboardState.IsKeyDown(Keys.Escape)) //if escape key is pressed
        {
            Close(); //close window
        }
    }

    // render frame événements que le système peut gérer
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        _shader.Use();

        // Bind the VAO
        GL.BindVertexArray(_VertexArrayObject);

        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        

        // OpenTK windows are what's known as "double-buffered". In essence, the window manages two buffers.
        // One is rendered to while the other is currently displayed by the window.
        // This avoids screen tearing, a visual artifact that can happen if the buffer is modified while being displayed.
        // After drawing, call this function to swap the buffers. If you don't, it won't display what you've rendered.
        SwapBuffers();
    }

    protected override void OnUnload()
    {
        // Unbind all the resources by binding the targets to 0/null.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        // Delete all the resources.
        GL.DeleteBuffer(_VertexBufferObject);
        GL.DeleteVertexArray(_VertexArrayObject);

        GL.DeleteProgram(_shader.Handle);
        
        _shader.Dispose();

        base.OnUnload();
    }


    // when load window
    protected override void OnLoad()
    {
        base.OnLoad();


        GL.ClearColor(0.2f, 0.3f, 0.6f, 1.0f); //set background color

        _VertexBufferObject = GL.GenBuffer(); //generate buffer
        
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VertexBufferObject); //bind buffer

        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw); //copy data to buffer
        
        _VertexArrayObject = GL.GenVertexArray(); //generate vertex array object
        GL.BindVertexArray(_VertexArrayObject); //bind vertex array object

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float),
            0); //set vertex attribute pointer
        GL.EnableVertexAttribArray(0);
        


        _shader = new Shader("shader.vert", "shader.frag"); //create shader
        _shader.Use();
    }


    // rezise window
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height); //set viewport
    }
}