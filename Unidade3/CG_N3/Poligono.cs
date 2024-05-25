#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace gcgcg
{
  internal class Poligono : Objeto
  {
    public Poligono(Objeto _paiRef, ref char _rotulo, List<Ponto4D> pontosPoligono) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.LineLoop;
      PrimitivaTamanho = 1;
      base.pontosLista = pontosPoligono;
      Atualizar();
    }

    public void Atualizar()
    {

      base.ObjetoAtualizar();
    }

    public List<Ponto4D> GetPontos ()
    {
      return this.pontosLista;
    }
    public void RemovePonto(int indicePontoRemover)
    {
      this.pontosLista.RemoveAt(indicePontoRemover);
    }
    public PrimitiveType GetPrimitiva()
    {
      return this.PrimitivaTipo;
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Poligono _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return retorno;
    }
#endif

  }
}
