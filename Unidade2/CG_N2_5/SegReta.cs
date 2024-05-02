#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class SegReta : Objeto
  {
    private Ponto4D pontoInicial = null;
    private Ponto4D pontoFinal = null;
    public SegReta(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5))
    {

    }

    public SegReta(Objeto _paiRef, ref char _rotulo, Ponto4D ptoIni, Ponto4D ptoFim) : base(_paiRef, ref _rotulo)
    {
      pontoInicial = ptoFim;
      pontoFinal = ptoFim;
      PrimitivaTipo = PrimitiveType.Lines;
      PrimitivaTamanho = 1;

      base.PontosAdicionar(ptoIni);
      base.PontosAdicionar(ptoFim);
      Atualizar();
    }

    public void AtualizarPosicao(Ponto4D ptoIni, Ponto4D ptoFim)
    {
      // ponto = pto;
  
      // base.PontosAlterar(ptoIni, base.pontosLista.IndexOf(pontoInicial));
      // base.PontosAlterar(ptoFim, base.pontosLista.IndexOf(pontoFinal));
      // pontoInicial = ptoIni;
      // pontoFinal = ptoFim;
      base.PontosAlterar(ptoIni, 0);
      base.PontosAlterar(ptoFim, 1);
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
      retorno = "__ Objeto SegReta _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return retorno;
    }
#endif

  }
}
