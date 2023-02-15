using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameEngine;

public class Window : GameWindow
{
    public Window(int width, int height, string title) : base(new GameWindowSettings
    {
        UpdateFrequency = 60,
        RenderFrequency = 60
    }, new NativeWindowSettings
    {
        Size = (width, height),
        Title = title,
        API = ContextAPI.OpenGL,
        APIVersion = new Version(4, 6),
        AutoLoadBindings = true,
        Flags = ContextFlags.Default
    })
    {
    }

    //create vertex array
    private readonly float[] _vertices =
    {
        // positions        // colors
        0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, // bottom left
        0.0f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f // top 
    };

    //private readonly uint[] _indices = {  // note that we start from 0!
    //    0, 1, 3,   // first triangle
    //    1, 2, 3    // second triangle
    //};

    private Stopwatch _timer;

    //create buffer
    private int _VertexBufferObject;
    //private int _ElementBufferObject;

    //create vertex array object
    private int _VertexArrayObject;


    //create shader
    private Shader _shader;

    // shader path
    private const string _ShaderPath = "../../../Asset/";

    // when load window
    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        _VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StreamCopy);

        _VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_VertexArrayObject);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // For create rectangle with EBO
        //_ElementBufferObject = GL.GenBuffer();
        //GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ElementBufferObject);
        // We also upload data to the EBO the same way as we did with VBOs.
        //GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
        // The EBO has now been properly setup. Go to the Render function to see how we draw our rectangle now!
        

        _shader = new Shader(_ShaderPath + "shader_vert.glsl", _ShaderPath + "shader_frag.glsl"); //create shader
        _shader.Use();

        // We start the stopwatch here as this method is only called once.
        _timer = new Stopwatch();
        _timer.Start();
    }

    // render frame events
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit); //clear screen

        _shader.Use(); // re-use shader

        // update the uniform color
        double timeValue = _timer.Elapsed.TotalSeconds;
        float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
        int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "ourColor");
        GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);

        // Bind the VAO so OpenGL knows to use it
        GL.BindVertexArray(_VertexArrayObject);

        GL.DrawArrays(PrimitiveType.Triangles, 0, 3); //draw triangle

        //draw behind the screen so swap Buffers
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

    // rezise window
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height); //set viewport
    }

    //when update frame 60/sec
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e); //update frame

        if (KeyboardState.IsKeyDown(Keys.Escape)) //if escape key is pressed
        {
            Close(); //close window
        }
    }
}