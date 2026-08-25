import { useState, useEffect, useCallback, startTransition } from 'react';
import ReactMarkdown from 'react-markdown';
import { useAuth } from '../../../context/useAuth';
import { API_BASE_URL as API_BASE } from '../../../config/api';

// Categoria fixa a 1 — Posts.CategoryId reutiliza a mesma tabela Category
// das tipologias de imóveis (schema pré-existente, não redesenhado aqui).
// Ver nota no chat: idealmente seria uma tabela PostCategory dedicada.
const DEFAULT_CATEGORY_ID = 1;

const PREVIEW_COMPONENTS = {
  h1: (props) => <h1 className="text-xl font-bold text-white mt-6 mb-3" {...props} />,
  h2: (props) => <h2 className="text-lg font-bold text-white mt-5 mb-2" {...props} />,
  h3: (props) => <h3 className="text-base font-bold text-white mt-4 mb-2" {...props} />,
  p: (props) => <p className="text-slate-300 text-sm leading-relaxed mb-3" {...props} />,
  a: (props) => <a className="text-orange-400 hover:text-orange-300 underline" {...props} />,
  ul: (props) => <ul className="list-disc list-inside text-slate-300 text-sm mb-3 space-y-1" {...props} />,
  ol: (props) => <ol className="list-decimal list-inside text-slate-300 text-sm mb-3 space-y-1" {...props} />,
  blockquote: (props) => <blockquote className="border-l-4 border-orange-500 pl-3 italic text-slate-400 text-sm my-3" {...props} />,
  code: (props) => <code className="bg-slate-950 px-1.5 py-0.5 rounded text-orange-300 text-xs" {...props} />,
  strong: (props) => <strong className="text-white font-semibold" {...props} />,
};

function slugify(text) {
  return text
    .toLowerCase()
    .trim()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '') // remove acentos
    .replace(/[^a-z0-9\s-]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-');
}

function Field({ label, value, onChange, placeholder, required, hint }) {
  return (
    <div>
      <label className="block text-xs font-medium text-slate-400 mb-1.5">
        {label} {required && <span className="text-red-400">*</span>}
      </label>
      <input
        type="text"
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

function Feedback({ message, type }) {
  if (!message) return null;
  return (
    <p className={`text-xs mt-2 ${type === 'error' ? 'text-red-400' : 'text-emerald-400'}`}>
      {type === 'success' ? '✓ ' : '✕ '}{message}
    </p>
  );
}

const EMPTY_FORM = {
  title: '',
  slug: '',
  summary: '',
  content: '',
  imageUrl: '',
  isPublished: false,
};

export default function AdminPosts() {
  const { user, token } = useAuth();

  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [slugTouched, setSlugTouched] = useState(false);
  const [formData, setFormData] = useState(EMPTY_FORM);
  const [showPreview, setShowPreview] = useState(false);
  const [feedback, setFeedback] = useState({ message: '', type: '' });
  const [submitting, setSubmitting] = useState(false);

  const fetchPosts = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/posts`);
      if (!res.ok) throw new Error();
      const data = await res.json();
      setPosts(Array.isArray(data) ? data : []);
    } catch {
      setError('Não foi possível carregar os posts.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    startTransition(() => {
      fetchPosts();
    });
  }, [fetchPosts]);

  const resetForm = () => {
    setFormData(EMPTY_FORM);
    setEditingId(null);
    setSlugTouched(false);
    setShowPreview(false);
    setFeedback({ message: '', type: '' });
  };

  const handleTitleChange = (title) => {
    setFormData((prev) => ({
      ...prev,
      title,
      slug: slugTouched ? prev.slug : slugify(title),
    }));
  };

  const handleEdit = (post) => {
    setEditingId(post.postId);
    setFormData({
      title: post.title,
      slug: post.slug,
      summary: post.summary ?? '',
      content: post.content ?? '',
      imageUrl: post.imageUrl ?? '',
      isPublished: !!post.isPublished,
    });
    setSlugTouched(true);
    setShowForm(true);
    setShowPreview(false);
    setFeedback({ message: '', type: '' });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setFeedback({ message: '', type: '' });

    try {
      const isEditing = editingId != null;
      const url = isEditing ? `${API_BASE}/posts/${editingId}` : `${API_BASE}/posts`;
      const method = isEditing ? 'PUT' : 'POST';

      const body = isEditing
        ? {
            authorId: user?.userId,
            categoryId: DEFAULT_CATEGORY_ID,
            title: formData.title,
            slug: formData.slug,
            content: formData.content,
            summary: formData.summary,
            imageUrl: formData.imageUrl || null,
            isPublished: formData.isPublished,
          }
        : {
            authorId: user?.userId,
            categoryId: DEFAULT_CATEGORY_ID,
            title: formData.title,
            slug: formData.slug,
            content: formData.content,
            summary: formData.summary,
            imageUrl: formData.imageUrl || null,
            status: formData.isPublished ? 'Published' : 'Draft',
          };

      const res = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(body),
      });

      if (!res.ok) throw new Error(await res.text());

      setFeedback({ message: isEditing ? 'Post atualizado com sucesso.' : 'Post criado com sucesso.', type: 'success' });
      resetForm();
      setShowForm(false);
      fetchPosts();
    } catch (err) {
      setFeedback({ message: err.message || 'Erro ao guardar o post.', type: 'error' });
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (postId) => {
    if (!window.confirm('Eliminar este post permanentemente?')) return;

    try {
      const res = await fetch(`${API_BASE}/posts/${postId}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) throw new Error();
      setPosts((prev) => prev.filter((p) => p.postId !== postId));
    } catch {
      setError('Não foi possível eliminar este post.');
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-white">Posts</h1>
          <p className="text-slate-500 text-sm mt-1">Gere os artigos do blog (Mercado em Foco).</p>
        </div>
        <button
          onClick={() => {
            if (showForm) {
              resetForm();
              setShowForm(false);
            } else {
              resetForm();
              setShowForm(true);
            }
          }}
          className="bg-orange-500 hover:bg-orange-600 text-white text-sm font-medium px-4 py-2.5 rounded-xl transition-all"
        >
          {showForm ? 'Cancelar' : '+ Novo Post'}
        </button>
      </div>

      {showForm && (
        <div className="bg-slate-900 border border-white/10 rounded-2xl p-6">
          <h2 className="font-semibold text-white text-sm mb-4">{editingId ? 'Editar Post' : 'Novo Post'}</h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <Field
              label="Título"
              value={formData.title}
              onChange={(e) => handleTitleChange(e.target.value)}
              placeholder="3 pormenores que valorizam a sua casa"
              required
            />
            <Field
              label="Slug"
              value={formData.slug}
              onChange={(e) => {
                setSlugTouched(true);
                setFormData({ ...formData, slug: e.target.value });
              }}
              placeholder="3-pormenores-que-valorizam-a-sua-casa"
              required
              hint="Gerado automaticamente a partir do título — podes editar."
            />
            <div>
              <label className="block text-xs font-medium text-slate-400 mb-1.5">Resumo</label>
              <textarea
                value={formData.summary}
                onChange={(e) => setFormData({ ...formData, summary: e.target.value })}
                placeholder="Resumo curto para a listagem (aparece nos cartões)"
                rows={2}
                maxLength={500}
                className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-2.5 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 resize-none"
              />
            </div>
            <Field
              label="URL da imagem de capa"
              value={formData.imageUrl}
              onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
              placeholder="https://..."
            />

            <div>
              <div className="flex items-center justify-between mb-1.5">
                <label className="block text-xs font-medium text-slate-400">
                  Conteúdo (Markdown) <span className="text-red-400">*</span>
                </label>
                <button
                  type="button"
                  onClick={() => setShowPreview((v) => !v)}
                  className="text-xs text-orange-400 hover:text-orange-300 font-medium"
                >
                  {showPreview ? 'Ver editor' : 'Ver pré-visualização'}
                </button>
              </div>

              {showPreview ? (
                <div className="w-full min-h-62.5 bg-slate-950 border border-white/10 rounded-xl px-4 py-3 overflow-y-auto">
                  {formData.content ? (
                    <ReactMarkdown components={PREVIEW_COMPONENTS}>{formData.content}</ReactMarkdown>
                  ) : (
                    <p className="text-slate-600 text-sm">Sem conteúdo para pré-visualizar.</p>
                  )}
                </div>
              ) : (
                <textarea
                  required
                  value={formData.content}
                  onChange={(e) => setFormData({ ...formData, content: e.target.value })}
                  placeholder={'Escreve em Markdown — ex:\n\n## Subtítulo\n\nTexto normal, **negrito**, [link](https://...)\n\n- item 1\n- item 2'}
                  rows={14}
                  className="w-full bg-slate-950 border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-orange-500 resize-none font-mono"
                />
              )}
            </div>

            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="isPublished"
                checked={formData.isPublished}
                onChange={(e) => setFormData({ ...formData, isPublished: e.target.checked })}
                className="rounded border-white/10 bg-slate-950 text-orange-500 focus:ring-orange-500"
              />
              <label htmlFor="isPublished" className="text-sm text-slate-400">Publicar imediatamente</label>
            </div>

            <Feedback {...feedback} />
            <button
              type="submit"
              disabled={submitting}
              className="bg-orange-500 hover:bg-orange-600 disabled:opacity-50 text-white text-sm font-medium px-5 py-2.5 rounded-xl transition-all"
            >
              {submitting ? 'A guardar...' : editingId ? 'Guardar alterações' : 'Criar Post'}
            </button>
          </form>
        </div>
      )}

      {error && (
        <div className="bg-red-500/10 border border-red-500/30 text-red-400 rounded-xl p-4 text-sm">
          {error}
        </div>
      )}

      <div className="bg-slate-900 border border-white/10 rounded-2xl overflow-hidden">
        {loading ? (
          <div className="p-6 space-y-2">
            {[1, 2, 3].map((i) => (
              <div key={i} className="h-16 bg-white/5 rounded-xl animate-pulse" />
            ))}
          </div>
        ) : posts.length === 0 ? (
          <p className="text-slate-600 text-sm p-6">Nenhum post criado ainda.</p>
        ) : (
          <div className="divide-y divide-white/5">
            {posts.map((post) => (
              <div key={post.postId} className="px-6 py-4 flex items-center justify-between gap-4">
                <div className="min-w-0 flex-1">
                  <div className="flex items-center gap-2 mb-1">
                    <p className="text-white text-sm font-medium truncate">{post.title}</p>
                    <span
                      className={`text-[10px] font-semibold px-2 py-0.5 rounded-full border shrink-0 ${
                        post.isPublished
                          ? 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20'
                          : 'bg-white/5 text-slate-400 border-white/10'
                      }`}
                    >
                      {post.isPublished ? 'Publicado' : 'Rascunho'}
                    </span>
                  </div>
                  <p className="text-slate-500 text-xs truncate">/{post.slug} · {post.viewCount ?? 0} vistas</p>
                </div>
                <div className="flex items-center gap-2 shrink-0">
                  <button
                    onClick={() => handleEdit(post)}
                    className="text-xs bg-white/5 hover:bg-white/10 border border-white/10 text-slate-300 px-3 py-1.5 rounded-lg transition-all"
                  >
                    Editar
                  </button>
                  <button
                    onClick={() => handleDelete(post.postId)}
                    className="text-xs bg-red-500/10 hover:bg-red-500/20 text-red-400 border border-red-500/20 px-3 py-1.5 rounded-lg transition-all"
                  >
                    Eliminar
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}