using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace gcgcg;

public class CuboBasicLighting
{
    private readonly float[] _vertices =
    {
        // Position          Normal
        -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, -1.0f, // Front face
        1.0f, -1.0f, -1.0f, 0.0f, 0.0f, -1.0f,
        1.0f, 1.0f, -1.0f, 0.0f, 0.0f, -1.0f,
        1.0f, 1.0f, -1.0f, 0.0f, 0.0f, -1.0f,
        -1.0f, 1.0f, -1.0f, 0.0f, 0.0f, -1.0f,
        -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, -1.0f,

        -1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f, // Back face
        1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        -1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        -1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f,

        -1.0f, 1.0f, 1.0f, -1.0f, 0.0f, 0.0f, // Left face
        -1.0f, 1.0f, -1.0f, -1.0f, 0.0f, 0.0f,
        -1.0f, -1.0f, -1.0f, -1.0f, 0.0f, 0.0f,
        -1.0f, -1.0f, -1.0f, -1.0f, 0.0f, 0.0f,
        -1.0f, -1.0f, 1.0f, -1.0f, 0.0f, 0.0f,
        -1.0f, 1.0f, 1.0f, -1.0f, 0.0f, 0.0f,

        1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, // Right face
        1.0f, 1.0f, -1.0f, 1.0f, 0.0f, 0.0f,
        1.0f, -1.0f, -1.0f, 1.0f, 0.0f, 0.0f,
        1.0f, -1.0f, -1.0f, 1.0f, 0.0f, 0.0f,
        1.0f, -1.0f, 1.0f, 1.0f, 0.0f, 0.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f,

        -1.0f, -1.0f, -1.0f, 0.0f, -1.0f, 0.0f, // Bottom face
        1.0f, -1.0f, -1.0f, 0.0f, -1.0f, 0.0f,
        1.0f, -1.0f, 1.0f, 0.0f, -1.0f, 0.0f,
        1.0f, -1.0f, 1.0f, 0.0f, -1.0f, 0.0f,
        -1.0f, -1.0f, 1.0f, 0.0f, -1.0f, 0.0f,
        -1.0f, -1.0f, -1.0f, 0.0f, -1.0f, 0.0f,

        -1.0f, 1.0f, -1.0f, 0.0f, 1.0f, 0.0f, // Top face
        1.0f, 1.0f, -1.0f, 0.0f, 1.0f, 0.0f,
        1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        -1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        -1.0f, 1.0f, -1.0f, 0.0f, 1.0f, 0.0f
    };

    private Shader _shader;
    private int _vertexBufferObject;
    private int _vertexArrayObject;

    public CuboBasicLighting(Shader shader)
    {
        _shader = shader;
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw);
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        var positionLocation = _shader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

        var normalLocation = _shader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float),
            3 * sizeof(float));
    }

    public void Atualizar(Vector3 posicaoLuz, Camera camera)
    {
        GL.BindVertexArray(_vertexArrayObject);
        _shader.Use();
        _shader.SetMatrix4("model", Matrix4.Identity);
        _shader.SetMatrix4("view", camera.GetViewMatrix());
        _shader.SetMatrix4("projection", camera.GetProjectionMatrix());

        _shader.SetVector3("objectColor", new Vector3(1.0f, 0.0f, 0.0f));
        _shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
        _shader.SetVector3("lightPos", posicaoLuz);
        _shader.SetVector3("viewPos", camera.Position);
        
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }
}