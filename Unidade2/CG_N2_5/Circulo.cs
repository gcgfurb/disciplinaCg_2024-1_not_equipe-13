#define CG_Debug

using System;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Circulo : Objeto
  {
    double raioCirculo;
    Ponto4D pontoCentral = null;
    double raioXraio;
    public Circulo(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, 0.1D, new Ponto4D(0.5,0.5)) {
      
    }



    public Circulo(Objeto _paiRef, ref char _rotulo, double raio, Ponto4D ptoCentro) : base(_paiRef, ref _rotulo)
    {
      
      PrimitivaTipo = PrimitiveType.Points;
      PrimitivaTamanho = 1;
      raioCirculo = raio;
      pontoCentral = ptoCentro;
      raioXraio = raio * raio;

      // Calcular pontos do círculo
            int numSegments = (int) (raioCirculo * 10 * 360);
            for (int i = 0; i < numSegments; i++)
            {
                double angle = 2 * Math.PI * i / numSegments;
                double x = raioCirculo * Math.Cos(angle) + ptoCentro.X;
                double y = raioCirculo * Math.Sin(angle) + ptoCentro.Y;
                PontosAdicionar(new Ponto4D(x, y));
            }

      // Sentido horário
      // base.PontosAdicionar(ptoCentro);
      // base.PontosAdicionar(new Ponto4D(ptoSupDir.X, ptoInfEsq.Y));
      // base.PontosAdicionar(ptoSupDir);
      // base.PontosAdicionar(new Ponto4D(ptoInfEsq.X, ptoSupDir.Y));
      Atualizar();
    }
    public bool VerificaSaidaCirculo(Ponto4D pontoMovel)
    {
      double deltaX_ = pontoMovel.X - pontoCentral.X;
      double deltaY_ = pontoMovel.Y - pontoCentral.Y;
      double comprimentoAtual_ = deltaX_ * deltaX_ + deltaY_ * deltaY_;
      return comprimentoAtual_ > raioXraio;
    }

    public void AtualizarPosicao(Ponto4D ptoCentro)
    {
      int numSegments = (int) (raioCirculo * 10 * 360);
      for (int i = 0; i < numSegments; i++)
      {
          double angle = 2 * Math.PI * i / numSegments;
          double x = raioCirculo * Math.Cos(angle) + ptoCentro.X;
          double y = raioCirculo * Math.Sin(angle) + ptoCentro.Y;
          PontosAlterar(new Ponto4D(x, y), i);
      }
      Atualizar();
    }

    private void Atualizar()
    {

      base.ObjetoAtualizar();
    }

  }
}