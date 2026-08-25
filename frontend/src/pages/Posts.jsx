import { useState, useEffect, useCallback, startTransition } from 'react';
import { Link } from 'react-router-dom';
import { API_BASE_URL as API_BASE } from '../config/api';

export default function Posts() {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');

  const fetchPosts = useCallback(async (term) => {
    setLoading(true);
    setError('');
    try {
      const url = term
        ? `${API_BASE}/posts/search?term=${encodeURIComponent(term)}`
        : `${API_BASE}/posts/latest?count=12`;
      const res = await fetch(url);
      if (!res.ok) throw new Error();
      const data = await res.json();
      setPosts(Array.isArray(data) ? data.filter((p) => p.isPublished) : []);
    } catch {
      setError('Não foi possível carregar os artigos.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    startTransition(() => {
      fetchPosts('');
    });
  }, [fetchPosts]);

  const handleSearch = (e) => {
    e.preventDefault();
    startTransition(() => {
      fetchPosts(searchTerm);
    });
  };

  return (
    <div className="min-h-screen bg-slate-950 text-white pt-28 pb-20 px-6 md:px-12">
      <div className="max-w-6xl mx-auto">
        <div className="text-center mb-10 space-y-3">
          <span className="text-orange-500 font-bold tracking-widest text-xs uppercase block">Mercado em Foco</span>
          <h1 className="text-3xl md:text-4xl font-black">Notícias e conselhos imobiliários</h1>
        </div>

        <form onSubmit={handleSearch} className="max-w-md mx-auto mb-12 flex gap-2">
          <input
            type="text"
            placeholder="Pesquisar artigos..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="flex-1 bg-slate-900 border border-white/10 rounded-xl px-4 py-2.5 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500"
          />
          <button
            type="submit"
            className="bg-orange-500 hover:bg-orange-600 text-white text-sm font-medium px-5 py-2.5 rounded-xl transition-all"
          >
            Procurar
          </button>
        </form>

        {error && <p className="text-red-400 text-sm text-center mb-6">{error}</p>}

        {loading ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {[1, 2, 3].map((i) => (
              <div key={i} className="h-64 bg-white/5 rounded-2xl animate-pulse" />
            ))}
          </div>
        ) : posts.length === 0 ? (
          <p className="text-slate-600 text-sm text-center">Ainda não há artigos publicados.</p>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {posts.map((post) => (
              <Link
                key={post.postId}
                to={`/mercado/${post.slug}`}
                className="bg-slate-900 border border-white/10 rounded-2xl overflow-hidden hover:border-orange-500/30 transition-all group"
              >
                <div className="aspect-video bg-slate-800 overflow-hidden">
                  <img
                    src={post.imageUrl || 'https://images.unsplash.com/photo-1560518883-ce09059eeffa?auto=format&fit=crop&w=600&q=80'}
                    alt={post.title}
                    className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                  />
                </div>
                <div className="p-5">
                  <h2 className="text-white font-bold text-lg mb-2 line-clamp-2">{post.title}</h2>
                  <p className="text-slate-400 text-sm line-clamp-3">{post.summary}</p>
                  <p className="text-slate-600 text-xs mt-3">
                    {post.createdAt
                      ? new Date(post.createdAt).toLocaleDateString('pt-PT', { day: '2-digit', month: 'long', year: 'numeric' })
                      : ''}
                    {' · '}{post.viewCount ?? 0} vistas
                  </p>
                </div>
              </Link>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}