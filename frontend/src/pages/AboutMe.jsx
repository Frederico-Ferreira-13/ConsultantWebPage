import { useState } from 'react';

export default function AboutMe() {
  const [activeTab, setActiveTab] = useState('ask'); // 'ask' ou 'quiz'
  const [question, setQuestion] = useState('');
  const [quizStep, setQuizStep] = useState(1);
  const [quizData, setQuizData] = useState({ tipo: '', orcamento: '', zona: '' });

  const handleQuizSelect = (field, value) => {
    setQuizData({ ...quizData, [field]: value });
    setQuizStep(quizStep + 1);
  };

  const resetQuiz = () => {
    setQuizData({ tipo: '', orcamento: '', zona: '' });
    setQuizStep(1);
  };

  return (
    <section className="min-h-screen bg-slate-950 text-white pt-32 pb-24 px-6 md:px-12 flex items-center relative overflow-hidden">
      {/* Luzes de Fundo (Glow Elétrico) */}
      <div className="absolute top-1/4 left-0 w-96 h-96 bg-orange-500/10 rounded-full blur-3xl pointer-events-none"></div>
      <div className="absolute bottom-1/4 right-0 w-96 h-96 bg-blue-500/10 rounded-full blur-3xl pointer-events-none"></div>

      <div className="max-w-6xl mx-auto w-full grid grid-cols-1 lg:grid-cols-12 gap-12 items-center relative z-10">
        
        {/* COLUNA ESQUERDA: STORYTELLING PREMIUM (5 Colunas) */}
        <div className="lg:col-span-5 space-y-6">
          <div className="inline-flex items-center gap-2 bg-white/5 border border-white/10 px-3 py-1 rounded-full text-xs font-semibold text-orange-500 uppercase tracking-widest">
            <span>⚡ O Consultor</span>
          </div>
          <h2 className="text-4xl md:text-5xl font-black tracking-tight leading-none">
            Estratégia Humana. <br />
            <span className="bg-linear-to-r from-orange-500 to-orange-400 bg-clip-text text-transparent">Performance Digital.</span>
          </h2>
          <p className="text-slate-400 text-base leading-relaxed">
            O mercado imobiliário mudou. Esqueça os panfletos na caixa do correio e as abordagens antigas. Utilizo análise de dados, marketing preditivo e tecnologia de ponta para posicionar o seu imóvel à frente dos compradores certos.
          </p>
          
          {/* Métricas de Impacto */}
          <div className="grid grid-cols-3 gap-4 pt-4 border-t border-white/10">
            <div>
              <p className="text-2xl font-black text-white">+45M€</p>
              <p className="text-xs text-slate-500 uppercase font-medium mt-1">Transacionados</p>
            </div>
            <div>
              <p className="text-2xl font-black text-orange-500">18 Dias</p>
              <p className="text-xs text-slate-500 uppercase font-medium mt-1">Média de Venda</p>
            </div>
            <div>
              <p className="text-2xl font-black text-white">98%</p>
              <p className="text-xs text-slate-500 uppercase font-medium mt-1">Clientes Felizes</p>
            </div>
          </div>
        </div>

        {/* COLUNA DIREITA: CONSOLA INTERATIVA (7 Colunas) */}
        <div className="lg:col-span-7 bg-slate-900/60 border border-white/10 rounded-3xl p-6 md:p-8 backdrop-blur-md shadow-2xl">
          
          {/* Navegação Interna da Consola */}
          <div className="flex border-b border-white/5 mb-6 gap-4">
            <button 
              onClick={() => setActiveTab('ask')}
              className={`pb-3 font-semibold text-sm transition-all relative ${
                activeTab === 'ask' ? 'text-white' : 'text-slate-500 hover:text-slate-300'
              }`}
            >
              Perguntas Rápidas
              {activeTab === 'ask' && <div className="absolute bottom-0 left-0 w-full h-0.5 bg-orange-500"></div>}
            </button>
            <button 
              onClick={() => setActiveTab('quiz')}
              className={`pb-3 font-semibold text-sm transition-all relative ${
                activeTab === 'quiz' ? 'text-white' : 'text-slate-500 hover:text-slate-300'
              }`}
            >
              Encontrar meu Imóvel (Quiz)
              {activeTab === 'quiz' && <div className="absolute bottom-0 left-0 w-full h-0.5 bg-orange-500"></div>}
            </button>
          </div>

          {/* ABA 1: FAZER PERGUNTAS */}
          {activeTab === 'ask' && (
            <div className="space-y-4 animate-fadeIn">
              <p className="text-sm text-slate-400">Tem alguma dúvida sobre impostos, mais-valias ou o processo de compra? Pergunte diretamente:</p>
              <div className="relative">
                <textarea 
                  value={question}
                  onChange={(e) => setQuestion(e.target.value)}
                  placeholder="Ex: Como funcionam as mais-valias na venda de habitação própria?"
                  className="w-full bg-slate-950 border border-white/10 rounded-2xl p-4 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 min-h-25 resize-none transition-colors"
                />
              </div>
              <button className="bg-orange-500 hover:bg-orange-600 text-white font-semibold text-sm px-6 py-3 rounded-xl transition-all shadow-lg shadow-orange-500/10">
                Enviar Pergunta para o Frederico
              </button>

              {/* FAQ Instantânea Integrada */}
              <div className="pt-4 border-t border-white/5 space-y-3">
                <p className="text-xs font-bold uppercase text-slate-500 tracking-wider">Perguntas Frequentes</p>
                <div className="p-3 bg-white/5 border border-white/5 rounded-xl text-xs cursor-pointer hover:bg-white/10 transition-colors">
                  💡 <span className="font-semibold text-slate-300">Quanto tempo demora em média a avaliação do meu imóvel?</span>
                </div>
                <div className="p-3 bg-white/5 border border-white/5 rounded-xl text-xs cursor-pointer hover:bg-white/10 transition-colors">
                  💼 <span className="font-semibold text-slate-300">Quais os documentos necessários para colocar uma casa à venda?</span>
                </div>
              </div>
            </div>
          )}

          {/* ABA 2: QUESTIONÁRIO INTELIGENTE */}
          {activeTab === 'quiz' && (
            <div className="min-h-70 flex flex-col justify-between animate-fadeIn">
              <div>
                <div className="flex justify-between items-center mb-4">
                  <span className="text-xs font-bold uppercase text-orange-500 tracking-wider">Passo {quizStep} de 3</span>
                  {quizStep > 1 && (
                    <button onClick={resetQuiz} className="text-xs text-slate-500 hover:text-white underline">Reiniciar</button>
                  )}
                </div>

                {/* Passo 1: Tipo de Imóvel */}
                {quizStep === 1 && (
                  <div className="space-y-3">
                    <h3 className="font-bold text-base">O que está à procura de comprar ou investir?</h3>
                    <div className="grid grid-cols-2 gap-3">
                      {['Moradia Moderna', 'Apartamento / Penthouse', 'Terreno para Construção', 'Investimento Comercial'].map((tipo) => (
                        <button 
                          key={tipo}
                          onClick={() => handleQuizSelect('tipo', tipo)}
                          className="p-4 bg-white/5 border border-white/10 rounded-xl text-sm font-medium hover:bg-orange-500/10 hover:border-orange-500 text-left transition-all"
                        >
                          {tipo}
                        </button>
                      ))}
                    </div>
                  </div>
                )}

                {/* Passo 2: Orçamento */}
                {quizStep === 2 && (
                  <div className="space-y-3">
                    <h3 className="font-bold text-base">Qual é o budget estimado para esta operação?</h3>
                    <div className="grid grid-cols-2 gap-3">
                      {['Até 250.000€', '250.000€ a 500.000€', '500.000€ a 1M€', 'Segmento de Luxo (+1M€)'].map((orcamento) => (
                        <button 
                          key={orcamento}
                          onClick={() => handleQuizSelect('orcamento', orcamento)}
                          className="p-4 bg-white/5 border border-white/10 rounded-xl text-sm font-medium hover:bg-orange-500/10 hover:border-orange-500 text-left transition-all"
                        >
                          {orcamento}
                        </button>
                      ))}
                    </div>
                  </div>
                )}

                {/* Passo 3: Localização / Obrigado */}
                {quizStep === 3 && (
                  <div className="space-y-4">
                    <h3 className="font-bold text-base">Perfeito! Deixe-me o seu contacto para receber o relatório de imóveis ideais:</h3>
                    <input 
                      type="email" 
                      placeholder="O seu melhor e-mail..."
                      className="w-full bg-slate-950 border border-white/10 rounded-2xl p-4 text-sm text-white focus:outline-none focus:border-orange-500"
                    />
                    <button 
                      onClick={() => setQuizStep(4)}
                      className="w-full bg-orange-500 hover:bg-orange-600 text-white font-semibold py-3 rounded-xl transition-all"
                    >
                      Gerar Portfólio Personalizado
                    </button>
                  </div>
                )}

                {/* Fim do Quiz */}
                {quizStep === 4 && (
                  <div className="text-center py-8 space-y-3">
                    <div className="w-12 h-12 bg-emerald-500/20 text-emerald-400 rounded-full flex items-center justify-center mx-auto text-xl">✓</div>
                    <h3 className="font-bold text-lg text-emerald-400">Pedido Recebido com Sucesso!</h3>
                    <p className="text-sm text-slate-400 max-w-sm mx-auto">
                      O algoritmo cruzou os seus dados. Em menos de 30 minutos vai receber no seu email os imóveis fora de mercado condizentes.
                    </p>
                  </div>
                )}
              </div>
            </div>
          )}

        </div>
      </div>
    </section>
  );
}