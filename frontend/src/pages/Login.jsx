import { useState } from 'react';
import { useAuth } from '../context/useAuth'; 
import { useNavigate } from 'react-router-dom';
 
export default function Login() {
  const [showRegister, setShowRegister] = useState(false);
  const [regData, setRegData] = useState({ name: '', email: '', password: '' });
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

 
  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      const user = await login(email, password);
      if (user.usersRoleId === 1) {
        navigate('/admin');
      } else {
        navigate('/dashboard/favoritos');
      }
    } catch (err) {
      setError(err.message || 'Erro ao iniciar sessão.');
    } finally {
      setIsLoading(false);
    }
  };
 
  const handleRegister = async (e) => {
    e.preventDefault();
    setError('');

    // Validação básica
    if (!regData.name.trim() || !regData.email.trim() || !regData.password) {
      setError('Preencha todos os campos.');
      return;
    }
    if (regData.password.length < 6) {
      setError('A palavra-passe deve ter pelo menos 6 caracteres.');
      return;
    }

    setIsLoading(true);

    try {
      const response = await fetch('http://localhost:5170/api/Users/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          fullName: regData.name,
          email: regData.email,
          password: regData.password,
          userName: regData.email,
          usersRoleId: 2
        }),
      });

      if (response.ok) {
        setShowRegister(false);
        setRegData({ name: '', email: '', password: '' });
        setError('');
        alert('Conta criada com sucesso! Já pode iniciar sessão.');
      } else {
        const errorText = await response.text();
        setError('Erro no registo: ' + errorText);
      }
    } catch (err) {
      setError('Erro ao conectar ao servidor.', err);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-950 flex items-center justify-center p-6 relative overflow-hidden">
      <button
        onClick={() => navigate(-1)}
        className="absolute top-8 left-8 text-slate-400 hover:text-white z-50 transition-colors"
      >
        ← Voltar
      </button>

      <div className="relative w-full max-w-4xl h-137.5 bg-slate-900 rounded-3xl overflow-hidden shadow-2xl border border-white/10 flex">
        {/* ── Mensagem de erro global ── */}
        {error && (
          <div className="absolute top-4 left-1/2 -translate-x-1/2 z-30 bg-red-500/90 text-white text-sm px-4 py-2 rounded-xl">
            {error}
          </div>
        )}

        {/* ── Formulário de Login ── */}
        <div className="w-1/2 p-12 flex flex-col justify-center text-white">
          <h1 className="text-3xl font-black mb-2">Entrar</h1>
          <p className="text-slate-400 text-sm mb-6">Bem-vindo de volta! Inicie sessão na sua conta.</p>
          <form onSubmit={handleLogin} className="space-y-4">
            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full bg-white/5 border border-white/10 p-4 rounded-xl outline-none focus:border-orange-500 text-sm transition-colors"
              required
            />
            <input
              type="password"
              placeholder="Palavra-passe"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full bg-white/5 border border-white/10 p-4 rounded-xl outline-none focus:border-orange-500 text-sm transition-colors"
              required
            />
            <button
              type="submit"
              disabled={isLoading}
              className="w-full bg-orange-500 hover:bg-orange-600 disabled:opacity-50 disabled:cursor-not-allowed py-4 rounded-xl font-bold transition-all"
            >
              {isLoading ? 'A processar...' : 'Entrar'}
            </button>
          </form>
        </div>

        {/* ── Formulário de Registo ── */}
        <div className="w-1/2 p-12 flex flex-col justify-center text-white">
          <h1 className="text-3xl font-black mb-2">Criar Conta</h1>
          <p className="text-slate-400 text-sm mb-6">Junte-se à nossa rede de consultores imobiliários.</p>
          <form onSubmit={handleRegister} className="space-y-4">
            <input
              type="text"
              placeholder="Nome Completo"
              value={regData.name}
              onChange={(e) => setRegData({ ...regData, name: e.target.value })}
              className="w-full bg-white/5 border border-white/10 p-4 rounded-xl outline-none focus:border-orange-500 text-sm transition-colors"
              required
            />
            <input
              type="email"
              placeholder="Email"
              value={regData.email}
              onChange={(e) => setRegData({ ...regData, email: e.target.value })}
              className="w-full bg-white/5 border border-white/10 p-4 rounded-xl outline-none focus:border-orange-500 text-sm transition-colors"
              required
            />
            <input
              type="password"
              placeholder="Palavra-passe (mín. 6 caracteres)"
              value={regData.password}
              onChange={(e) => setRegData({ ...regData, password: e.target.value })}
              className="w-full bg-white/5 border border-white/10 p-4 rounded-xl outline-none focus:border-orange-500 text-sm transition-colors"
              required
              minLength={6}
            />
            <button
              type="submit"
              disabled={isLoading}
              className="w-full bg-orange-500 hover:bg-orange-600 disabled:opacity-50 disabled:cursor-not-allowed py-4 rounded-xl font-bold transition-all"
            >
              {isLoading ? 'A processar...' : 'Registar-me'}
            </button>
          </form>
        </div>

        {/* ── Overlay laranja deslizante ──
            showRegister=false → overlay cobre o lado DIREITO (mostra login)
            showRegister=true  → overlay desliza para ESQUERDA (mostra registo)
        ── */}
        <div
          className={`absolute top-0 left-0 w-1/2 h-full bg-orange-600 z-20 transition-transform duration-700 ease-in-out flex flex-col items-center justify-center text-center p-12 text-white ${showRegister ? 'translate-x-0' : 'translate-x-full'
            }`}
        >
          {showRegister ? (
            <>
              <div className="w-16 h-16 bg-white/20 rounded-2xl flex items-center justify-center mb-6">
                <span className="text-white font-black text-2xl">F</span>
              </div>
              <h2 className="text-3xl font-black mb-4">Já tem conta?</h2>
              <p className="text-white/80 mb-8 text-sm leading-relaxed">
                Entre e explore os imóveis exclusivos e as ferramentas disponíveis.
              </p>
              <button
                onClick={() => { setShowRegister(false); setError(''); }}
                className="px-8 py-3 border-2 border-white rounded-full font-bold hover:bg-white hover:text-orange-600 transition-all"
              >
                Ir para Login
              </button>
            </>
          ) : (
            <>
              <div className="w-16 h-16 bg-white/20 rounded-2xl flex items-center justify-center mb-6">
                <span className="text-white font-black text-2xl">F</span>
              </div>
              <h2 className="text-3xl font-black mb-4">Novo por aqui?</h2>
              <p className="text-white/80 mb-8 text-sm leading-relaxed">
                Crie a sua conta e tenha acesso a ferramentas exclusivas — estudo de mercado, perfil DISC e muito mais.
              </p>
              <button
                onClick={() => { setShowRegister(true); setError(''); }}
                className="px-8 py-3 border-2 border-white rounded-full font-bold hover:bg-white hover:text-orange-600 transition-all"
              >
                Criar Conta
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  );
}