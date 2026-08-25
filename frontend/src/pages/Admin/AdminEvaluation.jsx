import { useState, useEffect, useCallback, startTransition } from 'react';
import { useAuth } from '../../../context/useAuth';
import { API_BASE_URL as API_BASE } from '../../../config/api';

const STATUS_OPTIONS = ['Pending', 'Completed', 'Cancelled'];

const STATUS_LABELS = {
  Pending: 'Pendente',
  Completed: 'Concluída',
  Cancelled: 'Cancelada',
};

const STATUS_STYLES = {
  Pending: 'bg-orange-500/10 text-orange-400 border-orange-500/20',
  Completed: 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20',
  Cancelled: 'bg-white/5 text-slate-400 border-white/10',
};

function StatusBadge({ status }) {
  return (
    <span className={`text-[10px] font-semibold px-2.5 py-1 rounded-full border ${STATUS_STYLES[status] ?? 'bg-white/5 text-slate-400 border-white/10'}`}>
      {STATUS_LABELS[status] ?? status}
    </span>
  );
}

export default function AdminEvaluations() {
  const { token } = useAuth();
 
  const [evaluations, setEvaluations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeFilter, setActiveFilter] = useState('Todos');
  const [updatingId, setUpdatingId] = useState(null);
 
  const fetchEvaluations = useCallback(async () => {
    if (!token) return;
 
    setLoading(true);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/PropertyEvaluation`, {
        headers: { Authorization: `Bearer ${token}` },
      });
 
      if (!res.ok) throw new Error('Não foi possível carregar as avaliações.');
 
      const data = await res.json();
      setEvaluations(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(err.message || 'Não foi possível carregar as avaliações.');
    } finally {
      setLoading(false);
    }
  }, [token]);
 
  useEffect(() => {
    startTransition(() => {
      fetchEvaluations();
    });
  }, [fetchEvaluations]);
 
  const handleStatusChange = async (evaluationId, newStatus) => {
    setUpdatingId(evaluationId);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/PropertyEvaluation/${evaluationId}/status`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ newStatus }),
      });
 
      if (!res.ok) throw new Error();
 
      setEvaluations((prev) =>
        prev.map((e) => (e.evaluationId === evaluationId ? { ...e, status: newStatus } : e))
      );
    } catch {
      setError('Não foi possível atualizar o estado desta avaliação.');
    } finally {
      setUpdatingId(null);
    }
  };
 
  const handleDelete = async (evaluationId) => {
    if (!window.confirm('Eliminar este pedido de avaliação permanentemente?')) return;
 
    setUpdatingId(evaluationId);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/PropertyEvaluation/${evaluationId}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${token}` },
      });
 
      if (!res.ok) throw new Error();
 
      setEvaluations((prev) => prev.filter((e) => e.evaluationId !== evaluationId));
    } catch {
      setError('Não foi possível eliminar esta avaliação.');
    } finally {
      setUpdatingId(null);
    }
  };
 
  const filteredEvaluations = activeFilter === 'Todos'
    ? evaluations
    : evaluations.filter((e) => e.status === activeFilter);
 
  const counts = STATUS_OPTIONS.reduce((acc, status) => {
    acc[status] = evaluations.filter((e) => e.status === status).length;
    return acc;
  }, {});
 
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-white">Avaliações de Imóvel</h1>
        <p className="text-slate-500 text-sm mt-1">
          {evaluations.length} pedido{evaluations.length !== 1 ? 's' : ''} de avaliação no total
        </p>
      </div>
 
      {/* Filtros por estado */}
      <div className="flex items-center gap-2 flex-wrap">
        {['Todos', ...STATUS_OPTIONS].map((status) => (
          <button
            key={status}
            onClick={() => setActiveFilter(status)}
            className={`text-xs font-medium px-4 py-2 rounded-xl border transition-all ${
              activeFilter === status
                ? 'bg-orange-500 border-orange-500 text-white'
                : 'bg-white/5 border-white/10 text-slate-400 hover:border-white/30'
            }`}
          >
            {status === 'Todos' ? 'Todos' : STATUS_LABELS[status]}
            {status !== 'Todos' && (
              <span className="ml-1.5 opacity-70">({counts[status] ?? 0})</span>
            )}
          </button>
        ))}
      </div>
 
      {error && (
        <div className="bg-red-500/10 border border-red-500/30 text-red-400 rounded-xl p-4 text-sm">
          {error}
        </div>
      )}
 
      {/* Lista de avaliações */}
      <div className="bg-slate-900 border border-white/10 rounded-2xl overflow-hidden">
        {loading ? (
          <div className="p-6 space-y-2">
            {[1, 2, 3, 4].map((i) => (
              <div key={i} className="h-20 bg-white/5 rounded-xl animate-pulse" />
            ))}
          </div>
        ) : filteredEvaluations.length === 0 ? (
          <p className="text-slate-600 text-sm p-6">Nenhuma avaliação encontrada para este filtro.</p>
        ) : (
          <div className="divide-y divide-white/5">
            {filteredEvaluations.map((ev) => (
              <div key={ev.evaluationId} className="px-6 py-4 flex flex-col md:flex-row md:items-start gap-3 md:gap-6">
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-1 flex-wrap">
                    <p className="text-white text-sm font-medium truncate">{ev.clientName}</p>
                    <StatusBadge status={ev.status} />
                  </div>
                  <p className="text-slate-500 text-xs truncate">{ev.clientContact}</p>
                  <p className="text-slate-400 text-xs mt-1">
                    {[ev.propertyType, ev.typology, ev.area ? `${ev.area} m²` : null, ev.condition]
                      .filter(Boolean)
                      .join(' · ')}
                  </p>
                  {ev.address && <p className="text-slate-600 text-xs mt-0.5">📍 {ev.address}</p>}
                  {ev.visitPreference && (
                    <p className="text-orange-400/80 text-xs mt-1">🗓 Visita preferida: {ev.visitPreference}</p>
                  )}
                  <p className="text-slate-700 text-[11px] mt-1">
                    {ev.createdAt ? new Date(ev.createdAt).toLocaleDateString('pt-PT', { day: '2-digit', month: '2-digit', year: 'numeric' }) : ''}
                  </p>
                </div>
 
                <div className="flex items-center gap-2 shrink-0">
                  <select
                    value={ev.status}
                    onChange={(e) => handleStatusChange(ev.evaluationId, e.target.value)}
                    disabled={updatingId === ev.evaluationId}
                    className="bg-slate-950 border border-white/10 rounded-lg px-3 py-1.5 text-xs text-white focus:outline-none focus:border-orange-500 disabled:opacity-50"
                  >
                    {STATUS_OPTIONS.map((s) => (
                      <option key={s} value={s}>{STATUS_LABELS[s]}</option>
                    ))}
                  </select>
                  <button
                    onClick={() => handleDelete(ev.evaluationId)}
                    disabled={updatingId === ev.evaluationId}
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