//TODO: testar se estes DEFINEs continuam funcionado
#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
// #define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
// using OpenTK.Mathematics;

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        private static Objeto mundo = null;

        private char rotuloAtual = '?';

        private Objeto objetoSelecionado = null;
        private Ponto ponto1 = null;
        private Ponto ponto2 = null;
        private Ponto ponto3 = null;
        private Ponto ponto4 = null;
        private SegReta reta1 = null;
        private SegReta reta2 = null;
        private SegReta reta3 = null;
        private SegReta reta4 = null;
        private SegReta eixo_x = null;
        private SegReta eixo_y = null;
        private Triangulo triangulo1 = null;
        private Triangulo triangulo2 = null;
        private Triangulo triangulo3 = null;
        private Retangulo retangulo = null;

        private readonly float[] _sruEixos =
        {
            0.0f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
            0.0f,  0.0f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
            0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f, /* Z+ */
        };

        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;

        private Shader _shaderVermelha;
        private Shader _shaderVerde;
        private Shader _shaderAzul;

        private Shader _shaderMagenta;

        private bool mouseMovtoPrimeiro = true;
        private Ponto4D mouseMovtoUltimo;

        private int contador = 0;

        private List<Ponto4D> pontos = new List<Ponto4D>();

        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
               : base(gameWindowSettings, nativeWindowSettings)
        {
            mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);

            #region Eixos: SRU  
            _vertexBufferObject_sruEixos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
            GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
            _vertexArrayObject_sruEixos = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
            _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
            #endregion

    
            pontos.Add(new Ponto4D(-0.5, -0.5));
            pontos.Add(new Ponto4D( 0.5, -0.5));
            pontos.Add(new Ponto4D(-0.5,  0.5));
            pontos.Add(new Ponto4D( 0.5,  0.5));
            pontos.Add(new Ponto4D( 0.0,  0.0));
            pontos.Add(new Ponto4D( 0.0,  0.5));
            pontos.Add(new Ponto4D( 0.5,  0.0));

            
            #region Objeto: ponto1 
            ponto1 = new Ponto(mundo, ref rotuloAtual, pontos[0]);
            ponto1.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
            ponto1.PrimitivaTamanho = 10;
            #endregion

            #region Objeto: ponto2 
            ponto2 = new Ponto(mundo, ref rotuloAtual, pontos[1]);
            ponto2.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
            ponto2.PrimitivaTamanho = 10;
            #endregion

            #region Objeto: ponto3 
            ponto3 = new Ponto(mundo, ref rotuloAtual, pontos[2]);
            ponto3.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
            ponto3.PrimitivaTamanho = 10;
            #endregion

            #region Objeto: ponto4 
            ponto4 = new Ponto(mundo, ref rotuloAtual, pontos[3]);
            ponto4.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
            ponto4.PrimitivaTamanho = 10;
            #endregion

#if CG_Privado

#endif

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

#if CG_Gizmo
            Sru3D();
#endif
            mundo.Desenhar();
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

            #region Teclado
            var input = KeyboardState;
            if (input.IsKeyPressed(Keys.Escape))
            {
                Close();
            }
            else
            {
                if (input.IsKeyPressed(Keys.Right))
                {

                }
                else
                {
                    if (input.IsKeyPressed(Keys.P))
                    {
                        Console.WriteLine(objetoSelecionado);
                    }
                    else
                    {
                        if (input.IsKeyPressed(Keys.Space))
                        {
                                contador ++;
                                if (contador == 0)
                                {

                                    #region Objeto: segmento de eixo_x      
                                    eixo_x = new SegReta(mundo, ref rotuloAtual, pontos[4], pontos[5]);
                                    eixo_x.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
                                    #endregion

                                    #region Objeto: segmento de eixo_y      
                                    eixo_y = new SegReta(mundo, ref rotuloAtual, pontos[4], pontos[6]);
                                    eixo_y.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
                                    #endregion

                                    #region Objeto: ponto1 
                                    ponto1 = new Ponto(mundo, ref rotuloAtual, pontos[0]);
                                    // ponto1.PrimitivaTipo = PrimitiveType.Points;
                                    ponto1.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    ponto1.PrimitivaTamanho = 10;
                                    #endregion

                                    #region Objeto: ponto2 
                                    ponto2 = new Ponto(mundo, ref rotuloAtual, pontos[1]);
                                    ponto2.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    ponto2.PrimitivaTamanho = 10;
                                    #endregion

                                    #region Objeto: ponto3 
                                    ponto3 = new Ponto(mundo, ref rotuloAtual, pontos[2]);
                                    ponto3.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    ponto3.PrimitivaTamanho = 10;
                                    #endregion

                                    #region Objeto: ponto4 
                                    ponto4 = new Ponto(mundo, ref rotuloAtual, pontos[3]);
                                    ponto4.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    ponto4.PrimitivaTamanho = 10;
                                    #endregion
                                }
                                else if (contador == 1)
                                {

                                    #region Objeto: ponto1 
                                    ponto1.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                    ponto2.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                    ponto3.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                    ponto4.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                    #endregion
                                    
                        
                                    #region Objeto: segmento de reta1  
                                    reta1 = new SegReta(mundo, ref rotuloAtual, pontos[0], pontos[1]);
                                    reta1.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    #endregion

                                    #region Objeto: segmento de reta2  
                                    reta2 = new SegReta(mundo, ref rotuloAtual, pontos[2], pontos[3]);
                                    reta2.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    #endregion
                                }
                                else if (contador == 2)
                                {
                                
                                    #region Objeto: segmento de reta3  
                                    reta3 = new SegReta(mundo, ref rotuloAtual, pontos[0], pontos[2]);
                                    reta3.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    #endregion

                                    #region Objeto: segmento de reta4  
                                    reta4 = new SegReta(mundo, ref rotuloAtual, pontos[1], pontos[3]);
                                    reta4.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    #endregion
                                }
                                else if (contador == 3) {
                                    reta3.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                }

                                else if (contador == 4)
                                {
                                    reta2.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                    reta1.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                    reta4.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                    #region Objeto: triângulo1  
                                    triangulo1 = new Triangulo(mundo, ref rotuloAtual, pontos[0], pontos[1], pontos[3]);
                                    triangulo1.PrimitivaTipo = PrimitiveType.TriangleFan;
                                    triangulo1.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    #endregion
                                }
                                else if (contador == 5)
                                {
                                    #region Objeto: triângulo2  
                                    triangulo2 = new Triangulo(mundo, ref rotuloAtual, pontos[0], pontos[1], pontos[3]);
                                    triangulo2.PrimitivaTipo = PrimitiveType.TriangleFan;
                                    triangulo2.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    #endregion

                                    #region Objeto: triângulo3  
                                    triangulo3 = new Triangulo(mundo, ref rotuloAtual, pontos[1], pontos[2], pontos[3]);
                                    triangulo3.PrimitivaTipo = PrimitiveType.TriangleFan;
                                    triangulo3.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    #endregion

                                }
                                else if (contador == 6) {
                                    #region Objeto: retângulo  
                                    retangulo = new Retangulo(mundo, ref rotuloAtual, pontos[0], pontos[3]);
                                    retangulo.PrimitivaTipo = PrimitiveType.TriangleFan;
                                    retangulo.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
                                    #endregion
                                    
                                    
                                }
                                else if (contador == 7) {
                                    triangulo1.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                    triangulo2.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                    triangulo3.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                    retangulo.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderInviseble.frag");
                                    contador = -1;           
                                }
                        }
                        else
                        {
                            if (input.IsKeyPressed(Keys.C))
                            {
                                mundo.OnUnload();
                                objetoSelecionado = null;
                            }
                        }
                    }
                }
            }
            #endregion


            //FIXME: o movimento do mouse em relação ao eixo X está certo. Mas tem um erro no eixo Y,,, aumentar o valor do Y aumenta o erro.
            if (input.IsKeyDown(Keys.LeftShift))
            {
                
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {

            }

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUnload()
        {
            mundo.OnUnload();
            GC.Collect();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject_sruEixos);
            GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

            GL.DeleteProgram(_shaderVermelha.Handle);
            GL.DeleteProgram(_shaderVerde.Handle);
            GL.DeleteProgram(_shaderAzul.Handle);
            GL.DeleteProgram(_shaderMagenta.Handle);

            base.OnUnload();

        }

#if CG_Gizmo
        private void Sru3D()
        {
#if CG_OpenGL && !CG_DirectX
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            // EixoX
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            // EixoY
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);
            // EixoZ
            _shaderAzul.Use();
            GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
        }
#endif

    }
}
