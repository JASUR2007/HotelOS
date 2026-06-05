import { create } from 'zustand';
import { fetchAdminMetrics, fetchNotificationsCenter, fetchOccupancyAnalytics, fetchRevenueAnalytics } from '../../api';
import type { AdminMetric, AnalyticsPoint, NotificationCenterItem, RevenuePoint } from '../../types';

interface DashboardState {
  metrics: AdminMetric[];
  occupancy: AnalyticsPoint[];
  revenue: RevenuePoint[];
  notifications: NotificationCenterItem[];
  loading: boolean;
  load: () => Promise<void>;
  prependNotification: (item: NotificationCenterItem) => void;
}

export const useDashboardStore = create<DashboardState>((set) => ({
  metrics: [],
  occupancy: [],
  revenue: [],
  notifications: [],
  loading: false,
  load: async () => {
    set({ loading: true });
    const [metrics, occupancy, revenue, notifications] = await Promise.all([
      fetchAdminMetrics(),
      fetchOccupancyAnalytics(),
      fetchRevenueAnalytics(),
      fetchNotificationsCenter(),
    ]);
    set({ metrics, occupancy, revenue, notifications, loading: false });
  },
  prependNotification: (item) => set((state) => ({ notifications: [item, ...state.notifications].slice(0, 12) })),
}));
