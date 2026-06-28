import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from './useAuth'; 

export function AdminRoute() {
    const { user } = useAuth();
    if(!user) return <Navigate to="/login" replace />;
    if(user.usersRoleId !== 1) return <Navigate to="/" replace />;
    return <Outlet />;
}