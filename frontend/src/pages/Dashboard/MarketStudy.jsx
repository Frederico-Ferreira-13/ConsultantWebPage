import { useState } from 'react';
import { API_BASE_URL as API_BASE } from '../../config/api';

const PROPERTY_TYPES = ['Apartamento', 'Moradia', 'Terreno', 'Escritório / Comercial', 'Outro'];
const TYPOLOGIES = ['T0', 'T1', 'T2', 'T3', 'T4', 'T5+'];
const CONDITIONS = ['Novo / Em planta', 'Excelente estado', 'Bom estado', 'Para recuperar'];
const TIME_SLOTS = ['09:00', '10:00', '11:00', '14:00', '15:00', '16:00', '17:00'];

const PRICE_RANGES = {
  'Apartamento': { T0: [80000, 140000], T1: [130000, 220000], T2: [180000, 320000], T3: [250000, 450000], T4: [350000, 600000], 'T5+': [500000, 900000] },
  'Moradia': { T0: [100000, 180000], T1: [160000, 280000], T2: [220000, 380000], T3: [300000, 550000], T4: [420000, 750000], 'T5+': [600000, 1200000] },
  'Terreno': { T0: [30000, 60000], T1: [40000, 80000], T2: [50000, 100000], T3: [60000, 120000], T4: [80000, 160000], 'T5+': [100000, 250000] },
  'Escritório / Comercial': { T0: [50000, 100000], T1: [80000, 150000], T2: [120000, 220000], T3: [180000, 320000], T4: [250000, 450000], 'T5+': [350000, 700000] },
  'Outro': { T0: [60000, 120000], T1: [90000, 160000], T2: [140000, 260000], T3: [200000, 380000], T4: [280000, 500000], 'T5+': [400000, 800000] },
};

const CONDITION_MULTIPLIER = { 'Novo / Em planta': 1.15, 'Excelente estado': 1.05, 'Bom estado': 1.0, 'Para recuperar': 0.82 };

function formatPrice(n) {
  return new Intl.NumberFormat('pt-PT', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(Math.round(n));
}

function getDayName(d) {
  const days = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'];
  return days[d.getDay()];
}

function getNextDays(n) {
  const result = [];
  const today = new Date();
  let offset = 1;
  while (result.length < n) {
    const d = new Date(today);
    d.setDate(today.getDate() + offset);
    offset++;
    if (d.getDay() !== 0 && d.getDay() !== 6) result.push(d);
  }
  return result;
}

export default function MarketStudy() {
  const [step, setStep] = useState(1); // 1=property info | 2=result+schedule
  const [form, setForm] = useState({
    type: '', typology: '', area: '', condition: '', zone: '', notes: '',
    name: '', email: '', phone: '',
    selectedDay: null, selectedTime: null,
  });
  const [submitted, setSubmitted] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState('');
  const [estimate, setEstimate] = useState(null);

  const days = getNextDays(6);

  function calcEstimate() {
    const range = PRICE_RANGES[form.type]?.[form.typology] || [150000, 300000];
    const mult = CONDITION_MULTIPLIER[form.condition] || 1;
    const areaFactor = form.area ? Math.max(0.7, Math.min(1.3, parseInt(form.area) / 100)) : 1;
    const low = range[0] * mult * areaFactor;
    const high = range[1] * mult * areaFactor;
    const mid = (low + high) / 2;
    return { low, mid, high };
  }

  function handleStep1(e) {
    e.preventDefault();
    const est = calcEstimate();
    setEstimate(est);
    setStep(2);
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setSubmitError('');
    setSubmitting(true);

    try {
      // IMPLEMENTADO: antes só fazia setSubmitted(true) — não gravava nada.
      // Agora envia o pedido para o PropertyEvaluationController já existente.
      // ConsultantId fixo a 1 (consultor único atual) — substituir por lógica
      // de distribuição quando essa funcionalidade for construída.
      const clientContact = form.phone ? `${form.email} | ${form.phone}` : form.email;
      const visitInfo = form.selectedDay && form.selectedTime
        ? ` — Visita preferida: ${getDayName(form.selectedDay)} ${form.selectedDay.getDate()}/${form.selectedDay.getMonth() + 1} às ${form.selectedTime}`
        : '';

      const res = await fetch(`${API_BASE}/PropertyEvaluation`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          consultantId: 1,
          userId: null,
          clientName: form.name,
          clientContact: clientContact + visitInfo,
          propertyType: form.type,
          address: form.zone,
          typology: form.typology,
          area: parseFloat(form.area),
          condition: form.condition,
        }),
      });

      if (!res.ok) {
        const errorText = await res.text();
        throw new Error(errorText || 'Não foi possível agendar a reunião. Tente novamente.');
      }

      setSubmitted(true);
    } catch (err) {
      setSubmitError(err.message || 'Não foi possível agendar a reunião. Tente novamente.');
    } finally {
      setSubmitting(false);
    }
  }

  const canSubmit = form.name && form.email && form.selectedDay && form.selectedTime;

  return (
    <div className="min-h-screen bg-slate-950 text-white pt-24 pb-24 px-6">
      <div className="max-w-3xl mx-auto">

        {/* Header */}
        <div className="text-center mb-10 space-y-3">
          <div className="inline-flex items-center gap-2 bg-white/5 border border-white/10 px-4 py-2 rounded-full text-xs font-semibold text-orange-400 uppercase tracking-widest">
            📊 Estudo de Mercado
          </div>
          <h1 className="text-3xl md:text-4xl font-black">
            Quanto vale o seu imóvel?
          </h1>
          <p className="text-slate-400 max-w-xl mx-auto text-sm leading-relaxed">
            Obtenha uma estimativa de mercado gratuita e agende uma reunião com o Frederico para uma avaliação completa e personalizada.
          </p>

          {/* Steps */}
          <div className="flex items-center justify-center gap-4 pt-2">
            {[1, 2].map(s => (
              <div key={s} className="flex items-center gap-2">
                <div className={`w-7 h-7 rounded-full text-xs font-bold flex items-center justify-center transition-all ${step >= s ? 'bg-orange-500 text-white' : 'bg-white/10 text-slate-500'}`}>
                  {step > s ? '✓' : s}
                </div>
                <span className={`text-xs ${step >= s ? 'text-white' : 'text-slate-600'}`}>
                  {s === 1 ? 'Dados do imóvel' : 'Estimativa + Reunião'}
                </span>
                {s < 2 && <div className="w-8 h-px bg-white/10" />}
              </div>
            ))}
          </div>
        </div>

        {/* STEP 1: Property form */}
        {step === 1 && (
          <form onSubmit={handleStep1} className="space-y-6">
            <div className="bg-slate-900 border border-white/10 rounded-2xl p-6 space-y-5">
              <p className="text-sm font-semibold text-white">Características do imóvel</p>

              {/* Type */}
              <div>
                <label className="text-xs text-slate-400 mb-2 block">Tipo de imóvel *</label>
                <div className="flex flex-wrap gap-2">
                  {PROPERTY_TYPES.map(t => (
                    <button
                      key={t} type="button"
                      onClick={() => setForm({ ...form, type: t })}
                      className={`px-3 py-1.5 rounded-xl text-xs font-medium border transition-all ${form.type === t ? 'bg-orange-500 border-orange-500 text-white' : 'bg-white/5 border-white/10 text-slate-300 hover:border-white/30'}`}
                    >
                      {t}
                    </button>
                  ))}
                </div>
              </div>

              {/* Typology + Condition */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="text-xs text-slate-400 mb-2 block">Tipologia *</label>
                  <div className="flex flex-wrap gap-2">
                    {TYPOLOGIES.map(t => (
                      <button
                        key={t} type="button"
                        onClick={() => setForm({ ...form, typology: t })}
                        className={`px-3 py-1.5 rounded-xl text-xs font-medium border transition-all ${form.typology === t ? 'bg-orange-500 border-orange-500 text-white' : 'bg-white/5 border-white/10 text-slate-300 hover:border-white/30'}`}
                      >
                        {t}
                      </button>
                    ))}
                  </div>
                </div>
                <div>
                  <label className="text-xs text-slate-400 mb-2 block">Estado de conservação *</label>
                  <div className="flex flex-wrap gap-2">
                    {CONDITIONS.map(c => (
                      <button
                        key={c} type="button"
                        onClick={() => setForm({ ...form, condition: c })}
                        className={`px-3 py-1.5 rounded-xl text-xs font-medium border transition-all ${form.condition === c ? 'bg-orange-500 border-orange-500 text-white' : 'bg-white/5 border-white/10 text-slate-300 hover:border-white/30'}`}
                      >
                        {c}
                      </button>
                    ))}
                  </div>
                </div>
              </div>

              {/* Area + Zone */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="text-xs text-slate-400 mb-2 block">Área útil aproximada (m²) *</label>
                  <input
                    required
                    type="number" min="10" max="2000"
                    placeholder="Ex: 95"
                    value={form.area}
                    onChange={e => setForm({ ...form, area: e.target.value })}
                    className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500"
                  />
                </div>
                <div>
                  <label className="text-xs text-slate-400 mb-2 block">Zona / Localização *</label>
                  <input
                    required
                    type="text"
                    placeholder="Ex: Lisboa, Cascais, Porto..."
                    value={form.zone}
                    onChange={e => setForm({ ...form, zone: e.target.value })}
                    className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500"
                  />
                </div>
              </div>

              <div>
                <label className="text-xs text-slate-400 mb-2 block">Informação adicional (opcional)</label>
                <textarea
                  rows={3}
                  placeholder="Ex: Vista mar, garagem, piscina, certificado energético..."
                  value={form.notes}
                  onChange={e => setForm({ ...form, notes: e.target.value })}
                  className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 resize-none"
                />
              </div>
            </div>

            <button
              type="submit"
              disabled={!form.type || !form.typology || !form.condition || !form.zone || !form.area}
              className="w-full bg-orange-500 hover:bg-orange-600 disabled:opacity-40 disabled:cursor-not-allowed text-white font-bold py-4 rounded-2xl transition-all text-lg"
            >
              Ver estimativa de valor →
            </button>
            <p className="text-center text-slate-600 text-xs">Estimativa baseada em dados de mercado de 2024–2025</p>
          </form>
        )}

        {/* STEP 2: Estimate + Booking */}
        {step === 2 && !submitted && (
          <div className="space-y-6">

            {/* Estimate card */}
            <div className="bg-slate-900 border border-orange-500/30 rounded-2xl p-6">
              <div className="flex items-start justify-between mb-6">
                <div>
                  <p className="text-xs text-orange-400 uppercase tracking-wider font-semibold mb-1">Estimativa de mercado</p>
                  <p className="text-slate-400 text-sm">{form.type} · {form.typology} · {form.zone}</p>
                </div>
                <span className="bg-orange-500/10 text-orange-400 text-xs px-3 py-1 rounded-full font-semibold border border-orange-500/20">
                  Indicativo
                </span>
              </div>

              {/* Price range */}
              <div className="grid grid-cols-3 gap-4 mb-6">
                <div className="text-center">
                  <p className="text-xs text-slate-500 mb-1">Mínimo</p>
                  <p className="text-lg font-bold text-slate-300">{formatPrice(estimate.low)}</p>
                </div>
                <div className="text-center bg-orange-500/10 border border-orange-500/20 rounded-xl py-3">
                  <p className="text-xs text-orange-400 mb-1">Estimativa central</p>
                  <p className="text-2xl font-black text-orange-400">{formatPrice(estimate.mid)}</p>
                </div>
                <div className="text-center">
                  <p className="text-xs text-slate-500 mb-1">Máximo</p>
                  <p className="text-lg font-bold text-slate-300">{formatPrice(estimate.high)}</p>
                </div>
              </div>

              {/* Visual bar */}
              <div className="relative h-3 bg-white/10 rounded-full mb-2">
                <div className="absolute left-0 top-0 h-full bg-white/20 rounded-full" style={{ width: '0%' }} />
                <div
                  className="absolute top-0 h-full bg-linear-to-r from-orange-600 to-orange-400 rounded-full"
                  style={{ left: '10%', right: '10%' }}
                />
                <div
                  className="absolute top-1/2 -translate-y-1/2 w-4 h-4 bg-orange-400 rounded-full border-2 border-white"
                  style={{ left: '48%' }}
                />
              </div>
              <p className="text-xs text-slate-500 text-center">Intervalo de valorização estimado para o mercado atual</p>

              <div className="mt-4 pt-4 border-t border-white/5 text-xs text-slate-500 flex items-start gap-2">
                <span>⚠️</span>
                <span>Esta estimativa é meramente indicativa. Fatores como localização exata, piso, orientação solar, garagem e documentação podem alterar significativamente o valor real. Uma avaliação presencial é sempre recomendada.</span>
              </div>
            </div>

            {/* Comparables teaser */}
            <div className="bg-slate-900/50 border border-white/10 rounded-2xl p-5">
              <p className="text-xs text-slate-500 uppercase tracking-wider font-semibold mb-3">Transações recentes na zona — dados indicativos</p>
              <div className="space-y-3">
                {[
                  { desc: `${form.type} ${form.typology} em bom estado`, price: estimate.mid * 0.94, days: 23 },
                  { desc: `${form.type} ${form.typology} renovado`, price: estimate.mid * 1.08, days: 41 },
                  { desc: `${form.type} ${form.typology} para recuperar`, price: estimate.mid * 0.79, days: 67 },
                ].map((t, i) => (
                  <div key={i} className="flex items-center justify-between text-sm">
                    <div>
                      <p className="text-slate-300 text-xs">{t.desc} · {form.zone}</p>
                      <p className="text-slate-600 text-xs">Vendido há {t.days} dias</p>
                    </div>
                    <p className="font-bold text-slate-200">{formatPrice(t.price)}</p>
                  </div>
                ))}
              </div>
              <p className="text-xs text-slate-600 mt-3">* Dados ilustrativos para referência. O Frederico partilhará dados reais e verificados na reunião.</p>
            </div>

            {/* Booking */}
            <div className="bg-slate-900 border border-white/10 rounded-2xl p-6 space-y-5">
              <div>
                <p className="text-sm font-semibold text-white mb-1">Agendar avaliação presencial gratuita</p>
                <p className="text-slate-500 text-xs">O Frederico vai ao seu imóvel, analisa o mercado local e apresenta uma estratégia personalizada — sem compromisso.</p>
              </div>

              {/* Days */}
              <div>
                <p className="text-xs text-slate-400 mb-2">Escolha um dia</p>
                <div className="flex gap-2 flex-wrap">
                  {days.map((d, i) => (
                    <button
                      key={i} type="button"
                      onClick={() => setForm({ ...form, selectedDay: d, selectedTime: null })}
                      className={`flex flex-col items-center px-3 py-2 rounded-xl border text-xs transition-all ${
                        form.selectedDay?.toDateString() === d.toDateString()
                          ? 'bg-orange-500 border-orange-500 text-white'
                          : 'bg-white/5 border-white/10 text-slate-300 hover:border-white/30'
                      }`}
                    >
                      <span className="font-semibold">{getDayName(d)}</span>
                      <span className="text-[11px] opacity-70">{d.getDate()}/{d.getMonth() + 1}</span>
                    </button>
                  ))}
                </div>
              </div>

              {/* Times */}
              {form.selectedDay && (
                <div>
                  <p className="text-xs text-slate-400 mb-2">Horário preferido</p>
                  <div className="flex flex-wrap gap-2">
                    {TIME_SLOTS.map(t => (
                      <button
                        key={t} type="button"
                        onClick={() => setForm({ ...form, selectedTime: t })}
                        className={`px-3 py-1.5 rounded-xl text-xs font-medium border transition-all ${
                          form.selectedTime === t
                            ? 'bg-orange-500 border-orange-500 text-white'
                            : 'bg-white/5 border-white/10 text-slate-300 hover:border-white/30'
                        }`}
                      >
                        {t}
                      </button>
                    ))}
                  </div>
                </div>
              )}

              {/* Contact */}
              <div className="space-y-3 pt-2 border-t border-white/5">
                <p className="text-xs text-slate-400 font-semibold">Os seus dados de contacto</p>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                  <input
                    required
                    type="text"
                    placeholder="Nome completo *"
                    value={form.name}
                    onChange={e => setForm({ ...form, name: e.target.value })}
                    className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500"
                  />
                  <input
                    required
                    type="email"
                    placeholder="Email *"
                    value={form.email}
                    onChange={e => setForm({ ...form, email: e.target.value })}
                    className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500"
                  />
                </div>
                <input
                  type="tel"
                  placeholder="Telefone / WhatsApp"
                  value={form.phone}
                  onChange={e => setForm({ ...form, phone: e.target.value })}
                  className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500"
                />
              </div>

              {submitError && (
                <p className="text-red-400 text-xs">{submitError}</p>
              )}
              <button
                onClick={handleSubmit}
                disabled={!canSubmit || submitting}
                className="w-full bg-orange-500 hover:bg-orange-600 disabled:opacity-40 disabled:cursor-not-allowed text-white font-bold py-4 rounded-2xl transition-all"
              >
                {submitting
                  ? 'A confirmar...'
                  : form.selectedDay && form.selectedTime
                    ? `Confirmar reunião — ${getDayName(form.selectedDay)} ${form.selectedDay.getDate()}/${form.selectedDay.getMonth() + 1} às ${form.selectedTime}`
                    : 'Selecione dia e hora para agendar'}
              </button>
            </div>

            <button onClick={() => setStep(1)} className="text-slate-500 hover:text-white text-sm transition-colors">
              ← Alterar dados do imóvel
            </button>
          </div>
        )}

        {/* Success */}
        {submitted && (
          <div className="text-center space-y-6 py-12">
            <div className="w-16 h-16 bg-emerald-500/20 text-emerald-400 rounded-full flex items-center justify-center mx-auto text-3xl">✓</div>
            <div>
              <h2 className="text-2xl font-black text-emerald-400 mb-2">Reunião agendada!</h2>
              <p className="text-slate-400 text-sm">
                O Frederico vai confirmar o agendamento para{' '}
                <strong className="text-white">
                  {form.selectedDay && `${getDayName(form.selectedDay)} ${form.selectedDay.getDate()}/${form.selectedDay.getMonth() + 1} às ${form.selectedTime}`}
                </strong>{' '}
                por email ou telefone.
              </p>
            </div>
            <div className="bg-slate-900 border border-white/10 rounded-2xl p-5 text-left max-w-sm mx-auto">
              <p className="text-xs text-slate-500 uppercase tracking-wider mb-3">O que esperar na reunião</p>
              <ul className="space-y-2 text-sm text-slate-300">
                {['Análise presencial do imóvel', 'Relatório de mercado local com dados reais', 'Estratégia personalizada de venda ou compra', 'Sem compromisso, sem custos'].map((item, i) => (
                  <li key={i} className="flex items-center gap-2">
                    <span className="text-orange-500">→</span> {item}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}