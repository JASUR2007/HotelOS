import { useEffect, useState, useRef } from 'react';
import DataTable from '../components/DataTable';
import Modal, { ConfirmDeleteModal } from '../components/Modal';
import StatusBadge from '../components/StatusBadge';
import { fetchRooms, createRoom, updateRoom, deleteRoom, fetchAmenities, uploadRoomImage, fetchBranches } from '../../api';
import type { RoomDto, CreateRoomDto, UpdateRoomDto, AmenityDto, HotelBranch } from '../../types';

const ROOM_TYPES = ['Single', 'Double', 'Suite', 'Accessible'];
const ROOM_STATUSES = ['Available', 'Occupied', 'Dirty', 'Cleaning', 'Maintenance', 'Reserved'];

interface RoomForm {
  branchId: number;
  roomNumber: string;
  type: string;
  status: string;
  pricePerNight: number;
  floor: number;
  description: string;
  guestCapacity: number;
  mainImage: string;
  images: string;
  amenityIds: string[];
}

const emptyForm: RoomForm = {
  branchId: 1, roomNumber: '', type: 'Single', status: 'Available',
  pricePerNight: 100, floor: 1, description: '', guestCapacity: 1,
  mainImage: '', images: '', amenityIds: [],
};

export default function RoomsPage() {
  const [rooms, setRooms] = useState<RoomDto[]>([]);
  const [amenities, setAmenities] = useState<AmenityDto[]>([]);
  const [branches, setBranches] = useState<HotelBranch[]>([]);
  const [branchFilter, setBranchFilter] = useState<number>(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<RoomDto | null>(null);
  const [form, setForm] = useState<RoomForm>(emptyForm);
  const [saving, setSaving] = useState(false);
  const [uploadingMain, setUploadingMain] = useState(false);
  const [uploadingGallery, setUploadingGallery] = useState(false);
  const mainInputRef = useRef<HTMLInputElement>(null);
  const galleryInputRef = useRef<HTMLInputElement>(null);

  async function loadRooms() {
    setLoading(true);
    setError(null);
    try {
      const [roomData, amenityData, branchData] = await Promise.all([fetchRooms(), fetchAmenities(), fetchBranches()]);
      setRooms(roomData);
      setAmenities(amenityData);
      setBranches(branchData);
    } catch {
      setError('Failed to load rooms. Is the backend running?');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { loadRooms(); }, []);

  function openCreate() {
    setForm({ ...emptyForm, branchId: branches[0]?.id ?? 1, pricePerNight: typeToPrice('Single') });
    setEditingId(null);
    setModal('create');
  }

  function openEdit(room: RoomDto) {
    setForm({
      branchId: room.branchId,
      roomNumber: room.roomNumber,
      type: room.type,
      status: room.status,
      pricePerNight: room.pricePerNight,
      floor: room.floor,
      description: room.description,
      guestCapacity: room.guestCapacity,
      mainImage: room.mainImage || '',
      images: room.images?.join(', ') || '',
      amenityIds: room.amenities || [],
    });
    setEditingId(room.id);
    setModal('edit');
  }

  function typeToPrice(type: string): number {
    const prices: Record<string, number> = { Single: 100, Double: 180, Suite: 350, Accessible: 150 };
    return prices[type] ?? 100;
  }

  async function handleSave() {
    setSaving(true);
    try {
      if (modal === 'create') {
        const req: CreateRoomDto = {
          branchId: form.branchId, roomNumber: form.roomNumber, type: form.type, floor: form.floor,
          pricePerNight: form.pricePerNight, guestCapacity: form.guestCapacity,
          description: form.description, mainImage: form.mainImage,
          images: form.images.split(',').map(s => s.trim()).filter(Boolean), amenityIds: form.amenityIds,
          status: form.status,
        };
        await createRoom(req);
      } else if (editingId) {
        const req: UpdateRoomDto = {
          branchId: form.branchId, roomNumber: form.roomNumber, type: form.type, status: form.status,
          pricePerNight: form.pricePerNight, floor: form.floor,
          description: form.description, guestCapacity: form.guestCapacity,
          mainImage: form.mainImage,
          images: form.images.split(',').map(s => s.trim()).filter(Boolean),
          amenities: form.amenityIds,
        };
        await updateRoom(editingId, req);
      }
      setModal(null);
      setForm(emptyForm);
      await loadRooms();
    } catch {
      setError('Failed to save room');
    } finally {
      setSaving(false);
    }
  }

  async function handleUploadMain(files: FileList | null) {
    if (!files || !files[0]) return;
    setUploadingMain(true);
    try {
      const result = await uploadRoomImage(files[0]);
      setForm({ ...form, mainImage: result.url });
    } catch {
      setError('Failed to upload main image');
    } finally {
      setUploadingMain(false);
    }
  }

  async function handleUploadGallery(files: FileList | null) {
    if (!files || files.length === 0) return;
    setUploadingGallery(true);
    try {
      const results = await Promise.all(Array.from(files).map(f => uploadRoomImage(f)));
      const urls = results.map(r => r.url);
      const existing = form.images ? form.images.split(',').map(s => s.trim()).filter(Boolean) : [];
      setForm({ ...form, images: [...existing, ...urls].join(', ') });
    } catch {
      setError('Failed to upload gallery images');
    } finally {
      setUploadingGallery(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    try {
      await deleteRoom(deleteTarget.id);
      setDeleteTarget(null);
      await loadRooms();
    } catch {
      setError('Failed to delete room');
    }
  }

  const typeColorMap: Record<string, string> = {
    Single: 'blue', Double: 'green', Suite: 'amber', Accessible: 'purple',
  };

  return (
    <div className="px-6 py-8">
      <div className="mb-8">
        <p className="text-xs uppercase tracking-[0.35em] text-accent">Hotel Management</p>
        <h1 className="mt-2 text-2xl font-bold text-primary">Room Operations</h1>
        <p className="mt-1 text-sm text-primary/50">Single, Double, Suite, Accessible</p>
      </div>

      {error && (
        <div className="mb-4 rounded-xl border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">{error}</div>
      )}

      <DataTable
        columns={[
          { key: 'roomNumber', label: 'Room', sortable: true, render: (r: RoomDto) => <span className="font-semibold">#{r.roomNumber}</span> },
          { key: 'type', label: 'Type', sortable: true, render: (r: RoomDto) => <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium bg-${typeColorMap[r.type] ?? 'gray'}-100 text-${typeColorMap[r.type] ?? 'gray'}-700`}>{r.type}</span> },
          { key: 'status', label: 'Status', sortable: true, render: (r: RoomDto) => <StatusBadge status={r.status} /> },
          { key: 'floor', label: 'Floor', sortable: true },
          { key: 'pricePerNight', label: 'Price/Night', sortable: true, render: (r: RoomDto) => `$${r.pricePerNight}` },
          { key: 'guestCapacity', label: 'Max Guests', sortable: true },
          { key: 'branchId', label: 'Branch', render: (r: RoomDto) => branches.find(b => b.id === r.branchId)?.name ?? `Branch #${r.branchId}` },
        ]}
        data={branchFilter ? rooms.filter(r => r.branchId === branchFilter) : rooms}
        keyExtractor={(r) => r.id}
        searchKeys={['roomNumber', 'type', 'status']}
        searchPlaceholder="Search rooms..."
        loading={loading}
        toolbar={
          <div className="flex items-center gap-3">
            <select value={branchFilter} onChange={(e) => setBranchFilter(Number(e.target.value))}
              className="rounded-lg border border-primary/10 px-3 py-2 text-sm outline-none focus:border-accent">
              <option value={0}>All Branches</option>
              {branches.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
            </select>
            <button onClick={openCreate} className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:bg-accent/90">
              + Add Room
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

      <Modal open={modal !== null} onClose={() => setModal(null)} title={modal === 'create' ? 'Add Room' : 'Edit Room'}>
        <div className="space-y-4">
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Branch</label>
            <select value={form.branchId} onChange={(e) => setForm({ ...form, branchId: Number(e.target.value) })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              {branches.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Room Number</label>
            <input value={form.roomNumber} onChange={(e) => setForm({ ...form, roomNumber: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Room Type</label>
            <select value={form.type} onChange={(e) => setForm({ ...form, type: e.target.value, pricePerNight: typeToPrice(e.target.value) })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              {ROOM_TYPES.map((t) => <option key={t}>{t}</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Status</label>
            <select value={form.status} onChange={(e) => setForm({ ...form, status: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              {ROOM_STATUSES.map((s) => <option key={s}>{s}</option>)}
            </select>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Floor</label>
              <input type="number" value={form.floor} onChange={(e) => setForm({ ...form, floor: +e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
            </div>
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Price/Night</label>
              <input type="number" value={form.pricePerNight} onChange={(e) => setForm({ ...form, pricePerNight: +e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
            </div>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Guest Capacity</label>
            <input type="number" value={form.guestCapacity} onChange={(e) => setForm({ ...form, guestCapacity: +e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Description</label>
            <input value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Main Image</label>
            <div className="flex gap-2">
              <input value={form.mainImage} onChange={(e) => setForm({ ...form, mainImage: e.target.value })}
                className="flex-1 rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" placeholder="/images/rooms/room-1.jpg" />
              <input ref={mainInputRef} type="file" accept="image/*" className="hidden" onChange={(e) => handleUploadMain(e.target.files)} />
              <button type="button" disabled={uploadingMain}
                onClick={() => mainInputRef.current?.click()}
                className="rounded-lg border border-accent/30 px-3 py-2.5 text-xs text-accent hover:bg-accent/5 disabled:opacity-50 whitespace-nowrap">
                {uploadingMain ? 'Uploading...' : 'Upload'}
              </button>
            </div>
            {form.mainImage && <p className="mt-1 text-xs text-primary/40 truncate">{form.mainImage}</p>}
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Gallery Images</label>
            <div className="flex gap-2">
              <input value={form.images} onChange={(e) => setForm({ ...form, images: e.target.value })}
                className="flex-1 rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" placeholder="/images/rooms/room-1.jpg, /images/rooms/room-1-lg.jpg" />
              <input ref={galleryInputRef} type="file" accept="image/*" multiple className="hidden" onChange={(e) => handleUploadGallery(e.target.files)} />
              <button type="button" disabled={uploadingGallery}
                onClick={() => galleryInputRef.current?.click()}
                className="rounded-lg border border-accent/30 px-3 py-2.5 text-xs text-accent hover:bg-accent/5 disabled:opacity-50 whitespace-nowrap">
                {uploadingGallery ? 'Uploading...' : 'Upload'}
              </button>
            </div>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Amenities</label>
            <div className="max-h-48 overflow-y-auto space-y-1 rounded-lg border border-primary/10 p-3">
              {amenities.map((a) => (
                <label key={a.id} className="flex items-center gap-2 cursor-pointer text-sm text-primary/80">
                  <input
                    type="checkbox"
                    checked={form.amenityIds.includes(a.name) || form.amenityIds.includes(String(a.id))}
                    onChange={() => {
                      const id = String(a.id);
                      const name = a.name;
                      setForm({
                        ...form,
                        amenityIds: form.amenityIds.includes(id) || form.amenityIds.includes(name)
                          ? form.amenityIds.filter(x => x !== id && x !== name)
                          : [...form.amenityIds, id],
                      });
                    }}
                    className="h-4 w-4 rounded border-primary/20 text-accent focus:ring-accent"
                  />
                  {a.name}
                </label>
              ))}
              {amenities.length === 0 && <p className="text-xs text-primary/40 italic">Loading amenities...</p>}
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
        title="Delete Room"
        message={`Are you sure you want to delete room #${deleteTarget?.roomNumber} (${deleteTarget?.type})? This action cannot be undone.`}
      />
    </div>
  );
}
