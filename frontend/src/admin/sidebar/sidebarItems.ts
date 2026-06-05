export interface SidebarGroup {
  id: string;
  label: string;
  icon: string;
  items: { title: string; path: string; roles?: string[] }[];
}

export const sidebarGroups: SidebarGroup[] = [
  {
    id: 'dashboard',
    label: 'Dashboard',
    icon: 'Zap',
    items: [
      { title: 'Operations Center', path: '/admin/dashboard', roles: ['SuperAdmin', 'Admin', 'Receptionist', 'Housekeeper', 'Technician', 'KitchenStaff', 'Accountant'] },
    ],
  },
  {
    id: 'hotel',
    label: 'Hotel Management',
    icon: 'Building',
    items: [
      { title: 'Rooms', path: '/admin/rooms', roles: ['SuperAdmin', 'Admin', 'Receptionist'] },
      { title: 'Bookings', path: '/admin/bookings', roles: ['SuperAdmin', 'Admin', 'Receptionist'] },
      { title: 'Housekeeping', path: '/admin/housekeeping', roles: ['SuperAdmin', 'Admin', 'Housekeeper'] },
      { title: 'Maintenance', path: '/admin/maintenance', roles: ['SuperAdmin', 'Admin', 'Technician'] },
    ],
  },
  {
    id: 'room-service',
    label: 'Room Service',
    icon: 'UtensilsCrossed',
    items: [
      { title: 'Orders', path: '/admin/orders', roles: ['SuperAdmin', 'Admin', 'KitchenStaff'] },
    ],
  },
  {
    id: 'finance',
    label: 'Finance',
    icon: 'CreditCard',
    items: [
      { title: 'Payments', path: '/admin/payments', roles: ['SuperAdmin', 'Admin', 'Accountant'] },
    ],
  },
  {
    id: 'access',
    label: 'Access Control',
    icon: 'Shield',
    items: [
      { title: 'Users', path: '/admin/users', roles: ['SuperAdmin', 'Admin'] },
      { title: 'Roles', path: '/admin/roles', roles: ['SuperAdmin', 'Admin'] },
      { title: 'Permissions', path: '/admin/permissions', roles: ['SuperAdmin', 'Admin'] },
      { title: 'Audit Logs', path: '/admin/audit-logs', roles: ['SuperAdmin', 'Admin'] },
    ],
  },
  {
    id: 'settings',
    label: 'Settings',
    icon: 'Settings',
    items: [
      { title: 'Settings', path: '/admin/settings', roles: ['SuperAdmin', 'Admin'] },
    ],
  },
];
