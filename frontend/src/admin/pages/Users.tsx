import { useEffect, useState } from 'react';
import DataTable from '../components/DataTable';
import Modal, { ConfirmDeleteModal } from '../components/Modal';
import { getHotelApiBaseUrl } from '../../api';
import type { AuthUser, UserRole } from '../../types';

const ROLE_NAME_TO_ID: Record<string, number> = {
  SuperAdmin: 1,
  Admin: 2,
  Receptionist: 3,
  Housekeeper: 4,
  Technician: 5,
  KitchenStaff: 6,
  Accountant: 8,
  Guest: 7,
};

const ROLES: UserRole[] = [
  'SuperAdmin', 'Admin', 'Receptionist', 'Housekeeper',
  'Technician', 'KitchenStaff', 'Accountant', 'Guest',
];

const API = getHotelApiBaseUrl();

function authHeaders(): Record<string, string> {
  const store = (window as unknown as Record<string, unknown>).__hotelos_auth_store__ as
    | { getState(): { accessToken?: string } }
    | undefined;
  const token = store?.getState().accessToken;
  return token ? { Authorization: `Bearer ${token}` } : {};
}

interface UserForm {
  email: string;
  displayName: string;
  password: string;
  role: UserRole;
}

const emptyForm: UserForm = { email: '', displayName: '', password: '', role: 'Guest' };

export default function UsersPage() {
  const [users, setUsers] = useState<AuthUser[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [editingUser, setEditingUser] = useState<AuthUser | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<AuthUser | null>(null);
  const [form, setForm] = useState<UserForm>(emptyForm);
  const [saving, setSaving] = useState(false);
  const [roleFilter, setRoleFilter] = useState<UserRole | 'All'>('All');

  async function loadUsers() {
    setLoading(true);
    setError(null);
    try {
      const res = await fetch(`${API}/admin/users`, {
        headers: { 'Content-Type': 'application/json', ...authHeaders() },
      });
      if (!res.ok) throw new Error('Failed to fetch users');
      const data = await res.json();
      const mapped = (data as any[]).map((u: any) => ({
        ...u,
        role: u.roles?.[0] ?? u.role ?? 'Guest',
        roles: undefined,
      }));
      setUsers(mapped);
    } catch {
      setError('Failed to load users. Is the backend running?');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { loadUsers(); }, []);

  const filteredUsers = roleFilter === 'All' ? users : users.filter((u) => u.role === roleFilter);

  function openCreate() {
    setForm({ ...emptyForm });
    setEditingUser(null);
    setModal('create');
  }

  function openEdit(user: AuthUser) {
    setForm({
      email: user.email,
      displayName: user.displayName,
      password: '',
      role: user.role,
    });
    setEditingUser(user);
    setModal('edit');
  }

  async function handleSave() {
    setSaving(true);
    setError(null);
    try {
      if (modal === 'create') {
        const res = await fetch(`${API}/admin/users`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json', ...authHeaders() },
          body: JSON.stringify({
            email: form.email,
            displayName: form.displayName,
            password: form.password,
            role: form.role,
          }),
        });
        if (!res.ok) throw new Error('Failed to create user');
      } else if (editingUser) {
        const res = await fetch(`${API}/admin/assign-role`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json', ...authHeaders() },
          body: JSON.stringify({
            userId: editingUser.id,
            roleId: ROLE_NAME_TO_ID[form.role] ?? 7,
          }),
        });
        if (!res.ok) throw new Error('Failed to update user role');

        if (form.displayName !== editingUser.displayName) {
          const updateRes = await fetch(`${API}/admin/users/${editingUser.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', ...authHeaders() },
            body: JSON.stringify({ displayName: form.displayName }),
          });
          if (!updateRes.ok) throw new Error('Failed to update display name');
        }
      }
      setModal(null);
      setForm(emptyForm);
      await loadUsers();
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Failed to save user');
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setError(null);
    try {
      const res = await fetch(`${API}/admin/users/${deleteTarget.id}`, {
        method: 'DELETE',
        headers: { ...authHeaders() },
      });
      if (!res.ok) throw new Error('Failed to delete user');
      setDeleteTarget(null);
      await loadUsers();
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Failed to delete user');
    }
  }

  const roleBadgeClass: Record<UserRole, string> = {
    SuperAdmin: 'bg-purple-100 text-purple-700',
    Admin: 'bg-sky-100 text-sky-700',
    Receptionist: 'bg-emerald-100 text-emerald-700',
    Housekeeper: 'bg-amber-100 text-amber-700',
    Technician: 'bg-rose-100 text-rose-700',
    KitchenStaff: 'bg-orange-100 text-orange-700',
    Accountant: 'bg-cyan-100 text-cyan-700',
    Guest: 'bg-slate-100 text-slate-600',
  };

  return (
    <div className="px-6 py-8">
      <div className="mb-8">
        <p className="text-xs uppercase tracking-[0.35em] text-accent">Administration</p>
        <h1 className="mt-2 text-2xl font-bold text-primary">User Management</h1>
        <p className="mt-1 text-sm text-primary/50">Manage platform users, roles, and permissions</p>
      </div>

      {error && (
        <div className="mb-4 rounded-xl border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">{error}</div>
      )}

      <DataTable
        columns={[
          { key: 'email', label: 'Email', sortable: true },
          { key: 'displayName', label: 'Display Name', sortable: true },
          {
            key: 'role', label: 'Role', sortable: true,
            render: (u: AuthUser) => (
              <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium uppercase tracking-[0.15em] ${roleBadgeClass[u.role] ?? 'bg-slate-100 text-slate-600'}`}>
                {u.role}
              </span>
            ),
          },
        ]}
        data={filteredUsers}
        keyExtractor={(u) => u.id}
        searchKeys={['email', 'displayName', 'role']}
        searchPlaceholder="Search users..."
        loading={loading}
        toolbar={
          <div className="flex items-center gap-3">
            <select
              value={roleFilter}
              onChange={(e) => setRoleFilter(e.target.value as UserRole | 'All')}
              className="rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent bg-white"
            >
              <option value="All">All Roles</option>
              {ROLES.map((r) => <option key={r}>{r}</option>)}
            </select>
            <span className="text-xs text-primary/40">{filteredUsers.length} of {users.length} users</span>
            <button onClick={openCreate} className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:bg-accent/90">
              + Add User
            </button>
          </div>
        }
        actions={(row) => (
          <>
            <button onClick={() => openEdit(row)} className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs text-primary/60 hover:bg-primary/5">Edit</button>
            <button onClick={() => setDeleteTarget(row)} className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs text-rose-600 hover:bg-rose-50">Delete</button>
          </>
        )}
      />

      <Modal open={modal !== null} onClose={() => setModal(null)} title={modal === 'create' ? 'Add User' : 'Edit User'}>
        <div className="space-y-4">
          {modal === 'create' && (
            <>
              <div>
                <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Email</label>
                <input type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })}
                  className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
              </div>
              <div>
                <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Password</label>
                <input type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })}
                  className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
              </div>
            </>
          )}
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Display Name</label>
            <input value={form.displayName} onChange={(e) => setForm({ ...form, displayName: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Role</label>
            <select value={form.role} onChange={(e) => setForm({ ...form, role: e.target.value as UserRole })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              {ROLES.map((r) => <option key={r}>{r}</option>)}
            </select>
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button onClick={() => setModal(null)} className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5">Cancel</button>
            <button onClick={handleSave} disabled={saving}
              className="rounded-lg bg-accent px-5 py-2.5 text-sm text-white hover:bg-accent/90 disabled:opacity-50">
              {saving ? 'Saving...' : modal === 'create' ? 'Create' : 'Save'}
            </button>
          </div>
        </div>
      </Modal>

      <ConfirmDeleteModal
        open={deleteTarget !== null}
        onClose={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        title="Delete User"
        message={`Are you sure you want to delete ${deleteTarget?.displayName} (${deleteTarget?.email})? This action cannot be undone.`}
      />
    </div>
  );
}
