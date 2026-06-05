import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';
import type { PermissionName } from '../../types';

const adminRoles = ['SuperAdmin', 'Admin', 'Receptionist', 'Housekeeper', 'Technician', 'KitchenStaff'];

export default function ProtectedRoute({ requiredPermissions = [], allowedRoles = adminRoles }: { requiredPermissions?: PermissionName[]; allowedRoles?: string[] }) {
  const location = useLocation();
  const accessToken = useAuthStore((s) => s.accessToken);
  const user = useAuthStore((s) => s.user);
  const hasPermission = useAuthStore((s) => s.hasPermission);

  if (!accessToken) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  }

  if (allowedRoles.length > 0 && (!user || !allowedRoles.includes(user.role))) {
    return <Navigate to="/login" replace />;
  }

  if (requiredPermissions.length > 0 && requiredPermissions.some((p) => !hasPermission(p))) {
    return <Navigate to="/admin/access-denied" replace />;
  }

  return <Outlet />;
}
