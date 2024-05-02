#define CG_Debug

using System;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class SrPalito : Objeto
  {

        private Ponto4D pontoIni;
        private Ponto4D pontoFim;
        private double angulo = 0.7854;
        

        public SrPalito(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, new Ponto4D( 0.0, 0.0), new Ponto4D(0.3, 0.3))
    {
    }

    public SrPalito(Objeto _paiRef, ref char _rotulo, Ponto4D ptoIni, Ponto4D ptoFim) : base(_paiRef, ref _rotulo)
    {
        pontoIni = ptoIni;
        pontoFim = ptoFim;

        PrimitivaTipo = PrimitiveType.LineStrip;
        PrimitivaTamanho = 1;

        base.PontosAdicionar(ptoIni);
        base.PontosAdicionar(ptoFim);
        Atualizar();
    }

    private void Atualizar()
    {
      base.ObjetoAtualizar();
    }

    public void AtualizarPosicao(char key)
        {
            Ponto4D novoPontoIni = null;
            Ponto4D novoPontoFim = null;

            switch (key) {
                case 'A':

                    double deltaX = pontoFim.X - pontoIni.X;
                    double deltaY = pontoFim.Y - pontoIni.Y;
                    double comprimentoAtual = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    double direcaoX = deltaX / comprimentoAtual;
                    double direcaoY = deltaY / comprimentoAtual;
                    double novoPontoFimX = pontoIni.X + (direcaoX * (comprimentoAtual - 0.025));
                    double novoPontoFimY = pontoIni.Y + (direcaoY * (comprimentoAtual - 0.025));
                    novoPontoFim = new Ponto4D(novoPontoFimX, novoPontoFimY);
                    break;
                case 'S':
                    double deltaX_ = pontoFim.X - pontoIni.X;
                    double deltaY_ = pontoFim.Y - pontoIni.Y;
                    double comprimentoAtual_ = Math.Sqrt(deltaX_ * deltaX_ + deltaY_ * deltaY_);
                    double direcaoX_ = deltaX_ / comprimentoAtual_;
                    double direcaoY_ = deltaY_ / comprimentoAtual_;
                    double novoPontoFimX_ = pontoIni.X + (direcaoX_ * (comprimentoAtual_ + 0.025));
                    double novoPontoFimY_ = pontoIni.Y + (direcaoY_ * (comprimentoAtual_ + 0.025));
                    novoPontoFim = new Ponto4D(novoPontoFimX_, novoPontoFimY_);
                    break;
                case 'X':
                    angulo += 0.2;
                    double raio = Math.Sqrt(Math.Pow(pontoFim.X - pontoIni.X, 2d) + Math.Pow(pontoFim.Y - pontoIni.Y, 2d));
                    double x = raio * Math.Cos(angulo) + pontoIni.X;
                    double y = raio * Math.Sin(angulo) + pontoIni.Y;
                    novoPontoFim = new Ponto4D(x, y);
                    break;
                case 'Z':
                    angulo -= 0.2;
                    double raio1 = Math.Sqrt(Math.Pow(pontoFim.X - pontoIni.X, 2d) + Math.Pow(pontoFim.Y - pontoIni.Y, 2d));
                    double x1 = raio1 * Math.Cos(angulo) + pontoIni.X;
                    double y1 = raio1 * Math.Sin(angulo) + pontoIni.Y;
                    novoPontoFim = new Ponto4D(x1, y1);
                    break;
                case 'Q':
                    novoPontoIni = new Ponto4D(pontoIni.X - 0.025, pontoIni.Y);
                    novoPontoFim = new Ponto4D(pontoFim.X - 0.025, pontoFim.Y);
                    base.PontosAlterar(novoPontoIni, base.pontosLista.IndexOf(pontoIni));
                    pontoIni = novoPontoIni;
                    break;
                case 'W':
                    novoPontoIni = new Ponto4D(pontoIni.X + 0.025, pontoIni.Y);
                    novoPontoFim = new Ponto4D(pontoFim.X + 0.025, pontoFim.Y);
                    base.PontosAlterar(novoPontoIni, base.pontosLista.IndexOf(pontoIni));
                    pontoIni = novoPontoIni;
                    break;
                default:
                    break;

            }
            base.PontosAlterar(novoPontoFim, base.pontosLista.IndexOf(pontoFim));
            
            pontoFim = novoPontoFim;
            Atualizar();
        }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto SrPalito _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return retorno;
    }
#endif

  }
}
