#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Triangulo : Objeto
  {
    public Triangulo(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, new Ponto4D(-0.5,-0.5), new Ponto4D(0.5,0.5), new Ponto4D(0.0, 0.0)) {
      
    }

    public Triangulo(Objeto _paiRef, ref char _rotulo, Ponto4D ptoA, Ponto4D ptoB, Ponto4D ptoC) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.Points;
      PrimitivaTamanho = 1;

      // Sentido horário
      base.PontosAdicionar(ptoA);
      base.PontosAdicionar(new Ponto4D(ptoA.X, ptoA.Y));
      base.PontosAdicionar(ptoB);
      base.PontosAdicionar(new Ponto4D(ptoB.X, ptoB.Y));
      base.PontosAdicionar(ptoC);
      base.PontosAdicionar(new Ponto4D(ptoC.X, ptoC.Y));
      Atualizar();
    }

    private void Atualizar()
    {

      base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Retangulo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
