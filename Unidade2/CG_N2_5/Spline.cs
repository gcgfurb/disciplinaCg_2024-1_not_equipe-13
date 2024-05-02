#define CG_Debug

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Spline : Objeto
  {

        // private Ponto4D pontoIni;
        // private Ponto4D pontoFim;
        // private double angulo = 0.7854;

        private Ponto4D esqAcima;
        private Ponto4D esqAbaixo;
        private Ponto4D dirAcima;
        private Ponto4D dirAbaixo;
        // private SegReta retaEsq = null;

        private Ponto pontoEsqAcima  = null;
        private Ponto pontoEsqAbaixo  = null;
        private Ponto pontoDirAcima = null;
        private Ponto pontoDirAbaixo = null;

        private Shader _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
        private Shader _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");

        private SegReta retaEsq = null;   
        private SegReta retaDir = null;     
        private SegReta retaSup = null; 

        private int contador = 0;

        private Ponto objetoSelecionado = null;

        List<Ponto> objetos = new List<Ponto>();
        List<Ponto> pontosCurvaBezier = new List<Ponto>();
        List<SegReta> retasCurvaBezier = new List<SegReta>();
        private static Objeto mundo = null;

        private char rotuloAtual = '?';
        

        public Spline(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, new Ponto4D(-0.5, 0.5), new Ponto4D(-0.5, -0.5), new Ponto4D( 0.5, 0.5), new Ponto4D( 0.5, -0.5))
    {
    }

    public Spline(Objeto _paiRef, ref char _rotulo, Ponto4D esqAcima, Ponto4D esqAbaixo, Ponto4D dirAcima, Ponto4D dirAbaixo) : base(_paiRef, ref _rotulo)
    {
        mundo = _paiRef;
        rotuloAtual = _rotulo;
        esqAcima = esqAcima;
        esqAbaixo = esqAbaixo;
        dirAcima = dirAcima;
        dirAbaixo = dirAbaixo;

        // #region Objeto: ponto  
        pontoEsqAcima = new Ponto(mundo, ref rotuloAtual, esqAcima);
        // objetos[0] = pontoEsqAcima;
        pontoEsqAbaixo = new Ponto(mundo, ref rotuloAtual, esqAbaixo);
        // objetos[1] = pontoEsqAbaixo;
        pontoDirAcima = new Ponto(mundo, ref rotuloAtual, dirAcima);
        // objetos[2] = pontoDirAcima;
        pontoDirAbaixo = new Ponto(mundo, ref rotuloAtual, dirAbaixo);
        // pontoDirAbaixo.ShaderObjeto = _shaderVermelha;new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
        // objetos[3] = pontoDirAbaixo;
        objetos.Add(pontoEsqAbaixo);
        objetos.Add(pontoDirAbaixo);
        objetos.Add(pontoDirAcima);
        objetos.Add(pontoEsqAcima);
        objetoSelecionado = objetos[0];
        objetoSelecionado.ShaderObjeto = _shaderVermelha;

       

        retaEsq = new SegReta(mundo, ref rotuloAtual, esqAbaixo, esqAcima);
        retaDir = new SegReta(mundo, ref rotuloAtual, dirAbaixo, dirAcima);       
        retaSup = new SegReta(mundo, ref rotuloAtual, esqAcima, dirAcima);

        PontosCurva(false);


    }

    public void AtualizarPosicao(char key)
    {
        Ponto4D novoPonto = null;

        switch (key) {
            case ' ':
                contador +=1;
                objetos[contador].ShaderObjeto = _shaderVermelha;
                objetoSelecionado = objetos[contador];
                if(contador > 0)
                {
                    objetos[contador - 1].ShaderObjeto = _shaderBranca;
                }
                else
                {
                    objetos[3].ShaderObjeto = _shaderBranca;
                }
                if(contador == 3)
                {
                    contador = -1;
                }
                break;

            case 'C':
                Ponto4D pontoAnterior = objetoSelecionado.getPonto();
                double x = pontoAnterior.X;
                double y = pontoAnterior.Y;
                novoPonto = new Ponto4D (x, y + 0.05);
                objetoSelecionado.AtualizarPosicao(novoPonto);
                PontosCurva(true);
                break;
            case 'B':
                Ponto4D pontoAnterior1 = objetoSelecionado.getPonto();
                double x1 = pontoAnterior1.X;
                double y1 = pontoAnterior1.Y;
                novoPonto = new Ponto4D (x1, y1 - 0.05);
                objetoSelecionado.AtualizarPosicao(novoPonto);
                PontosCurva(true);
                break;
            case 'E':
                Ponto4D pontoAnterior2 = objetoSelecionado.getPonto();
                double x2 = pontoAnterior2.X;
                double y2 = pontoAnterior2.Y;
                novoPonto = new Ponto4D (x2 - 0.05, y2);
                objetoSelecionado.AtualizarPosicao(novoPonto);
                PontosCurva(true);
                break;
            case 'D':
                Ponto4D pontoAnterior3 = objetoSelecionado.getPonto();
                double x3 = pontoAnterior3.X;
                double y3 = pontoAnterior3.Y;
                novoPonto = new Ponto4D (x3 + 0.05, y3);
                objetoSelecionado.AtualizarPosicao(novoPonto);
                PontosCurva(true);
                break;
            case 'R':
                // , , , 
                objetos[0].AtualizarPosicao(new Ponto4D(-0.5, -0.5));
                objetos[1].AtualizarPosicao(new Ponto4D( 0.5, -0.5));
                objetos[2].AtualizarPosicao(new Ponto4D( 0.5,  0.5));
                objetos[3].AtualizarPosicao(new Ponto4D(-0.5,  0.5));
                PontosCurva(true);
                break;
            default:
                break;

        }

        retaEsq.AtualizarPosicao(objetos[0].getPonto(), objetos[3].getPonto());
        retaDir.AtualizarPosicao(objetos[1].getPonto(), objetos[2].getPonto());
        retaSup.AtualizarPosicao(objetos[3].getPonto(), objetos[2].getPonto());

    }
    private void PontosCurva(bool atualiza) {
        Vector3[] pontosControle = new Vector3[4];
        pontosControle[0] = new Vector3((float) objetos[0].getPonto().X, (float)objetos[0].getPonto().Y, 0);
        pontosControle[1] = new Vector3((float) objetos[3].getPonto().X, (float)objetos[3].getPonto().Y, 0);
        pontosControle[2] = new Vector3((float) objetos[2].getPonto().X, (float)objetos[2].getPonto().Y, 0);
        pontosControle[3] = new Vector3((float) objetos[1].getPonto().X, (float)objetos[1].getPonto().Y, 0);

        Vector3 curvaBezier(Vector3[] pontosControle, float t)
        {
            Vector3 P0 = pontosControle[0];
            Vector3 P1 = pontosControle[1];
            Vector3 P2 = pontosControle[2];
            Vector3 P3 = pontosControle[3];

            Vector3 Q0 = P0 + (P1 - P0) * t;
            Vector3 Q1 = P1 + (P2 - P1) * t;
            Vector3 Q2 = P2 + (P3 - P2) * t;

            Vector3 R0 = Q0 + (Q1 - Q0) * t;
            Vector3 R1 = Q1 + (Q2 - Q1) * t;

            return R0 + (R1 - R0) * t;
        }

        int numPontos = 11;
        float deltaT = 1f / (numPontos - 1);
        for (int i = 0; i < numPontos; i++)
        {
            float t = i * deltaT;
            Vector3 posicaoCurva = curvaBezier(pontosControle, t);

            if (atualiza)
            {
                pontosCurvaBezier[i].AtualizarPosicao(new Ponto4D (posicaoCurva.X ,posicaoCurva.Y));
                if(i > 0) {
                    retasCurvaBezier[i-1].AtualizarPosicao(new Ponto4D (pontosCurvaBezier[i-1].getPonto().X, pontosCurvaBezier[i-1].getPonto().Y), new Ponto4D (pontosCurvaBezier[i].getPonto().X, pontosCurvaBezier[i].getPonto().Y));
                }
            }
            else
            {
                Ponto p = new Ponto (mundo, ref rotuloAtual, new Ponto4D (posicaoCurva.X ,posicaoCurva.Y))
                {
                    PrimitivaTamanho = 1
                };
                p.ShaderObjeto = _shaderVermelha;
                pontosCurvaBezier.Add(p);

                if (i > 0) {
                    SegReta r = new SegReta(mundo, ref rotuloAtual, new Ponto4D (pontosCurvaBezier[i-1].getPonto().X, pontosCurvaBezier[i-1].getPonto().Y), new Ponto4D (pontosCurvaBezier[i].getPonto().X, pontosCurvaBezier[i].getPonto().Y));
                    retasCurvaBezier.Add(r);
                }

                
            }
        }
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
