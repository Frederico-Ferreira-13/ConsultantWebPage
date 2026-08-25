import { useEffect, useState, useRef } from 'react';
import { getProperties } from '../services/realEstateService.js';
import PropertyCard, { VideoModal } from '../components/PropertyCard.jsx';
 
function CategoryRow({ title, properties }) {
  const scrollRef = useRef(null);
  const [canScrollLeft, setCanScrollLeft] = useState(false);
  const [canScrollRight, setCanScrollRight] = useState(true);
 
  const checkScroll = () => {
    const el = scrollRef.current;
    if (!el) return;
    setCanScrollLeft(el.scrollLeft > 0);
    setCanScrollRight(el.scrollLeft + el.clientWidth < el.scrollWidth - 10);
  };
 
  const scroll = (dir) => {
    const el = scrollRef.current;
    if (!el) return;
    el.scrollBy({ left: dir * 600, behavior: 'smooth' });
    setTimeout(checkScroll, 400);
  };
 
  if (!properties.length) return null;
 
  return (
    <div className="mb-10">
      <h2 className="text-white font-bold text-lg md:text-xl mb-4 px-6 md:px-12">{title}</h2>
      <div className="relative group/row">
        {canScrollLeft && (
          <button
            onClick={() => scroll(-1)}
            className="absolute left-0 top-0 bottom-0 z-20 w-12 bg-linear-to-r from-slate-950 to-transparent flex items-center justify-center opacity-0 group-hover/row:opacity-100 transition-opacity"
          >
            <div className="w-8 h-8 bg-white/20 hover:bg-white/30 rounded-full flex items-center justify-center transition-all">
              <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 19l-7-7 7-7" />
              </svg>
            </div>
          </button>
        )}
 
        <div
          ref={scrollRef}
          onScroll={checkScroll}
          className="flex gap-3 overflow-x-auto scrollbar-hide px-6 md:px-12 pb-4"
          style={{ scrollSnapType: 'x mandatory' }}
        >
          {properties.map((p) => (
            <div
              key={p.realEstateId}
              className="flex-none w-64 md:w-72"
              style={{ scrollSnapAlign: 'start' }}
            >
              <PropertyCard property={p} />
            </div>
          ))}
        </div>
 
        {canScrollRight && (
          <button
            onClick={() => scroll(1)}
            className="absolute right-0 top-0 bottom-0 z-20 w-12 bg-linear-to-l from-slate-950 to-transparent flex items-center justify-center opacity-0 group-hover/row:opacity-100 transition-opacity"
          >
            <div className="w-8 h-8 bg-white/20 hover:bg-white/30 rounded-full flex items-center justify-center transition-all">
              <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 5l7 7-7 7" />
              </svg>
            </div>
          </button>
        )}
      </div>
    </div>
  );
}
 
function HeroProperty({ property, onPlayVideo }) {
  if (!property) return null;
 
  return (
    <div className="relative h-[60vh] md:h-[70vh] overflow-hidden mb-8">
      <img
        src={property.thumbnailUrl || 'https://images.unsplash.com/photo-1560518883-ce09059eeffa?auto=format&fit=crop&w=1600&q=80'}
        alt={property.title}
        className="absolute inset-0 w-full h-full object-cover"
      />
      <div className="absolute inset-0 bg-linear-to-r from-slate-950 via-slate-950/60 to-transparent" />
      <div className="absolute inset-0 bg-linear-to-t from-slate-950 via-transparent to-transparent" />
 
      <div className="relative h-full flex items-end pb-12 px-6 md:px-12">
        <div className="max-w-lg">
          <span className="inline-block bg-orange-500 text-white text-[10px] font-bold uppercase tracking-widest px-3 py-1 rounded-full mb-4">
            Destaque
          </span>
          <h1 className="text-white text-3xl md:text-4xl font-black mb-3 leading-tight">
            {property.title}
          </h1>
          <p className="text-slate-300 text-sm mb-2">
            {property.address?.city && `${property.address.city} · `}{property.status}
          </p>
          <p className="text-orange-500 font-black text-2xl mb-6">
            {property.price?.amount
              ? new Intl.NumberFormat('pt-PT', { style: 'currency', currency: property.price.currency || 'EUR', maximumFractionDigits: 0 }).format(property.price.amount)
              : 'Preço sob consulta'}
          </p>
          <div className="flex items-center gap-3">
            <button
              onClick={() => onPlayVideo(property)}
              className="flex items-center gap-2 bg-white text-slate-900 font-bold px-5 py-2.5 rounded-xl hover:bg-orange-500 hover:text-white transition-all text-sm"
            >
              <svg className="w-4 h-4 fill-current ml-0.5" viewBox="0 0 24 24">
                <path d="M8 5v14l11-7z" />
              </svg>
              Ver apresentação
            </button>
            {property.externalSourceUrl && (
              <a
                href={property.externalSourceUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center gap-2 bg-white/20 hover:bg-white/30 border border-white/30 text-white font-semibold px-5 py-2.5 rounded-xl transition-all text-sm"
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
                </svg>
                Ver no site
              </a>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
 
export default function Properties() {
  const [properties, setProperties] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [activeVideoProperty, setActiveVideoProperty] = useState(null);
 
  useEffect(() => {
    let isMounted = true;
    getProperties()
      .then(data => {
        if (!isMounted) return;
        setProperties(Array.isArray(data) ? data : data?.value || []);
      })
      .catch(err => {
        if (!isMounted) return;
        console.error('Erro na API:', err);
        setError('Não foi possível carregar os imóveis. Tenta novamente mais tarde.');
      })
      .finally(() => {
        if (isMounted) setLoading(false);
      });
    return () => { isMounted = false; };
  }, []);
 
  const featured = properties.find(p => p.isFeatured) ?? properties[0];
  const withVideo = properties.filter(p => p.videoUrl || p.externalSourceUrl);
  const available = properties.filter(p => p.status === 'Available' || p.status === 'Disponível');
  const apartments = properties.filter(p => p.realEstateType?.typeName?.toLowerCase().includes('apartamento'));
  const houses = properties.filter(p => p.realEstateType?.typeName?.toLowerCase().includes('moradia'));
  const others = properties.filter(p =>
    !p.realEstateType?.typeName?.toLowerCase().includes('apartamento') &&
    !p.realEstateType?.typeName?.toLowerCase().includes('moradia')
  );
 
  return (
    <div className="min-h-screen bg-slate-950">
 
      {loading && (
        <div className="flex items-center justify-center h-screen">
          <div className="w-8 h-8 border-2 border-orange-500 border-t-transparent rounded-full animate-spin" />
        </div>
      )}
 
      {!loading && error && (
        <div className="flex items-center justify-center h-screen px-6">
          <div className="bg-red-500/10 border border-red-500/30 text-red-400 rounded-2xl p-6 text-center max-w-md">
            {error}
          </div>
        </div>
      )}
 
      {!loading && !error && properties.length === 0 && (
        <div className="flex items-center justify-center h-screen px-6">
          <div className="bg-white/5 border border-white/10 rounded-2xl p-12 text-center max-w-md">
            <p className="text-slate-300 font-semibold mb-1">Ainda não há imóveis disponíveis</p>
            <p className="text-slate-500 text-sm">Volta brevemente — novas angariações são adicionadas regularmente.</p>
          </div>
        </div>
      )}
 
      {!loading && !error && properties.length > 0 && (
        <>
          <HeroProperty property={featured} onPlayVideo={setActiveVideoProperty} />
 
          {withVideo.length > 0 && (
            <CategoryRow title="🎬 Com vídeo de apresentação" properties={withVideo} />
          )}
          {available.length > 0 && (
            <CategoryRow title="✅ Disponíveis agora" properties={available} />
          )}
          {apartments.length > 0 && (
            <CategoryRow title="🏢 Apartamentos" properties={apartments} />
          )}
          {houses.length > 0 && (
            <CategoryRow title="🏡 Moradias" properties={houses} />
          )}
          {others.length > 0 && (
            <CategoryRow title="📦 Outros imóveis" properties={others} />
          )}
 
          <div className="px-6 md:px-12 pb-16">
            <h2 className="text-white font-bold text-lg md:text-xl mb-6">Todos os imóveis</h2>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
              {properties.map(p => (
                <PropertyCard key={p.realEstateId} property={p} />
              ))}
            </div>
          </div>
        </>
      )}
 
      {activeVideoProperty && (
        <VideoModal property={activeVideoProperty} onClose={() => setActiveVideoProperty(null)} />
      )}
    </div>
  );
}