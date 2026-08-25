import { BrowserRouter, Routes, Route, Outlet, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext.jsx';
import { ProtectedRoute } from './context/ProtectedRoute.jsx';
import { AdminRoute } from './context/AdminRoute.jsx';
 
import Navbar from './components/Navbar.jsx';
 
import Home from './pages/Home.jsx';
import AboutMe from './pages/AboutMe.jsx';
import Properties from './pages/Properties.jsx';
import PropertyDetail from './pages/PropertyDetail.jsx';
import Posts from './pages/Posts.jsx';
import PostDetail from './pages/PostDetail.jsx';
import Login from './pages/Login.jsx';
 
import DashboardLayout from './pages/Dashboard/DashboardLayout.jsx';
import Favorites from './pages/Dashboard/Favorites.jsx';
import Settings from './pages/Dashboard/Settings.jsx';
import MarketStudy from './pages/Dashboard/MarketStudy.jsx';
import MarketDashboard from './pages/Dashboard/MarketDashboard.jsx';
import DiscQuiz from './pages/Dashboard/Discquiz.jsx';
 
import AdminLayout from './pages/Admin/AdminLayout.jsx';
import Overview from './pages/Admin/Overview.jsx';
import AdminProperties from './pages/Admin/Content/AdminProperties.jsx';


// Placeholders — criar à medida que construirmos cada secção
import AdminPosts from './pages/Admin/Content/AdminPosts.jsx';
import AdminAboutMe from './pages/Admin/Content/AdminAboutMe.jsx';
import AdminTestimonials from "./pages/Admin/Content/AdminTestimonials.jsx";
// import AdminLeads from './pages/Admin/CRM/Leads.jsx';
// import AdminEvaluations from './pages/Admin/CRM/Evaluations.jsx';
import AdminDisc from './pages/Admin/Content/AdminDisc.jsx';
// import AdminClients from './pages/Admin/Users/Clients.jsx';

function LayoutComNavbar() {
  return (
    <>
      <Navbar />
      <div className="pt-24">
        <Outlet />
      </div>
    </>
  );
}
 
function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
 
          {/* ── ROTAS PÚBLICAS ── */}
          <Route element={<LayoutComNavbar />}>
            <Route path="/" element={<Home />} />
            <Route path="/sobre" element={<AboutMe />} />
            <Route path="/imoveis" element={<Properties />} />
            <Route path="/imoveis/:id" element={<PropertyDetail />} />
            <Route path="/mercado" element={<Posts />} />
            <Route path="mercado/:slug" element={<PostDetail />} />
          </Route>
 
          <Route path="/login" element={<Login />} />          
 
          {/* ── DASHBOARD DO CLIENTE ── */}
          <Route element={<LayoutComNavbar />}>
            <Route element={<ProtectedRoute />}>
              <Route path="/avaliacao" element={<MarketStudy />} />
              <Route path="/observatorio" element={<MarketDashboard />} />
              <Route path="/discquiz" element={<DiscQuiz />} />
              
              <Route path="/dashboard" element={<DashboardLayout />}>
                <Route path="favoritos" element={<Favorites />} />
                <Route path="definicoes" element={<Settings />} />
              </Route>
            </Route>
          </Route>
 
          {/* ── ADMIN ── */}
          <Route element={<AdminRoute />}>
            <Route path="/admin" element={<AdminLayout />}>
              <Route index element={<Overview />} />
              <Route path="gestaoimoveis" element={<AdminProperties />} /> 
              <Route path="testemunhos" element={<AdminTestimonials />} />
              <Route path="posts" element={<AdminPosts />} />
              <Route path="sobre-mim" element={<AdminAboutMe />} />
              <Route path="disc" element={<AdminDisc />} />
              
              {/* ← DESCOMENTADO */}
              {/* Descomenta à medida que criamos cada página:
              
             
              
              <Route path="leads" element={<AdminLeads />} />
              <Route path="avaliacoes" element={<AdminEvaluations />} />
              
              <Route path="clientes" element={<AdminClients />} /> */}
            </Route>
          </Route>
 
          {/* 404 */}
          <Route path="*" element={<Navigate to="/" replace />} />
 
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
 
export default App;

