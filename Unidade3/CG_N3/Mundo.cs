﻿#define CG_DEBUG
#define CG_Gizmo      
#define CG_OpenGL      
// #define CG_OpenTK
// #define CG_DirectX      
#define CG_Privado  

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static Objeto mundo = null;

    private char rotuloAtual = '?';
    private Objeto objetoSelecionado = null;

    private Poligono poligonoEmConstrucao = null;
    List<Ponto4D> pontosPoligonoEmConstrucao = new List<Ponto4D>();
    List<Poligono> poligonos = new List<Poligono>();
    int objetoSelecionadoPosicao = 0;
    private bool inclusaoFilhoAtiva = false;

    private readonly float[] _sruEixos =
    [
       0.0f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f,  0.0f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f,  0.0f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
    ];

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private int _vertexBufferObject_bbox;
    private int _vertexArrayObject_bbox;

    private Shader _shaderBranca;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      Utilitario.Diretivas();
#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      #region Cores
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      #endregion

      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion

      // #region Objeto: polígono qualquer  
      // List<Ponto4D> pontosPoligonoBandeira = new List<Ponto4D>();
      // pontosPoligonoBandeira.Add(new Ponto4D(0.25, 0.25));  // A = (0.25, 0.25)
      // pontosPoligonoBandeira.Add(new Ponto4D(0.75, 0.25));  // B = (0.75, 0.25)
      // pontosPoligonoBandeira.Add(new Ponto4D(0.75, 0.75));  // C = (0.75, 0.75)
      // pontosPoligonoBandeira.Add(new Ponto4D(0.50, 0.50));  // D = (0.50, 0.50)
      // pontosPoligonoBandeira.Add(new Ponto4D(0.25, 0.75));  // E = (0.25, 0.75)
      // objetoSelecionado = new Poligono(mundo, ref rotuloAtual, pontosPoligonoBandeira);
      // #endregion
      // #region declara um objeto filho ao polígono
      // List<Ponto4D> pontosPoligonoTriangulo = new List<Ponto4D>();
      // pontosPoligonoTriangulo.Add(new Ponto4D(0.50, 0.50)); // F = (0.50, 0.50)
      // pontosPoligonoTriangulo.Add(new Ponto4D(0.75, 0.75)); // G = (0.75, 0.75)
      // pontosPoligonoTriangulo.Add(new Ponto4D(0.25, 0.75)); // H = (0.25, 0.75)
      // objetoSelecionado = new Poligono(objetoSelecionado, ref rotuloAtual, pontosPoligonoTriangulo);
      // #endregion

      
    }

    private Ponto4D GetPontoMouse()
    {
          int janelaLargura = ClientSize.X;
          int janelaAltura = ClientSize.Y;
          Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
          Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);
          return sruPonto;
    }

    private double GetDistancia(Ponto4D p1, Ponto4D p2)
    {
      double dx = p1.X - p2.X;
      double dy = p1.Y - p2.Y;
      return Math.Sqrt(dx * dx + dy * dy); //obs: como o pbjetivo é apenas saber qual está mais próximo, daria para dispensar a extração da raiz quadrada
    }

    private int GetIndicePontoMaisProximo(List<Ponto4D> pontos, Ponto4D pontoMouse)
    {
      int tamanhoListaPontos = pontos.Count;
      if (pontos == null || tamanhoListaPontos == 0)
        return -1;
      
      int pontoMaisProximoIndice = 0;
      double distanciaMaisProxima = GetDistancia(pontos[0], pontoMouse);
      for (int i = 0; i < tamanhoListaPontos; i++)
      {
        double distancia = GetDistancia(pontos[i], pontoMouse);
        if (distancia < distanciaMaisProxima)
        {
          distanciaMaisProxima = distancia;
          pontoMaisProximoIndice = i;
        }
      }
      return pontoMaisProximoIndice;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      mundo.Desenhar(new Transformacao4D());

#if CG_Gizmo      
      Gizmo_Sru3D();
      Gizmo_BBox();
#endif
      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
      #region Teclado
      var estadoTeclado = KeyboardState;
      if (estadoTeclado.IsKeyDown(Keys.Escape))
        Close();
      if (estadoTeclado.IsKeyPressed(Keys.Space))
      {
        objetoSelecionado ??= mundo;
        objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
        inclusaoFilhoAtiva = true;
      }
      if (estadoTeclado.IsKeyPressed(Keys.Enter))
      {
        if (inclusaoFilhoAtiva) 
        {
          // objetoSelecionado.FilhoAdicionar(poligonoEmConstrucao);
          // objetoSelecionado.ObjetoAtualizar();
          pontosPoligonoEmConstrucao = new List<Ponto4D>();
          poligonoEmConstrucao = null;
          inclusaoFilhoAtiva = false;
        }
        else{
          poligonos.Add(poligonoEmConstrucao);
          pontosPoligonoEmConstrucao = new List<Ponto4D>();
          objetoSelecionado = poligonoEmConstrucao;
          poligonoEmConstrucao = null;
        }
      }
      if (estadoTeclado.IsKeyPressed(Keys.D))
      {
        if (objetoSelecionado != null) 
        {
          poligonos.RemoveAt(objetoSelecionadoPosicao);
          objetoSelecionado.PontosLimpar();
          objetoSelecionado = null;
        }
      }
      if (estadoTeclado.IsKeyDown(Keys.V) && objetoSelecionado != null)
      {
        Poligono poligonoSelecionado = (Poligono) objetoSelecionado;
        int pontoMaisProximoMouse = GetIndicePontoMaisProximo (poligonoSelecionado.GetPontos(), GetPontoMouse());
        poligonoSelecionado.PontosAlterar(GetPontoMouse(), pontoMaisProximoMouse);
        poligonoSelecionado.Atualizar();
        objetoSelecionado = poligonoSelecionado;
      }
      if (estadoTeclado.IsKeyPressed(Keys.E) && objetoSelecionado != null)
      {
        Poligono poligonoSelecionado = (Poligono) objetoSelecionado;
        int pontoMaisProximoMouse = GetIndicePontoMaisProximo (poligonoSelecionado.GetPontos(), GetPontoMouse());
        poligonoSelecionado.RemovePonto(pontoMaisProximoMouse);
        poligonoSelecionado.Atualizar();
        objetoSelecionado = poligonoSelecionado;
      }
      if (estadoTeclado.IsKeyPressed(Keys.P) && objetoSelecionado != null)
      {
        Poligono poligonoSelecionado = (Poligono) objetoSelecionado;
        if (poligonoSelecionado.GetPrimitiva().Equals( PrimitiveType.LineLoop))
        {
          poligonoSelecionado.PrimitivaTipo = PrimitiveType.LineStrip;
        }
        else
        {
          poligonoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
        }
      }
      if (estadoTeclado.IsKeyPressed(Keys.R))
      {
        objetoSelecionado.ShaderObjeto = _shaderVermelha;
      }
      if (estadoTeclado.IsKeyPressed(Keys.G))
      {
        objetoSelecionado.ShaderObjeto = _shaderVerde;
      }
      if (estadoTeclado.IsKeyPressed(Keys.B))
      {
        objetoSelecionado.ShaderObjeto = _shaderAzul;
      }
      if (estadoTeclado.IsKeyPressed(Keys.C))
        mundo.GrafocenaImprimir("");

      if (estadoTeclado.IsKeyPressed(Keys.P) && objetoSelecionado != null)
        Console.WriteLine(objetoSelecionado.ToString());
      if (estadoTeclado.IsKeyPressed(Keys.M) && objetoSelecionado != null)
        objetoSelecionado.MatrizImprimir();
      if (estadoTeclado.IsKeyPressed(Keys.I) && objetoSelecionado != null)
        objetoSelecionado.MatrizAtribuirIdentidade();
      if (estadoTeclado.IsKeyPressed(Keys.Left) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Up) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Down) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
      // if (estadoTeclado.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null)
      //   objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
      // if (estadoTeclado.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null)
      //   objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
      if (estadoTeclado.IsKeyPressed(Keys.Home) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(0.7, 0.7, 0.7);
      if (estadoTeclado.IsKeyPressed(Keys.End) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(1.5, 1.5, 1.5);
      // if (estadoTeclado.IsKeyPressed(Keys.D1) && objetoSelecionado != null)
      //   objetoSelecionado.MatrizRotacao(10);
      // if (estadoTeclado.IsKeyPressed(Keys.D2) && objetoSelecionado != null)
      //   objetoSelecionado.MatrizRotacao(-10);
      if (estadoTeclado.IsKeyPressed(Keys.D3) && objetoSelecionado != null)
        // objetoSelecionado.MatrizRotacao(10);
        objetoSelecionado.MatrizRotacaoZBBox(10);
      if (estadoTeclado.IsKeyPressed(Keys.D4) && objetoSelecionado != null)
        // objetoSelecionado.MatrizRotacao(-10);
        objetoSelecionado.MatrizRotacaoZBBox(-10);
      #endregion
      
      #region  Mouse

      if (MouseState.IsButtonPressed(MouseButton.Left))
      {
        Console.WriteLine("MouseState.IsButtonPressed(MouseButton.Left)");
        Console.WriteLine("__ Valores do Espaço de Tela");
        Console.WriteLine("Vector2 mousePosition: " + MousePosition);
        Console.WriteLine("Vector2i windowSize: " + ClientSize);
        int nrPoligonos = poligonos.Count;
        for (int i = 0; i < nrPoligonos; i++) {
          if (poligonos[i].Bbox().Dentro(GetPontoMouse()))
          {
            bool dentroScanLine = ScanLine(poligonos[i].GetPontos(), GetPontoMouse());
            if (dentroScanLine){
              objetoSelecionado = poligonos[i];
              objetoSelecionadoPosicao = i;
            break;
            }
          }
          else
          {
            objetoSelecionado = null;
          }
        }
      }

      // if (MouseState.IsButtonDown(MouseButton.Right) && objetoSelecionado != null)
      // {
      //   Console.WriteLine("MouseState.IsButtonDown(MouseButton.Right)");
      // }
      if (MouseState.IsButtonDown(MouseButton.Right) && poligonoEmConstrucao != null)
      {
        Console.WriteLine("MouseState.IsButtonDown(MouseButton.Right)");
        if (pontosPoligonoEmConstrucao.Count == 2)
        {
          pontosPoligonoEmConstrucao[1] = GetPontoMouse();
          poligonoEmConstrucao.Atualizar();
        }
      }
      if (MouseState.IsButtonPressed(MouseButton.Right))
      {
        if (pontosPoligonoEmConstrucao.Count == 0) {
        pontosPoligonoEmConstrucao.Add(GetPontoMouse());
        pontosPoligonoEmConstrucao.Add(GetPontoMouse());
          if (inclusaoFilhoAtiva)
          {
            poligonoEmConstrucao = new Poligono(objetoSelecionado, ref rotuloAtual, pontosPoligonoEmConstrucao);
          }
          else
          {
            poligonoEmConstrucao = new Poligono(mundo, ref rotuloAtual, pontosPoligonoEmConstrucao);
          }
        }
        else
        {
          pontosPoligonoEmConstrucao.Add(GetPontoMouse());
          poligonoEmConstrucao.Atualizar();
        }
      }
      if (MouseState.IsButtonReleased(MouseButton.Right))
      {
        Console.WriteLine("MouseState.IsButtonReleased(MouseButton.Right)");
      }

      #endregion

    }

        protected bool ScanLine(List<Ponto4D> vertices, Ponto4D ponto4D)
        {
          int nrVertices = vertices.Count;
          if (nrVertices < 3)
          {
            return false;
          }
          bool dentro = false;
          for (int i = 0, j = nrVertices - 1; i < nrVertices; j = i++)
          {
            if (((vertices[i].Y > ponto4D.Y) != (vertices[j].Y > ponto4D.Y)) &&
                (ponto4D.X < (vertices[j].X - vertices[i].X) * (ponto4D.Y - vertices[i].Y) / (vertices[j].Y - vertices[i].Y) + vertices[i].X))
            {
                dentro = !dentro;
            } 
          }
          return dentro;
        }

        protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
      GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnUnload()
    {
      mundo.OnUnload();

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

      GL.DeleteBuffer(_vertexBufferObject_bbox);
      GL.DeleteVertexArray(_vertexArrayObject_bbox);

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);

      base.OnUnload();
    }

#if CG_Gizmo
    private void Gizmo_Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      var transform = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("transform", transform);
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("transform", transform);
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("transform", transform);
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

#if CG_Gizmo
    private void Gizmo_BBox()   //FIXME: não é atualizada com as transformações globais
    {
      if (objetoSelecionado != null)
      {

#if CG_OpenGL && !CG_DirectX

        float[] _bbox =
        {
        (float) objetoSelecionado.Bbox().ObterMenorX, (float) objetoSelecionado.Bbox().ObterMenorY, 0.0f, // A
        (float) objetoSelecionado.Bbox().ObterMaiorX, (float) objetoSelecionado.Bbox().ObterMenorY, 0.0f, // B
        (float) objetoSelecionado.Bbox().ObterMaiorX, (float) objetoSelecionado.Bbox().ObterMaiorY, 0.0f, // C
        (float) objetoSelecionado.Bbox().ObterMenorX, (float) objetoSelecionado.Bbox().ObterMaiorY, 0.0f  // D
      };

        _vertexBufferObject_bbox = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_bbox);
        GL.BufferData(BufferTarget.ArrayBuffer, _bbox.Length * sizeof(float), _bbox, BufferUsageHint.StaticDraw);
        _vertexArrayObject_bbox = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject_bbox);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        var transform = Matrix4.Identity;
        GL.BindVertexArray(_vertexArrayObject_bbox);
        _shaderAmarela.SetMatrix4("transform", transform);
        _shaderAmarela.Use();
        GL.DrawArrays(PrimitiveType.LineLoop, 0, (_bbox.Length / 3));

#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
      }
    }
#endif    

  }
}
