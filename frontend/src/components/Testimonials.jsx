import { useState, useEffect, useCallback, startTransition } from 'react';
import { API_BASE_URL as API_BASE } from '../config/api';

const CONSULTANT_ID = 1;

function Stars({ value, size = 'w-4 h-4' }) {
  return (
    <div className="flex items-center gap-0.5">
      {[1, 2, 3, 4, 5].map((n) => (
        <svg
          key={n}
          className={`${size} ${n <= value ? 'text-orange-500 fill-orange-500' : 'text-slate-700 fill-none'}`}
          viewBox="0 0 24 24"
          stroke="currentColor"
          strokeWidth="1.5"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z"
          />
        </svg>
      ))}
    </div>
  );
}

function StarInput({ value, onChange }) {
  const [hover, setHover] = useState(0);
  return (
    <div className="flex items-center gap-1">
      {[1, 2, 3, 4, 5].map((n) => (
        <button
          key={n}
          type="button"
          onMouseEnter={() => setHover(n)}
          onMouseLeave={() => setHover(0)}
          onClick={() => onChange(n)}
        >
          <Stars value={hover || value} size="w-6 h-6" />
        </button>
      ))}
    </div>
  );
}
 
export default function Testimonials() {
  const [testimonials, setTestimonials] = useState([]);
  const [loading, setLoading] = useState(true);
 
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ clientName: '', testimonialText: '', rating: 0 });
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState('');
  const [submitted, setSubmitted] = useState(false);
 
  const fetchTestimonials = useCallback(async () => {
    setLoading(true);
    try {
      const res = await fetch(`${API_BASE}/testimonials/consultant/${CONSULTANT_ID}`);
      if (!res.ok) throw new Error();
      const data = await res.json();
      setTestimonials(Array.isArray(data) ? data : []);
    } catch {
      setTestimonials([]);
    } finally {
      setLoading(false);
    }
  }, []);
 
  useEffect(() => {
    startTransition(() => {
      fetchTestimonials();
    });
  }, [fetchTestimonials]);
 
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (form.rating === 0) {
      setSubmitError('Escolha uma classificação de 1 a 5 estrelas.');
      return;
    }
 
    setSubmitting(true);
    setSubmitError('');
    try {
      const res = await fetch(`${API_BASE}/testimonials`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          consultantId: CONSULTANT_ID,
          clientName: form.clientName,
          testimonialText: form.testimonialText,
          rating: form.rating,
        }),
      });
 
      if (!res.ok) {
        const errorText = await res.text();
        throw new Error(errorText || 'Não foi possível enviar o testemunho.');
      }
 
      setSubmitted(true);
      setForm({ clientName: '', testimonialText: '', rating: 0 });
    } catch (err) {
      setSubmitError(err.message || 'Não foi possível enviar o testemunho.');
    } finally {
      setSubmitting(false);
    }
  };
 
  return (
    <section className="bg-slate-950 py-20 px-6 md:px-12">
      <div className="max-w-5xl mx-auto">
        <div className="text-center mb-12">
          <span className="text-orange-500 font-bold tracking-widest text-xs uppercase mb-3 block">
            O que dizem os clientes
          </span>
          <h2 className="text-3xl md:text-4xl font-black text-white">Testemunhos</h2>
        </div>
 
        {loading ? (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            {[1, 2, 3].map((i) => (
              <div key={i} className="h-40 bg-white/5 rounded-2xl animate-pulse" />
            ))}
          </div>
        ) : testimonials.length === 0 ? (
          <p className="text-slate-600 text-sm text-center">Ainda não há testemunhos publicados.</p>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-12">
            {testimonials.map((t) => (
              <div key={t.testimonialId} className="bg-slate-900 border border-white/10 rounded-2xl p-6 flex flex-col">
                <Stars value={t.rating} />
                <p className="text-slate-300 text-sm leading-relaxed mt-4 flex-1">
                  &ldquo;{t.testimonialText}&rdquo;
                </p>
                <p className="text-white text-sm font-semibold mt-4">{t.clientName}</p>
              </div>
            ))}
          </div>
        )}
 
        {/* Deixar um testemunho */}
        <div className="max-w-lg mx-auto">
          {!showForm && !submitted && (
            <button
              onClick={() => setShowForm(true)}
              className="w-full text-center text-orange-400 hover:text-orange-300 text-sm font-medium transition-colors"
            >
              Já trabalhou comigo? Deixe o seu testemunho →
            </button>
          )}
 
          {showForm && !submitted && (
            <form onSubmit={handleSubmit} className="bg-slate-900 border border-white/10 rounded-2xl p-6 space-y-4">
              <p className="text-sm font-semibold text-white mb-1">Deixe o seu testemunho</p>
              <div>
                <label className="text-xs text-slate-500 mb-1.5 block">A sua classificação</label>
                <StarInput value={form.rating} onChange={(rating) => setForm({ ...form, rating })} />
              </div>
              <input
                required
                type="text"
                placeholder="O seu nome"
                value={form.clientName}
                onChange={(e) => setForm({ ...form, clientName: e.target.value })}
                className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500"
              />
              <textarea
                required
                rows={4}
                minLength={10}
                placeholder="Conte a sua experiência (mín. 10 caracteres)..."
                value={form.testimonialText}
                onChange={(e) => setForm({ ...form, testimonialText: e.target.value })}
                className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 resize-none"
              />
              {submitError && <p className="text-red-400 text-xs">{submitError}</p>}
              <div className="flex items-center gap-3">
                <button
                  type="submit"
                  disabled={submitting}
                  className="flex-1 bg-orange-500 hover:bg-orange-600 disabled:opacity-50 text-white font-semibold py-3 rounded-xl transition-all"
                >
                  {submitting ? 'A enviar...' : 'Enviar testemunho'}
                </button>
                <button
                  type="button"
                  onClick={() => setShowForm(false)}
                  className="text-slate-500 hover:text-white text-sm px-4"
                >
                  Cancelar
                </button>
              </div>
            </form>
          )}
 
          {submitted && (
            <div className="bg-slate-900 border border-emerald-500/20 rounded-2xl p-6 text-center">
              <div className="w-10 h-10 bg-emerald-500/20 text-emerald-400 rounded-full flex items-center justify-center mx-auto mb-3 text-lg">✓</div>
              <p className="font-semibold text-emerald-400 mb-1">Obrigado pelo seu testemunho!</p>
              <p className="text-slate-500 text-xs">Vai ficar visível assim que for revisto.</p>
            </div>
          )}
        </div>
      </div>
    </section>
  );
}