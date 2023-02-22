﻿using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
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
        Flags = ContextFlags.Default,
    })
    {
    }

    //create vertex array ( 36 vertices (6 faces * 2 triangles * 3 vertices each))
    private readonly float[] _vertices =
    {
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,

        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,

        -0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 1.0f, 0.0f,

        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,

        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f
    };

    private readonly uint[] _indices =
    {
        0, 2, 3, // top right, bottom left, top left
        0, 1, 2 // top right, bottom right, bottom left
    };

    //create buffer
    private int _vertexBufferObject;
    private int _elementBufferObject;

    //create vertex array object
    private int _vertexArrayObject;

    //create shader
    private Shader _shader;

    //create texture
    private Texture _texture1;
    private Texture _texture2;

    // We create a double to hold how long has passed since the program was opened.
    private double _time;

    // Then, we create two matrices to hold our view and projection. They're initialized at the bottom of OnLoad.
    // The view matrix is what you might consider the "camera". It represents the current viewport in the window.
    private Matrix4 _view;

    // This represents how the vertices will be projected. It's hard to explain through comments,
    // so check out the web version for a good demonstration of what this does.
    private Matrix4 _projection;


    // when load window
    protected override void OnLoad()
    {
        base.OnLoad();

        //OpenGL stores all its depth information in a z-buffer, also known as a depth buffer
        GL.Enable(EnableCap.DepthTest);

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        // The vertex array object is now created and bound.
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        // The vertex buffer object is now created and bound.
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw);

        // The element buffer object is now created and bound.
        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
            BufferUsageHint.StaticDraw);

        // The shaders have been modified to include the texture coordinates, check them out after finishing the OnLoad function.
        _shader = new Shader("shader_vert.glsl", "shader_frag.glsl");
        _shader.Use();

        // The texture coordinates are now passed to the shader as an attribute, so we need to enable them and set the pointers.
        var vertexLocation = _shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
            3 * sizeof(float));

        _texture1 = new Texture("wall.png");
        _texture2 = new Texture("smile.png");

        _shader.SetInt("texture0", 0);
        _shader.SetInt("texture1", 1);
        
        // if transform don't change then set it in OnLoad
        Matrix4 rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90.0f)); // rotate 90 degrees
        Matrix4 scale = Matrix4.CreateScale(0.5f, 0.5f, 0.5f); // scale 0.5f
        Matrix4 trans = rotation * scale; // multiply rotation and scale
        _shader.SetMatrix4("transform", trans);

        // A bit farther away from us. (distance from camera to object)
        _view = Matrix4.CreateTranslation(0.0f, 0.0f, -2.0f);

        _projection =
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f,
                100.0f);
    }

    // render frame events
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        
        //clearing the color buffer, we can clear the depth buffer by specifying the ClearBufferMask.DepthBufferBit bit in the glClear function:
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // We add the time elapsed since last frame, times 4.0 to speed up animation, to the total amount of time passed.
        _time += 10.0 * e.Time;

        GL.BindVertexArray(_vertexArrayObject);

        _texture1.Use(TextureUnit.Texture0);
        _texture2.Use(TextureUnit.Texture1);
        _shader.Use();

        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

        // Finally, we have the model matrix. This determines the position of the model. (add rotation to model)
        var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));

        // We set the matrices in the shader.
        _shader.SetMatrix4("model", model);
        _shader.SetMatrix4("view", _view);
        _shader.SetMatrix4("projection", _projection);

        //draw behind the screen so swap Buffers
        SwapBuffers();

        base.OnRenderFrame(e);
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
        base.OnUpdateFrame(e);
        

        if (!IsFocused) // Check to see if the window is focused
        {
            return;
        }

        var input = KeyboardState;

        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }

    }
    
}