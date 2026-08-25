import { Link } from 'react-router-dom';

const QUICK_LINKS = [
  { to: '/imoveis', label: 'Imóveis' },
  { to: '/sobre', label: 'Sobre Mim' },
  { to: '/observatorio', label: 'Mercado' },
];

const TOOLS_LINKS = [
  { to: '/discquiz', label: 'Perfil DISC' },
  { to: '/avaliacao', label: 'Estudo de Mercado' },
  { to: '/login', label: 'Área Pessoal' },
];

const SOCIAL_LINKS = [
  {
    label: 'Instagram',
    href: 'https://instagram.com',
    icon: 'M12 2c2.717 0 3.056.01 4.122.06 1.065.05 1.79.217 2.428.465.66.254 1.216.598 1.772 1.153a4.902 4.902 0 011.153 1.772c.247.637.415 1.363.465 2.428.047 1.066.06 1.405.06 4.122 0 2.717-.01 3.056-.06 4.122-.05 1.065-.218 1.79-.465 2.428a4.883 4.883 0 01-1.153 1.772 4.915 4.915 0 01-1.772 1.153c-.637.247-1.363.415-2.428.465-1.066.047-1.405.06-4.122.06-2.717 0-3.056-.01-4.122-.06-1.065-.05-1.79-.218-2.428-.465a4.89 4.89 0 01-1.772-1.153 4.904 4.904 0 01-1.153-1.772c-.248-.637-.415-1.363-.465-2.428C2.013 15.056 2 14.717 2 12c0-2.717.01-3.056.06-4.122.05-1.065.217-1.79.465-2.428a4.902 4.902 0 011.153-1.772A4.888 4.888 0 015.45 2.525c.638-.248 1.362-.415 2.428-.465C8.944 2.013 9.283 2 12 2z',
  },
  {
    label: 'Facebook',
    href: 'https://facebook.com',
    icon: 'M22 12c0-5.523-4.477-10-10-10S2 6.477 2 12c0 4.991 3.657 9.128 8.438 9.879v-6.99h-2.54V12h2.54V9.797c0-2.506 1.492-3.89 3.777-3.89 1.094 0 2.238.195 2.238.195v2.46h-1.26c-1.243 0-1.63.771-1.63 1.562V12h2.773l-.443 2.89h-2.33v6.99C18.343 21.128 22 16.991 22 12z',
  },
  {
    label: 'LinkedIn',
    href: 'https://linkedin.com',
    icon: 'M19 3a2 2 0 012 2v14a2 2 0 01-2 2H5a2 2 0 01-2-2V5a2 2 0 012-2h14zM8.34 18.34V9.68H5.67v8.66h2.67zM7.01 8.55a1.55 1.55 0 100-3.1 1.55 1.55 0 000 3.1zM18.34 18.34v-4.83c0-2.59-1.38-3.79-3.22-3.79-1.48 0-2.15.82-2.52 1.39v-1.2H9.93c.03.7 0 8.43 0 8.43h2.67v-4.71c0-.25.02-.5.1-.68.2-.5.67-1.03 1.45-1.03 1.03 0 1.44.78 1.44 1.93v4.49h2.75z',
  },
];

export default function Footer() {
  const year = new Date().getFullYear();

  return (
    <footer className="bg-slate-950 border-t border-white/5 text-slate-400 pt-16 pb-8 px-6 md:px-12">
      <div className="max-w-6xl mx-auto">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-10 mb-12">
          {/* Marca */}
          <div className="md:col-span-2 space-y-4">
            <Link to="/" className="flex items-center gap-2 group w-fit">
              <div className="w-9 h-9 bg-linear-to-tr from-orange-500 to-orange-600 rounded-xl flex items-center justify-center shadow-lg shadow-orange-500/20 group-hover:rotate-6 transition-transform">
                <span className="text-white font-black text-base">F</span>
              </div>
              <div className="flex flex-col">
                <span className="text-white font-bold tracking-tight text-sm leading-none">FREDERICO</span>
                <span className="text-orange-500 text-[9px] tracking-[0.2em] font-semibold uppercase mt-1">Consultor</span>
              </div>
            </Link>
            <p className="text-sm leading-relaxed max-w-sm">
              Consultoria imobiliária moderna, apoiada em dados e tecnologia, para tornar cada compra ou venda de casa mais simples e transparente.
            </p>
            <div className="flex items-center gap-2 pt-2">
              {SOCIAL_LINKS.map((social) => (
                <a
                  key={social.label}
                  href={social.href}
                  target="_blank"
                  rel="noopener noreferrer"
                  title={social.label}
                  className="w-9 h-9 flex items-center justify-center bg-white/5 hover:bg-orange-500/10 border border-white/10 hover:border-orange-500/30 rounded-lg text-slate-400 hover:text-orange-400 transition-all"
                >
                  <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
                    <path d={social.icon} />
                  </svg>
                </a>
              ))}
            </div>
          </div>

          {/* Links rápidos */}
          <div>
            <h3 className="text-white text-xs font-bold uppercase tracking-wider mb-4">Navegação</h3>
            <ul className="space-y-2.5">
              {QUICK_LINKS.map((link) => (
                <li key={link.to}>
                  <Link to={link.to} className="text-sm hover:text-orange-400 transition-colors">
                    {link.label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Ferramentas + contacto */}
          <div>
            <h3 className="text-white text-xs font-bold uppercase tracking-wider mb-4">Ferramentas</h3>
            <ul className="space-y-2.5 mb-6">
              {TOOLS_LINKS.map((link) => (
                <li key={link.to}>
                  <Link to={link.to} className="text-sm hover:text-orange-400 transition-colors">
                    {link.label}
                  </Link>
                </li>
              ))}
            </ul>
            <a href="mailto:admin@consultant.pt" className="text-sm hover:text-orange-400 transition-colors block">
              admin@consultant.pt
            </a>
          </div>
        </div>

        <div className="border-t border-white/5 pt-6 flex flex-col sm:flex-row items-center justify-between gap-3">
          <p className="text-xs text-slate-600">© {year} Frederico Consultor. Todos os direitos reservados.</p>
          <p className="text-xs text-slate-700">Feito com dedicação em Portugal 🇵🇹</p>
        </div>
      </div>
    </footer>
  );
}