#define CG_Debug

using System;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Circulo : Objeto
  {
    public Circulo(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, 0.1D, new Ponto4D(0.5,0.5)) {
      
    }



    public Circulo(Objeto _paiRef, ref char _rotulo, double raio, Ponto4D ptoCentro) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.Points;
      PrimitivaTamanho = 1;

      // Calcular pontos do círculo
            int numSegments = 72;
            for (int i = 0; i < numSegments; i++)
            {
                double angle = 2 * Math.PI * i / numSegments;
                double x = raio * Math.Cos(angle) + ptoCentro.X;
                double y = raio * Math.Sin(angle) + ptoCentro.Y;
                PontosAdicionar(new Ponto4D(x, y));
            }

      // Sentido horário
      // base.PontosAdicionar(ptoCentro);
      // base.PontosAdicionar(new Ponto4D(ptoSupDir.X, ptoInfEsq.Y));
      // base.PontosAdicionar(ptoSupDir);
      // base.PontosAdicionar(new Ponto4D(ptoInfEsq.X, ptoSupDir.Y));
      Atualizar();
    }

    private void Atualizar()
    {

      base.ObjetoAtualizar();
    }

  }
}