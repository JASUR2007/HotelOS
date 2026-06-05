import { create } from 'zustand';

export interface NotificationItem {
  id: string;
  title: string;
  message: string;
  type: 'info' | 'warning' | 'critical' | 'success';
  time: string;
  read: boolean;
}

interface NotificationState {
  items: NotificationItem[];
  unreadCount: number;
  addNotification: (item: NotificationItem) => void;
  markRead: (id: string) => void;
  markAllRead: () => void;
  clearAll: () => void;
}

const demoNotifications: NotificationItem[] = [
  { id: 'n1', title: 'Room 302 maintenance escalated', message: 'AC not cooling - critical priority assigned', type: 'critical', time: '2m ago', read: false },
  { id: 'n2', title: 'Guest checked in', message: 'Amelia Stone assigned to Room 101', type: 'success', time: '5m ago', read: false },
  { id: 'n3', title: 'Payment received', message: 'INV-10021 - $420.00 completed', type: 'success', time: '8m ago', read: false },
  { id: 'n4', title: 'Housekeeping completed', message: 'Room 205 marked as Clean', type: 'info', time: '12m ago', read: false },
  { id: 'n5', title: 'Booking expired', message: 'Room 207 reservation expired - no payment', type: 'warning', time: '15m ago', read: true },
];

export const useNotificationStore = create<NotificationState>((set) => ({
  items: demoNotifications,
  unreadCount: demoNotifications.filter((n) => !n.read).length,
  addNotification: (item) =>
    set((s) => ({
      items: [item, ...s.items].slice(0, 50),
      unreadCount: s.unreadCount + 1,
    })),
  markRead: (id) =>
    set((s) => {
      const items = s.items.map((n) => (n.id === id ? { ...n, read: true } : n));
      return { items, unreadCount: items.filter((n) => !n.read).length };
    }),
  markAllRead: () =>
    set((s) => ({
      items: s.items.map((n) => ({ ...n, read: true })),
      unreadCount: 0,
    })),
  clearAll: () => set({ items: [], unreadCount: 0 }),
}));
