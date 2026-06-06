import type { PermissionName, UserRole } from '../../types';

export interface SidebarGroup {
  id: string;
  label: string;
  icon: string;
  items: { title: string; path: string; roles?: UserRole[]; permissions?: PermissionName[] }[];
}

export const sidebarGroups: SidebarGroup[] = [
  {
    id: 'dashboard',
    label: 'Dashboard',
    icon: 'Zap',
    items: [
      { title: 'Operations Center', path: '/admin/dashboard', roles: ['SuperAdmin', 'Admin', 'Receptionist', 'Housekeeper', 'Technician', 'KitchenStaff', 'Accountant'], permissions: ['view_dashboard'] },
    ],
  },
  {
    id: 'hotel',
    label: 'Hotel Management',
    icon: 'Building',
    items: [
      { title: 'Rooms', path: '/admin/rooms', roles: ['SuperAdmin', 'Admin', 'Receptionist'], permissions: ['manage_rooms'] },
      { title: 'Bookings', path: '/admin/bookings', roles: ['SuperAdmin', 'Admin', 'Receptionist'], permissions: ['create_booking'] },
      { title: 'Housekeeping', path: '/admin/housekeeping', roles: ['SuperAdmin', 'Admin', 'Housekeeper'], permissions: ['manage_rooms'] },
      { title: 'Maintenance', path: '/admin/maintenance', roles: ['SuperAdmin', 'Admin', 'Technician'], permissions: ['view_maintenances'] },
    ],
  },
  {
    id: 'room-service',
    label: 'Room Service',
    icon: 'UtensilsCrossed',
    items: [
      { title: 'Orders', path: '/admin/orders', roles: ['SuperAdmin', 'Admin', 'KitchenStaff'], permissions: ['create_orders'] },
    ],
  },
  {
    id: 'finance',
    label: 'Finance',
    icon: 'CreditCard',
    items: [
      { title: 'Payments', path: '/admin/payments', roles: ['SuperAdmin', 'Admin', 'Accountant'], permissions: ['manage_payments'] },
    ],
  },
  {
    id: 'access',
    label: 'Access Control',
    icon: 'Shield',
    items: [
      { title: 'Users', path: '/admin/users', roles: ['SuperAdmin', 'Admin'], permissions: ['manage_users'] },
      { title: 'Roles', path: '/admin/roles', roles: ['SuperAdmin', 'Admin'], permissions: ['manage_roles'] },
      { title: 'Permissions', path: '/admin/permissions', roles: ['SuperAdmin', 'Admin'], permissions: ['manage_permissions'] },
      { title: 'Audit Logs', path: '/admin/audit-logs', roles: ['SuperAdmin', 'Admin'], permissions: ['view_audit_logs'] },
    ],
  },
  {
    id: 'settings',
    label: 'Settings',
    icon: 'Settings',
    items: [
      { title: 'Settings', path: '/admin/settings', roles: ['SuperAdmin', 'Admin'], permissions: ['view_settings'] },
    ],
  },
];
