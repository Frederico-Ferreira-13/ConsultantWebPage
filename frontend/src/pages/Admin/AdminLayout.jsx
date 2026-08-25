import { useState } from 'react';
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/useAuth';

const NAV_GROUPS = [
  {
    label: 'Geral',
    items: [
      {
        to: '/admin',
        end: true,
        icon: (
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
              d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
          </svg>
        ),
        label: 'Visão geral',
      },
    ],
  },
  {
    label: 'Conteúdo',
    items: [
      {
        to: '/admin/gestaoimoveis',
        icon: (
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
              d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
          </svg>
        ),
        label: 'Imóveis',
      },
      {
        to: '/admin/posts',
        icon: (
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
              d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
        ),
        label: 'Posts',
      },
      {
        to: '/admin/sobre-mim',
        icon: (
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
              d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
          </svg>
        ),
        label: 'Sobre mim',
      },
      {
        to: '/admin/testemunhos',
        icon: (
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
              d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
          </svg>
        ),
        label: 'Testemunhos',
      },
    ],
  },
  {
    label: 'CRM',
    items: [
      {
        to: '/admin/leads',
        icon: (
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
              d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
        ),
        label: 'Leads',
      },
      {
        to: '/admin/avaliacoes',
        icon: (
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
              d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
          </svg>
        ),
        label: 'Avaliações',
      },
      {
        to: '/admin/disc',
        icon: (
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
              d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
          </svg>
        ),
        label: 'Perfis DISC',
      },
    ],
  },
  {
    label: 'Utilizadores',
    items: [
      {
        to: '/admin/clientes',
        icon: (
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
              d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
          </svg>
        ),
        label: 'Clientes',
      },
    ],
  },
];


export default function AdminLayout() {
    const { user, logout } = useAuth();
    const navigate = useNavigate();
    const [collapsed, setCollapsed] = useState(false);
    
    const handleLogout = () => {
        logout();
        navigate('/');
    };

    return (
        <div className="min-h-screen bg-slate-950 flex">
 
      {/* ── SIDEBAR ── */}
      <aside
        className={`flex flex-col shrink-0 border-r border-white/8 bg-slate-950 transition-all duration-300 ${
          collapsed ? 'w-16' : 'w-60'
        }`}
      >
        {/* Logo / topo */}
        <div className="flex items-center gap-3 px-4 py-5 border-b border-white/8">
          <Link to="/" className="flex items-center gap-3 group shrink-0">
            <div className="w-8 h-8 bg-orange-500 rounded-xl flex items-center justify-center shadow-lg shadow-orange-500/20 group-hover:rotate-6 transition-transform shrink-0">
              <span className="text-white font-black text-sm">F</span>
            </div>
            {!collapsed && (
              <div className="flex flex-col overflow-hidden">
                <span className="text-white font-semibold text-sm leading-none whitespace-nowrap">Admin</span>
                <span className="text-orange-500 text-[10px] tracking-widest uppercase leading-none mt-1 whitespace-nowrap">Painel</span>
              </div>
            )}
          </Link>
          <button
            onClick={() => setCollapsed(!collapsed)}
            className={`ml-auto text-slate-500 hover:text-white transition-colors ${collapsed ? 'hidden' : ''}`}
            aria-label="Recolher sidebar"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M11 19l-7-7 7-7m8 14l-7-7 7-7" />
            </svg>
          </button>
          {collapsed && (
            <button
              onClick={() => setCollapsed(false)}
              className="mx-auto text-slate-500 hover:text-white transition-colors"
              aria-label="Expandir sidebar"
            >
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 5l7 7-7 7M5 5l7 7-7 7" />
              </svg>
            </button>
          )}
        </div>
 
        {/* Navegação */}
        <nav className="flex-1 overflow-y-auto py-4 space-y-6 px-2">
          {NAV_GROUPS.map((group) => (
            <div key={group.label}>
              {!collapsed && (
                <p className="text-[10px] font-semibold uppercase tracking-widest text-slate-600 px-2 mb-2">
                  {group.label}
                </p>
              )}
              <ul className="space-y-0.5">
                {group.items.map((item) => (
                  <li key={item.to}>
                    <NavLink
                      to={item.to}
                      end={item.end}
                      className={({ isActive }) =>
                        `flex items-center gap-3 px-2 py-2 rounded-xl text-sm transition-all group ${
                          isActive
                            ? 'bg-orange-500/10 text-orange-400'
                            : 'text-slate-400 hover:text-white hover:bg-white/5'
                        }`
                      }
                    >
                      {({ isActive }) => (
                        <>
                          <span className={`shrink-0 ${isActive ? 'text-orange-400' : 'text-slate-500 group-hover:text-white'} transition-colors`}>
                            {item.icon}
                          </span>
                          {!collapsed && (
                            <span className="whitespace-nowrap">{item.label}</span>
                          )}
                        </>
                      )}
                    </NavLink>
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </nav>
 
        {/* Utilizador / logout */}
        <div className="border-t border-white/8 p-3">
          <div className={`flex items-center gap-3 ${collapsed ? 'justify-center' : ''}`}>
            <div className="w-8 h-8 rounded-xl bg-slate-800 border border-white/10 flex items-center justify-center shrink-0">
              <span className="text-white text-xs font-semibold">
                {user?.name?.charAt(0)?.toUpperCase() ?? 'A'}
              </span>
            </div>
            {!collapsed && (
              <div className="flex-1 min-w-0">
                <p className="text-white text-xs font-medium truncate">{user?.name ?? 'Admin'}</p>
                <p className="text-slate-500 text-[10px] truncate">{user?.email ?? ''}</p>
              </div>
            )}
            {!collapsed && (
              <button
                onClick={handleLogout}
                className="text-slate-500 hover:text-red-400 transition-colors shrink-0"
                aria-label="Sair"
                title="Sair"
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                    d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                </svg>
              </button>
            )}
          </div>
        </div>
      </aside>
 
      {/* ── CONTEÚDO PRINCIPAL ── */}
      <div className="flex-1 flex flex-col min-w-0">
 
        {/* Topbar */}
        <header className="h-14 border-b border-white/8 flex items-center justify-between px-6 bg-slate-950 shrink-0">
          <div className="flex items-center gap-3">
            {/* Botão collapse mobile / atalho */}
            <button
              onClick={() => setCollapsed(!collapsed)}
              className="text-slate-500 hover:text-white transition-colors md:hidden"
              aria-label="Menu"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
            <span className="text-slate-500 text-sm hidden md:block">
              Bem-vindo, <span className="text-white">{user?.name ?? 'Admin'}</span>
            </span>
          </div>
 
          <div className="flex items-center gap-3">
            {/* Notificações (placeholder para SignalR no futuro) */}
            <button
              className="relative text-slate-500 hover:text-white transition-colors"
              aria-label="Notificações"
              title="Notificações"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
                  d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
              </svg>
              {/* Badge de notificação — vai ligar ao SignalR */}
              <span className="absolute top-0 right-0 w-2 h-2 bg-orange-500 rounded-full" />
            </button>
 
            {/* Link para site público */}
            <Link
              to="/"
              className="text-slate-500 hover:text-white transition-colors text-xs flex items-center gap-1.5 border border-white/10 hover:border-white/20 px-3 py-1.5 rounded-lg"
            >
              <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                  d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
              </svg>
              Ver site
            </Link>
          </div>
        </header>
 
        {/* Área de conteúdo */}
        <main className="flex-1 overflow-auto p-6 text-white">
          <Outlet />
        </main>
      </div>
    </div>
  );
}