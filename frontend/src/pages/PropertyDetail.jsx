import { useState, useEffect, useCallback } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/useAuth.jsx';
import {
  getPropertyById,
  getPropertyMedia,
  getPropertyCategories,
  getPropertyComments,
  addPropertyComment,
  requestPropertyInformation,
  scheduleVisit,
} from '../services/realEstateService.js';
import { getAverageRating, getUserRating, addOrUpdateRating } from '../services/ratingsService.js';
import { getUserFavorites, toggleFavorite } from '../services/favoritesService.js';

function formatPrice(amount, currency = 'EUR') {
  if (amount == null) return 'Preço sob consulta';
  return new Intl.NumberFormat('pt-PT', {
    style: 'currency',
    currency,
    maximumFractionDigits: 0,
  }).format(amount);
}

function getVideoEmbed(url) {
  if (!url) return null;
  const yt = url.match(/(?:youtu\.be\/|youtube\.com\/(?:watch\?v=|embed\/|shorts\/))([^&?/\s]+)/);
  if (yt) return { type: 'youtube', id: yt[1] };
  const vi = url.match(/vimeo\.com\/(\d+)/);
  if (vi) return { type: 'vimeo', id: vi[1] };
  if (url.match(/\.(mp4|webm|ogg)(\?|$)/i)) return { type: 'direct', url };
  return null;
}

function StarIcon({ filled }) {
  return (
    <svg
      className={`w-5 h-5 ${filled ? 'text-orange-500 fill-orange-500' : 'text-slate-600 fill-none'}`}
      viewBox="0 0 24 24"
      stroke="currentColor"
      strokeWidth="1.5"
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z"
      />
    </svg>
  );
}

export default function PropertyDetail() {
  const { id } = useParams();
  const { user, token } = useAuth();
  const navigate = useNavigate();

  const [property, setProperty] = useState(null);
  const [media, setMedia] = useState([]);
  const [categories, setCategories] = useState([]);
  const [comments, setComments] = useState([]);
  const [activeMedia, setActiveMedia] = useState(0);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [avgRating, setAvgRating] = useState(0);
  const [userRating, setUserRating] = useState(0);
  const [hoverStar, setHoverStar] = useState(0);
  const [ratingSubmitting, setRatingSubmitting] = useState(false);

  const [isFavorite, setIsFavorite] = useState(false);
  const [favoriteLoading, setFavoriteLoading] = useState(false);

  const [newComment, setNewComment] = useState('');
  const [commentSubmitting, setCommentSubmitting] = useState(false);
  const [commentFeedback, setCommentFeedback] = useState('');

  const [infoForm, setInfoForm] = useState({ clientName: '', clientEmail: '', message: '' });
  const [infoSubmitting, setInfoSubmitting] = useState(false);
  const [infoFeedback, setInfoFeedback] = useState({ message: '', type: '' });

  const [visitDate, setVisitDate] = useState('');
  const [visitSubmitting, setVisitSubmitting] = useState(false);
  const [visitFeedback, setVisitFeedback] = useState({ message: '', type: '' });

  const refreshComments = useCallback(async () => {
    try {
      const data = await getPropertyComments(id);
      setComments(Array.isArray(data) ? data : []);
    } catch {
      // silencioso — não é crítico
    }
  }, [id]);

  // Dados públicos do imóvel
  useEffect(() => {
    let mounted = true;

    async function load() {
      setLoading(true);
      setError('');
      try {
        const [propData, mediaData, catData, commentsData, avgData] = await Promise.all([
          getPropertyById(id),
          getPropertyMedia(id).catch(() => []),
          getPropertyCategories(id).catch(() => []),
          getPropertyComments(id).catch(() => []),
          getAverageRating(id).catch(() => ({ average: 0 })),
        ]);
        if (!mounted) return;
        setProperty(propData);
        setMedia(Array.isArray(mediaData) ? mediaData : []);
        setCategories(Array.isArray(catData) ? catData : []);
        setComments(Array.isArray(commentsData) ? commentsData : []);
        setAvgRating(avgData?.average ?? 0);
      } catch {
        if (mounted) setError('Não foi possível carregar este imóvel. Pode ter sido removido.');
      } finally {
        if (mounted) setLoading(false);
      }
    }

    load();
    return () => { mounted = false; };
  }, [id]);

  // Dados dependentes do utilizador autenticado
  useEffect(() => {
    let mounted = true;

    async function syncUserData() {
      // O "reset" ao trocar de utilizador (ex: logout) também fica dentro da
      // função async, e não solto no corpo do effect — resolve o aviso do
      // eslint-plugin-react-hooks (set-state-in-effect).
      if (!user?.userId || !token) {
        if (mounted) {
          setUserRating(0);
          setIsFavorite(false);
        }
        return;
      }

      try {
        const [ratingData, favs] = await Promise.all([
          getUserRating(id, user.userId),
          getUserFavorites(user.userId).catch(() => []),
        ]);
        if (!mounted) return;
        setUserRating(ratingData?.ratingValue?.value ?? ratingData?.value ?? 0);
        setIsFavorite(
          Array.isArray(favs) && favs.some((f) => Number(f.realEstateId) === Number(id))
        );
      } catch {
        // silencioso
      }
    }

    syncUserData();
    return () => { mounted = false; };
  }, [id, user?.userId, token]);

  const handleToggleFavorite = async () => {
    if (!user) {
      navigate('/login');
      return;
    }
    setFavoriteLoading(true);
    try {
      const result = await toggleFavorite(id);
      setIsFavorite(!!result.isFavorite);
    } catch {
      // silencioso
    } finally {
      setFavoriteLoading(false);
    }
  };

  const handleRate = async (stars) => {
    if (!user) {
      navigate('/login');
      return;
    }
    setRatingSubmitting(true);
    try {
      await addOrUpdateRating(Number(id), stars);
      setUserRating(stars);
      const avgData = await getAverageRating(id).catch(() => null);
      if (avgData) setAvgRating(avgData.average ?? 0);
    } catch {
      // silencioso
    } finally {
      setRatingSubmitting(false);
    }
  };

  const handleCommentSubmit = async (e) => {
    e.preventDefault();
    if (!user) {
      navigate('/login');
      return;
    }
    if (!newComment.trim()) return;

    setCommentSubmitting(true);
    setCommentFeedback('');
    try {
      await addPropertyComment(id, newComment.trim());
      setNewComment('');
      await refreshComments();
      setCommentFeedback('Comentário publicado.');
    } catch (err) {
      setCommentFeedback(err.response?.data || 'Erro ao publicar o comentário.');
    } finally {
      setCommentSubmitting(false);
    }
  };

  const handleInfoSubmit = async (e) => {
    e.preventDefault();
    setInfoSubmitting(true);
    setInfoFeedback({ message: '', type: '' });
    try {
      await requestPropertyInformation(id, infoForm);
      setInfoFeedback({ message: 'Pedido enviado. O consultor entrará em contacto brevemente.', type: 'success' });
      setInfoForm({ clientName: '', clientEmail: '', message: '' });
    } catch (err) {
      setInfoFeedback({ message: err.response?.data || 'Erro ao enviar o pedido.', type: 'error' });
    } finally {
      setInfoSubmitting(false);
    }
  };

  const handleVisitSubmit = async (e) => {
    e.preventDefault();
    if (!user) {
      navigate('/login');
      return;
    }
    if (!visitDate) return;

    const preferredDate = new Date(visitDate);
    if (preferredDate <= new Date()) {
      setVisitFeedback({ message: 'A data deve ser no futuro.', type: 'error' });
      return;
    }

    setVisitSubmitting(true);
    setVisitFeedback({ message: '', type: '' });
    try {
      await scheduleVisit(id, preferredDate.toISOString());
      setVisitFeedback({ message: 'Visita agendada. Aguarde a confirmação do consultor.', type: 'success' });
      setVisitDate('');
    } catch (err) {
      setVisitFeedback({ message: err.response?.data || 'Erro ao agendar a visita.', type: 'error' });
    } finally {
      setVisitSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-slate-950 flex items-center justify-center">
        <div className="w-8 h-8 border-2 border-orange-500 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (error || !property) {
    return (
      <div className="min-h-screen bg-slate-950 flex items-center justify-center px-6">
        <div className="bg-red-500/10 border border-red-500/30 text-red-400 rounded-2xl p-8 text-center max-w-md">
          <p className="mb-4">{error || 'Imóvel não encontrado.'}</p>
          <Link to="/imoveis" className="text-white underline text-sm">
            ← Voltar aos imóveis
          </Link>
        </div>
      </div>
    );
  }

  const activeItem = media[activeMedia];
  const activeEmbed = activeItem?.mediaType === 'video' ? getVideoEmbed(activeItem.mediaUrl) : null;

  return (
    <div className="min-h-screen bg-slate-950 text-white">
      {/* ── Breadcrumb ── */}
      <div className="px-6 md:px-12 pt-4">
        <Link to="/imoveis" className="text-slate-500 hover:text-white text-sm transition-colors inline-flex items-center gap-1">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 19l-7-7 7-7" />
          </svg>
          Todos os imóveis
        </Link>
      </div>

      {/* ── Galeria ── */}
      <div className="px-6 md:px-12 pt-4">
        <div className="relative aspect-video md:aspect-21/9 rounded-2xl overflow-hidden bg-slate-900 border border-white/10">
          {activeEmbed?.type === 'youtube' && (
            <iframe
              src={`https://www.youtube.com/embed/${activeEmbed.id}?rel=0`}
              className="w-full h-full"
              allow="autoplay; fullscreen"
              allowFullScreen
            />
          )}
          {activeEmbed?.type === 'vimeo' && (
            <iframe
              src={`https://player.vimeo.com/video/${activeEmbed.id}`}
              className="w-full h-full"
              allow="autoplay; fullscreen"
              allowFullScreen
            />
          )}
          {activeEmbed?.type === 'direct' && (
            <video src={activeEmbed.url} className="w-full h-full object-cover" controls autoPlay />
          )}
          {!activeEmbed && (
            <img
              src={activeItem?.mediaUrl || property.thumbnailUrl || 'https://images.unsplash.com/photo-1560518883-ce09059eeffa?auto=format&fit=crop&w=1600&q=80'}
              alt={property.title}
              className="w-full h-full object-cover"
            />
          )}

          {property.status && (
            <span className="absolute top-4 left-4 bg-slate-950/80 backdrop-blur-md text-xs font-semibold text-white px-3 py-1.5 rounded-lg">
              {property.status}
            </span>
          )}
        </div>

        {/* Thumbnails */}
        {media.length > 1 && (
          <div className="flex gap-2 mt-3 overflow-x-auto pb-1">
            {media.map((m, i) => (
              <button
                key={m.mediaId ?? i}
                onClick={() => setActiveMedia(i)}
                className={`relative shrink-0 w-20 h-14 rounded-lg overflow-hidden border-2 transition-all ${
                  i === activeMedia ? 'border-orange-500' : 'border-transparent opacity-70 hover:opacity-100'
                }`}
              >
                {m.mediaType === 'video' ? (
                  <div className="w-full h-full bg-slate-800 flex items-center justify-center">
                    <svg className="w-4 h-4 text-white fill-current" viewBox="0 0 24 24"><path d="M8 5v14l11-7z" /></svg>
                  </div>
                ) : (
                  <img src={m.mediaUrl} alt="" className="w-full h-full object-cover" />
                )}
              </button>
            ))}
          </div>
        )}
      </div>

      {/* ── Conteúdo principal ── */}
      <div className="max-w-7xl mx-auto px-6 md:px-12 py-10 grid grid-cols-1 lg:grid-cols-3 gap-10">

        {/* Coluna principal */}
        <div className="lg:col-span-2 space-y-8">

          {/* Cabeçalho */}
          <div>
            <div className="flex flex-wrap items-center gap-2 mb-3">
              {property.realEstateType?.typeName && (
                <span className="text-xs font-semibold text-orange-400 bg-orange-500/10 border border-orange-500/20 px-2.5 py-1 rounded-full">
                  {property.realEstateType.typeName}
                </span>
              )}
              {categories.map((c) => (
                <span key={c.categoriesId} className="text-xs font-medium text-slate-300 bg-white/5 border border-white/10 px-2.5 py-1 rounded-full">
                  {c.categoryName}
                </span>
              ))}
            </div>

            <h1 className="text-3xl md:text-4xl font-black leading-tight mb-2">{property.title}</h1>

            <p className="text-slate-400 text-sm flex items-center gap-1.5">
              <svg className="w-4 h-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17.657 16.657L13.414 20.9a2 2 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              {property.address?.street && `${property.address.street}, `}
              {property.city?.cityName || property.address?.city}
            </p>

            <div className="flex items-center gap-3 mt-4">
              <div className="flex items-center gap-0.5">
                {[1, 2, 3, 4, 5].map((star) => (
                  <button
                    key={star}
                    disabled={ratingSubmitting}
                    onMouseEnter={() => setHoverStar(star)}
                    onMouseLeave={() => setHoverStar(0)}
                    onClick={() => handleRate(star)}
                    title={user ? 'Avaliar este imóvel' : 'Inicie sessão para avaliar'}
                  >
                    <StarIcon filled={star <= (hoverStar || userRating)} />
                  </button>
                ))}
              </div>
              <span className="text-slate-500 text-sm">
                {avgRating > 0 ? `${avgRating.toFixed(1)} de média` : 'Sem avaliações ainda'}
              </span>
            </div>
          </div>

          {/* Descrição */}
          {property.description && (
            <div className="bg-slate-900 border border-white/10 rounded-2xl p-6">
              <h2 className="font-semibold text-white mb-3">Sobre o imóvel</h2>
              <p className="text-slate-300 text-sm leading-relaxed whitespace-pre-line">{property.description}</p>
            </div>
          )}

          {property.externalSourceUrl && (
            <a
              href={property.externalSourceUrl}
              target="_blank"
              rel="noopener noreferrer"
              className="inline-flex items-center gap-2 text-sm text-slate-400 hover:text-white transition-colors"
            >
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
              </svg>
              Ver anúncio original
            </a>
          )}

          {/* Comentários */}
          <div className="bg-slate-900 border border-white/10 rounded-2xl p-6">
            <h2 className="font-semibold text-white mb-4">Comentários ({comments.length})</h2>

            {user ? (
              <form onSubmit={handleCommentSubmit} className="mb-6 space-y-2">
                <textarea
                  value={newComment}
                  onChange={(e) => setNewComment(e.target.value)}
                  placeholder="Deixe uma pergunta ou comentário sobre este imóvel..."
                  rows={3}
                  maxLength={1000}
                  className="w-full bg-slate-950 border border-white/10 rounded-xl p-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 resize-none transition-colors"
                />
                <div className="flex items-center justify-between">
                  {commentFeedback && <p className="text-xs text-slate-500">{commentFeedback}</p>}
                  <button
                    type="submit"
                    disabled={commentSubmitting || !newComment.trim()}
                    className="ml-auto bg-orange-500 hover:bg-orange-600 disabled:opacity-50 text-white text-xs font-semibold px-4 py-2 rounded-lg transition-all"
                  >
                    {commentSubmitting ? 'A publicar...' : 'Publicar comentário'}
                  </button>
                </div>
              </form>
            ) : (
              <div className="mb-6 bg-white/5 border border-white/10 rounded-xl p-4 text-sm text-slate-400">
                <Link to="/login" className="text-orange-400 hover:text-orange-300 font-medium">Inicie sessão</Link> para comentar.
              </div>
            )}

            {comments.length === 0 ? (
              <p className="text-slate-600 text-sm">Seja o primeiro a comentar este imóvel.</p>
            ) : (
              <div className="space-y-4">
                {comments.map((c) => (
                  <div key={c.commentsId} className="border-t border-white/5 pt-4 first:border-0 first:pt-0">
                    <div className="flex items-center justify-between mb-1">
                      <span className="text-sm font-semibold text-white">
                        {c.users?.fullName || c.users?.userName || `Utilizador #${c.userId}`}
                      </span>
                      <span className="text-xs text-slate-600">
                        {new Date(c.createdAt).toLocaleDateString('pt-PT')}
                        {c.isEdited ? ' · editado' : ''}
                      </span>
                    </div>
                    <p className="text-slate-300 text-sm">{c.commentText}</p>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Sidebar */}
        <div className="lg:col-span-1 space-y-4 lg:sticky lg:top-24 self-start">

          {/* Preço + ações rápidas */}
          <div className="bg-slate-900 border border-white/10 rounded-2xl p-6">
            <p className="text-orange-500 font-black text-3xl mb-1">
              {formatPrice(property.price?.amount, property.price?.currency)}
            </p>
            {property.consultant?.fullName && (
              <p className="text-slate-500 text-xs">Consultor: {property.consultant.fullName}</p>
            )}

            <div className="flex gap-2 mt-5">
              <button
                onClick={handleToggleFavorite}
                disabled={favoriteLoading}
                className={`flex-1 flex items-center justify-center gap-2 text-sm font-semibold py-2.5 rounded-xl border transition-all ${
                  isFavorite
                    ? 'bg-orange-500/10 border-orange-500/30 text-orange-400'
                    : 'bg-white/5 border-white/10 text-slate-300 hover:border-white/30'
                }`}
              >
                <svg className={`w-4 h-4 ${isFavorite ? 'fill-current' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5"
                    d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                </svg>
                {isFavorite ? 'Nos favoritos' : 'Guardar'}
              </button>
            </div>
          </div>

          {/* Pedir informação — público */}
          <div className="bg-slate-900 border border-white/10 rounded-2xl p-6">
            <h3 className="font-semibold text-white text-sm mb-1">Pedir informação</h3>
            <p className="text-slate-500 text-xs mb-4">Resposta em menos de 24h úteis.</p>
            <form onSubmit={handleInfoSubmit} className="space-y-3">
              <input
                required
                type="text"
                placeholder="Nome"
                value={infoForm.clientName}
                onChange={(e) => setInfoForm({ ...infoForm, clientName: e.target.value })}
                className="w-full bg-slate-950 border border-white/10 rounded-xl px-3 py-2.5 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 transition-colors"
              />
              <input
                required
                type="email"
                placeholder="Email"
                value={infoForm.clientEmail}
                onChange={(e) => setInfoForm({ ...infoForm, clientEmail: e.target.value })}
                className="w-full bg-slate-950 border border-white/10 rounded-xl px-3 py-2.5 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 transition-colors"
              />
              <textarea
                required
                rows={3}
                placeholder="A sua mensagem..."
                value={infoForm.message}
                onChange={(e) => setInfoForm({ ...infoForm, message: e.target.value })}
                className="w-full bg-slate-950 border border-white/10 rounded-xl px-3 py-2.5 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 resize-none transition-colors"
              />
              {infoFeedback.message && (
                <p className={`text-xs ${infoFeedback.type === 'error' ? 'text-red-400' : 'text-emerald-400'}`}>
                  {infoFeedback.message}
                </p>
              )}
              <button
                type="submit"
                disabled={infoSubmitting}
                className="w-full bg-orange-500 hover:bg-orange-600 disabled:opacity-50 text-white text-sm font-semibold py-2.5 rounded-xl transition-all"
              >
                {infoSubmitting ? 'A enviar...' : 'Enviar pedido'}
              </button>
            </form>
          </div>

          {/* Agendar visita — requer sessão */}
          <div className="bg-slate-900 border border-white/10 rounded-2xl p-6">
            <h3 className="font-semibold text-white text-sm mb-1">Agendar visita</h3>
            <p className="text-slate-500 text-xs mb-4">Escolha o dia e hora que lhe é mais conveniente.</p>

            {user ? (
              <form onSubmit={handleVisitSubmit} className="space-y-3">
                <input
                  required
                  type="datetime-local"
                  value={visitDate}
                  onChange={(e) => setVisitDate(e.target.value)}
                  className="w-full bg-slate-950 border border-white/10 rounded-xl px-3 py-2.5 text-sm text-white focus:outline-none focus:border-orange-500 transition-colors [color-scheme:dark]"
                />
                {visitFeedback.message && (
                  <p className={`text-xs ${visitFeedback.type === 'error' ? 'text-red-400' : 'text-emerald-400'}`}>
                    {visitFeedback.message}
                  </p>
                )}
                <button
                  type="submit"
                  disabled={visitSubmitting}
                  className="w-full bg-white/10 hover:bg-white/20 disabled:opacity-50 text-white text-sm font-semibold py-2.5 rounded-xl transition-all"
                >
                  {visitSubmitting ? 'A agendar...' : 'Marcar visita'}
                </button>
              </form>
            ) : (
              <Link
                to="/login"
                className="block text-center bg-white/5 hover:bg-white/10 border border-white/10 text-slate-300 text-sm font-medium py-2.5 rounded-xl transition-all"
              >
                Inicie sessão para agendar
              </Link>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}