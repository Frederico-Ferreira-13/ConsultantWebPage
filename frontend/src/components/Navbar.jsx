import { Link, useNavigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { useAuth } from '../context/useAuth';

export default function Navbar() {  
  const [isScrolled, setIsScrolled] = useState(false);
  const { user, logout } = useAuth();  
  const navigate = useNavigate();

  useEffect(() => {    
    const handleScroll = () => setIsScrolled(window.scrollY > 50);
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  const handleLogout = () => {
    logout()
    navigate('/');
  };

  const handleAvaliarClick = (e) => {
    if(!user) {
      e.preventDefault();
      navigate('/login');
    }
  };

  const dashboardLink = user?.usersRoleId === 1 ? '/admin' : '/dashboard/favoritos';
 
  return (
    <nav className={`fixed z-50 bg-slate-950/70 backdrop-blur-2xl border border-white/10 rounded-2xl shadow-[0_8px_30px_rgb(0,0,0,0.4)] transition-all duration-500 ease-in-out ${
      isScrolled ? 'top-6 right-6 w-16 py-6 flex flex-col items-center' : 'top-4 left-1/2 -translate-x-1/2 w-[95%] max-w-6xl px-6 py-4 flex flex-row items-center justify-between'
    }`}>
      
      {/* 1. LOGO */}
      <Link to="/" className="flex items-center space-x-2 group cursor-pointer shrink-0">
        <div className="w-10 h-10 bg-linear-to-tr from-orange-500 to-orange-600 rounded-xl flex items-center justify-center shadow-lg shadow-orange-500/20 group-hover:rotate-6 transition-transform">
          <span className="text-white font-black text-lg">F</span>
        </div>
        {!isScrolled && (
          <div className="flex flex-col">
            <span className="text-white font-bold tracking-tight text-sm leading-none">FREDERICO</span>
            <span className="text-orange-500 text-[9px] tracking-[0.2em] font-semibold uppercase mt-1">Consultor</span>
          </div>
        )}
      </Link>
 
      {/* 2. LINKS PRINCIPAIS */}
      <div className={`flex items-center transition-all duration-500 ${isScrolled ? 'flex-col gap-6 mt-8' : 'hidden md:flex flex-row gap-8'}`}>
        <NavLink to="/imoveis" icon="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" label="Imóveis" isScrolled={isScrolled} />
        <NavLink to="/sobre" icon="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" label="Sobre Mim" isScrolled={isScrolled} />
        <NavLink to="/observatorio" icon="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" label="Mercado" isScrolled={isScrolled} />
        
        {user ? (
          /* Utilizador logado — mostra link para o dashboard e logout */
          <>
            <Link
              to={dashboardLink}
              className="group flex items-center justify-center text-slate-300 hover:text-white transition-colors"
              title={user.usersRoleId === 1 ? 'Painel Admin' : 'Área Pessoal'}
            >
              <div className="p-3 bg-white/5 rounded-xl border border-white/5 group-hover:bg-white/10 transition-all">
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                    d={user.usersRoleId === 1
                      ? "M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                      : "M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
                    }
                  />
                </svg>
              </div>
              {!isScrolled && (
                <span className="ml-3 text-sm font-medium">
                  {user.usersRoleId === 1 ? 'Admin' : 'Área Pessoal'}
                </span>
              )}
            </Link>
 
            <button
              onClick={handleLogout}
              className="group flex items-center justify-center text-slate-300 hover:text-red-500 transition-colors"
              title="Sair"
            >
              <div className="p-3 bg-white/5 rounded-xl border border-white/5 group-hover:bg-red-500/10 transition-all">
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                    d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                </svg>
              </div>
              {!isScrolled && <span className="ml-3 text-sm font-medium">Sair</span>}
            </button>
          </>
        ) : (
          /* Não logado — mostra Login */
          <NavLink
            to="/login"
            icon="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
            label="Login"
            isScrolled={isScrolled}
          />
        )}
      </div>
 
      {/* 3. BOTÕES DE AÇÃO */}
      <div className={`flex items-center transition-all duration-500 ${isScrolled ? 'flex-col gap-4 mt-6' : 'flex-row gap-3'}`}>
        <Link
          to="/dashboard/favoritos"
          className="p-3 bg-white/5 rounded-xl text-slate-400 hover:text-orange-500 transition-colors relative"
          title="Favoritos"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
              d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
          </svg>
        </Link>
        <Link
          to="/avaliacao"
          onClick={handleAvaliarClick}
          className="bg-orange-500 hover:bg-orange-600 text-white font-semibold rounded-xl transition-all duration-300 shrink-0 flex items-center justify-center p-3"
          title="Avaliar Imóvel"
        >
          {isScrolled
            ? <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z" /></svg>
            : <span className="px-4 py-1 text-sm">Avaliar</span>
          }
        </Link>
      </div>
    </nav>
  );
}
 
function NavLink({ to, icon, label, isScrolled }) {
  return (
    <Link to={to} className="group flex items-center justify-center text-slate-300 hover:text-white transition-colors" title={label}>
      <div className="p-3 bg-white/5 rounded-xl border border-white/5 group-hover:bg-white/10 transition-all">
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d={icon} />
        </svg>
      </div>
      {!isScrolled && <span className="ml-3 text-sm font-medium">{label}</span>}
    </Link>
  );
}