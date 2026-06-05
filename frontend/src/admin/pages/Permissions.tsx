import { useEffect, useState } from 'react';
import DataTable from '../components/DataTable';
import Modal, { ConfirmDeleteModal } from '../components/Modal';
import { getHotelApiBaseUrl } from '../../api';
import type { PermissionRecord } from '../../types';

const API = getHotelApiBaseUrl();

function authHeaders(): Record<string, string> {
  const store = (window as unknown as Record<string, unknown>).__hotelos_auth_store__ as
    | { getState(): { accessToken?: string } }
    | undefined;
  const token = store?.getState().accessToken;
  return token ? { Authorization: `Bearer ${token}` } : {};
}

interface PermissionForm {
  name: string;
  description: string;
}

const emptyForm: PermissionForm = { name: '', description: '' };

export default function PermissionsPage() {
  const [permissions, setPermissions] = useState<PermissionRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [editingPermission, setEditingPermission] = useState<PermissionRecord | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<PermissionRecord | null>(null);
  const [form, setForm] = useState<PermissionForm>(emptyForm);
  const [saving, setSaving] = useState(false);

  async function loadPermissions() {
    setLoading(true);
    setError(null);
    try {
      const res = await fetch(`${API}/admin/permissions`, {
        headers: { 'Content-Type': 'application/json', ...authHeaders() },
      });
      if (!res.ok) throw new Error('Failed to fetch permissions');
      setPermissions(await res.json());
    } catch {
      setError('Failed to load permissions. Is the backend running?');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { loadPermissions(); }, []);

  function openCreate() {
    setForm({ ...emptyForm });
    setEditingPermission(null);
    setModal('create');
  }

  function openEdit(perm: PermissionRecord) {
    setForm({
      name: perm.name,
      description: perm.description,
    });
    setEditingPermission(perm);
    setModal('edit');
  }

  async function handleSave() {
    setSaving(true);
    setError(null);
    try {
      if (modal === 'create') {
        const res = await fetch(`${API}/admin/permissions`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json', ...authHeaders() },
          body: JSON.stringify({ name: form.name, description: form.description }),
        });
        if (!res.ok) throw new Error('Failed to create permission');
      } else if (editingPermission) {
        const res = await fetch(`${API}/admin/permissions/${editingPermission.id}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json', ...authHeaders() },
          body: JSON.stringify({ name: form.name, description: form.description }),
        });
        if (!res.ok) throw new Error('Failed to update permission');
      }
      setModal(null);
      setForm(emptyForm);
      await loadPermissions();
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Failed to save permission');
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setError(null);
    try {
      const res = await fetch(`${API}/admin/permissions/${deleteTarget.id}`, {
        method: 'DELETE',
        headers: { ...authHeaders() },
      });
      if (!res.ok) throw new Error('Failed to delete permission');
      setDeleteTarget(null);
      await loadPermissions();
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Failed to delete permission');
    }
  }


  return (
    <div className="px-6 py-8">
      <div className="mb-8">
        <p className="text-xs uppercase tracking-[0.35em] text-accent">Administration</p>
        <h1 className="mt-2 text-2xl font-bold text-primary">Permission Management</h1>
        <p className="mt-1 text-sm text-primary/50">Granular access control definitions</p>
      </div>

      {error && (
        <div className="mb-4 rounded-xl border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">{error}</div>
      )}

      <DataTable
        columns={[
          {
            key: 'name', label: 'Name', sortable: true,
            render: (p: PermissionRecord) => (
              <code className="rounded bg-primary/5 px-2 py-1 text-xs font-mono text-accent">
                {p.name}
              </code>
            ),
          },
          { key: 'description', label: 'Description', sortable: true },
        ]}
        data={permissions}
        keyExtractor={(p) => p.id}
        searchKeys={['name', 'description']}
        searchPlaceholder="Search permissions..."
        loading={loading}
        toolbar={
          <button onClick={openCreate} className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:bg-accent/90">
            + Add Permission
          </button>
        }
        actions={(row) => (
          <>
            <button onClick={() => openEdit(row)} className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs text-primary/60 hover:bg-primary/5">Edit</button>
            <button onClick={() => setDeleteTarget(row)} className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs text-rose-600 hover:bg-rose-50">Delete</button>
          </>
        )}
      />

      <Modal open={modal !== null} onClose={() => setModal(null)} title={modal === 'create' ? 'Add Permission' : 'Edit Permission'}>
        <div className="space-y-4">
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Permission Name</label>
            <input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })}
              placeholder="e.g. manage_billing"
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Description</label>
            <input value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })}
              placeholder="What this permission allows"
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
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
        title="Delete Permission"
        message={`Are you sure you want to delete the permission "${deleteTarget?.name}"? Roles using this permission will be affected.`}
      />
    </div>
  );
}
