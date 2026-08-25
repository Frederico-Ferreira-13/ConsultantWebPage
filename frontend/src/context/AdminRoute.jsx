import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from './useAuth';
import { ROLES } from '../config/roles';

export function AdminRoute() {
    const { user } = useAuth();
    if(!user) return <Navigate to="/login" replace />;
    if(user.usersRoleId !== ROLES.ADMIN) return <Navigate to="/" replace />;
    return <Outlet />;
}