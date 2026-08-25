import { useState, useEffect, useCallback, useRef } from 'react';
import { useAuth } from '../../context/useAuth';
import { API_BASE_URL as API_BASE } from '../../config/api';
import { ROLES } from '../../config/roles';

function Section({ title, description, children }) {
  return (
    <div className="bg-slate-900 rounded-2xl border border-white/10 overflow-hidden">
      <div className="px-6 py-4 border-b border-white/5">
        <h2 className="font-semibold text-white text-sm">{title}</h2>
        {description && <p className="text-slate-500 text-xs mt-0.5">{description}</p>}
      </div>
      <div className="p-6">{children}</div>
    </div>
  );
}

function Field({ label, type = 'text', value, onChange, placeholder, hint }) {
  return (
    <div>
      <label className="block text-xs font-medium text-slate-400 mb-1.5">{label}</label>
      <input
        type={type}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-2.5 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 transition-colors"
      />
      {hint && <p className="text-slate-600 text-xs mt-1">{hint}</p>}
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

function RoleBadge({ roleId }) {
  const config = {
    1: { label: 'Admin', color: 'bg-orange-500/10 text-orange-400 border-orange-500/20' },
    2: { label: 'Consultor', color: 'bg-blue-500/10 text-blue-400 border-blue-500/20' },
    3: { label: 'Cliente', color: 'bg-white/5 text-slate-400 border-white/10' },
  };
  const c = config[roleId] ?? config[3];
  return (
    <span className={`text-[10px] font-semibold px-2 py-0.5 rounded-full border ${c.color}`}>
      {c.label}
    </span>
  );
}

export default function AdminSettings() {
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

  // Foto
  const [photoPreview, setPhotoPreview] = useState(null);
  const [photoFeedback, setPhotoFeedback] = useState({ message: '', type: '' });

  // Utilizadores pendentes
  const [pendingUsers, setPendingUsers] = useState([]);
  const [usersLoading, setUsersLoading] = useState(true);

  // Novo consultor
  const [newConsultant, setNewConsultant] = useState({ name: '', email: '', password: '' });
  const [newConsultantFeedback, setNewConsultantFeedback] = useState({ message: '', type: '' });
  const [newConsultantLoading, setNewConsultantLoading] = useState(false);

  // Usar ref para o token para evitar re-renders desnecessários
   const tokenRef = useRef(token);
    useEffect(() => {
        tokenRef.current = token;
    }, [token]);

  const fetchUsers = useCallback(async () => {
    setUsersLoading(true);
    try {
      const res = await fetch(`${API_BASE}/users/pending`, {
        headers: { Authorization: `Bearer ${tokenRef.current}` },
      });
      if (res.ok) {
        const pending = await res.json();
        setPendingUsers(pending.filter(u => u.usersRoleId !== ROLES.ADMIN));
      }
    } catch {
      // silencioso
    } finally {
      setUsersLoading(false);
    }
  }, []); // sem dependências — usa tokenRef internamente

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
    setPhotoFeedback({ message: 'Imagem selecionada.', type: 'success' });
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
      setPasswordFeedback({ message: 'Mínimo 8 caracteres.', type: 'error' });
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

  const handleApproveUser = async (userId) => {
    try {
      await fetch(`${API_BASE}/users/${userId}/approve`, {
        method: 'PATCH',
        headers: { Authorization: `Bearer ${token}` },
      });
      fetchUsers();
    } catch {
      // silencioso
    }
  };

  const handleDeactivateUser = async (userId) => {
    try {
      await fetch(`${API_BASE}/users/${userId}/deactivate`, {
        method: 'PATCH',
        headers: { Authorization: `Bearer ${token}` },
      });
      fetchUsers();
    } catch {
      // silencioso
    }
  };

  const handleAddConsultant = async (e) => {
    e.preventDefault();
    setNewConsultantFeedback({ message: '', type: '' });
    setNewConsultantLoading(true);
    try {
      const res = await fetch(`${API_BASE}/users/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          fullName: newConsultant.name,
          userName: newConsultant.email.split('@')[0],
          email: newConsultant.email,
          password: newConsultant.password,
        }),
      });
      if (!res.ok) throw new Error(await res.text());
      setNewConsultantFeedback({ message: 'Consultor adicionado com sucesso.', type: 'success' });
      setNewConsultant({ name: '', email: '', password: '' });
      fetchUsers();
    } catch (err) {
      setNewConsultantFeedback({ message: err.message || 'Erro ao adicionar consultor.', type: 'error' });
    } finally {
      setNewConsultantLoading(false);
    }
  };

  return (
    <div className="max-w-2xl space-y-6">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-white">Definições</h1>
        <p className="text-slate-500 text-sm mt-1">Gira o seu perfil, credenciais e utilizadores da plataforma.</p>
      </div>

      {/* Foto de perfil */}
      <Section title="Foto de perfil" description="Imagem visível no painel. Máx. 2MB.">
        <div className="flex items-center gap-6">
          <div className="w-20 h-20 rounded-2xl bg-slate-800 border border-white/10 overflow-hidden flex items-center justify-center shrink-0">
            {photoPreview ? (
              <img src={photoPreview} alt="Preview" className="w-full h-full object-cover" />
            ) : (
              <span className="text-2xl font-black text-slate-500">
                {user?.name?.charAt(0)?.toUpperCase() ?? 'A'}
              </span>
            )}
          </div>
          <div>
            <label className="cursor-pointer bg-white/5 hover:bg-white/10 border border-white/10 text-slate-300 text-xs font-medium px-4 py-2 rounded-lg transition-colors">
              Escolher imagem
              <input type="file" accept="image/*" className="hidden" onChange={handlePhotoChange} />
            </label>
            <Feedback {...photoFeedback} />
          </div>
        </div>
      </Section>

      {/* Dados pessoais */}
      <Section title="Dados pessoais" description="Atualize o seu email e nome de utilizador.">
        <form onSubmit={handleProfileSave} className="space-y-4">
          <Field
            label="Email"
            type="email"
            value={profileData.email}
            onChange={(e) => setProfileData({ ...profileData, email: e.target.value })}
            placeholder="admin@consultant.pt"
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

      {/* Alterar password */}
      <Section title="Alterar password" description="Mínimo 8 caracteres.">
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

      {/* Gestão de consultores */}
      <Section title="Gestão de consultores" description="Adicione consultores e gira o acesso à plataforma.">
        <form onSubmit={handleAddConsultant} className="space-y-3 mb-6 pb-6 border-b border-white/5">
          <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Adicionar consultor</p>
          <Field
            label="Nome completo"
            value={newConsultant.name}
            onChange={(e) => setNewConsultant({ ...newConsultant, name: e.target.value })}
            placeholder="Nome do consultor"
          />
          <Field
            label="Email"
            type="email"
            value={newConsultant.email}
            onChange={(e) => setNewConsultant({ ...newConsultant, email: e.target.value })}
            placeholder="consultor@email.pt"
          />
          <Field
            label="Password temporária"
            type="password"
            value={newConsultant.password}
            onChange={(e) => setNewConsultant({ ...newConsultant, password: e.target.value })}
            placeholder="••••••••"
            hint="O consultor deve alterar no primeiro acesso."
          />
          <Feedback {...newConsultantFeedback} />
          <button
            type="submit"
            disabled={newConsultantLoading}
            className="bg-white/10 hover:bg-white/20 disabled:opacity-50 text-white text-sm font-medium px-5 py-2.5 rounded-xl transition-all"
          >
            {newConsultantLoading ? 'A adicionar...' : '+ Adicionar consultor'}
          </button>
        </form>

        <div>
          <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-3">
            Utilizadores pendentes de aprovação
          </p>
          {usersLoading ? (
            <div className="space-y-2">
              {[1, 2].map(i => (
                <div key={i} className="h-12 bg-white/5 rounded-xl animate-pulse" />
              ))}
            </div>
          ) : pendingUsers.length === 0 ? (
            <p className="text-slate-600 text-sm">Nenhum utilizador pendente.</p>
          ) : (
            <div className="space-y-2">
              {pendingUsers.map((u) => (
                <div
                  key={u.userId}
                  className="flex items-center justify-between bg-white/5 border border-white/5 rounded-xl px-4 py-3"
                >
                  <div>
                    <div className="flex items-center gap-2">
                      <span className="text-white text-sm font-medium">{u.fullName ?? u.name}</span>
                      <RoleBadge roleId={u.usersRoleId} />
                    </div>
                    <p className="text-slate-500 text-xs">{u.email}</p>
                  </div>
                  <div className="flex items-center gap-2">
                    <button
                      onClick={() => handleApproveUser(u.userId)}
                      className="text-xs bg-emerald-500/10 hover:bg-emerald-500/20 text-emerald-400 border border-emerald-500/20 px-3 py-1.5 rounded-lg transition-all"
                    >
                      Aprovar
                    </button>
                    <button
                      onClick={() => handleDeactivateUser(u.userId)}
                      className="text-xs bg-red-500/10 hover:bg-red-500/20 text-red-400 border border-red-500/20 px-3 py-1.5 rounded-lg transition-all"
                    >
                      Rejeitar
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </Section>
    </div>
  );
}














