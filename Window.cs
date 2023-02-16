using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
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
        //Position          Texture coordinates
        0.5f, 0.5f, 0.0f, 1.0f, 1.0f, // top right
        0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
        -0.5f, 0.5f, 0.0f, 0.0f, 1.0f // top left
    };

    private readonly uint[] _indices =
    {
        0, 1, 3,
        1, 2, 3
    };
    
    //create buffer
    private int _vertexBufferObject;
    private int _elementBufferObject;

    //create vertex array object
    private int _vertexArrayObject;

    //create shader
    private Shader _shader;

    //create texture
    private Texture _texture;

    // shader path
    private const string ShaderPath = "../../../Asset/Shaders/";

    // texture path
    private const string TexturePath = "../../../Asset/Textures/";

    // when load window
    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw);

        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
            BufferUsageHint.StaticDraw);

        // The shaders have been modified to include the texture coordinates, check them out after finishing the OnLoad function.
        _shader = new Shader(ShaderPath + "shader_vert.glsl", ShaderPath + "shader_frag.glsl");
        _shader.Use();
        
        var vertexLocation = _shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        
        var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
            3 * sizeof(float));

        _texture = new Texture(TexturePath + "wall.jpg");
        _texture.Use(TextureUnit.Texture0);
    }

    // render frame events
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.BindVertexArray(_vertexArrayObject);

        _texture.Use(TextureUnit.Texture0);
        _shader.Use();

        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

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
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);

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