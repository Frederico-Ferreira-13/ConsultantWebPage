import { useState } from "react";
import { API_BASE_URL as API_BASE } from '../../config/api';

const QUESTIONS = [
    {
        id: 1,
        text: 'Quando enfrenta um obstáculo inesperado, a sua reação mais natural é:',
        options: [
            { label: 'Agir imediatamente e encontrar uma solução rápida', type: 'D' },
            { label: 'Envolver outros para resolver o problema em conjunto', type: 'I' },
            { label: 'Manter a calma e seguir um processo estabelecido', type: 'S' },
            { label: 'Analisar os dados antes de tomar qualquer decisão', type: 'C' },
        ],
    },
    {
        id: 2,
        text: 'Na hora de tomar uma decisão importante (como comprar casa), prefere:',
    options: [
      { label: 'Decidir rapidamente e avançar sem hesitação', type: 'D' },
      { label: 'Falar com pessoas de confiança e ouvir opiniões', type: 'I' },
      { label: 'Tomar tempo, pesar bem as opções antes de agir', type: 'S' },
      { label: 'Recolher toda a informação possível e comparar', type: 'C' },
    ],
  },
  {
    id: 3,
    text: 'Num processo de negociação, tende a priorizar:',
    options: [
      { label: 'Obter o melhor resultado possível, mesmo que exija firmeza', type: 'D' },
      { label: 'Que todas as partes saiam satisfeitas e a relação seja boa', type: 'I' },
      { label: 'Estabilidade e garantir que não há riscos desnecessários', type: 'S' },
      { label: 'Que tudo seja justo, documentado e dentro das regras', type: 'C' },
    ],
  },
  {
    id: 4,
    text: 'Quando pensa no imóvel ideal, o que pesa mais na sua decisão?',
    options: [
      { label: 'O potencial de valorização e o retorno do investimento', type: 'D' },
      { label: 'O ambiente, o bairro e a qualidade de vida para a família', type: 'I' },
      { label: 'A segurança, a estabilidade e um sítio para criar raízes', type: 'S' },
      { label: 'As características técnicas, a documentação e o preço justo', type: 'C' },
    ],
  },
  {
    id: 5,
    text: 'Como prefere receber informação sobre imóveis?',
    options: [
      { label: 'Um resumo direto com os pontos mais importantes', type: 'D' },
      { label: 'Uma conversa presencial com histórias e contexto', type: 'I' },
      { label: 'Relatórios detalhados enviados com antecedência para analisar', type: 'S' },
      { label: 'Dados, comparações, estatísticas e toda a documentação', type: 'C' },
    ],
  },
  {
    id: 6,
    text: 'Quando visita um imóvel, o que lhe desperta mais atenção imediatamente?',
    options: [
      { label: 'O potencial de transformação e o que pode ser feito', type: 'D' },
      { label: 'A energia do espaço e se consegue imaginar a sua vida lá', type: 'I' },
      { label: 'O estado de conservação e a solidez da construção', type: 'S' },
      { label: 'Os acabamentos, medidas, orientação solar e anomalias', type: 'C' },
    ],
  },
  {
    id: 7,
    text: 'Qual das seguintes afirmações descreve melhor a sua relação com o dinheiro?',
    options: [
      { label: 'Invisto com confiança quando vejo uma boa oportunidade', type: 'D' },
      { label: 'O dinheiro é importante, mas a felicidade da família vem primeiro', type: 'I' },
      { label: 'Prefiro ser conservador e garantir sempre uma reserva de segurança', type: 'S' },
      { label: 'Analiso tudo ao detalhe antes de comprometer qualquer valor', type: 'C' },
    ],
  },
  {
    id: 8,
    text: 'Como descreveria a sua comunicação com o consultor ideal?',
    options: [
      { label: 'Direto, eficiente, sem rodeios — o meu tempo é valioso', type: 'D' },
      { label: 'Aberto e caloroso — quero sentir que me conhece como pessoa', type: 'I' },
      { label: 'Paciente e consistente — prefiro acompanhamento regular e sem pressão', type: 'S' },
      { label: 'Preciso e profissional — quero respostas concretas e documentadas', type: 'C' },
    ],
  },
  {
    id: 9,
    text: 'O que mais o incomoda num processo de compra ou venda?',
    options: [
      { label: 'Processos lentos, burocracia e falta de decisão', type: 'D' },
      { label: 'Falta de comunicação ou sentir que não sou ouvido', type: 'I' },
      { label: 'Mudanças de última hora e pressão para decidir rapidamente', type: 'S' },
      { label: 'Informação incompleta, erros ou falta de transparência', type: 'C' },
    ],
  },
  {
    id: 10,
    text: 'Ao longo do processo imobiliário, o que mais valoriza no consultor?',
    options: [
      { label: 'Resultados concretos e capacidade de negociação', type: 'D' },
      { label: 'Empatia, entusiasmo e capacidade de criar confiança', type: 'I' },
      { label: 'Fiabilidade, paciência e acompanhamento personalizado', type: 'S' },
      { label: 'Conhecimento técnico, rigor e transparência total', type: 'C' },
    ],
  },
  {
    id: 11,
    text: 'Quando um negócio não corre como esperado, como reage?',
    options: [
      { label: 'Parto para o próximo — não fico preso no passado', type: 'D' },
      { label: 'Preciso de falar sobre isso e perceber o que correu mal', type: 'I' },
      { label: 'Fico afetado mas processo com calma e sigo em frente', type: 'S' },
      { label: 'Analiso o que aconteceu ao detalhe para não repetir o erro', type: 'C' },
    ],
  },
  {
    id: 12,
    text: 'O que o faria avançar rapidamente numa decisão de compra?',
    options: [
      { label: 'Sentir que é uma oportunidade única que não posso perder', type: 'D' },
      { label: 'Sentir que o consultor acredita genuinamente que é a escolha certa', type: 'I' },
      { label: 'Ter tempo para refletir e sentir segurança total na decisão', type: 'S' },
      { label: 'Ter todos os dados confirmados e as condições claras', type: 'C' },
    ],
  },
];

const PROFILES = {
    D: {
    label: 'Dominante',
    emoji: '🦁',
    color: '#E54040',
    bg: '#FEF2F2',
    border: '#FECACA',
    tagline: 'Orientado para resultados. Decide rápido, quer o melhor negócio.',
    description:
      'É uma pessoa de ação — analisa rapidamente e toma decisões com confiança. Valoriza eficiência e espera que o consultor seja direto, competente e focado em resultados. Não gosta de perder tempo com detalhes irrelevantes.',
    howToWork:
      'O Frederico vai preparar uma seleção focada de imóveis com alto potencial, apresentar dados de forma clara e objetiva, e garantir que o processo de negociação é conduzido com firmeza e sem burocracia desnecessária.',
    cta: 'Ver imóveis de alto potencial',
    ctaLink: '/imoveis?filter=featured',
  },
  I: {
    label: 'Influente',
    emoji: '🌟',
    color: '#F59E0B',
    bg: '#FFFBEB',
    border: '#FDE68A',
    tagline: 'Movido pelas emoções e pela ligação humana. O imóvel tem de "falar".',
    description:
      'Para si, comprar casa é uma experiência emocional e social. O imóvel tem de criar uma ligação imediata — a energia do espaço, a visão de como a sua vida ali vai ser. A relação com o consultor é tão importante quanto o próprio imóvel.',
    howToWork:
      'O Frederico vai criar uma experiência de procura personalizada, ouvir a sua visão de vida e ajudá-lo a imaginar o futuro em cada espaço. O foco é na conexão humana e na construção de confiança ao longo do processo.',
    cta: 'Contar a minha história ao Frederico',
    ctaLink: '/sobre',
  },
  S: {
    label: 'Estável',
    emoji: '🏡',
    color: '#10B981',
    bg: '#F0FDF4',
    border: '#A7F3D0',
    tagline: 'Segurança em primeiro lugar. Quer criar raízes com tranquilidade.',
    description:
      'Valoriza a estabilidade e a segurança acima de tudo. Não gosta de pressão e prefere tomar decisões bem pensadas. Para si, a casa é um porto seguro — um lugar para criar raízes e viver com tranquilidade. Aprecia um consultor paciente e de confiança.',
    howToWork:
      'O Frederico vai acompanhá-lo ao seu ritmo, sem pressas nem pressão. Vai garantir que tem toda a informação necessária, explicar cada passo do processo e estar disponível sempre que precisar de esclarecer alguma dúvida.',
    cta: 'Começar um processo sem pressão',
    ctaLink: '/avaliacao',
  },
  C: {
    label: 'Conscencioso',
    emoji: '🔬',
    color: '#6366F1',
    bg: '#EEF2FF',
    border: '#C7D2FE',
    tagline: 'Analítico e rigoroso. Quer dados, factos e total transparência.',
    description:
      'Antes de decidir, precisa de ter todos os dados. Analisa documentação, compara preços, verifica condições técnicas e não avança sem ter certezas. Valoriza um consultor rigoroso, transparente e que apresenta informação completa e verificada.',
    howToWork:
      'O Frederico vai preparar relatórios detalhados com análise comparativa de mercado, documentação completa de cada imóvel e um processo totalmente transparente — para que tome a sua decisão com total confiança e sem surpresas.',
    cta: 'Pedir estudo de mercado detalhado',
    ctaLink: '/avaliacao',
  },
};

function shuffleOptions(options) {
  return [...options].sort(() => Math.random() - 0.5);
}

export default function DiscQuiz() {
  const [step, setStep] = useState('intro'); // intro | quiz | result
  const [currentQ, setCurrentQ] = useState(0);
  const [answers, setAnswers] = useState({});
  const [shuffled] = useState(() => QUESTIONS.map(q => ({ ...q, options: shuffleOptions(q.options) })));
  const [leadForm, setLeadForm] = useState({ name: '', email: '', phone: '' });
  const [submitted, setSubmitted] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState('');
 
  const progress = Math.round((currentQ / QUESTIONS.length) * 100);
 
  function handleAnswer(type) {
    const next = { ...answers, [currentQ]: type };
    setAnswers(next);
    if (currentQ + 1 < QUESTIONS.length) {
      setCurrentQ(currentQ + 1);
    } else {
      setStep('result');
    }
  }
 
  function calcProfile() {
    const counts = { D: 0, I: 0, S: 0, C: 0 };
    Object.values(answers).forEach(t => { counts[t] = (counts[t] || 0) + 1; });
    return Object.entries(counts).sort((a, b) => b[1] - a[1])[0][0];
  }
 
  async function handleLeadSubmit(e) {
    e.preventDefault();
    setSubmitError('');
    setSubmitting(true);

    try {
      // IMPLEMENTADO: antes só fazia setSubmitted(true) — não gravava nada.
      // Agora envia o lead para o LeadsController já existente, incluindo o
      // resultado do perfil DISC na mensagem para o consultor ter contexto
      // imediato sobre como abordar este lead.
      const profileKey = calcProfile();
      const profile = PROFILES[profileKey];
      const message = `Resultado do teste DISC: Perfil ${profile.label} (${profileKey}). ${profile.tagline}`;

      const res = await fetch(`${API_BASE}/leads`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          realEstateId: null,
          consultantId: null,
          clientName: leadForm.name,
          email: leadForm.email,
          phone: leadForm.phone,
          message,
        }),
      });

      if (!res.ok) {
        const errorText = await res.text();
        throw new Error(errorText || 'Não foi possível enviar o seu perfil. Tente novamente.');
      }

      setSubmitted(true);
    } catch (err) {
      setSubmitError(err.message || 'Não foi possível enviar o seu perfil. Tente novamente.');
    } finally {
      setSubmitting(false);
    }
  }
 
  if (step === 'intro') {
    return (
      <div className="min-h-screen bg-slate-950 text-white pt-28 pb-20 px-6">
        <div className="max-w-2xl mx-auto text-center space-y-8">
          <div className="inline-flex items-center gap-2 bg-white/5 border border-white/10 px-4 py-2 rounded-full text-xs font-semibold text-orange-400 uppercase tracking-widest">
            ⚡ Descubra o seu perfil
          </div>
          <h1 className="text-4xl md:text-5xl font-black tracking-tight leading-none">
            Que tipo de <br />
            <span className="text-orange-500">comprador é você?</span>
          </h1>
          <p className="text-slate-400 text-lg leading-relaxed max-w-xl mx-auto">
            12 perguntas. 3 minutos. Um perfil que ajuda o Frederico a trabalhar exatamente como você precisa — e a encontrar o imóvel certo mais depressa.
          </p>
 
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
            {Object.entries(PROFILES).map(([key, p]) => (
              <div key={key} className="bg-white/5 border border-white/10 rounded-2xl p-4 text-center space-y-2">
                <div className="text-3xl">{p.emoji}</div>
                <div className="font-semibold text-white text-xs uppercase tracking-wider">{p.label}</div>
                <div className="text-slate-500 text-xs leading-snug">{p.tagline.split('.')[0]}</div>
              </div>
            ))}
          </div>
 
          <button
            onClick={() => setStep('quiz')}
            className="bg-orange-500 hover:bg-orange-600 active:scale-95 text-white font-bold text-lg px-10 py-4 rounded-2xl transition-all shadow-xl shadow-orange-500/20"
          >
            Iniciar o teste →
          </button>
          <p className="text-slate-600 text-xs">100% gratuito · Sem registo · Resultado imediato</p>
        </div>
      </div>
    );
  }
 
  if (step === 'quiz') {
    const q = shuffled[currentQ];
    return (
      <div className="min-h-screen bg-slate-950 text-white pt-24 pb-20 px-6">
        <div className="max-w-2xl mx-auto">
          {/* Progress */}
          <div className="mb-8">
            <div className="flex justify-between text-xs text-slate-500 mb-2">
              <span>Pergunta {currentQ + 1} de {QUESTIONS.length}</span>
              <span>{progress}%</span>
            </div>
            <div className="h-1.5 bg-white/10 rounded-full overflow-hidden">
              <div
                className="h-full bg-orange-500 rounded-full transition-all duration-500"
                style={{ width: `${progress}%` }}
              />
            </div>
          </div>
 
          {/* Question */}
          <div className="mb-8">
            <p className="text-xl md:text-2xl font-bold leading-snug text-white">{q.text}</p>
          </div>
 
          {/* Options */}
          <div className="space-y-3">
            {q.options.map((opt, i) => (
              <button
                key={i}
                onClick={() => handleAnswer(opt.type)}
                className="w-full text-left bg-white/5 hover:bg-orange-500/10 border border-white/10 hover:border-orange-500/50 rounded-2xl px-5 py-4 text-sm text-slate-300 hover:text-white transition-all group"
              >
                <span className="inline-block w-6 h-6 rounded-full border border-white/20 group-hover:border-orange-500 text-center text-xs leading-6 mr-3 font-semibold shrink-0">
                  {String.fromCharCode(65 + i)}
                </span>
                {opt.label}
              </button>
            ))}
          </div>
 
          {currentQ > 0 && (
            <button
              onClick={() => setCurrentQ(currentQ - 1)}
              className="mt-6 text-slate-500 hover:text-white text-sm transition-colors"
            >
              ← Pergunta anterior
            </button>
          )}
        </div>
      </div>
    );
  }
 
  if (step === 'result') {
    const profileKey = calcProfile();
    const profile = PROFILES[profileKey];
 
    const counts = { D: 0, I: 0, S: 0, C: 0 };
    Object.values(answers).forEach(t => { counts[t] = (counts[t] || 0) + 1; });
    const total = QUESTIONS.length;
 
    return (
      <div className="min-h-screen bg-slate-950 text-white pt-24 pb-24 px-6">
        <div className="max-w-2xl mx-auto space-y-8">
 
          {/* Hero result */}
          <div
            className="rounded-3xl p-8 border text-center"
            style={{ background: profile.bg, borderColor: profile.border }}
          >
            <div className="text-5xl mb-3">{profile.emoji}</div>
            <div
              className="inline-block text-xs font-bold uppercase tracking-widest px-3 py-1 rounded-full mb-3"
              style={{ background: profile.color + '22', color: profile.color }}
            >
              Perfil {profile.label}
            </div>
            <h2 className="text-2xl font-black mb-2" style={{ color: '#1e293b' }}>
              {profile.tagline}
            </h2>
            <p className="text-slate-600 text-sm leading-relaxed">{profile.description}</p>
          </div>
 
          {/* Score breakdown */}
          <div className="bg-white/5 border border-white/10 rounded-2xl p-6">
            <p className="text-xs text-slate-500 uppercase tracking-wider font-semibold mb-4">Distribuição do seu perfil</p>
            <div className="space-y-3">
              {Object.entries(PROFILES).map(([key, p]) => {
                const pct = Math.round((counts[key] / total) * 100);
                return (
                  <div key={key} className="flex items-center gap-3">
                    <span className="text-sm w-28 shrink-0" style={{ color: key === profileKey ? p.color : '#64748b' }}>
                      {p.emoji} {p.label}
                    </span>
                    <div className="flex-1 h-2 bg-white/10 rounded-full overflow-hidden">
                      <div
                        className="h-full rounded-full transition-all duration-700"
                        style={{ width: `${pct}%`, background: key === profileKey ? p.color : '#334155' }}
                      />
                    </div>
                    <span className="text-xs text-slate-500 w-8 text-right">{pct}%</span>
                  </div>
                );
              })}
            </div>
          </div>
 
          {/* How Frederico will work */}
          <div className="bg-slate-900 border border-white/10 rounded-2xl p-6">
            <p className="text-xs text-slate-500 uppercase tracking-wider font-semibold mb-3">Como o Frederico vai trabalhar consigo</p>
            <p className="text-slate-300 text-sm leading-relaxed">{profile.howToWork}</p>
          </div>
 
          {/* Lead capture */}
          <div className="bg-white/5 border border-white/10 rounded-2xl p-6">
            {!submitted ? (
              <>
                <p className="text-sm font-semibold text-white mb-1">Receba o seu relatório de perfil por email</p>
                <p className="text-slate-500 text-xs mb-4">O Frederico vai analisar o resultado e entrar em contacto com imóveis adaptados ao seu perfil.</p>
                <form onSubmit={handleLeadSubmit} className="space-y-3">
                  <input
                    required
                    type="text"
                    placeholder="O seu nome"
                    value={leadForm.name}
                    onChange={e => setLeadForm({ ...leadForm, name: e.target.value })}
                    className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500"
                  />
                  <input
                    required
                    type="email"
                    placeholder="Email"
                    value={leadForm.email}
                    onChange={e => setLeadForm({ ...leadForm, email: e.target.value })}
                    className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500"
                  />
                  <input
                    required
                    type="tel"
                    placeholder="Telefone"
                    value={leadForm.phone}
                    onChange={e => setLeadForm({ ...leadForm, phone: e.target.value })}
                    className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500"
                  />
                  {submitError && (
                    <p className="text-red-400 text-xs">{submitError}</p>
                  )}
                  <button
                    type="submit"
                    disabled={submitting}
                    className="w-full bg-orange-500 hover:bg-orange-600 disabled:opacity-50 text-white font-semibold py-3 rounded-xl transition-all"
                  >
                    {submitting ? 'A enviar...' : 'Enviar perfil para o Frederico'}
                  </button>
                </form>
              </>
            ) : (
              <div className="text-center py-4 space-y-2">
                <div className="w-10 h-10 bg-emerald-500/20 text-emerald-400 rounded-full flex items-center justify-center mx-auto text-lg">✓</div>
                <p className="font-semibold text-emerald-400">Perfil enviado com sucesso!</p>
                <p className="text-slate-500 text-xs">O Frederico vai analisar o seu perfil e entrar em contacto em breve.</p>
              </div>
            )}
          </div>
 
          <button
            onClick={() => { setStep('intro'); setCurrentQ(0); setAnswers({}); setSubmitted(false); }}
            className="w-full text-slate-500 hover:text-white text-sm transition-colors py-2"
          >
            Refazer o teste
          </button>
        </div>
      </div>
    );
  }
}