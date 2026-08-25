import { useState, useEffect, useCallback, startTransition } from 'react';
import { useAuth } from '../../../context/useAuth';
import { API_BASE_URL as API_BASE } from '../../../config/api';

function Section({ title, description, children }) {
    return (
        <div className='bg-slate-900 rounded-2xl border border-white/10 overflow-hidden'>
            <div className='px-6 py-4 border-b border-white/5'>
                <h2 className='font-semibold text-white text-sm'>{title}</h2>
                {description && <p className="text-slate-500 text-xs mt-0.5">{description}</p>}
            </div>
            <div className='p-6'>{children}</div>
        </div>
    );
}

function Field({ label, value, onChange, placeholder, type = 'text', hint }) {
    return (
        <div>
            <label className='block text-xs font-medium text-slate-400 mb-1.5'>{label}</label>
            <input 
                type={type}
                value={value}
                onChange={onChange}
                placeholder={placeholder}
                className='w-full bg-slate-950 border-white/10 rounded-x1 px-4 py-2.5 text-sm text-white placeholder-slate-600
                focus:outline-none focus:border-orange-500 transition-colors'
            />
            {hint && <p className='text-slate-600 text-xs mt-1'>{hint}</p>}
        </div>
    );
}

function Feedback({ message, type }) {
    if (!message) return null;
    return (
    <p className={`text-xs mt-2 ${type === 'error'  ? 'text-red-400' : 'text-emerald-400'}`}>
        {type === 'success' ? '✓ ' : '✕ '}{message}
    </p>
    );
}

const EMPTY_FORM = {
    bio: '',
    profileImageUrl: '',
    linkedIn: '',
    instagram: '',
    x: '',
    facebook: '',
};

export default function AdminAboutMe() {
    const { user, token } = useAuth();

    const [formData, setFormData] = useState(EMPTY_FORM);
    const {isNew, setIsNew} = useState(true);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [feedback, setFeedback] = useState({ message: '', type: '' });

    const fetchProfile = useCallback(async () => {
        if (!user?.userId) return;

        setLoading(true);
        try{
            const res = await fetch(`${API_BASE}/AboutMe?userId=${user.userId}`, {
                headers: { Authorization: `Bearer ${token}`},
            });

            if (res.ok) {
                const data = await res.json();
                setFormData({
                    bio: data.bio ?? '',
                    profileImageUrl: data.profileImageUrl ?? '',
                    yearsOfExperience: data.yearsOfExperience ?? '',
                    linkedIn: data.socialUrl_Linkedin ?? '',
                    instagram: data.socialUrl_Instagram ?? '',
                    x: data.socialUrl_X ?? '',
                    facebook: data.socialUrl_Facebook ?? '',
                });
                setIsNew(false);
            } else {
                setFormData(EMPTY_FORM);
                setIsNew(true);
            }
        } catch {
        setFeedback({ message: 'Não foi possível carregar o perfil.', type: 'error' });
        } finally {
        setLoading(false);
        }
    }, [user?.userId, token]);

    useEffect(() => {
        startTransition(() => {
        fetchProfile();
        });
    }, [fetchProfile]);

     const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setFeedback({ message: '', type: '' });
 
    try {
      const url = isNew ? `${API_BASE}/AboutMe` : `${API_BASE}/AboutMe/${user.userId}`;
      const method = isNew ? 'POST' : 'PUT';
 
      const res = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          userId: user.userId,
          bio: formData.bio,
          profileImageUrl: formData.profileImageUrl || null,
          yearsOfExperience: parseInt(formData.yearsOfExperience, 10) || 0,
          linkedIn: formData.linkedIn || null,
          instagram: formData.instagram || null,
          x: formData.x || null,
          facebook: formData.facebook || null,
        }),
      });
 
      if (!res.ok) throw new Error(await res.text());
 
      setFeedback({ message: 'Perfil "Sobre Mim" atualizado com sucesso.', type: 'success' });
      setIsNew(false);
    } catch (err) {
      setFeedback({ message: err.message || 'Erro ao guardar o perfil.', type: 'error' });
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="max-w-2xl space-y-4">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-32 bg-white/5 rounded-2xl animate-pulse" />
        ))}
      </div>
    );
  }

  return (
    <div className="max-w-2xl space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-white">Sobre Mim</h1>
        <p className="text-slate-500 text-sm mt-1">
          Este conteúdo aparece na página pública "Sobre Mim" do site.
        </p>
      </div>
 
      <form onSubmit={handleSubmit} className="space-y-6">
        <Section title="Biografia" description="Apresentação que os visitantes veem na página pública.">
          <div className="space-y-4">
            <div>
              <label className="block text-xs font-medium text-slate-400 mb-1.5">Bio</label>
              <textarea
                value={formData.bio}
                onChange={(e) => setFormData({ ...formData, bio: e.target.value })}
                placeholder="Fale sobre o seu percurso, experiência e abordagem ao mercado imobiliário..."
                rows={6}
                className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 resize-none"
              />
            </div>
            <Field
              label="Anos de experiência"
              type="number"
              value={formData.yearsOfExperience}
              onChange={(e) => setFormData({ ...formData, yearsOfExperience: e.target.value })}
              placeholder="10"
            />
            <Field
              label="URL da foto de perfil"
              value={formData.profileImageUrl}
              onChange={(e) => setFormData({ ...formData, profileImageUrl: e.target.value })}
              placeholder="https://..."
            />
          </div>
        </Section>
 
        <Section title="Redes sociais" description="Links opcionais, aparecem como ícones na página pública.">
          <div className="space-y-4">
            <Field
              label="LinkedIn"
              value={formData.linkedIn}
              onChange={(e) => setFormData({ ...formData, linkedIn: e.target.value })}
              placeholder="https://linkedin.com/in/..."
            />
            <Field
              label="Instagram"
              value={formData.instagram}
              onChange={(e) => setFormData({ ...formData, instagram: e.target.value })}
              placeholder="https://instagram.com/..."
            />
            <Field
              label="X (Twitter)"
              value={formData.x}
              onChange={(e) => setFormData({ ...formData, x: e.target.value })}
              placeholder="https://x.com/..."
            />
            <Field
              label="Facebook"
              value={formData.facebook}
              onChange={(e) => setFormData({ ...formData, facebook: e.target.value })}
              placeholder="https://facebook.com/..."
            />
          </div>
        </Section>
 
        <Feedback {...feedback} />
        <button
          type="submit"
          disabled={saving}
          className="bg-orange-500 hover:bg-orange-600 disabled:opacity-50 text-white text-sm font-medium px-5 py-2.5 rounded-xl transition-all"
        >
          {saving ? 'A guardar...' : 'Guardar alterações'}
        </button>
      </form>
    </div>
  );
}