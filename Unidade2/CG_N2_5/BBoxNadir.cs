#define CG_Debug
#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.

using System;
using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
// using OpenTK.Mathematics;

namespace gcgcg
{
  internal class BBoxNadir : Objeto
  {
    private double menorX, menorY, menorZ, maiorX, maiorY, maiorZ;
    private readonly Ponto4D centro = new();

    Circulo circuloMaior = null;
    Circulo circuloMovel = null;
    Ponto4D pontoCentralMovel = null;

    Ponto pontoMovel = null;
    Quadrado quadrado = null;

    Shader _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");

    Shader _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
    private int _vertexBufferObject_bbox;
    private int _vertexArrayObject_bbox;

    private readonly Shader _shaderAmarela;

     private static Objeto mundo = null;

    private char rotuloAtual = '?';

    Ponto4D pontoSuperiorDir;
    Ponto4D pontoInferiorEsq;

    public BBoxNadir(Objeto _paiRef, ref char _rotulo) : base (_paiRef, ref _rotulo) 
    {

        mundo = _paiRef;
        rotuloAtual = _rotulo;

      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      // FIXME: falta deletar ..
      // GL.DeleteProgram(_shaderAmarela.Handle);
      // FIXME: deveria ser removido na classe BBox.
      //GL.DeleteBuffer(_vertexBufferObject_bbox);
      //GL.DeleteVertexArray(_vertexArrayObject_bbox);

      Ponto4D pontoCentral = new Ponto4D(0.30, 0.30);
      double raio = 0.30;

        #region Objeto: circulo
        circuloMaior = new Circulo(mundo, ref rotuloAtual, raio, pontoCentral);
        circuloMaior.ShaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
        #endregion

        double quarentaCincoGraus = 0.785398;
        double x = raio * Math.Cos(quarentaCincoGraus) + pontoCentral.X;
        double y = raio * Math.Sin(quarentaCincoGraus) + pontoCentral.Y;
        pontoSuperiorDir = new Ponto4D(x, y);
        pontoInferiorEsq = new Ponto4D(x - (2 * (pontoSuperiorDir.X - pontoCentral.X)), y - (2 * (pontoSuperiorDir.Y - pontoCentral.Y)));;
        
        //elementos alteráveis
        quadrado = new Quadrado (mundo, ref rotuloAtual, pontoInferiorEsq,pontoSuperiorDir);
        pontoCentralMovel = new Ponto4D(0.30, 0.30);
        pontoMovel = new Ponto(mundo, ref rotuloAtual, pontoCentralMovel)
        {
            PrimitivaTamanho = 10
        };
        circuloMovel = new Circulo(mundo, ref rotuloAtual, 0.1, pontoCentralMovel);
        

    }

    public void Atualizar(Transformacao4D matriz, List<Ponto4D> pontosLista)
    {
      Ponto4D pto = pontosLista[0];
      pto = matriz.MultiplicarPonto(pto);

      menorX = pto.X; menorY = pto.Y; menorZ = pto.Z;
      maiorX = pto.X; maiorY = pto.Y; maiorZ = pto.Z;

      for (var i = 1; i < pontosLista.Count; i++)
      {
        pto = pontosLista[i];
        pto = matriz.MultiplicarPonto(pto);
        Atualizar(pto);
      }

      ProcessarCentro();
    }

    public void AtualizarPosicao(char key)
    {
      Ponto4D novoPonto = null;

        switch (key) {
            case ' ':
                // contador +=1;
                // objetos[contador].ShaderObjeto = _shaderVermelha;
                // objetoSelecionado = objetos[contador];
                // if(contador > 0)
                // {
                //     objetos[contador - 1].ShaderObjeto = _shaderBranca;
                // }
                // else
                // {
                //     objetos[3].ShaderObjeto = _shaderBranca;
                // }
                // if(contador == 3)
                // {
                //     contador = -1;
                // }
                break;

            case 'C':
                Ponto4D pontoAnterior = pontoMovel.getPonto();
                double x = pontoAnterior.X;
                double y = pontoAnterior.Y;
                novoPonto = new Ponto4D (x, y + 0.05);
                bool saiuQuadrado = AtualizaQuadrado(novoPonto);
                
                if (saiuQuadrado) {
                  if(circuloMaior.VerificaSaidaCirculo(novoPonto))
                  {
                    return;
                  }
                  else{
                    pontoMovel.AtualizarPosicao(novoPonto);
                    circuloMovel.AtualizarPosicao(novoPonto);
                  }

                }
                else
                {
                  pontoMovel.AtualizarPosicao(novoPonto);
                  circuloMovel.AtualizarPosicao(novoPonto);
                };

                // PontosCurva(true);
                break;
            case 'B':
                Ponto4D pontoAnterior1 = pontoMovel.getPonto();
                double x1 = pontoAnterior1.X;
                double y1 = pontoAnterior1.Y;
                novoPonto = new Ponto4D (x1, y1 - 0.05);

                bool saiuQuadrado1 = AtualizaQuadrado(novoPonto);

                if (saiuQuadrado1) {
                  if(circuloMaior.VerificaSaidaCirculo(novoPonto))
                  {
                    return;
                  }
                  else{
                    pontoMovel.AtualizarPosicao(novoPonto);
                    circuloMovel.AtualizarPosicao(novoPonto);
                  }

                }
                else
                {
                  pontoMovel.AtualizarPosicao(novoPonto);
                  circuloMovel.AtualizarPosicao(novoPonto);
                };
                // PontosCurva(true);
                break;
            case 'E':
                Ponto4D pontoAnterior2 = pontoMovel.getPonto();
                double x2 = pontoAnterior2.X;
                double y2 = pontoAnterior2.Y;
                novoPonto = new Ponto4D (x2 - 0.05, y2);

                bool saiuQuadrado2 = AtualizaQuadrado(novoPonto);

                if (saiuQuadrado2) {
                  if(circuloMaior.VerificaSaidaCirculo(novoPonto))
                  {
                    return;
                  }
                  else{
                    pontoMovel.AtualizarPosicao(novoPonto);
                    circuloMovel.AtualizarPosicao(novoPonto);
                  }

                }
                else
                {
                  pontoMovel.AtualizarPosicao(novoPonto);
                  circuloMovel.AtualizarPosicao(novoPonto);
                };
                // PontosCurva(true);
                break;
            case 'D':
                Ponto4D pontoAnterior3 = pontoMovel.getPonto();
                double x3 = pontoAnterior3.X;
                double y3 = pontoAnterior3.Y;
                novoPonto = new Ponto4D (x3 + 0.05, y3);

                bool saiuQuadrado3 = AtualizaQuadrado(novoPonto);

                if (saiuQuadrado3) {
                  if(circuloMaior.VerificaSaidaCirculo(novoPonto))
                  {
                    return;
                  }
                  else{
                    pontoMovel.AtualizarPosicao(novoPonto);
                    circuloMovel.AtualizarPosicao(novoPonto);
                  }

                }
                else
                {
                  pontoMovel.AtualizarPosicao(novoPonto);
                  circuloMovel.AtualizarPosicao(novoPonto);
                };
                // PontosCurva(true);
                break;
            case 'R':
                // , , , 
                // objetos[0].AtualizarPosicao(new Ponto4D(-0.5, -0.5));
                // objetos[1].AtualizarPosicao(new Ponto4D( 0.5, -0.5));
                // objetos[2].AtualizarPosicao(new Ponto4D( 0.5,  0.5));
                // objetos[3].AtualizarPosicao(new Ponto4D(-0.5,  0.5));
                // PontosCurva(true);
                break;
            default:
                break;

        }

        // retaEsq.AtualizarPosicao(objetos[0].getPonto(), objetos[3].getPonto());
        // retaDir.AtualizarPosicao(objetos[1].getPonto(), objetos[2].getPonto());
        // retaSup.AtualizarPosicao(objetos[3].getPonto(), objetos[2].getPonto());
    }

    private bool AtualizaQuadrado(Ponto4D novoPonto)
    {
      double limiteSuperior = pontoSuperiorDir.Y;
      double limiteInferior = pontoInferiorEsq.Y;
      double limiteDireito = pontoSuperiorDir.X;
      double limiteEsquerdo = pontoInferiorEsq.X;
      if (novoPonto.Y > limiteSuperior || novoPonto.Y < limiteInferior || novoPonto.X > limiteDireito || novoPonto.X < limiteEsquerdo) {
        quadrado.ShaderObjeto = _shaderVermelha;
        return true;
      }
      else {
        quadrado.ShaderObjeto = _shaderBranca;
        return false;
      }
    }

    private void Atualizar(Ponto4D pto)
    {
      if (pto.X < menorX)
        menorX = pto.X;
      else
      {
        if (pto.X > maiorX) maiorX = pto.X;
      }
      if (pto.Y < menorY)
        menorY = pto.Y;
      else
      {
        if (pto.Y > maiorY) maiorY = pto.Y;
      }
      if (pto.Z < menorZ)
        menorZ = pto.Z;
      else
      {
        if (pto.Z > maiorZ) maiorZ = pto.Z;
      }
    }

    /// Calcula o ponto do centro da BBox.
    public void ProcessarCentro()
    {
      centro.X = (maiorX + menorX) / 2;
      centro.Y = (maiorY + menorY) / 2;
      centro.Z = (maiorZ + menorZ) / 2;
    }

    /// Verifica se um ponto está dentro da BBox.
    //FIXME: tem duas rotinas de dentro, aqui e na matematica
    public bool Dentro(Ponto4D pto)
    {
      if (pto.X >= ObterMenorX && pto.X <= ObterMaiorX &&
          pto.Y >= ObterMenorY && pto.Y <= ObterMaiorY &&
          pto.Z >= ObterMenorZ && pto.Z <= ObterMaiorZ)
      {
        return true;
      }
      return false;
    }

    /// Obter menor valor X da BBox.
    public double ObterMenorX => menorX;

    /// Obter menor valor Y da BBox.
    public double ObterMenorY => menorY;

    /// Obter menor valor Z da BBox.
    public double ObterMenorZ => menorZ;

    /// Obter maior valor X da BBox.
    public double ObterMaiorX => maiorX;

    /// Obter maior valor Y da BBox.
    public double ObterMaiorY => maiorY;

    /// Obter maior valor Z da BBox.
    public double ObterMaiorZ => maiorZ;

    /// Obter ponto do centro da BBox.
    public Ponto4D ObterCentro => centro;

#if CG_Gizmo
    public void Desenhar(Transformacao4D matrizGrafo)
    {

#if CG_OpenGL && !CG_DirectX

      float[] _bbox =
      {
        (float) menorX,   (float) menorY,   0.0f, // A - canto esquerdo/inferior
        (float) maiorX,   (float) menorY,   0.0f, // B - canto direito/inferior
        (float) maiorX,   (float) maiorY,   0.0f, // C - canto direito/superior
        (float) menorX,   (float) maiorY,   0.0f, // D - canto esquerdo/superior
        (float) centro.X, (float) centro.Y, 0.0f  // E - centro BBox
      };

      _vertexBufferObject_bbox = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_bbox);
      GL.BufferData(BufferTarget.ArrayBuffer, _bbox.Length * sizeof(float), _bbox, BufferUsageHint.StaticDraw);
      _vertexArrayObject_bbox = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_bbox);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);

      //      var transform = Matrix4.Identity;
      // matrizGrafo = matrizGrafo.MultiplicarMatriz(matriz);

      GL.BindVertexArray(_vertexArrayObject_bbox);
      // _shaderAmarela.SetMatrix4("transform", transform);
      _shaderAmarela.SetMatrix4("transform", matrizGrafo.ObterDadosOpenTK());

      _shaderAmarela.Use();
      GL.DrawArrays(PrimitiveType.LineLoop, 0, ((_bbox.Length - 1) / 3));   // desenha a BBox
      GL.PointSize(20);
      GL.DrawArrays(PrimitiveType.Points, ((_bbox.Length - 1) / 3), 1);     // desenha ponto centro BBox

#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "_____ BBox: \n";
      retorno += "menorX: " + menorX + " - maiorX: " + maiorX + "\n";
      retorno += "menorY: " + menorY + " - maiorY: " + maiorY + "\n";
      retorno += "menorZ: " + menorZ + " - maiorZ: " + maiorZ + "\n";
      retorno += "  centroX: " + centro.X + " - centroY: " + centro.Y + " - centroZ: " + centro.Z + "\n";
      retorno += "__________________________________ \n";
      return (retorno);
    }
#endif

  }
}