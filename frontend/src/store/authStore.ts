import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { loginManager } from '../api';
import type { AuthUser, PermissionName } from '../types';

interface AuthState {
	accessToken: string | null;
	refreshToken: string | null;
	user: AuthUser | null;
	expiresAt: number | null;
	login: (email: string, password: string) => Promise<AuthUser>;
	logout: () => void;
	hasPermission: (permission: PermissionName) => boolean;
	isStaff: () => boolean;
}

const initialState = { accessToken: null, refreshToken: null, user: null, expiresAt: null };

export const useAuthStore = create<AuthState>()(
	persist(
		(set, get) => ({
			...initialState,
			login: async (email, password) => {
				const result = await loginManager({ email, password });
				set({
					accessToken: result.accessToken,
					refreshToken: result.refreshToken,
					user: result.user,
					expiresAt: Date.now() + result.expiresIn * 1000,
				});
				return result.user;
			},
			logout: () => set({ ...initialState }),
			hasPermission: (permission: PermissionName) => get().user?.permissions.includes(permission) ?? false,
			isStaff: () => {
				const role = get().user?.role;
				return Boolean(role && role !== 'Guest');
			},
		}),
		{ name: 'hotelos-frontend-auth', partialize: (state) => ({ accessToken: state.accessToken, refreshToken: state.refreshToken, user: state.user }) },
	),
);

(window as unknown as Record<string, unknown>).__hotelos_auth_store__ = useAuthStore;