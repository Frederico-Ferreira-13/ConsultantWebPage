import { useState, useEffect, useCallback, useRef, startTransition  } from 'react';
import { useAuth } from "../../../context/useAuth.jsx";
import { API_BASE_URL as API_BASE } from '../../../config/api';

function Section({ title, children }) {
  return (
    <div className="bg-slate-900 rounded-2xl border border-white/10 overflow-hidden mb-6">
      <div className="px-6 py-4 border-b border-white/5">
        <h2 className="font-semibold text-white text-sm">{title}</h2>
      </div>
      <div className="p-6">{children}</div>
    </div>
  );
}

function Field({ label, type = 'text', value, onChange, placeholder, required, hint }) {
  return (
    <div>
      <label className="block text-xs font-medium text-slate-400 mb-1.5">
        {label} {required && <span className="text-red-400">*</span>}
      </label>
      <input
        type={type}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        required={required}
        className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-2.5 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 transition-colors"
      />
      {hint && <p className="text-slate-600 text-xs mt-1">{hint}</p>}
    </div>
  );
}

function Select({ label, value, onChange, options, required }) {
  return (
    <div>
      <label className="block text-xs font-medium text-slate-400 mb-1.5">
        {label} {required && <span className="text-red-400">*</span>}
      </label>
      <select
        value={value}
        onChange={onChange}
        required={required}
        className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-2.5 text-sm text-white focus:outline-none focus:border-orange-500 transition-colors"
      >
        <option value="">Selecionar...</option>
        {options.map(opt => (
          <option key={opt.value} value={opt.value}>{opt.label}</option>
        ))}
      </select>
    </div>
  );
}


function Feedback({ message, type }) {
  if (!message) return null;
  return (
    <p className={`text-xs mt-2 ${type === 'error' ? 'text-red-400' : 'text-emerald-400'}`}>
      {type === 'success' ? '✓ ' : '✕ '}{message}
    </p>
  );
}

export default function AdminProperties() {
  const { token, user } = useAuth();
  const [properties, setProperties] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [showImport, setShowImport] = useState(false);

  const [cities, setCities] = useState([]);
  const [citiesLoading, setCitiesLoading] = useState(true);
  
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    price: '',
    currency: 'EUR',
    street: '',
    number: '',
    city: '',
    postalCode: '',
    country: 'Portugal',
    complement: '',
    realEstateTypeId: '',
    cityId: '',
    thumbnailUrl: '',
    status: 'Disponível',
    isFeatured: false,
    typology: '', // NOVO: T0–T5+, usado para associar a categoria de tipologia após criação
  });


  // Media states
    const [mediaList, setMediaList] = useState([]);
    const [videoUrl, setVideoUrl] = useState('');
    const [uploadingVideo, setUploadingVideo] = useState(false);
    const videoInputRef = useRef(null);
    
    const [importUrl, setImportUrl] = useState('');
    const [feedback, setFeedback] = useState({ message: '', type: '' });
    const [submitting, setSubmitting] = useState(false);

    const propertyTypes = [
        { value: 1, label: 'Apartamento' },
        { value: 2, label: 'Moradia' },
        { value: 3, label: 'Terreno' },
        { value: 4, label: 'Escritório' },
        { value: 5, label: 'Armazém' },
        { value: 6, label: 'Garagem' },
        { value: 7, label: 'Prédio' },
    ];

    // NOVO: tipologia — usada para filtrar imóveis por número de quartos
    // (ver RealEstateRepository.SearchAsync, que filtra por Category.CategoryName).
    const typologyOptions = ['T0', 'T1', 'T2', 'T3', 'T4', 'T5+'].map(t => ({ value: t, label: t }));

    const fetchCities = useCallback(async () => {
        setCitiesLoading(true);
        try {
        const res = await fetch(`${API_BASE}/cities`, {
            headers: { Authorization: `Bearer ${token}` },
        });
        if (res.ok) {
            const data = await res.json();
            // Adapta conforme a estrutura do teu Result<T>
            // Se o backend devolve diretamente o array:
            const formatted = data.map(c => ({
            value: c.cityId,
            label: c.cityName,
            }));
            setCities(formatted);
        } else {
            console.error('Erro ao carregar cidades:', res.status);
        }
        } catch (err) {
        console.error('Erro de rede ao carregar cidades:', err);
        } finally {
        setCitiesLoading(false);
        }
    }, [token]);

    useEffect(() => {
        startTransition(() => {
        fetchCities();
        });
    }, [fetchCities]);

    const getCityName = (cityId) => {
        const city = cities.find(c => c.value === parseInt(cityId));
        return city?.label || '';
    };

    const fetchProperties = useCallback(async () => {
        setLoading(true);
        try {
        const res = await fetch(`${API_BASE}/realestate`, {
            headers: { Authorization: `Bearer ${token}` },
        });
        if (res.ok) {
            const data = await res.json();
            setProperties(data);
        }
        } catch {
        // silencioso
        } finally {
        setLoading(false);
        }
    }, [token]);

    useEffect(() => {
        startTransition(() => {
        fetchProperties();
        });
    }, [fetchProperties]);

  // ── UPLOAD DE MEDIA — via backend (MediaUploadController), autenticado ──
    // CORRIGIDO: antes fazia-se upload direto ao Cloudinary a partir do browser
    // com um "unsigned upload preset" visível no bundle JS — qualquer pessoa
    // podia usar essas credenciais para fazer upload arbitrário para a conta.
    // Agora o ficheiro é enviado ao nosso próprio backend, autenticado com o
    // token do consultor, que valida tipo/tamanho e só depois fala com o
    // Cloudinary do lado do servidor.
    const uploadMedia = async (file, mediaKind /* 'image' | 'video' */) => {
        const formData = new FormData();
        formData.append('file', file);

        const res = await fetch(`${API_BASE}/mediaupload/${mediaKind}`, {
            method: 'POST',
            headers: { Authorization: `Bearer ${token}` },
            body: formData,
        });

        if (!res.ok) {
            const errorText = await res.text();
            throw new Error(errorText || 'Erro no upload do ficheiro.');
        }

        const data = await res.json();
        return data.url;
    };

    const handleImageUpload = async (e) => {
        const files = Array.from(e.target.files);
        if (!files.length) return;

        setUploadingVideo(true);
        try {
        const uploads = await Promise.all(
            files.map(file => uploadMedia(file, 'image'))
        );
        
        const newMedia = uploads.map((url, i) => ({
            url,
            type: 'image',
            isMain: mediaList.length === 0 && i === 0,
        }));
        
        setMediaList(prev => [...prev, ...newMedia]);
        } catch {
        setFeedback({ message: 'Erro ao fazer upload das imagens.', type: 'error' });
        } finally {
        setUploadingVideo(false);
        }
    };

    const handleVideoUpload = async (e) => {
        const file = e.target.files[0];
        if (!file) return;

        setUploadingVideo(true);
        try {
        const url = await uploadMedia(file, 'video');
        setMediaList(prev => [...prev, { url, type: 'video', isMain: false }]);
        } catch {
        setFeedback({ message: 'Erro ao fazer upload do vídeo.', type: 'error' });
        } finally {
        setUploadingVideo(false);
        }
    };

  const handleVideoUrlAdd = () => {
    if (!videoUrl.trim()) return;
    setMediaList(prev => [...prev, { url: videoUrl, type: 'video', isMain: false }]);
    setVideoUrl('');
  };

  const removeMedia = (index) => {
    setMediaList(prev => prev.filter((_, i) => i !== index));
  };

  const setMainImage = (index) => {
    setMediaList(prev => prev.map((m, i) => ({ ...m, isMain: i === index })));
  };

  // ── CRIAR IMÓVEL ──
  const handleCreate = async (e) => {
    e.preventDefault();
    setFeedback({ message: '', type: '' });
    setSubmitting(true);

    try {
      const mainImage = mediaList.find(m => m.isMain && m.type === 'image');
      const thumbnail = mainImage?.url || mediaList.find(m => m.type === 'image')?.url || '';

      const payload = {
        title: formData.title,
        description: formData.description || null,
        price: parseFloat(formData.price),
        currency: formData.currency,
        street: formData.street,
        number: formData.number,
        city: formData.city,
        postalCode: formData.postalCode,
        country: formData.country,
        complement: formData.complement || null,
        realEstateTypeId: parseInt(formData.realEstateTypeId),
        cityId: parseInt(formData.cityId),
        thumbnailUrl: thumbnail || null,
        externalSourceUrl: null,
        externalReferenceId: null,
        status: formData.status || 'Disponível',
        isFeatured: formData.isFeatured,
        consultantId: user?.userId || 0,
      };

      const res = await fetch(`${API_BASE}/realestate`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        const errorText = await res.text();
        throw new Error(errorText);
      }
      
      const propertyData = await res.json();
      const propertyId = propertyData.realEstateId;

      // NOVO: associa a categoria de tipologia (T0–T5+), se selecionada.
      // Resolve o CategoriesId a partir do nome via o novo endpoint
      // GET /api/category/by-name/{name} e sincroniza com o imóvel.
      // Falhas aqui não abortam a criação do imóvel — a tipologia é um
      // metadado complementar, não um requisito para o imóvel existir.
      if (formData.typology) {
        try {
          const categoryRes = await fetch(`${API_BASE}/category/by-name/${formData.typology}`, {
            headers: { Authorization: `Bearer ${token}` },
          });

          if (categoryRes.ok) {
            const category = await categoryRes.json();
            await fetch(`${API_BASE}/realestatecategories/property/${propertyId}/sync`, {
              method: 'PUT',
              headers: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${token}`,
              },
              body: JSON.stringify({ categoryIds: [category.categoriesId] }),
            });
          }
        } catch {
          // Silencioso — a tipologia pode ser associada manualmente mais tarde.
        }
      }

      // 2. Adicionar media (fotos + vídeos)
      if (mediaList.length > 0) {
        const mediaPayload = mediaList.map(m => ({
          realEstateId: propertyId,
          mediaUrl: m.url,
          mediaType: m.type,
          isMain: m.isMain,
        }));

        // CORRIGIDO: era '/realestatema/batch' (typo, faltava "dia" no fim de
        // "realestatemedia"), o que fazia o upload de fotos/vídeos falhar
        // sempre com 404 sem nenhum aviso visível ao utilizador.
        await fetch(`${API_BASE}/realestatemedia/batch`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({ mediaList: mediaPayload }),
        });
      }

      setFeedback({ message: 'Imóvel criado com sucesso!', type: 'success' });
      resetForm();
      setShowForm(false);
      fetchProperties();
    } catch (err) {
      setFeedback({ message: err.message || 'Erro ao criar imóvel.', type: 'error' });
    } finally {
      setSubmitting(false);
    }
  };

  const resetForm = () => {
    setFormData({
      title: '', description: '', price: '', currency: 'EUR',
      street: '', number: '', city: '', postalCode: '', country: 'Portugal',
      complement: '', realEstateTypeId: '', cityId: '', thumbnailUrl: '',
      status: 'Disponível', isFeatured: false, typology: '',
    });
    setMediaList([]);
    setVideoUrl('');
  };

  // ── IMPORTAR POR URL ──
  const handleImport = async (e) => {
    e.preventDefault();
    setFeedback({ message: '', type: '' });
    setSubmitting(true);

    try {
      const res = await fetch(`${API_BASE}/realestate/import`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ url: importUrl }),
      });

      if (!res.ok) throw new Error(await res.text());
      
      setFeedback({ message: 'Imóvel importado com sucesso!', type: 'success' });
      setImportUrl('');
      setShowImport(false);
      fetchProperties();
    } catch (err) {
      setFeedback({ message: err.message || 'Erro ao importar imóvel.', type: 'error' });
    } finally {
      setSubmitting(false);
    }
  };

  const handleStatusChange = async (id, isActive) => {
    try {
      await fetch(`${API_BASE}/realestate/${id}/status`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ isActive }),
      });
      fetchProperties();
    } catch {
      // silencioso
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-white">Imóveis</h1>
          <p className="text-slate-500 text-sm mt-1">Gira os imóveis da plataforma.</p>
        </div>
        <div className="flex items-center gap-3">
          <button
            onClick={() => { setShowImport(!showImport); setShowForm(false); setFeedback({ message: '', type: '' }); }}
            className="bg-white/5 hover:bg-white/10 border border-white/10 text-slate-300 text-sm font-medium px-4 py-2.5 rounded-xl transition-all"
          >
            {showImport ? 'Cancelar' : 'Importar URL'}
          </button>
          <button
            onClick={() => { setShowForm(!showForm); setShowImport(false); setFeedback({ message: '', type: '' }); }}
            className="bg-orange-500 hover:bg-orange-600 text-white text-sm font-medium px-4 py-2.5 rounded-xl transition-all"
          >
            {showForm ? 'Cancelar' : '+ Novo Imóvel'}
          </button>
        </div>
      </div>

      {/* Import URL */}
      {showImport && (
        <Section title="Importar por URL">
          <form onSubmit={handleImport} className="space-y-4">
            <Field
              label="URL do imóvel (RE/MAX, Century 21, etc.)"
              value={importUrl}
              onChange={(e) => setImportUrl(e.target.value)}
              placeholder="https://www.remax.pt/..."
              required
            />
            <Feedback {...feedback} />
            <button
              type="submit"
              disabled={submitting}
              className="bg-orange-500 hover:bg-orange-600 disabled:opacity-50 text-white text-sm font-medium px-5 py-2.5 rounded-xl transition-all"
            >
              {submitting ? 'A importar...' : 'Importar'}
            </button>
          </form>
        </Section>
      )}

      {/* Create Form */}
      {showForm && (
        <Section title="Novo Imóvel">
          <form onSubmit={handleCreate} className="space-y-6">
            {/* Dados básicos */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Field
                label="Título"
                value={formData.title}
                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                placeholder="T3 em Lisboa"
                required
              />
              <Select
                label="Tipo de Imóvel"
                value={formData.realEstateTypeId}
                onChange={(e) => setFormData({ ...formData, realEstateTypeId: e.target.value })}
                options={propertyTypes}
                required
              />
              <Select
                label="Tipologia"
                value={formData.typology}
                onChange={(e) => setFormData({ ...formData, typology: e.target.value })}
                options={typologyOptions}
              />
              <Field
                label="Preço"
                type="number"
                value={formData.price}
                onChange={(e) => setFormData({ ...formData, price: e.target.value })}
                placeholder="250000"
                required
              />
              <Select
                label="Moeda"
                value={formData.currency}
                onChange={(e) => setFormData({ ...formData, currency: e.target.value })}
                options={[
                  { value: 'EUR', label: 'EUR (€)' },
                  { value: 'USD', label: 'USD ($)' },
                ]}
              />
              <Field
                label="Rua"
                value={formData.street}
                onChange={(e) => setFormData({ ...formData, street: e.target.value })}
                placeholder="Avenida da Liberdade"
                required
              />
              <Field
                label="Número"
                value={formData.number}
                onChange={(e) => setFormData({ ...formData, number: e.target.value })}
                placeholder="123"
                required
              />
              <Select
                label="Cidade"
                value={formData.cityId}
                onChange={(e) => {
                  const selectedCityId = e.target.value;
                  setFormData({
                    ...formData,
                    cityId: selectedCityId,
                    city: getCityName(selectedCityId),
                  });
                }}
                options={cities}
                required
                disabled={citiesLoading}
              />
              {citiesLoading && <p className="text-slate-500 text-xs mt-1 col-span-2">A carregar cidades...</p>}
              <Field
                label="Código Postal"
                value={formData.postalCode}
                onChange={(e) => setFormData({ ...formData, postalCode: e.target.value })}
                placeholder="1000-001"
                required
              />
              <Field
                label="País"
                value={formData.country}
                onChange={(e) => setFormData({ ...formData, country: e.target.value })}
                placeholder="Portugal"
                required
              />
              <Field
                label="Complemento"
                value={formData.complement}
                onChange={(e) => setFormData({ ...formData, complement: e.target.value })}
                placeholder="3º Esq."
              />
              <Select
                label="Status"
                value={formData.status}
                onChange={(e) => setFormData({ ...formData, status: e.target.value })}
                options={[
                  { value: 'Disponível', label: 'Disponível' },
                  { value: 'Reservado', label: 'Reservado' },
                  { value: 'Vendido', label: 'Vendido' },
                ]}
              />
            </div>

            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="isFeatured"
                checked={formData.isFeatured}
                onChange={(e) => setFormData({ ...formData, isFeatured: e.target.checked })}
                className="rounded border-white/10 bg-slate-950 text-orange-500 focus:ring-orange-500"
              />
              <label htmlFor="isFeatured" className="text-sm text-slate-400">Destacar imóvel</label>
            </div>

            <div>
              <label className="block text-xs font-medium text-slate-400 mb-1.5">Descrição</label>
              <textarea
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                placeholder="Descrição detalhada do imóvel..."
                rows={4}
                className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-2.5 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 transition-colors resize-none"
              />
            </div>

            {/* ── MEDIA: FOTOS + VÍDEOS ── */}
            <div className="border-t border-white/10 pt-6">
              <h3 className="text-sm font-semibold text-white mb-4">Fotos e Vídeos</h3>

              {/* Upload de imagens */}
              <div className="mb-4">
                <label className="cursor-pointer bg-white/5 hover:bg-white/10 border border-white/10 border-dashed text-slate-300 text-xs font-medium px-4 py-3 rounded-xl transition-colors flex items-center justify-center gap-2">
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                  {uploadingVideo ? 'A carregar...' : 'Carregar fotos'}
                  <input type="file" accept="image/*" multiple className="hidden" onChange={handleImageUpload} />
                </label>
              </div>

              {/* Upload de vídeo + URL */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
                <div>
                  <label className="cursor-pointer bg-white/5 hover:bg-white/10 border border-white/10 border-dashed text-slate-300 text-xs font-medium px-4 py-3 rounded-xl transition-colors flex items-center justify-center gap-2">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
                    </svg>
                    {uploadingVideo ? 'A carregar...' : 'Carregar vídeo'}
                    <input type="file" accept="video/*" className="hidden" onChange={handleVideoUpload} ref={videoInputRef} />
                  </label>
                </div>
                <div className="flex gap-2">
                  <input
                    type="url"
                    value={videoUrl}
                    onChange={(e) => setVideoUrl(e.target.value)}
                    placeholder="Ou colar link do vídeo (YouTube, Vimeo...)"
                    className="flex-1 bg-slate-950 border border-white/10 rounded-xl px-4 py-2.5 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 transition-colors"
                  />
                  <button
                    type="button"
                    onClick={handleVideoUrlAdd}
                    className="bg-white/10 hover:bg-white/20 text-white text-xs font-medium px-3 py-2 rounded-xl transition-all"
                  >
                    Adicionar
                  </button>
                </div>
              </div>

              {/* Preview das media */}
              {mediaList.length > 0 && (
                <div className="grid grid-cols-4 md:grid-cols-6 gap-2">
                  {mediaList.map((media, index) => (
                    <div key={index} className="relative group aspect-square rounded-xl overflow-hidden bg-slate-800 border border-white/10">
                      {media.type === 'image' ? (
                        <img src={media.url} alt="" className="w-full h-full object-cover" />
                      ) : (
                        <div className="w-full h-full flex items-center justify-center">
                          <svg className="w-8 h-8 text-slate-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
                          </svg>
                        </div>
                      )}

                      <span className={`absolute top-1 left-1 text-[9px] font-bold px-1.5 py-0.5 rounded ${media.type === 'video' ? 'bg-red-500/80 text-white' : 'bg-blue-500/80 text-white'}`}>
                        {media.type === 'video' ? 'VÍDEO' : 'IMG'}
                      </span>

                      {media.isMain && (
                        <span className="absolute top-1 right-1 text-[9px] font-bold px-1.5 py-0.5 rounded bg-orange-500 text-white">
                          PRINCIPAL
                        </span>
                      )}

                      <div className="absolute inset-0 bg-black/60 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center gap-1">
                        {media.type === 'image' && !media.isMain && (
                          <button
                            type="button"
                            onClick={() => setMainImage(index)}
                            className="text-[10px] bg-orange-500 text-white px-2 py-1 rounded"
                          >
                            Principal
                          </button>
                        )}
                        <button
                          type="button"
                          onClick={() => removeMedia(index)}
                          className="text-[10px] bg-red-500 text-white px-2 py-1 rounded"
                        >
                          Remover
                        </button>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>

            <Feedback {...feedback} />
            <button
              type="submit"
              disabled={submitting || uploadingVideo}
              className="bg-orange-500 hover:bg-orange-600 disabled:opacity-50 text-white text-sm font-medium px-5 py-2.5 rounded-xl transition-all"
            >
              {submitting ? 'A guardar...' : 'Criar Imóvel'}
            </button>
          </form>
        </Section>
      )}

      {/* Lista */}
      <Section title={`Lista de Imóveis (${properties.length})`}>
        {loading ? (
          <div className="space-y-2">
            {[1, 2, 3].map(i => (
              <div key={i} className="h-16 bg-white/5 rounded-xl animate-pulse" />
            ))}
          </div>
        ) : properties.length === 0 ? (
          <p className="text-slate-600 text-sm">Nenhum imóvel registado.</p>
        ) : (
          <div className="space-y-2">
            {properties.map((p) => (
              <div
                key={p.realEstateId}
                className="flex items-center justify-between bg-white/5 border border-white/5 rounded-xl px-4 py-3"
              >
                <div className="flex items-center gap-3">
                  {p.thumbnailUrl ? (
                    <img src={p.thumbnailUrl} alt="" className="w-12 h-12 rounded-lg object-cover" />
                  ) : (
                    <div className="w-12 h-12 rounded-lg bg-slate-800 flex items-center justify-center">
                      <span className="text-slate-600 text-xs">🏠</span>
                    </div>
                  )}
                  <div>
                    <p className="text-white text-sm font-medium">{p.title}</p>
                    <p className="text-slate-500 text-xs">
                      {p.price?.amount?.toLocaleString('pt-PT')} {p.price?.currency} • {p.status}
                    </p>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <button
                    onClick={() => handleStatusChange(p.realEstateId, !p.isActive)}
                    className={`text-xs px-3 py-1.5 rounded-lg transition-all ${
                      p.isActive !== false
                        ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20'
                        : 'bg-red-500/10 text-red-400 border border-red-500/20'
                    }`}
                  >
                    {p.isActive !== false ? 'Ativo' : 'Inativo'}
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </Section>
    </div>
  );
}