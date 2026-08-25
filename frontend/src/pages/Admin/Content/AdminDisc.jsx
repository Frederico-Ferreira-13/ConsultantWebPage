import { useState, useEffect, useCallback, startTransition } from 'react';
import { useAuth } from '../../../context/useAuth';
import { API_BASE_URL as API_BASE } from '../../../config/api';

const PROFILE_META = {
  D: { label: 'Dominante', emoji: '🦁', color: '#E54040', bg: 'bg-red-500/10', border: 'border-red-500/20', text: 'text-red-400' },
  I: { label: 'Influente', emoji: '🌟', color: '#F59E0B', bg: 'bg-amber-500/10', border: 'border-amber-500/20', text: 'text-amber-400' },
  S: { label: 'Estável', emoji: '🏡', color: '#10B981', bg: 'bg-emerald-500/10', border: 'border-emerald-500/20', text: 'text-emerald-400' },
  C: { label: 'Consciencioso', emoji: '🔬', color: '#6366F1', bg: 'bg-indigo-500/10', border: 'border-indigo-500/20', text: 'text-indigo-400' },
};

function extractProfile(message) {
  if (!message) return null;
  const match = message.match(/Perfil \w+ \(([DISC])\)/);
  return match ? match[1] : null;
}

export default function AdminDisc() {
  const { token } = useAuth();
  const [leads, setLeads] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeFilter, setActiveFilter] = useState('Todos');
 
  const fetchLeads = useCallback(async () => {
    if (!token) return;
 
    setLoading(true);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/leads`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) throw new Error();
      const data = await res.json();
      setLeads(Array.isArray(data) ? data : []);
    } catch {
      setError('Não foi possível carregar os resultados do DISC.');
    } finally {
      setLoading(false);
    }
  }, [token]);
 
  useEffect(() => {
    startTransition(() => {
      fetchLeads();
    });
  }, [fetchLeads]);
 
  const discLeads = leads
    .map((lead) => ({ ...lead, discProfile: extractProfile(lead.message) }))
    .filter((lead) => lead.discProfile);
 
  const counts = discLeads.reduce((acc, lead) => {
    acc[lead.discProfile] = (acc[lead.discProfile] ?? 0) + 1;
    return acc;
  }, {});
 
  const filteredLeads = activeFilter === 'Todos'
    ? discLeads
    : discLeads.filter((l) => l.discProfile === activeFilter);
 
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-white">Perfis DISC</h1>
        <p className="text-slate-500 text-sm mt-1">
          {discLeads.length} pessoa{discLeads.length !== 1 ? 's' : ''} fizeram o teste DISC
        </p>
      </div>
 
      {/* Distribuição por perfil */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {Object.entries(PROFILE_META).map(([key, meta]) => (
          <div key={key} className={`bg-slate-900 border rounded-2xl p-5 ${meta.border}`}>
            <div className="text-2xl mb-2">{meta.emoji}</div>
            <p className={`text-2xl font-black ${meta.text}`}>{counts[key] ?? 0}</p>
            <p className="text-slate-500 text-xs mt-1">{meta.label}</p>
          </div>
        ))}
      </div>
 
      {/* Filtros */}
      <div className="flex items-center gap-2 flex-wrap">
        {['Todos', 'D', 'I', 'S', 'C'].map((key) => (
          <button
            key={key}
            onClick={() => setActiveFilter(key)}
            className={`text-xs font-medium px-4 py-2 rounded-xl border transition-all ${
              activeFilter === key
                ? 'bg-orange-500 border-orange-500 text-white'
                : 'bg-white/5 border-white/10 text-slate-400 hover:border-white/30'
            }`}
          >
            {key === 'Todos' ? 'Todos' : `${PROFILE_META[key].emoji} ${PROFILE_META[key].label}`}
          </button>
        ))}
      </div>
 
      {error && (
        <div className="bg-red-500/10 border border-red-500/30 text-red-400 rounded-xl p-4 text-sm">
          {error}
        </div>
      )}
 
      {/* Lista */}
      <div className="bg-slate-900 border border-white/10 rounded-2xl overflow-hidden">
        {loading ? (
          <div className="p-6 space-y-2">
            {[1, 2, 3].map((i) => (
              <div key={i} className="h-16 bg-white/5 rounded-xl animate-pulse" />
            ))}
          </div>
        ) : filteredLeads.length === 0 ? (
          <p className="text-slate-600 text-sm p-6">Nenhum resultado do DISC encontrado para este filtro.</p>
        ) : (
          <div className="divide-y divide-white/5">
            {filteredLeads.map((lead) => {
              const meta = PROFILE_META[lead.discProfile];
              return (
                <div key={lead.inquiryId} className="px-6 py-4 flex items-center justify-between gap-4">
                  <div className="min-w-0 flex-1">
                    <div className="flex items-center gap-2 mb-1">
                      <p className="text-white text-sm font-medium truncate">{lead.clientName}</p>
                      <span className={`text-[10px] font-semibold px-2 py-0.5 rounded-full border ${meta.bg} ${meta.border} ${meta.text}`}>
                        {meta.emoji} {meta.label}
                      </span>
                    </div>
                    <p className="text-slate-500 text-xs truncate">
                      {lead.contact?.email ?? '—'}
                      {lead.contact?.phoneNumber ? ` · ${lead.contact.phoneNumber}` : ''}
                    </p>
                    <p className="text-slate-700 text-[11px] mt-1">
                      {lead.createdAt ? new Date(lead.createdAt).toLocaleDateString('pt-PT', { day: '2-digit', month: '2-digit', year: 'numeric' }) : ''}
                    </p>
                  </div>
                  <span className="text-[10px] font-semibold px-2.5 py-1 rounded-full border bg-white/5 text-slate-400 border-white/10 shrink-0">
                    {lead.status}
                  </span>
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}