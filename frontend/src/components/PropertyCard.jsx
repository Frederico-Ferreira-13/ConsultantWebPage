import { Link } from 'react-router-dom';

function formatPrice(amount, currency = 'EUR') {
    if(amount == null) return 'Preço sob consulta';
    return new Intl.NumberFormat('pt-PT', {
        style: 'currency',
        currency,
        maximumFractionDigits: 0,
    }).format(amount);
}

export default function PropertyCard({ property }) {
   const {
        realEstateId,
        title,
        thumbnailUrl,
        price,
        address,
        status,
    } = property;

    return (
        <Link
            to={`/imoveis/${realEstateId}`}
            className="group bg-slate-900 rounded-2xl overflow-hidden border border-white/10 hover:border-orange-500/50 transition-all shadow-lg hover:shadow-orange-500/10 block"
        >
            <div className="relative aspect-4/3 overflow-hidden bg-slate-800">
                {thumbnailUrl ? (
                    <img
                        src={thumbnailUrl}
                        alt={title}
                        className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                    />
                ) : (
                    <div className="w-full h-full flex items-center justify-center text-slate-600 text-sm">
                        Sem imagem
                    </div>
                )}

                {status && (
                    <span className="absolute top-3 left-3 bg-slate-950/80 backdrop-blur-md text-xs font-semibold text-white px-3 py-1 rounded-lg">
                        {status}
                    </span>
                )}
            </div>

            <div className="p-5 space-y-2">
                <h3 className="text-white font-bold text-lg leading-snug line-clamp-2">
                    {title}
                </h3>

                {address?.city && (
                    <p className="text-slate-400 text-sm flex items-center gap-1">
                        <svg className="w-4 h-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17.657 16.657L13.414 20.9a2 2 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                        </svg>
                        {address.city}
                    </p>
                )}

                <p className="text-orange-500 font-bold text-xl pt-1">
                    {formatPrice(price?.amount, price?.currency)}
                </p>
            </div>
        </Link>
    );
}