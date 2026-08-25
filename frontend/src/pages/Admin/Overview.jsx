import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../context/useAuth';
import { API_BASE_URL as API_BASE } from '../../config/api';

function StatCard({ label, value, icon, accent = 'orange', loading }) {
  const accentClasses = {
    orange: 'text-orange-400 bg-orange-500/10 border-orange-500/20',
    blue: 'text-blue-400 bg-blue-500/10 border-blue-500/20',
    red: 'text-red-400 bg-red-500/10 border-red-500/20',
  };

  return (
    <div className="bg-slate-900 border border-white/10 rounded-2xl p-6 flex items-start justify-between">
      <div>
        <p className="text-slate-500 text-xs uppercase tracking-wider font-semibold mb-2">{label}</p>
        {loading ? (
          <div className="h-8 w-14 bg-white/5 rounded animate-pulse" />
        ) : (
          <p className="text-3xl font-black text-white">{value}</p>
        )}
      </div>
      <div className={`p-3 rounded-xl border shrink-0 ${accentClasses[accent]}`}>
        {icon}
      </div>
    </div>
  );
}

const LEAD_STATUS_STYLES = {
  Novo: 'bg-blue-500/10 text-blue-400 border-blue-500/20',
  'Em contacto': 'bg-orange-500/10 text-orange-400 border-orange-500/20',
  Fechado: 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20',
};

const QUICK_LINKS = [
  { to: '/admin/gestaoimoveis', label: 'Gerir imóveis' },
  { to: '/admin/leads', label: 'Ver leads' },
  { to: '/admin/avaliacoes', label: 'Avaliações pendentes' },
  { to: '/admin/definicoes', label: 'Aprovar utilizadores' },
];

export default function Overview() {
  const { user, token } = useAuth();

  const [properties, setProperties] = useState([]);
  const [leads, setLeads] = useState([]);
  const [pendingEvaluations, setPendingEvaluations] = useState([]);
  const [pendingUsers, setPendingUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let isMounted = true;

    async function fetchAll() {
      if (!user?.userId || !token) return;

      setLoading(true);
      setError('');
      try {
        const headers = { Authorization: `Bearer ${token}` };

        const [propsRes, leadsRes, evalRes, usersRes] = await Promise.all([
          fetch(`${API_BASE}/realestate`, { headers }),
          fetch(`${API_BASE}/leads`, { headers }),
          fetch(`${API_BASE}/PropertyEvaluation/consultant/${user.userId}/pending`, { headers }),
          fetch(`${API_BASE}/users/pending`, { headers }),
        ]);

        if (!isMounted) return;

        setProperties(propsRes.ok ? await propsRes.json() : []);
        setLeads(leadsRes.ok ? await leadsRes.json() : []);

        const evalData = evalRes.ok ? await evalRes.json() : { hasProfile: [] };
        setPendingEvaluations(Array.isArray(evalData) ? evalData : evalData.hasProfile ?? []);

        setPendingUsers(usersRes.ok ? await usersRes.json() : []);
      } catch {
        if (isMounted) {
          setError('Não foi possível carregar os dados do painel.');
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    }

    fetchAll();

    return () => {
      isMounted = false;
    };
  }, [user?.userId, token]);

  const newLeadsCount = leads.filter((l) => l.status === 'Novo').length;

  const recentLeads = [...leads]
    .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
    .slice(0, 5);

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-white">
          Bem-vindo, {user?.name?.split(' ')[0] ?? 'Admin'}
        </h1>
        <p className="text-slate-500 text-sm mt-1">Aqui está o que se passa na tua plataforma hoje.</p>
      </div>

      {error && (
        <div className="bg-red-500/10 border border-red-500/30 text-red-400 rounded-xl p-4 text-sm">
          {error}
        </div>
      )}

      {/* KPIs */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard
          label="Imóveis ativos"
          value={properties.length}
          loading={loading}
          accent="orange"
          icon={
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
            </svg>
          }
        />
        <StatCard
          label="Leads por tratar"
          value={newLeadsCount}
          loading={loading}
          accent="blue"
          icon={
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
          }
        />
        <StatCard
          label="Avaliações pendentes"
          value={pendingEvaluations.length}
          loading={loading}
          accent="orange"
          icon={
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
            </svg>
          }
        />
        <StatCard
          label="Utilizadores por aprovar"
          value={pendingUsers.length}
          loading={loading}
          accent="red"
          icon={
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
            </svg>
          }
        />
      </div>

      {/* Leads recentes + acesso rápido */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 bg-slate-900 border border-white/10 rounded-2xl overflow-hidden">
          <div className="px-6 py-4 border-b border-white/5 flex items-center justify-between">
            <h2 className="font-semibold text-white text-sm">Leads recentes</h2>
            <Link to="/admin/leads" className="text-xs text-orange-400 hover:text-orange-300 font-medium">
              Ver todos →
            </Link>
          </div>

          {loading ? (
            <div className="p-6 space-y-3">
              {[1, 2, 3].map((i) => (
                <div key={i} className="h-12 bg-white/5 rounded-xl animate-pulse" />
              ))}
            </div>
          ) : recentLeads.length === 0 ? (
            <p className="text-slate-600 text-sm p-6">Ainda não há leads registados.</p>
          ) : (
            <div className="divide-y divide-white/5">
              {recentLeads.map((lead) => (
                <div key={lead.inquiryId} className="px-6 py-4 flex items-center justify-between gap-4">
                  <div className="min-w-0">
                    <p className="text-white text-sm font-medium truncate">{lead.clientName}</p>
                    <p className="text-slate-500 text-xs truncate">
                      {lead.contact?.email ?? lead.message?.slice(0, 60) ?? '—'}
                    </p>
                  </div>
                  <span
                    className={`shrink-0 text-[10px] font-semibold px-2.5 py-1 rounded-full border ${
                      LEAD_STATUS_STYLES[lead.status] ?? 'bg-white/5 text-slate-400 border-white/10'
                    }`}
                  >
                    {lead.status}
                  </span>
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="bg-slate-900 border border-white/10 rounded-2xl p-6 space-y-3">
          <h2 className="font-semibold text-white text-sm mb-2">Acesso rápido</h2>
          {QUICK_LINKS.map((link) => (
            <Link
              key={link.to}
              to={link.to}
              className="flex items-center justify-between bg-white/5 hover:bg-white/10 border border-white/5 rounded-xl px-4 py-3 text-sm text-slate-300 hover:text-white transition-all"
            >
              {link.label}
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 5l7 7-7 7" />
              </svg>
            </Link>
          ))}
        </div>
      </div>
    </div>
  );
}