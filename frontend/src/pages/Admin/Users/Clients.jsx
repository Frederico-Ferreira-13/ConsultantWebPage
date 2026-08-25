import { useState, useEffect, useCallback, startTransition } from "react";
import { useAuth } from "../../../context/AuthContext";
import { API_BASE_URL as API_BASE } from '../../../config/api';
import { ROLES } from "../../../config/roles";

const ROLE_LABELS = {
    [ROLES.ADMIN]: 'Admin',
    [ROLES.CONSULTANT]: 'Consultant',
    [ROLES.CLIENT]: 'Client',
};

const ROLE_STYLES = {
  [ROLES.ADMIN]: 'bg-orange-500/10 text-orange-400 border-orange-500/20',
  [ROLES.CONSULTANT]: 'bg-blue-500/10 text-blue-400 border-blue-500/20',
  [ROLES.CLIENT]: 'bg-white/5 text-slate-400 border-white/10',
};

function RoleBadge({ roleId }) {
  return (
    <span className={`text-[10px] font-semibold px-2 py-0.5 rounded-full border ${ROLE_STYLES[roleId] ?? ROLE_STYLES[ROLES.CLIENT]}`}>
      {ROLE_LABELS[roleId] ?? 'Desconhecido'}
    </span>
  );
}

export default function Clients() {
const { user, token } = useAuth();
  const isAdmin = user?.usersRoleId === ROLES.ADMIN;
 
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [roleFilter, setRoleFilter] = useState('Todos');
  const [actioningId, setActioningId] = useState(null);
 
  const fetchUsers = useCallback(async () => {
    if (!token) return;
 
    setLoading(true);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/users`, {
        headers: { Authorization: `Bearer ${token}` },
      });
 
      if (!res.ok) throw new Error('Não foi possível carregar os utilizadores.');
 
      const data = await res.json();
      setUsers(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(err.message || 'Não foi possível carregar os utilizadores.');
    } finally {
      setLoading(false);
    }
  }, [token]);
 
  useEffect(() => {
    startTransition(() => {
      fetchUsers();
    });
  }, [fetchUsers]);
 
  const handleToggleActive = async (targetUser) => {
    setActioningId(targetUser.userId);
    setError('');
    try {
      const action = targetUser.isActive ? 'deactivate' : 'activate';
      const res = await fetch(`${API_BASE}/users/${targetUser.userId}/${action}`, {
        method: 'PATCH',
        headers: { Authorization: `Bearer ${token}` },
      });
 
      if (!res.ok) throw new Error(await res.text());
 
      setUsers((prev) =>
        prev.map((u) => (u.userId === targetUser.userId ? { ...u, isActive: !u.isActive } : u))
      );
    } catch (err) {
      setError(err.message || 'Não foi possível alterar o estado desta conta.');
    } finally {
      setActioningId(null);
    }
  };
 
  const handleApprove = async (targetUser) => {
    setActioningId(targetUser.userId);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/users/${targetUser.userId}/approve`, {
        method: 'PATCH',
        headers: { Authorization: `Bearer ${token}` },
      });
 
      if (!res.ok) throw new Error(await res.text());
 
      setUsers((prev) =>
        prev.map((u) => (u.userId === targetUser.userId ? { ...u, isApproved: true } : u))
      );
    } catch (err) {
      setError(err.message || 'Não foi possível aprovar esta conta.');
    } finally {
      setActioningId(null);
    }
  };
 
  const roleFilterOptions = ['Todos', ROLES.ADMIN, ROLES.CONSULTANT, ROLES.CLIENT];
 
  const filteredUsers = roleFilter === 'Todos'
    ? users
    : users.filter((u) => u.usersRoleId === roleFilter);
 
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-white">Clientes e Utilizadores</h1>
        <p className="text-slate-500 text-sm mt-1">
          {users.length} utilizador{users.length !== 1 ? 'es' : ''} registado{users.length !== 1 ? 's' : ''}
        </p>
      </div>
 
      {/* Filtros por perfil */}
      <div className="flex items-center gap-2 flex-wrap">
        {roleFilterOptions.map((role) => (
          <button
            key={role}
            onClick={() => setRoleFilter(role)}
            className={`text-xs font-medium px-4 py-2 rounded-xl border transition-all ${
              roleFilter === role
                ? 'bg-orange-500 border-orange-500 text-white'
                : 'bg-white/5 border-white/10 text-slate-400 hover:border-white/30'
            }`}
          >
            {role === 'Todos' ? 'Todos' : ROLE_LABELS[role]}
          </button>
        ))}
      </div>
 
      {error && (
        <div className="bg-red-500/10 border border-red-500/30 text-red-400 rounded-xl p-4 text-sm">
          {error}
        </div>
      )}
 
      {/* Lista de utilizadores */}
      <div className="bg-slate-900 border border-white/10 rounded-2xl overflow-hidden">
        {loading ? (
          <div className="p-6 space-y-2">
            {[1, 2, 3, 4].map((i) => (
              <div key={i} className="h-16 bg-white/5 rounded-xl animate-pulse" />
            ))}
          </div>
        ) : filteredUsers.length === 0 ? (
          <p className="text-slate-600 text-sm p-6">Nenhum utilizador encontrado para este filtro.</p>
        ) : (
          <div className="divide-y divide-white/5">
            {filteredUsers.map((u) => (
              <div key={u.userId} className="px-6 py-4 flex flex-col md:flex-row md:items-center gap-3 md:gap-6">
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-1 flex-wrap">
                    <p className="text-white text-sm font-medium truncate">{u.fullName}</p>
                    <RoleBadge roleId={u.usersRoleId} />
                    {!u.isApproved && (
                      <span className="text-[10px] font-semibold px-2 py-0.5 rounded-full bg-yellow-500/10 text-yellow-400 border border-yellow-500/20">
                        Pendente
                      </span>
                    )}
                    {!u.isActive && (
                      <span className="text-[10px] font-semibold px-2 py-0.5 rounded-full bg-red-500/10 text-red-400 border border-red-500/20">
                        Inativo
                      </span>
                    )}
                  </div>
                  <p className="text-slate-500 text-xs truncate">{u.email} {u.phone ? `· ${u.phone}` : ''}</p>
                  <p className="text-slate-700 text-[11px] mt-1">
                    Registado em {new Date(u.createdAt).toLocaleDateString('pt-PT', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                  </p>
                </div>
 
                {isAdmin && (
                  <div className="flex items-center gap-2 shrink-0">
                    {!u.isApproved && (
                      <button
                        onClick={() => handleApprove(u)}
                        disabled={actioningId === u.userId}
                        className="text-xs bg-emerald-500/10 hover:bg-emerald-500/20 text-emerald-400 border border-emerald-500/20 px-3 py-1.5 rounded-lg transition-all disabled:opacity-50"
                      >
                        Aprovar
                      </button>
                    )}
                    <button
                      onClick={() => handleToggleActive(u)}
                      disabled={actioningId === u.userId || u.userId === user?.userId}
                      title={u.userId === user?.userId ? 'Não podes desativar a tua própria conta aqui' : ''}
                      className={`text-xs px-3 py-1.5 rounded-lg transition-all disabled:opacity-50 border ${
                        u.isActive
                          ? 'bg-red-500/10 hover:bg-red-500/20 text-red-400 border-red-500/20'
                          : 'bg-emerald-500/10 hover:bg-emerald-500/20 text-emerald-400 border-emerald-500/20'
                      }`}
                    >
                      {u.isActive ? 'Desativar' : 'Ativar'}
                    </button>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}