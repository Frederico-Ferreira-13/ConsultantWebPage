import { useState, useEffect, useCallback, startTransition } from 'react';
import { useAuth } from '../../../context/useAuth';
import { API_BASE_URL as API_BASE } from '../../../config/api';

const CONSULTANT_ID = 1;

function Stars({ value }) {
  return (
    <div className="flex items-center gap-0.5">
      {[1, 2, 3, 4, 5].map((n) => (
        <svg
          key={n}
          className={`w-3.5 h-3.5 ${n <= value ? 'text-orange-500 fill-orange-500' : 'text-slate-700 fill-none'}`}
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

export default function AdminTestimonials() {
  const { token } = useAuth();
  const [tab, setTab] = useState('pending'); // 'pending' | 'published'
  const [pending, setPending] = useState([]);
  const [published, setPublished] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [actioningId, setActioningId] = useState(null);

  const fetchAll = useCallback(async () => {
    if (!token) return;

    setLoading(true);
    setError('');
    try {
      const headers = { Authorization: `Bearer ${token}` };
      const [pendingRes, publishedRes] = await Promise.all([
        fetch(`${API_BASE}/testimonials/consultant/${CONSULTANT_ID}/moderation`, { headers }),
        fetch(`${API_BASE}/testimonials/consultant/${CONSULTANT_ID}`, { headers }),
      ]);

      setPending(pendingRes.ok ? await pendingRes.json() : []);
      setPublished(publishedRes.ok ? await publishedRes.json() : []);
    } catch {
      setError('Não foi possível carregar os testemunhos.');
    } finally {
      setLoading(false);
    }
  }, [token]);

  useEffect(() => {
    startTransition(() => {
      fetchAll();
    });
  }, [fetchAll]);

  const handleSetVisibility = async (testimonialId, isVisible) => {
    setActioningId(testimonialId);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/testimonials/${testimonialId}/visibility`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ isVisible }),
      });

      if (!res.ok) throw new Error();

      // Move o item entre as duas listas localmente, sem recarregar tudo
      if (isVisible) {
        const moved = pending.find((t) => t.testimonialId === testimonialId);
        setPending((prev) => prev.filter((t) => t.testimonialId !== testimonialId));
        if (moved) setPublished((prev) => [{ ...moved, isVisible: true }, ...prev]);
      } else {
        const moved = published.find((t) => t.testimonialId === testimonialId);
        setPublished((prev) => prev.filter((t) => t.testimonialId !== testimonialId));
        if (moved) setPending((prev) => [{ ...moved, isVisible: false }, ...prev]);
      }
    } catch {
      setError('Não foi possível atualizar a visibilidade deste testemunho.');
    } finally {
      setActioningId(null);
    }
  };

  const handleDelete = async (testimonialId) => {
    if (!window.confirm('Eliminar este testemunho permanentemente?')) return;

    setActioningId(testimonialId);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/testimonials/${testimonialId}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!res.ok) throw new Error();

      setPending((prev) => prev.filter((t) => t.testimonialId !== testimonialId));
      setPublished((prev) => prev.filter((t) => t.testimonialId !== testimonialId));
    } catch {
      setError('Não foi possível eliminar este testemunho.');
    } finally {
      setActioningId(null);
    }
  };

  const list = tab === 'pending' ? pending : published;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-white">Testemunhos</h1>
        <p className="text-slate-500 text-sm mt-1">Modera os testemunhos submetidos pelos clientes.</p>
      </div>

      {/* Tabs */}
      <div className="flex items-center gap-2">
        <button
          onClick={() => setTab('pending')}
          className={`text-xs font-medium px-4 py-2 rounded-xl border transition-all ${
            tab === 'pending'
              ? 'bg-orange-500 border-orange-500 text-white'
              : 'bg-white/5 border-white/10 text-slate-400 hover:border-white/30'
          }`}
        >
          Pendentes <span className="ml-1.5 opacity-70">({pending.length})</span>
        </button>
        <button
          onClick={() => setTab('published')}
          className={`text-xs font-medium px-4 py-2 rounded-xl border transition-all ${
            tab === 'published'
              ? 'bg-orange-500 border-orange-500 text-white'
              : 'bg-white/5 border-white/10 text-slate-400 hover:border-white/30'
          }`}
        >
          Publicados <span className="ml-1.5 opacity-70">({published.length})</span>
        </button>
      </div>

      {error && (
        <div className="bg-red-500/10 border border-red-500/30 text-red-400 rounded-xl p-4 text-sm">
          {error}
        </div>
      )}

      <div className="bg-slate-900 border border-white/10 rounded-2xl overflow-hidden">
        {loading ? (
          <div className="p-6 space-y-2">
            {[1, 2, 3].map((i) => (
              <div key={i} className="h-20 bg-white/5 rounded-xl animate-pulse" />
            ))}
          </div>
        ) : list.length === 0 ? (
          <p className="text-slate-600 text-sm p-6">
            {tab === 'pending' ? 'Não há testemunhos por moderar.' : 'Ainda não há testemunhos publicados.'}
          </p>
        ) : (
          <div className="divide-y divide-white/5">
            {list.map((t) => (
              <div key={t.testimonialId} className="px-6 py-4 flex flex-col md:flex-row md:items-start gap-3 md:gap-6">
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-1">
                    <p className="text-white text-sm font-medium">{t.clientName}</p>
                    <Stars value={t.rating} />
                  </div>
                  <p className="text-slate-400 text-sm">{t.testimonialText}</p>
                  <p className="text-slate-700 text-[11px] mt-1">
                    {t.createdAt ? new Date(t.createdAt).toLocaleDateString('pt-PT', { day: '2-digit', month: '2-digit', year: 'numeric' }) : ''}
                  </p>
                </div>

                <div className="flex items-center gap-2 shrink-0">
                  {tab === 'pending' ? (
                    <button
                      onClick={() => handleSetVisibility(t.testimonialId, true)}
                      disabled={actioningId === t.testimonialId}
                      className="text-xs bg-emerald-500/10 hover:bg-emerald-500/20 text-emerald-400 border border-emerald-500/20 px-3 py-1.5 rounded-lg transition-all disabled:opacity-50"
                    >
                      Aprovar
                    </button>
                  ) : (
                    <button
                      onClick={() => handleSetVisibility(t.testimonialId, false)}
                      disabled={actioningId === t.testimonialId}
                      className="text-xs bg-white/5 hover:bg-white/10 text-slate-300 border border-white/10 px-3 py-1.5 rounded-lg transition-all disabled:opacity-50"
                    >
                      Ocultar
                    </button>
                  )}
                  <button
                    onClick={() => handleDelete(t.testimonialId)}
                    disabled={actioningId === t.testimonialId}
                    className="text-xs bg-red-500/10 hover:bg-red-500/20 text-red-400 border border-red-500/20 px-3 py-1.5 rounded-lg transition-all disabled:opacity-50"
                  >
                    Eliminar
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}