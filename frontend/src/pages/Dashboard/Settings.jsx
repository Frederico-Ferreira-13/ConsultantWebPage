import { useState } from 'react';
import { useAuth } from '../../context/useAuth';
 
const API_BASE = 'http://localhost:5170/api';

function Section({ title, description, children }) {
  return (
    <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden">
      <div className="px-6 py-4 border-b border-slate-100">
        <h2 className="font-semibold text-slate-900 text-sm">{title}</h2>
        {description && <p className="text-slate-500 text-xs mt-0.5">{description}</p>}
      </div>
      <div className="p-6">{children}</div>
    </div>
  );
}

function Field({ label, type = 'text', value, onChange, placeholder, hint }) {
  return (
    <div>
      <label className="block text-xs font-medium text-slate-700 mb-1.5">{label}</label>
      <input
        type={type}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        className="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm text-slate-900 focus:outline-none focus:border-orange-400 transition-colors"
      />
      {hint && <p className="text-slate-400 text-xs mt-1">{hint}</p>}
    </div>
  );
}

function Feedback({ message, type }) {
  if (!message) return null;
  return (
    <p className={`text-xs mt-2 ${type === 'error' ? 'text-red-500' : 'text-emerald-600'}`}>
      {type === 'success' ? '✓ ' : '✕ '}{message}
    </p>
  );
}

export default function Settings() {
  const { user, token } = useAuth();
 
  // Perfil
  const [profileData, setProfileData] = useState({
    email: user?.email ?? '',
    userName: user?.userName ?? '',
  });
  const [profileFeedback, setProfileFeedback] = useState({ message: '', type: '' });
  const [profileLoading, setProfileLoading] = useState(false);
 
  // Password
  const [passwordData, setPasswordData] = useState({
    oldPassword: '',
    newPassword: '',
    confirmPassword: '',
  });
  const [passwordFeedback, setPasswordFeedback] = useState({ message: '', type: '' });
  const [passwordLoading, setPasswordLoading] = useState(false);
 
  // Foto de perfil
  const [photoPreview, setPhotoPreview] = useState(null);
  const [photoFeedback, setPhotoFeedback] = useState({ message: '', type: '' });
 
  const handlePhotoChange = (e) => {
    const file = e.target.files[0];
    if (!file) return;
    if (file.size > 2 * 1024 * 1024) {
      setPhotoFeedback({ message: 'A imagem não pode exceder 2MB.', type: 'error' });
      return;
    }
    const reader = new FileReader();
    reader.onload = () => setPhotoPreview(reader.result);
    reader.readAsDataURL(file);
    setPhotoFeedback({ message: 'Imagem selecionada. Guarda as alterações para aplicar.', type: 'success' });
  };
 
  const handleProfileSave = async (e) => {
    e.preventDefault();
    setProfileLoading(true);
    setProfileFeedback({ message: '', type: '' });
    try {
      const res = await fetch(`${API_BASE}/users/${user.userId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          userName: profileData.userName,
          email: profileData.email,
          phone: null,
          profilePicture: photoPreview ?? null,
          facebookUrl: null,
          instagramUrl: null,
          xUrl: null,
        }),
      });
      if (!res.ok) throw new Error(await res.text());
      setProfileFeedback({ message: 'Perfil atualizado com sucesso.', type: 'success' });
    } catch (err) {
      setProfileFeedback({ message: err.message || 'Erro ao atualizar perfil.', type: 'error' });
    } finally {
      setProfileLoading(false);
    }
  };
 
  const handlePasswordSave = async (e) => {
    e.preventDefault();
    setPasswordFeedback({ message: '', type: '' });
    if (passwordData.newPassword !== passwordData.confirmPassword) {
      setPasswordFeedback({ message: 'As passwords não coincidem.', type: 'error' });
      return;
    }
    if (passwordData.newPassword.length < 8) {
      setPasswordFeedback({ message: 'A nova password deve ter pelo menos 8 caracteres.', type: 'error' });
      return;
    }
    setPasswordLoading(true);
    try {
      const res = await fetch(`${API_BASE}/users/${user.userId}/password`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          oldPassword: passwordData.oldPassword,
          newPassword: passwordData.newPassword,
        }),
      });
      if (!res.ok) throw new Error(await res.text());
      setPasswordFeedback({ message: 'Password alterada com sucesso.', type: 'success' });
      setPasswordData({ oldPassword: '', newPassword: '', confirmPassword: '' });
    } catch (err) {
      setPasswordFeedback({ message: err.message || 'Erro ao alterar password.', type: 'error' });
    } finally {
      setPasswordLoading(false);
    }
  };
 
  return (
    <div className="max-w-2xl space-y-6">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-slate-900">Definições</h1>
        <p className="text-slate-500 text-sm mt-1">Gira os seus dados pessoais e credenciais de acesso.</p>
      </div>
 
      {/* ── Foto de perfil ── */}
      <Section title="Foto de perfil" description="Imagem visível no seu perfil. Máx. 2MB.">
        <div className="flex items-center gap-6">
          <div className="w-20 h-20 rounded-2xl bg-slate-100 border border-slate-200 overflow-hidden flex items-center justify-center shrink-0">
            {photoPreview ? (
              <img src={photoPreview} alt="Preview" className="w-full h-full object-cover" />
            ) : (
              <span className="text-2xl font-black text-slate-400">
                {user?.name?.charAt(0)?.toUpperCase() ?? 'U'}
              </span>
            )}
          </div>
          <div>
            <label className="cursor-pointer bg-slate-100 hover:bg-slate-200 border border-slate-200 text-slate-700 text-xs font-medium px-4 py-2 rounded-lg transition-colors">
              Escolher imagem
              <input type="file" accept="image/*" className="hidden" onChange={handlePhotoChange} />
            </label>
            <Feedback {...photoFeedback} />
          </div>
        </div>
      </Section>
 
      {/* ── Dados pessoais ── */}
      <Section title="Dados pessoais" description="Atualize o seu email e nome de utilizador.">
        <form onSubmit={handleProfileSave} className="space-y-4">
          <Field
            label="Email"
            type="email"
            value={profileData.email}
            onChange={(e) => setProfileData({ ...profileData, email: e.target.value })}
            placeholder="o-seu@email.pt"
          />
          <Field
            label="Nome de utilizador"
            value={profileData.userName}
            onChange={(e) => setProfileData({ ...profileData, userName: e.target.value })}
            placeholder="username"
          />
          <Feedback {...profileFeedback} />
          <button
            type="submit"
            disabled={profileLoading}
            className="bg-orange-500 hover:bg-orange-600 disabled:opacity-50 text-white text-sm font-medium px-5 py-2.5 rounded-xl transition-all"
          >
            {profileLoading ? 'A guardar...' : 'Guardar alterações'}
          </button>
        </form>
      </Section>
 
      {/* ── Alterar password ── */}
      <Section title="Alterar password" description="Escolha uma password segura com pelo menos 8 caracteres.">
        <form onSubmit={handlePasswordSave} className="space-y-4">
          <Field
            label="Password atual"
            type="password"
            value={passwordData.oldPassword}
            onChange={(e) => setPasswordData({ ...passwordData, oldPassword: e.target.value })}
            placeholder="••••••••"
          />
          <Field
            label="Nova password"
            type="password"
            value={passwordData.newPassword}
            onChange={(e) => setPasswordData({ ...passwordData, newPassword: e.target.value })}
            placeholder="••••••••"
            hint="Mínimo 8 caracteres."
          />
          <Field
            label="Confirmar nova password"
            type="password"
            value={passwordData.confirmPassword}
            onChange={(e) => setPasswordData({ ...passwordData, confirmPassword: e.target.value })}
            placeholder="••••••••"
          />
          <Feedback {...passwordFeedback} />
          <button
            type="submit"
            disabled={passwordLoading}
            className="bg-orange-500 hover:bg-orange-600 disabled:opacity-50 text-white text-sm font-medium px-5 py-2.5 rounded-xl transition-all"
          >
            {passwordLoading ? 'A alterar...' : 'Alterar password'}
          </button>
        </form>
      </Section>
    </div>
  );
}