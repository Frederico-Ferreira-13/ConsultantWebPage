import { useState, useEffect, useCallback, startTransition } from 'react';
import { useAuth } from '../../../context/useAuth';
import { API_BASE_URL as API_BASE } from '../../../config/api';
import { ROLES } from '../../../config/roles';

const STATUS_OPTIONS = ['Novo', 'Em contacto', 'Fechado'];

const STATUS_STYLES = {
  Novo: 'bg-blue-500/10 text-blue-400 border-blue-500/20',
  'Em contacto': 'bg-orange-500/10 text-orange-400 border-orange-500/20',
  Fechado: 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20',
};

function StatusBadge({ status }) {
  return (
    <span className={`text-[10px] font-semibold px-2.5 py-1 rounded-full border ${STATUS_STYLES[status] ?? 'bg-white/5 text-slate-400 border-white/10'}`}>
      {status}
    </span>
  );
}

export default function Leads() {
  const { user, token } = useAuth();
  const isAdmin = user?.usersRoleId === ROLES.ADMIN;

  const [leads, setLeads] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeFilter, setActiveFilter] = useState('Todos');
  const [updatingId, setUpdatingId] = useState(null);

  const fetchLeads = useCallback(async () => {
    if (!token) return;

    setLoading(true);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/leads`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!res.ok) throw new Error('Não foi possível carregar os leads.');

      const data = await res.json();
      setLeads(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(err.message || 'Não foi possível carregar os leads.');
    } finally {
      setLoading(false);
    }
  }, [token]);

  useEffect(() => {
    startTransition(() => {
      fetchLeads();
    });
  }, [fetchLeads]);

  const handleStatusChange = async (leadId, newStatus) => {
    setUpdatingId(leadId);
    try {
      const res = await fetch(`${API_BASE}/leads/${leadId}/status`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ newStatus }),
      });

      if (!res.ok) throw new Error();

      setLeads((prev) =>
        prev.map((l) => (l.inquiryId === leadId ? { ...l, status: newStatus } : l))
      );
    } catch {
      setError('Não foi possível atualizar o estado deste lead.');
    } finally {
      setUpdatingId(null);
    }
  };

  const handleAssignToMe = async (leadId) => {
    if (!user?.userId) return;

    setUpdatingId(leadId);
    try {
      const res = await fetch(`${API_BASE}/leads/${leadId}/assign`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ consultantId: user.userId }),
      });

      if (!res.ok) throw new Error();

      setLeads((prev) =>
        prev.map((l) => (l.inquiryId === leadId ? { ...l, consultantId: user.userId } : l))
      );
    } catch {
      setError('Não foi possível atribuir este lead.');
    } finally {
      setUpdatingId(null);
    }
  };

  const filteredLeads = activeFilter === 'Todos'
    ? leads
    : leads.filter((l) => l.status === activeFilter);

  const counts = STATUS_OPTIONS.reduce((acc, status) => {
    acc[status] = leads.filter((l) => l.status === status).length;
    return acc;
  }, {});

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-white">Leads</h1>
          <p className="text-slate-500 text-sm mt-1">
            {leads.length} lead{leads.length !== 1 ? 's' : ''} no total
          </p>
        </div>
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
            {status}
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

      {/* Lista de leads */}
      <div className="bg-slate-900 border border-white/10 rounded-2xl overflow-hidden">
        {loading ? (
          <div className="p-6 space-y-2">
            {[1, 2, 3, 4].map((i) => (
              <div key={i} className="h-16 bg-white/5 rounded-xl animate-pulse" />
            ))}
          </div>
        ) : filteredLeads.length === 0 ? (
          <p className="text-slate-600 text-sm p-6">Nenhum lead encontrado para este filtro.</p>
        ) : (
          <div className="divide-y divide-white/5">
            {filteredLeads.map((lead) => (
              <div key={lead.inquiryId} className="px-6 py-4 flex flex-col md:flex-row md:items-center gap-3 md:gap-6">
                {/* Info principal */}
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-1">
                    <p className="text-white text-sm font-medium truncate">{lead.clientName}</p>
                    <StatusBadge status={lead.status} />
                    {!lead.consultantId && (
                      <span className="text-[10px] font-semibold px-2 py-0.5 rounded-full bg-white/5 text-slate-500 border border-white/10">
                        Sem consultor
                      </span>
                    )}
                  </div>
                  <p className="text-slate-500 text-xs truncate">
                    {lead.contact?.email ?? '—'}
                    {lead.contact?.phoneNumber ? ` · ${lead.contact.phoneNumber}` : ''}
                  </p>
                  {lead.message && (
                    <p className="text-slate-600 text-xs mt-1 line-clamp-2">{lead.message}</p>
                  )}
                  <p className="text-slate-700 text-[11px] mt-1">
                    {lead.createdAt ? new Date(lead.createdAt).toLocaleDateString('pt-PT', { day: '2-digit', month: '2-digit', year: 'numeric' }) : ''}
                  </p>
                </div>

                {/* Ações */}
                <div className="flex items-center gap-2 shrink-0">
                  <select
                    value={lead.status}
                    onChange={(e) => handleStatusChange(lead.inquiryId, e.target.value)}
                    disabled={updatingId === lead.inquiryId}
                    className="bg-slate-950 border border-white/10 rounded-lg px-3 py-1.5 text-xs text-white focus:outline-none focus:border-orange-500 disabled:opacity-50"
                  >
                    {STATUS_OPTIONS.map((s) => (
                      <option key={s} value={s}>{s}</option>
                    ))}
                  </select>

                  {isAdmin && !lead.consultantId && (
                    <button
                      onClick={() => handleAssignToMe(lead.inquiryId)}
                      disabled={updatingId === lead.inquiryId}
                      className="text-xs bg-white/5 hover:bg-white/10 border border-white/10 text-slate-300 px-3 py-1.5 rounded-lg transition-all disabled:opacity-50 whitespace-nowrap"
                    >
                      Atribuir a mim
                    </button>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}