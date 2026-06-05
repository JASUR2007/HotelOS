import { useEffect, useState } from 'react';
import DataTable from '../components/DataTable';
import Modal, { ConfirmDeleteModal } from '../components/Modal';
import { getHotelApiBaseUrl } from '../../api';
import type { RoleRecord, PermissionName } from '../../types';

const ALL_PERMISSIONS: PermissionName[] = [
  'create_booking', 'update_booking', 'delete_booking',
  'manage_rooms', 'manage_users', 'manage_roles', 'manage_permissions',
  'create_orders', 'update_orders',
  'resolve_maintenance', 'assign_maintenance',
  'view_dashboard', 'view_settings', 'view_maintenances',
  'manage_payments', 'view_reports',
];

const API = getHotelApiBaseUrl();

function authHeaders(): Record<string, string> {
  const store = (window as unknown as Record<string, unknown>).__hotelos_auth_store__ as
    | { getState(): { accessToken?: string } }
    | undefined;
  const token = store?.getState().accessToken;
  return token ? { Authorization: `Bearer ${token}` } : {};
}

interface RoleForm {
  name: string;
  permissions: PermissionName[];
}

const emptyForm: RoleForm = { name: '', permissions: [] };

export default function RolesPage() {
  const [roles, setRoles] = useState<RoleRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [editingRole, setEditingRole] = useState<RoleRecord | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<RoleRecord | null>(null);
  const [form, setForm] = useState<RoleForm>(emptyForm);
  const [saving, setSaving] = useState(false);

  async function loadRoles() {
    setLoading(true);
    setError(null);
    try {
      const res = await fetch(`${API}/admin/roles`, {
        headers: { 'Content-Type': 'application/json', ...authHeaders() },
      });
      if (!res.ok) throw new Error('Failed to fetch roles');
      setRoles(await res.json());
    } catch {
      setError('Failed to load roles. Is the backend running?');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { loadRoles(); }, []);

  function openCreate() {
    setForm({ ...emptyForm });
    setEditingRole(null);
    setModal('create');
  }

  function openEdit(role: RoleRecord) {
    setForm({
      name: role.name,
      permissions: [...role.permissions],
    });
    setEditingRole(role);
    setModal('edit');
  }

  function togglePermission(p: PermissionName) {
    setForm((prev) => ({
      ...prev,
      permissions: prev.permissions.includes(p)
        ? prev.permissions.filter((x) => x !== p)
        : [...prev.permissions, p],
    }));
  }

  async function handleSave() {
    setSaving(true);
    setError(null);
    try {
      if (modal === 'create') {
        const res = await fetch(`${API}/admin/roles`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json', ...authHeaders() },
          body: JSON.stringify({ name: form.name, permissionNames: form.permissions }),
        });
        if (!res.ok) throw new Error('Failed to create role');
      } else if (editingRole) {
        let res = await fetch(`${API}/admin/roles/${editingRole.id}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json', ...authHeaders() },
          body: JSON.stringify({ name: form.name }),
        });
        if (!res.ok) throw new Error('Failed to update role');

        const existingPerms = new Set(editingRole.permissions);
        const newPerms = new Set(form.permissions);

        const toAssign = form.permissions.filter((p) => !existingPerms.has(p));
        const toRemove = editingRole.permissions.filter((p) => !newPerms.has(p));

        for (const p of toAssign) {
          res = await fetch(`${API}/admin/assign-permission`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', ...authHeaders() },
            body: JSON.stringify({ roleId: editingRole.id, permissionId: 0, permissionName: p }),
          });
          if (!res.ok) throw new Error(`Failed to assign permission: ${p}`);
        }

        for (const p of toRemove) {
          res = await fetch(`${API}/admin/roles/${editingRole.id}/permissions/by-name/${p}`, {
            method: 'DELETE',
            headers: { ...authHeaders() },
          });
          if (!res.ok) throw new Error(`Failed to remove permission: ${p}`);
        }
      }
      setModal(null);
      setForm(emptyForm);
      await loadRoles();
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Failed to save role');
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setError(null);
    try {
      const res = await fetch(`${API}/admin/roles/${deleteTarget.id}`, {
        method: 'DELETE',
        headers: { ...authHeaders() },
      });
      if (!res.ok) throw new Error('Failed to delete role');
      setDeleteTarget(null);
      await loadRoles();
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Failed to delete role');
    }
  }

  function permissionLabel(p: PermissionName): string {
    return p.replace(/_/g, ' ').replace(/\b\w/g, (c) => c.toUpperCase());
  }

  return (
    <div className="px-6 py-8">
      <div className="mb-8">
        <p className="text-xs uppercase tracking-[0.35em] text-accent">Administration</p>
        <h1 className="mt-2 text-2xl font-bold text-primary">Role Management</h1>
        <p className="mt-1 text-sm text-primary/50">Define roles and their permission scopes</p>
      </div>

      {error && (
        <div className="mb-4 rounded-xl border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">{error}</div>
      )}

      <DataTable
        columns={[
          { key: 'name', label: 'Name', sortable: true },
          {
            key: 'permissions', label: 'Permissions',
            render: (r: RoleRecord) => (
              <div className="flex flex-col gap-1 py-1">
                {r.permissions?.map((p) => (
                  <span key={p} className="inline-flex items-center gap-1.5 text-xs text-emerald-700">
                    <span className="text-emerald-500 font-bold">✓</span>
                    {permissionLabel(p)}
                  </span>
                ))}
              </div>
            ),
          },
        ]}
        data={roles}
        keyExtractor={(r) => r.id}
        searchKeys={['name']}
        searchPlaceholder="Search roles..."
        loading={loading}
        toolbar={
          <button onClick={openCreate} className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:bg-accent/90">
            + Add Role
          </button>
        }
        actions={(row) => (
          <>
            <button onClick={() => openEdit(row)} className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs text-primary/60 hover:bg-primary/5">Edit</button>
            <button onClick={() => setDeleteTarget(row)} className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs text-rose-600 hover:bg-rose-50">Delete</button>
          </>
        )}
      />

      <Modal open={modal !== null} onClose={() => setModal(null)} title={modal === 'create' ? 'Add Role' : 'Edit Role'}>
        <div className="space-y-4">
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Role Name</label>
            <input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Permissions</label>
            <div className="max-h-64 overflow-y-auto space-y-2 rounded-lg border border-primary/10 p-3">
              {ALL_PERMISSIONS.map((p) => (
                <label key={p} className="flex items-center gap-2 cursor-pointer text-sm text-primary/80">
                  <input
                    type="checkbox"
                    checked={form.permissions.includes(p)}
                    onChange={() => togglePermission(p)}
                    className="h-4 w-4 rounded border-primary/20 text-accent focus:ring-accent"
                  />
                  {permissionLabel(p)}
                </label>
              ))}
            </div>
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
        title="Delete Role"
        message={`Are you sure you want to delete the role "${deleteTarget?.name}"? Users assigned this role may lose permissions.`}
      />
    </div>
  );
}
