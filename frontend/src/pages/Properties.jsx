import { useEffect, useState } from "react";
import { getProperties } from '../services/realEstateService.js';
import PropertyCard from "../components/PropertyCard.jsx";

export default function Properties() {
    const [properties, setProperties] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        let isMounted = true;

       getProperties()
            .then(data => {
                if (!isMounted) return;
                setProperties(Array.isArray(data) ? data : data?.value || []);
            })
            .catch(err => {
                if (!isMounted) return;
                console.error("Erro na API:", err);
                setError("Não foi possível carregar os imóveis. Tenta novamente mais tarde.");
            })
            .finally(() => {
                if (isMounted) setLoading(false);
            });

        return () => { isMounted = false; };
    }, []);

    return (
        <div className="min-h-screen bg-slate-950 px-6 md:px-12 py-12">
            <div className="max-w-6xl mx-auto">
                <h1 className="text-white text-3xl md:text-4xl font-black mb-2">Imóveis Disponíveis</h1>
                <p className="text-slate-400 mb-10">
                    {!loading && !error && `${properties.length} ${properties.length === 1 ? 'imóvel encontrado' : 'imóveis encontrados'}`}
                </p>

                {loading && (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {Array.from({ length: 6 }).map((_, i) => (
                            <div key={i} className="bg-slate-900 rounded-2xl overflow-hidden border border-white/10 animate-pulse">
                                <div className="aspect-4/3 bg-slate-800" />
                                <div className="p-5 space-y-3">
                                    <div className="h-4 bg-slate-800 rounded w-3/4" />
                                    <div className="h-3 bg-slate-800 rounded w-1/2" />
                                    <div className="h-5 bg-slate-800 rounded w-1/3" />
                                </div>
                            </div>
                        ))}
                    </div>
                )}

                {!loading && error && (
                    <div className="bg-red-500/10 border border-red-500/30 text-red-400 rounded-2xl p-6 text-center">
                        {error}
                    </div>
                )}

                {!loading && !error && properties.length === 0 && (
                    <div className="bg-white/5 border border-white/10 rounded-2xl p-12 text-center">
                        <p className="text-slate-300 font-semibold mb-1">Ainda não há imóveis disponíveis</p>
                        <p className="text-slate-500 text-sm">Volta brevemente — novas angariações são adicionadas regularmente.</p>
                    </div>
                )}

                {!loading && !error && properties.length > 0 && (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {properties.map(p => (
                            <PropertyCard key={p.realEstateId} property={p} />
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}