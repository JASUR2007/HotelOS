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
      { title: 'Operations Center', path: '/admin/dashboard' },
    ],
  },
  {
    id: 'hotel',
    label: 'Hotel Management',
    icon: 'Building',
    items: [
      { title: 'Rooms', path: '/admin/rooms' },
      { title: 'Bookings', path: '/admin/bookings' },
      { title: 'Housekeeping', path: '/admin/housekeeping' },
      { title: 'Maintenance', path: '/admin/maintenance' },
    ],
  },
  {
    id: 'room-service',
    label: 'Room Service',
    icon: 'UtensilsCrossed',
    items: [
      { title: 'Orders', path: '/admin/orders' },
    ],
  },
  {
    id: 'finance',
    label: 'Finance',
    icon: 'CreditCard',
    items: [
      { title: 'Payments', path: '/admin/payments' },
    ],
  },
  {
    id: 'access',
    label: 'Access Control',
    icon: 'Shield',
    items: [
      { title: 'Users', path: '/admin/users' },
      { title: 'Roles', path: '/admin/roles' },
      { title: 'Permissions', path: '/admin/permissions' },
      { title: 'Audit Logs', path: '/admin/audit-logs' },
    ],
  },
  // Monitoring/infrastructure pages are intentionally omitted from the main sidebar.
  {
    id: 'settings',
    label: 'Settings',
    icon: 'Settings',
    items: [
      { title: 'Settings', path: '/admin/settings' },
    ],
  },
];
