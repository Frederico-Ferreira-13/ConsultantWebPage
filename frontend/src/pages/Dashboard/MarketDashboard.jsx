import { useState } from "react"
import { useAuth } from "../../context/useAuth";

export default function MarketDashboard() {
    const { user } = useAuth();
    const [zone, setZone] = useState(user?.preferredZone || 'Selecione uma zona');
    const [isEditing, setIsEditing] = useState(false);

    const handleZoneChange = (newZone) => {
        setZone(newZone);
        setIsEditing(false); // Fecha o modo de edição
        // Aqui, futuramente, disparas o envio para a tua base de dados (notificação)
        console.log(`Utilizador mudou a pesquisa para: ${newZone}`);
    };

    const marketData = {
        avgPrice: 2450,
        variation: 3.2,
        volume: 142,
        history: [2200, 2250, 2300, 2380, 2450]
    };

    return (
    <div className="p-6 md:p-12 text-white bg-slate-950 rounded-2xl">
      <div className="max-w-5xl mx-auto">
        <header className="mb-10">
          <h1 className="text-4xl font-black mb-2">Observatório de Mercado</h1>
          <p className="text-slate-400">Acompanhe a valorização imobiliária em tempo real.</p>
        </header>

        {/* Seletor de Zona */}
        <div className="bg-slate-900 p-6 rounded-2xl border border-white/10 mb-8 flex items-center justify-between">
          {isEditing ? (
            <div className="flex gap-2 w-full">
              <input 
                className="bg-slate-800 border border-white/10 rounded-lg px-4 py-2 w-full text-white outline-none focus:border-orange-500"
                placeholder="Escreva a zona..."
                onBlur={(e) => handleZoneChange(e.target.value)}
                autoFocus
              />
            </div>
          ) : (
            <>
              <span className="font-bold">Zona monitorizada: <span className="text-orange-500">{zone}</span></span>
              <button 
                onClick={() => setIsEditing(true)}
                className="text-xs bg-white/5 px-4 py-2 rounded-lg hover:bg-white/10 transition-all"
              >
                Alterar zona
              </button>
            </>
          )}
        </div>

        {/* KPIs principais */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div className="bg-slate-900 p-6 rounded-2xl border border-white/10">
            <p className="text-slate-500 text-sm mb-1">Preço Médio / m²</p>
            <p className="text-3xl font-black">{marketData.avgPrice.toLocaleString('pt-PT')} €</p>
          </div>
          <div className="bg-slate-900 p-6 rounded-2xl border border-white/10">
            <p className="text-slate-500 text-sm mb-1">Valorização (semestral)</p>
            <p className="text-3xl font-black text-emerald-500">+{marketData.variation}%</p>
          </div>
          <div className="bg-slate-900 p-6 rounded-2xl border border-white/10">
            <p className="text-slate-500 text-sm mb-1">Transações na zona</p>
            <p className="text-3xl font-black text-orange-500">{marketData.volume}</p>
          </div>
        </div>

        {/* Gráfico */}
        <div className="bg-slate-900 p-8 rounded-2xl border border-white/10">
          <h3 className="font-bold mb-6">Evolução do Valor m² nos últimos 5 meses</h3>
          <div className="flex items-end gap-4 h-40">
            {marketData.history.map((val, i) => (
              <div key={i} className="flex-1 flex flex-col items-center gap-2">
                <div 
                  className="w-full bg-orange-500 rounded-t-lg transition-all" 
                  style={{ height: `${(val / 2500) * 100}%` }}
                />
                <span className="text-[10px] text-slate-500">Mês {i + 1}</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
