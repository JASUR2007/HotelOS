import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface UiState {
  sidebarCollapsed: boolean;
  sidebarGroups: Record<string, boolean>;
  theme: 'light' | 'dark';
  toggleSidebar: () => void;
  toggleGroup: (group: string) => void;
  setTheme: (theme: 'light' | 'dark') => void;
}

export const useUiStore = create<UiState>()(
  persist(
    (set) => ({
      sidebarCollapsed: false,
      sidebarGroups: {},
      theme: 'light',
      toggleSidebar: () => set((s) => ({ sidebarCollapsed: !s.sidebarCollapsed })),
      toggleGroup: (group) =>
        set((s) => ({
          sidebarGroups: { ...s.sidebarGroups, [group]: !s.sidebarGroups[group] },
        })),
      setTheme: (theme) => set({ theme }),
    }),
    { name: 'hotelos-admin-ui' },
  ),
);
