import { useState, useRef, useEffect } from 'react';
import { Link } from 'react-router-dom';
 
function formatPrice(amount, currency = 'EUR') {
  if (amount == null) return 'Preço sob consulta';
  return new Intl.NumberFormat('pt-PT', {
    style: 'currency',
    currency,
    maximumFractionDigits: 0,
  }).format(amount);
}
 
// Extrai o ID de um URL do YouTube ou Vimeo
function getVideoEmbed(url) {
  if (!url) return null;
  // YouTube
  const yt = url.match(/(?:youtu\.be\/|youtube\.com\/(?:watch\?v=|embed\/|shorts\/))([^&?/\s]+)/);
  if (yt) return { type: 'youtube', id: yt[1] };
  // Vimeo
  const vi = url.match(/vimeo\.com\/(\d+)/);
  if (vi) return { type: 'vimeo', id: vi[1] };
  // URL direto (Cloudinary, etc.)
  if (url.match(/\.(mp4|webm|ogg)(\?|$)/i)) return { type: 'direct', url };
  return null;
}
 
// Modal de vídeo completo (exportado para uso no Hero)
export function VideoModal({ property, onClose }) {
  const videoRef = useRef(null);
  const embed = getVideoEmbed(property.videoUrl || property.externalSourceUrl);
 
  useEffect(() => {
    const handleKey = (e) => { if (e.key === 'Escape') onClose(); };
    document.addEventListener('keydown', handleKey);
    document.body.style.overflow = 'hidden';
    return () => {
      document.removeEventListener('keydown', handleKey);
      document.body.style.overflow = '';
    };
  }, [onClose]);
 
  return (
    <div
      className="fixed inset-0 z-50 bg-black/90 flex items-center justify-center p-4"
      onClick={onClose}
    >
      <div
        className="w-full max-w-4xl bg-slate-950 rounded-2xl overflow-hidden shadow-2xl"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Vídeo */}
        <div className="relative aspect-video bg-black">
          {embed?.type === 'youtube' && (
            <iframe
              src={`https://www.youtube.com/embed/${embed.id}?autoplay=1&rel=0`}
              className="w-full h-full"
              allow="autoplay; fullscreen"
              allowFullScreen
            />
          )}
          {embed?.type === 'vimeo' && (
            <iframe
              src={`https://player.vimeo.com/video/${embed.id}?autoplay=1`}
              className="w-full h-full"
              allow="autoplay; fullscreen"
              allowFullScreen
            />
          )}
          {embed?.type === 'direct' && (
            <video
              ref={videoRef}
              src={embed.url}
              className="w-full h-full object-cover"
              controls
              autoPlay
            />
          )}
          {!embed && property.thumbnailUrl && (
            <img src={property.thumbnailUrl} alt={property.title} className="w-full h-full object-cover" />
          )}
          {/* Botão fechar */}
          <button
            onClick={onClose}
            className="absolute top-4 right-4 w-9 h-9 bg-black/60 hover:bg-black/80 text-white rounded-full flex items-center justify-center transition-all"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
 
        {/* Info do imóvel */}
        <div className="p-6 flex items-start justify-between gap-4">
          <div className="flex-1 min-w-0">
            <h2 className="text-white font-bold text-xl mb-1 truncate">{property.title}</h2>
            <p className="text-slate-400 text-sm">
              {property.address?.city && `${property.address.city} · `}
              {property.status}
            </p>
          </div>
          <div className="shrink-0 text-right">
            <p className="text-orange-500 font-black text-2xl">
              {formatPrice(property.price?.amount, property.price?.currency)}
            </p>
            <div className="flex items-center gap-2 mt-2 justify-end">
              {property.externalSourceUrl && (
                <a
                  href={property.externalSourceUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-xs bg-white/10 hover:bg-white/20 text-white px-3 py-1.5 rounded-lg transition-all flex items-center gap-1"
                >
                  <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
                  </svg>
                  Ver no site
                </a>
              )}
              <Link
                to={`/imoveis/${property.realEstateId}`}
                className="text-xs bg-orange-500 hover:bg-orange-600 text-white px-3 py-1.5 rounded-lg transition-all"
              >
                Ver detalhes
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
 
// Card estilo Netflix
export default function PropertyCard({ property, featured = false }) {
  const [isHovered, setIsHovered] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const hoverTimer = useRef(null);
  const videoRef = useRef(null);
 
  const embed = getVideoEmbed(property.videoUrl || property.externalSourceUrl);
  const hasVideo = !!embed;
 
  const handleMouseEnter = () => {
    hoverTimer.current = setTimeout(() => setIsHovered(true), 400);
  };
 
  const handleMouseLeave = () => {
    clearTimeout(hoverTimer.current);
    setIsHovered(false);
    if (videoRef.current) {
      videoRef.current.pause();
      videoRef.current.currentTime = 0;
    }
  };
 
  const handleClick = (e) => {
    if (hasVideo) {
      e.preventDefault();
      setShowModal(true);
    }
  };
 
  return (
    <>
      <div
        className={`relative group cursor-pointer transition-all duration-300 ${
          isHovered ? 'z-20 scale-105' : 'z-10'
        } ${featured ? 'col-span-2 row-span-2' : ''}`}
        onMouseEnter={handleMouseEnter}
        onMouseLeave={handleMouseLeave}
        onClick={handleClick}
      >
        {/* Card base */}
        <Link
          to={hasVideo ? '#' : `/imoveis/${property.realEstateId}`}
          onClick={hasVideo ? (e) => e.preventDefault() : undefined}
          className={`block bg-slate-900 rounded-xl overflow-hidden border border-white/10 transition-all duration-300 ${
            isHovered ? 'border-white/30 shadow-2xl shadow-black/50' : ''
          }`}
        >
          {/* Imagem / Vídeo */}
          <div className={`relative overflow-hidden bg-slate-800 ${featured ? 'aspect-video' : 'aspect-4/3'}`}>
            {/* Thumbnail */}
            <img
              src={property.thumbnailUrl || 'https://images.unsplash.com/photo-1560518883-ce09059eeffa?auto=format&fit=crop&w=600&q=80'}
              alt={property.title}
              className={`w-full h-full object-cover transition-all duration-500 ${
                isHovered && hasVideo ? 'opacity-0' : 'opacity-100 group-hover:scale-105'
              }`}
            />
 
            {/* Preview de vídeo no hover — só para vídeos diretos (Cloudinary) */}
            {isHovered && embed?.type === 'direct' && (
              <video
                ref={videoRef}
                src={embed.url}
                className="absolute inset-0 w-full h-full object-cover"
                autoPlay
                muted
                loop
                playsInline
              />
            )}
 
            {/* Preview YouTube/Vimeo no hover — iframe muted */}
            {isHovered && embed?.type === 'youtube' && (
              <iframe
                src={`https://www.youtube.com/embed/${embed.id}?autoplay=1&mute=1&controls=0&loop=1&playlist=${embed.id}&rel=0`}
                className="absolute inset-0 w-full h-full"
                allow="autoplay"
              />
            )}
 
            {/* Badge de status */}
            {property.status && (
              <span className="absolute top-3 left-3 bg-slate-950/80 backdrop-blur-md text-[10px] font-semibold text-white px-2.5 py-1 rounded-lg z-10">
                {property.status}
              </span>
            )}
 
            {/* Ícone de vídeo */}
            {hasVideo && !isHovered && (
              <div className="absolute bottom-3 right-3 w-8 h-8 bg-black/60 backdrop-blur-sm rounded-full flex items-center justify-center">
                <svg className="w-3.5 h-3.5 text-white fill-current ml-0.5" viewBox="0 0 24 24">
                  <path d="M8 5v14l11-7z" />
                </svg>
              </div>
            )}
 
            {/* Overlay hover com botões */}
            {isHovered && (
              <div className="absolute inset-x-0 bottom-0 bg-gradient-to-t from-black/80 to-transparent p-4 z-10">
                <div className="flex items-center gap-2">
                  <button
                    className="w-9 h-9 bg-white rounded-full flex items-center justify-center hover:bg-orange-500 transition-colors shrink-0"
                    onClick={(e) => { e.stopPropagation(); setShowModal(true); }}
                  >
                    <svg className="w-4 h-4 text-slate-900 fill-current ml-0.5" viewBox="0 0 24 24">
                      <path d="M8 5v14l11-7z" />
                    </svg>
                  </button>
                  {property.externalSourceUrl && (
                    <a
                      href={property.externalSourceUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="w-9 h-9 bg-white/20 hover:bg-white/30 border border-white/40 rounded-full flex items-center justify-center transition-all"
                      onClick={(e) => e.stopPropagation()}
                      title="Ver no site original"
                    >
                      <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
                      </svg>
                    </a>
                  )}
                </div>
              </div>
            )}
          </div>
 
          {/* Info */}
          <div className={`p-4 space-y-1.5 ${isHovered ? 'bg-slate-800' : ''} transition-colors`}>
            <h3 className="text-white font-semibold text-sm leading-snug line-clamp-2">{property.title}</h3>
            {property.address?.city && (
              <p className="text-slate-400 text-xs flex items-center gap-1">
                <svg className="w-3 h-3 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17.657 16.657L13.414 20.9a2 2 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                {property.address.city}
              </p>
            )}
            <p className="text-orange-500 font-bold text-base">
              {formatPrice(property.price?.amount, property.price?.currency)}
            </p>
          </div>
        </Link>
      </div>
 
      {showModal && (
        <VideoModal property={property} onClose={() => setShowModal(false)} />
      )}
    </>
  );
}