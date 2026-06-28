import { useState } from 'react';
import { Link } from 'react-router-dom';

export default function Home() {
    const [searchQuery, setSearchQuery] = useState('');

    return (
        <div className='min-h-screen bg-slate-50 text-slate-800 font-sans antialiased'>

            {/* 1. HERO SECTION */}
            <section className='relative bg-slate-950 text-white py-24 md:py-32 px-6 overflow-hidden'>
                <div className='absolute top-0 right-0 w-96 h-96 bg-orange-500/10 rounded-full blur-3xl pointer-events-none'></div>
                <div className='absolute bottom-0 left-0 w-96 h-96 bg-blue-600/10 rounded-full blur-3xl pointer-events-none'></div>

                <div className='max-w-5xl mx-auto text-center relative z-10'>
                    <span className='text-orange-500 font-semibold tracking-widest text-sm uppercase bg-orange-500/10 px-3 py-1 rounded-full'>
                        Serviço Imobiliário Personalizado
                    </span>
                    <h1 className="text-4xl md:text-6xl font-extrabold tracking-tight mt-4 mb-6 leading-tight">
                        Onde as grandes histórias <br />
                        <span className="bg-linear-to-r from-orange-400 to-orange-600 bg-clip-text text-transparent">
                            ganham um novo lar.
                        </span>
                    </h1>
                    <p className="text-base md:text-xl text-slate-400 max-w-2xl mx-auto mb-10">
                        Encontre o imóvel ideal com o acompanhamento de um consultor focado em transformar os seus objetivos em realidade.
                    </p>

                    <div className="bg-white p-2 rounded-2xl shadow-2xl flex flex-col md:flex-row gap-2 max-w-3xl mx-auto text-slate-900 border border-slate-200">
                        <div className="flex-1 flex items-center px-3">
                            <svg className="w-5 h-5 text-slate-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
                            <input 
                                type="text" 
                                placeholder="Ex: Apartamento T2 em Lisboa..." 
                                className="w-full py-3 bg-transparent focus:outline-none text-slate-700"
                                value={searchQuery}
                                onChange={(e) => setSearchQuery(e.target.value)}
                            />
                        </div>
                        <button className="bg-orange-500 hover:bg-orange-600 active:scale-95 text-white font-medium px-8 py-3.5 rounded-xl transition-all shadow-lg shadow-orange-500/20">
                            Descobrir Imóveis
                        </button>
                    </div>
                </div>
            </section>

            {/* 2. SECÇÃO DO CONSULTOR */}
            <section className="max-w-6xl mx-auto py-20 px-6">
                <div className="grid grid-cols-1 lg:grid-cols-12 gap-12 items-center">
                    <div className="lg:col-span-5 space-y-6">
                        <div className="w-12 h-1 bg-orange-500 rounded"></div>
                        <h2 className="text-3xl md:text-4xl font-bold text-slate-900 tracking-tight">
                            Olá, sou o Frederico. <br />
                            O seu parceiro nesta viagem.
                        </h2>
                        <p className="text-slate-600 leading-relaxed">
                            Com uma visão moderna e apoiado nas melhores ferramentas tecnológicas do mercado, foco-me em descomplicar o processo de compra e venda de casa. 
                        </p>
                        <div className="pt-4">
                            <Link to="/sobre" className="inline-flex items-center text-orange-500 font-semibold hover:text-orange-600 transition-colors">
                                Saber mais sobre o meu percurso
                                <svg className="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M14 5l7 7m0 0l-7 7m7-7H3" /></svg>
                            </Link>
                        </div>
                    </div>
                    <div className="lg:col-span-7">
                        <div className="relative rounded-2xl overflow-hidden shadow-2xl bg-slate-900 aspect-video group border border-slate-200">
                            <img src="https://images.unsplash.com/photo-1560518883-ce09059eeffa?auto=format&fit=crop&w=800&q=80" alt="Consultor" className="w-full h-full object-cover opacity-80 group-hover:scale-105 transition-transform duration-500" />
                            <div className="absolute inset-0 flex items-center justify-center">
                                <button className="w-16 h-16 bg-orange-500 hover:bg-orange-600 text-white rounded-full flex items-center justify-center shadow-xl shadow-orange-500/30 active:scale-90 transition-transform">
                                    <svg className="w-8 h-8 fill-current ml-1" viewBox="0 0 24 24"><path d="M8 5v14l11-7z"></path></svg>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* 3. NOVA SECÇÃO: DISC QUIZ (Foco em Conversão) */}
            <section className="py-20 bg-slate-900 text-white text-center px-6">
                <div className="max-w-3xl mx-auto">
                    <span className="text-orange-500 font-bold tracking-widest text-xs uppercase mb-4 block">Ferramenta Exclusiva</span>
                    <h2 className="text-3xl md:text-4xl font-bold mb-6">Como toma as suas decisões?</h2>
                    <p className="text-slate-400 mb-10 text-lg">
                        Saber como prefere comunicar e decidir é a chave para um negócio imobiliário sem stress. Descubra o seu perfil DISC e entenda como posso ajudá-lo melhor.
                    </p>
                    <Link to="/discquiz" className="inline-flex items-center bg-orange-500 hover:bg-orange-600 text-white font-bold px-8 py-4 rounded-xl transition-all shadow-lg shadow-orange-500/20 active:scale-95">
                        Fazer Teste DISC Gratuito
                        <svg className="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 5l7 7-7 7" /></svg>
                    </Link>
                </div>
            </section>

            {/* 4. SECÇÃO DE MERCADO */}
            <section className="bg-slate-100 py-16 px-6">
                <div className="max-w-6xl mx-auto">
                    <div className="flex flex-col md:flex-row justify-between items-end mb-10 gap-4">
                        <div>
                            <h2 className="text-3xl font-bold text-slate-900">Mercado em Foco</h2>
                            <p className="text-slate-500 text-sm mt-1">Notas e conselhos sobre a atualidade imobiliária.</p>
                        </div>
                        <Link to="/mercado" className="text-sm font-semibold bg-white border border-slate-300 hover:bg-slate-50 px-4 py-2 rounded-xl text-slate-700">Ver todas as notícias</Link>
                    </div>
                    {/* Grid de Posts */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <article className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 hover:shadow-md transition-shadow">
                            <span className="text-xs font-bold text-orange-500 uppercase">Dicas</span>
                            <h3 className="text-lg font-bold text-slate-900 mt-2 mb-3">3 pormenores que valorizam a sua casa</h3>
                            <p className="text-slate-600 text-sm">Pequenas reparações e organização podem aumentar o valor do seu imóvel.</p>
                        </article>
                        <article className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 hover:shadow-md transition-shadow">
                            <span className="text-xs font-bold text-orange-500 uppercase">Mercado</span>
                            <h3 className="text-lg font-bold text-slate-900 mt-2 mb-3">Tendências de 2026: Comprar ou esperar?</h3>
                            <p className="text-slate-600 text-sm">Análise sobre taxas de juro e oportunidades de crédito atuais.</p>
                        </article>
                    </div>
                </div>
            </section>

        </div>
    );
}