import { useEffect, useState } from 'react';
import { DataTable, Modal, ConfirmDeleteModal, StatusBadge } from '../components';
import type { Column } from '../components/DataTable';
import { fetchBranches, createBranch, updateBranch, deleteBranch } from '../../api';
import type { HotelBranch, CreateBranchDto, UpdateBranchDto } from '../../types';

export default function Branches() {
  const [branches, setBranches] = useState<HotelBranch[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [saving, setSaving] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState<HotelBranch | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<HotelBranch | null>(null);

  const [formName, setFormName] = useState('');
  const [formAddress, setFormAddress] = useState('');
  const [formCity, setFormCity] = useState('');
  const [formCountry, setFormCountry] = useState('');
  const [formPhone, setFormPhone] = useState('');
  const [formEmail, setFormEmail] = useState('');
  const [formStatus, setFormStatus] = useState('Active');

  async function load() {
    setLoading(true);
    try {
      const data = await fetchBranches();
      setBranches(data);
    } catch { setError('Failed to load branches'); }
    finally { setLoading(false); }
  }

  useEffect(() => { load(); }, []);

  function openCreate() {
    setEditing(null);
    setFormName(''); setFormAddress(''); setFormCity(''); setFormCountry(''); setFormPhone(''); setFormEmail(''); setFormStatus('Active');
    setShowModal(true);
  }

  function openEdit(b: HotelBranch) {
    setEditing(b);
    setFormName(b.name); setFormAddress(b.address); setFormCity(b.city); setFormCountry(b.country); setFormPhone(b.phone); setFormEmail(b.email); setFormStatus(b.status);
    setShowModal(true);
  }

  async function handleSave() {
    setSaving(true); setError('');
    try {
      if (editing) {
        await updateBranch(editing.id, { name: formName, address: formAddress, city: formCity, country: formCountry, phone: formPhone, email: formEmail, status: formStatus } as UpdateBranchDto);
        setSuccess('Branch updated');
      } else {
        await createBranch({ name: formName, address: formAddress, city: formCity, country: formCountry, phone: formPhone, email: formEmail } as CreateBranchDto);
        setSuccess('Branch created');
      }
      setShowModal(false);
      load();
    } catch (e: unknown) { setError(e instanceof Error ? e.message : 'Failed to save'); }
    finally { setSaving(false); }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setSaving(true);
    try {
      await deleteBranch(deleteTarget.id);
      setSuccess('Branch deleted');
      setDeleteTarget(null);
      load();
    } catch (e: unknown) { setError(e instanceof Error ? e.message : 'Failed to delete'); }
    finally { setSaving(false); }
  }

  useEffect(() => { if (success) { const t = setTimeout(() => setSuccess(''), 4000); return () => clearTimeout(t); } }, [success]);

  const columns: Column<HotelBranch>[] = [
    { key: 'name', label: 'Name', sortable: true },
    { key: 'city', label: 'City', sortable: true },
    { key: 'country', label: 'Country', sortable: true },
    { key: 'phone', label: 'Phone' },
    { key: 'email', label: 'Email' },
    { key: 'status', label: 'Status', render: (r) => <StatusBadge status={r.status} /> },
    { key: 'createdAt', label: 'Created', sortable: true },
  ];

  return (
    <div className="p-6">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-primary">Hotel Branches</h1>
        <button onClick={openCreate} className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:opacity-90">+ Add Branch</button>
      </div>

      {error && <div className="mb-4 rounded-lg bg-red-50 p-3 text-sm text-red-600">{error}</div>}
      {success && <div className="mb-4 rounded-lg bg-emerald-50 p-3 text-sm text-emerald-600">{success}</div>}

      <DataTable
        columns={columns}
        data={branches}
        keyExtractor={(r) => r.id}
        searchKeys={['name', 'city', 'country']}
        searchPlaceholder="Search branches..."
        loading={loading}
        actions={(row) => (
          <div className="flex gap-2">
            <button onClick={() => openEdit(row)} className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs text-primary/60 hover:bg-primary/5">Edit</button>
            <button onClick={() => setDeleteTarget(row)} className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs text-rose-600 hover:bg-rose-50">Delete</button>
          </div>
        )}
      />

      <Modal open={showModal} onClose={() => setShowModal(false)} title={editing ? 'Edit Branch' : 'Add Branch'}>
        <div className="grid grid-cols-2 gap-4">
          <div className="col-span-2">
            <label className="mb-1 block text-xs font-medium text-primary/60">Name</label>
            <input value={formName} onChange={(e) => setFormName(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div className="col-span-2">
            <label className="mb-1 block text-xs font-medium text-primary/60">Address</label>
            <input value={formAddress} onChange={(e) => setFormAddress(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-primary/60">City</label>
            <input value={formCity} onChange={(e) => setFormCity(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-primary/60">Country</label>
            <input value={formCountry} onChange={(e) => setFormCountry(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-primary/60">Phone</label>
            <input value={formPhone} onChange={(e) => setFormPhone(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-primary/60">Email</label>
            <input value={formEmail} onChange={(e) => setFormEmail(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          {editing && (
            <div>
              <label className="mb-1 block text-xs font-medium text-primary/60">Status</label>
              <select value={formStatus} onChange={(e) => setFormStatus(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
                <option value="Active">Active</option>
                <option value="Inactive">Inactive</option>
              </select>
            </div>
          )}
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <button onClick={() => setShowModal(false)} className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5">Cancel</button>
          <button onClick={handleSave} disabled={saving} className="rounded-lg bg-accent px-5 py-2.5 text-sm text-white hover:opacity-90 disabled:opacity-50">
            {saving ? 'Saving...' : editing ? 'Update' : 'Create'}
          </button>
        </div>
      </Modal>

      <ConfirmDeleteModal
        open={!!deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        title="Delete Branch"
        message={`Are you sure you want to delete "${deleteTarget?.name}"? This action cannot be undone.`}
      />
    </div>
  );
}
