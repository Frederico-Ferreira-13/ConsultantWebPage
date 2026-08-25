import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../../context/useAuth";
import { API_BASE_URL } from "../../config/api";

function FavoriteCard({ property, onRemove }) {
  return (
    <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden hover:shadow-md transition-shadow group">
      {/* Imagem */}
      <div className="relative aspect-video bg-slate-100 overflow-hidden">
        <img
          src={property.thumbnailUrl || 'https://images.unsplash.com/photo-1560518883-ce09059eeffa?auto=format&fit=crop&w=600&q=80'}
          alt={property.title}
          className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
        />
        {/* Botão remover favorito */}
        <button
          onClick={() => onRemove(property.realEstateId)}
          className="absolute top-3 right-3 w-8 h-8 bg-white rounded-full flex items-center justify-center shadow-md text-red-400 hover:text-red-600 hover:scale-110 transition-all"
          title="Remover dos favoritos"
        >
          <svg className="w-4 h-4 fill-current" viewBox="0 0 24 24">
            <path d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
          </svg>
        </button>
        {/* Badge de estado */}
        <div className="absolute bottom-3 left-3">
          <span className="bg-white/90 backdrop-blur-sm text-slate-700 text-[10px] font-semibold px-2 py-1 rounded-lg">
            {property.status ?? 'Disponível'}
          </span>
        </div>
      </div>

      {/* Info */}
      <div className="p-4">
        <h3 className="font-semibold text-slate-900 text-sm mb-1 line-clamp-1">{property.title}</h3>
        <p className="text-slate-500 text-xs mb-3 line-clamp-1">
          {property.address ?? property.city ?? 'Localização não disponível'}
        </p>
        <div className="flex items-center justify-between">
          <span className="text-orange-600 font-bold text-sm">
            {property.price
              ? new Intl.NumberFormat('pt-PT', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(property.price)
              : '—'}
          </span>
          <Link
            to={`/imoveis/${property.realEstateId}`}
            className="text-xs text-slate-500 hover:text-orange-600 font-medium transition-colors"
          >
            Ver imóvel →
          </Link>
        </div>
      </div>
    </div>
  );
}

function EmptyFavorites() {
  return (
    <div className="flex flex-col items-center justify-center py-24 text-center">
      <div className="w-16 h-16 bg-orange-50 rounded-2xl flex items-center justify-center mb-6">
        <svg className="w-8 h-8 text-orange-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
            d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
        </svg>
      </div>
      <h3 className="text-slate-800 font-semibold text-lg mb-2">Ainda não tem favoritos</h3>
      <p className="text-slate-500 text-sm max-w-xs mb-8">
        Guarde os imóveis que lhe interessam para os encontrar facilmente mais tarde.
      </p>
      <Link
        to="/imoveis"
        className="bg-orange-500 hover:bg-orange-600 text-white font-medium px-6 py-2.5 rounded-xl transition-all text-sm"
      >
        Explorar imóveis
      </Link>
    </div>
  );
}

export default function Favorites() {
  const { user, token } = useAuth();
  const [favorites, setFavorites] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Lógica de fetch integrada no useEffect para evitar avisos do ESLint
  useEffect(() => {
    let isMounted = true;

    const fetchFavorites = async () => {
      if (!user?.userId || !token) {
        setLoading(false);
        return;
      }

      setLoading(true);
      setError('');
      try {
        const response = await fetch(`${API_BASE_URL}/favorites/${user.userId}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        
        if (!response.ok) throw new Error('Não foi possível carregar os favoritos.');
        
        const data = await response.json();
        if (isMounted) setFavorites(data);
      } catch (err) {
        if (isMounted) setError(err.message);
      } finally {
        if (isMounted) setLoading(false);
      }
    };

    fetchFavorites();
    return () => { isMounted = false; };
  }, [user?.userId, token]);

  const handleRemove = async (realEstateId) => {
    try {
      await fetch(`${API_BASE_URL}/favorites/toggle/${realEstateId}`, {
        method: 'POST',
        headers: { Authorization: `Bearer ${token}` },
      });
      setFavorites((prev) => prev.filter((f) => f.realEstateId !== realEstateId));
    } catch {
      // falha silenciosa
    }
  };

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-slate-900">Os meus favoritos</h1>
        <p className="text-slate-500 text-sm mt-1">
          {favorites.length > 0
            ? `${favorites.length} imóvel${favorites.length !== 1 ? 'is' : ''} guardado${favorites.length !== 1 ? 's' : ''}`
            : 'Imóveis que guardou para consultar mais tarde'}
        </p>
      </div>

      {loading && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {[1, 2, 3].map((i) => (
            <div key={i} className="bg-white rounded-2xl border border-slate-200 overflow-hidden animate-pulse">
              <div className="aspect-video bg-slate-100" />
              <div className="p-4 space-y-2">
                <div className="h-4 bg-slate-100 rounded w-3/4" />
                <div className="h-3 bg-slate-100 rounded w-1/2" />
                <div className="h-4 bg-slate-100 rounded w-1/4 mt-3" />
              </div>
            </div>
          ))}
        </div>
      )}

      {!loading && error && (
        <div className="bg-red-50 border border-red-200 rounded-2xl p-6 text-center">
          <p className="text-red-600 text-sm">{error}</p>
        </div>
      )}

      {!loading && !error && favorites.length === 0 && <EmptyFavorites />}

      {!loading && !error && favorites.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {favorites.map((fav) => (
            <FavoriteCard
              key={fav.favoritesId}
              property={fav.realEstate ?? fav}
              onRemove={handleRemove}
            />
          ))}
        </div>
      )}
    </div>
  );
}