import { useState, useEffect } from 'react';
import { useAuthStore } from '../store/authStore';
import { useNavigate } from 'react-router-dom';
import { fetchMaintenanceTickets } from '../api';

interface MyReservation {
  id: number;
  roomNumber: string;
  status: string;
  checkInDate: string;
  checkOutDate: string;
  nights: number;
  total: number;
}

interface MyOrder {
  id: number;
  roomNumber: string;
  guestName: string;
  items?: string[];
  status: string;
  total: number;
}

interface MyMaintenance {
  id: number;
  roomNumber: string;
  title: string;
  priority: string;
  status: string;
}

export default function Profile() {
  const user = useAuthStore((state) => state.user);
  const logout = useAuthStore((state) => state.logout);
  const navigate = useNavigate();

  const [activeTab, setActiveTab] = useState<'profile' | 'reservations' | 'orders' | 'maintenance'>('profile');
  const [reservations, setReservations] = useState<MyReservation[]>([]);
  const [orders, setOrders] = useState<MyOrder[]>([]);
  const [maintenance, setMaintenance] = useState<MyMaintenance[]>([]);
  const [loadingR, setLoadingR] = useState(false);
  const [loadingO, setLoadingO] = useState(false);
  const [loadingM, setLoadingM] = useState(false);

  const [orderRoom, setOrderRoom] = useState('');
  const [orderItem, setOrderItem] = useState('Breakfast set');
  const [orderStatus, setOrderStatus] = useState('');

  const [maintRoom, setMaintRoom] = useState('');
  const [maintTitle, setMaintTitle] = useState('');
  const [maintDesc, setMaintDesc] = useState('');
  const [maintStatus, setMaintStatus] = useState('');
  const [cancelStatus, setCancelStatus] = useState('');
  const [cancelTarget, setCancelTarget] = useState<number | null>(null);

  const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';

  const activeReservation = reservations.find(r => r.status === 'Confirmed' || r.status === 'HELD' || r.status === 'Active');

  useEffect(() => {
    if (user) {
      setLoadingR(true);
      fetch(`${apiBaseUrl}/reception/my-reservations?email=${user.email}`)
        .then(r => r.ok ? r.json() : [])
        .then((data: MyReservation[]) => {
          setReservations(data);
          const active = data.find(r => r.status === 'Confirmed' || r.status === 'HELD' || r.status === 'Active');
          if (active) {
            setOrderRoom(active.roomNumber);
            setMaintRoom(active.roomNumber);
          }
        })
        .catch(() => {})
        .finally(() => setLoadingR(false));
    }
  }, [user]);

  useEffect(() => {
    if (activeTab === 'orders' && activeReservation) {
      setLoadingO(true);
      fetch(`${apiBaseUrl}/room/orders`)
        .then(r => r.ok ? r.json() : [])
        .then(setOrders)
        .catch(() => {})
        .finally(() => setLoadingO(false));
    }
  }, [activeTab, activeReservation]);

  useEffect(() => {
    if (activeTab === 'maintenance' && activeReservation) {
      setLoadingM(true);
      fetchMaintenanceTickets()
        .then(setMaintenance)
        .catch(() => {})
        .finally(() => setLoadingM(false));
    }
  }, [activeTab, activeReservation]);

  function handleLogout() {
    logout();
    navigate('/');
  }

  async function placeOrder(e: React.FormEvent) {
    e.preventDefault();
    setOrderStatus('');
    try {
      const res = await fetch(`${apiBaseUrl}/room/orders`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ roomNumber: orderRoom, guestName: user?.displayName || 'Guest', items: [orderItem] }),
      });
      if (!res.ok) throw new Error('Failed to place order');
      setOrderStatus('Order placed successfully!');
      setOrderRoom('');
      loadOrders();
    } catch (err) {
      setOrderStatus(err instanceof Error ? err.message : 'Failed');
    }
  }

  const loadOrders = () => {
    if (activeReservation) {
      setLoadingO(true);
      fetch(`${apiBaseUrl}/room/orders`)
        .then(r => r.ok ? r.json() : [])
        .then(setOrders)
        .catch(() => {})
        .finally(() => setLoadingO(false));
    }
  };

  const loadMaintenance = () => {
    if (activeReservation) {
      setLoadingM(true);
      fetchMaintenanceTickets()
        .then(setMaintenance)
        .catch(() => {})
        .finally(() => setLoadingM(false));
    }
  };

  async function confirmCancel(id: number) {
    setCancelTarget(null);
    setCancelStatus('');
    try {
      const res = await fetch(`${apiBaseUrl}/reception/bookings/${id}/cancel`, { method: 'POST' });
      if (!res.ok) throw new Error('Failed to cancel');
      setCancelStatus('Booking cancelled — 50% refund will be processed.');
      setReservations((prev) => prev.map((r) => r.id === id ? { ...r, status: 'Cancelled' } : r));
    } catch (err) {
      setCancelStatus(err instanceof Error ? err.message : 'Failed');
    }
  }

  async function submitMaintenance(e: React.FormEvent) {
    e.preventDefault();
    setMaintStatus('');
    try {
      const token = (window as unknown as Record<string, unknown>).__hotelos_auth_store__ as
        | { getState(): { accessToken?: string } }
        | undefined;
      const accessToken = token?.getState().accessToken ?? '';
      const res = await fetch(`${apiBaseUrl}/maintenance`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
        },
        body: JSON.stringify({ roomNumber: maintRoom, title: maintTitle, description: maintDesc, priority: 'Medium' }),
      });
      if (!res.ok) throw new Error('Failed to submit request');
      setMaintStatus('Maintenance request submitted!');
      setMaintRoom('');
      setMaintTitle('');
      setMaintDesc('');
      loadMaintenance();
    } catch (err) {
      setMaintStatus(err instanceof Error ? err.message : 'Failed');
    }
  }

  if (!user) {
    return (
      <section className="container mx-auto max-w-xl px-4 py-28 text-center">
        <p className="text-sm uppercase tracking-[0.35em] text-accent">HotelOS</p>
        <h1 className="h2 mt-3">Profile</h1>
        <p className="mt-6 text-primary/50">Please log in to view your profile.</p>
        <a href="/login" className="btn btn-primary mt-6 inline-block">Sign In</a>
      </section>
    );
  }

  const tabs: { key: typeof activeTab; label: string }[] = [
    { key: 'profile', label: 'Profile' },
    { key: 'reservations', label: 'My Reservations' },
    { key: 'orders', label: 'My Orders' },
    { key: 'maintenance', label: 'Maintenance' },
  ];

  return (
    <section className="container mx-auto max-w-4xl px-4 py-28">
      <div className="mb-8 text-center">
        <p className="text-sm uppercase tracking-[0.35em] text-accent">HotelOS</p>
        <h1 className="h2 mt-3">Customer Portal</h1>
      </div>

      <div className="flex gap-2 mb-8 overflow-x-auto pb-2">
        {tabs.map(tab => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            className={`px-5 py-2.5 text-sm font-medium whitespace-nowrap transition border ${
              activeTab === tab.key
                ? 'bg-accent text-white border-accent'
                : 'bg-white text-primary/70 border-primary/10 hover:border-accent'
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {activeTab === 'profile' && (
        <div className="border border-primary/10 bg-white p-8 shadow-sm">
          <div className="mb-6 flex items-center gap-4">
            <div className="flex h-16 w-16 items-center justify-center rounded-full bg-accent text-xl font-bold text-white">
              {user.displayName.charAt(0).toUpperCase()}
            </div>
            <div>
              <h2 className="text-lg font-semibold">{user.displayName}</h2>
              <p className="text-sm text-primary/50">{user.email}</p>
            </div>
          </div>
          <div className="space-y-4 border-t border-primary/10 pt-6">
            <div className="flex justify-between">
              <span className="text-sm text-primary/50">Role</span>
              <span className="text-sm font-medium">{user.role}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-sm text-primary/50">Active Room</span>
              <span className="text-sm font-medium text-accent">{activeReservation?.roomNumber ?? '—'}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-sm text-primary/50">Permissions</span>
              <span className="text-sm font-medium">{user.permissions.length}</span>
            </div>
          </div>
          <div className="mt-8 space-y-3">
            <a href="/favorites" className="btn btn-secondary w-full text-center">My Favorites</a>
            <a href="/checkout" className="btn btn-secondary w-full text-center">New Booking</a>
            <button onClick={handleLogout} className="btn btn-primary w-full">Sign Out</button>
          </div>
        </div>
      )}

      {activeTab === 'reservations' && (
        <div className="border border-primary/10 bg-white p-8 shadow-sm">
          <h2 className="text-lg font-semibold mb-6">My Reservations</h2>
          {loadingR ? (
            <p className="text-primary/50">Loading...</p>
          ) : reservations.length === 0 ? (
            <div className="text-center py-8">
              <p className="text-primary/50 mb-4">No reservations yet.</p>
              <a href="/" className="btn btn-primary">Browse Rooms</a>
            </div>
          ) : (
            <div className="space-y-4">
              {reservations.map(res => {
                const canCancel = res.status === 'Confirmed' || res.status === 'HELD';
                return (
                <div key={res.id} className="border border-primary/10 p-4 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3">
                  <div>
                    <p className="font-semibold">Room {res.roomNumber}</p>
                    <p className="text-sm text-primary/50">{res.checkInDate} → {res.checkOutDate} · {res.nights} nights</p>
                  </div>
                  <div className="flex items-center gap-3">
                    <span className={`px-3 py-1 rounded-full text-xs font-semibold ${
                      res.status === 'Confirmed' ? 'bg-emerald-100 text-emerald-700' :
                      res.status === 'HELD' ? 'bg-amber-100 text-amber-700' :
                      res.status === 'EXPIRED' ? 'bg-red-100 text-red-700' :
                      'bg-gray-100 text-gray-700'
                    }`}>
                      {res.status}
                    </span>
                    <span className="font-bold">${res.total}</span>
                    {canCancel && (
                      <button onClick={() => setCancelTarget(res.id)} className="rounded-lg border border-red-200 px-3 py-1.5 text-xs font-medium text-red-600 hover:bg-red-50">
                        Cancel
                      </button>
                    )}
                  </div>
                </div>
              );})}
              {cancelStatus && <p className="text-center text-sm text-emerald-700">{cancelStatus}</p>}
            </div>
          )}

          {cancelTarget !== null && (
            <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40" onClick={() => setCancelTarget(null)}>
              <div className="mx-4 w-full max-w-md rounded-2xl bg-white p-8 shadow-2xl" onClick={(e) => e.stopPropagation()}>
                <h3 className="text-lg font-bold text-primary">Cancel Booking?</h3>
                <p className="mt-3 text-sm text-primary/70">
                  Only <strong>50% of the total amount</strong> will be refunded to you. This action cannot be undone.
                </p>
                <div className="mt-6 flex justify-end gap-3">
                  <button
                    onClick={() => setCancelTarget(null)}
                    className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5"
                  >
                    Keep Booking
                  </button>
                  <button
                    onClick={() => confirmCancel(cancelTarget)}
                    className="rounded-lg bg-red-600 px-5 py-2.5 text-sm font-medium text-white hover:bg-red-700"
                  >
                    Yes, Cancel — 50% Refund
                  </button>
                </div>
              </div>
            </div>
          )}
        </div>
      )}

      {activeTab === 'orders' && !activeReservation && (
        <div className="border border-primary/10 bg-white p-8 shadow-sm text-center">
          <p className="text-primary/50 mb-4">You need an active booking to place food orders.</p>
          <a href="/rooms" className="btn btn-primary">Book a Room</a>
        </div>
      )}

      {activeTab === 'maintenance' && !activeReservation && (
        <div className="border border-primary/10 bg-white p-8 shadow-sm text-center">
          <p className="text-primary/50 mb-4">You need an active booking to submit maintenance requests.</p>
          <a href="/rooms" className="btn btn-primary">Book a Room</a>
        </div>
      )}

      {activeTab === 'orders' && activeReservation && (
        <div className="space-y-6">
          <div className="border border-primary/10 bg-white p-8 shadow-sm">
            <h2 className="text-lg font-semibold mb-4">Place Food Order — Room #{activeReservation.roomNumber}</h2>
            <form onSubmit={placeOrder} className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm uppercase tracking-[0.25em] text-primary/50">Room Number</label>
                  {reservations.length > 0 ? (
                    <select
                      className="mt-2 w-full border border-primary/10 bg-white px-4 py-3 outline-none text-primary focus:border-accent"
                      value={orderRoom}
                      onChange={(e) => setOrderRoom(e.target.value)}
                      required
                    >
                      {reservations.map((reservation) => (
                        <option key={reservation.id} value={reservation.roomNumber}>
                          {reservation.roomNumber}
                        </option>
                      ))}
                    </select>
                  ) : (
                    <input
                      className="mt-2 w-full border border-primary/10 bg-gray-50 px-4 py-3 outline-none text-primary/60"
                      value={orderRoom}
                      readOnly
                    />
                  )}
                </div>
                <label className="block text-sm uppercase tracking-[0.25em] text-primary/50">
                  Item
                  <select className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent"
                    value={orderItem} onChange={e => setOrderItem(e.target.value)}>
                    <option>Breakfast set</option>
                    <option>Club sandwich</option>
                    <option>Sparkling water</option>
                  </select>
                </label>
              </div>
              <button type="submit" className="btn btn-primary w-full">Place Order</button>
              {orderStatus && <p className="text-center text-sm text-emerald-700">{orderStatus}</p>}
            </form>
          </div>

          <div className="border border-primary/10 bg-white p-8 shadow-sm">
            <h2 className="text-lg font-semibold mb-6">My Orders</h2>
            {loadingO ? <p className="text-primary/50">Loading...</p> : orders.length === 0 ? (
              <p className="text-primary/50">No orders yet.</p>
            ) : (
              <div className="space-y-3">
                {orders.map(order => (
                  <div key={order.id} className="flex justify-between items-center border border-primary/10 p-4">
                    <div>
                      <p className="font-semibold">Room {order.roomNumber}</p>
                      <p className="text-sm text-primary/50">{order.guestName}</p>
                    </div>
                    <div className="flex items-center gap-3">
                      <span className={`px-3 py-1 rounded-full text-xs font-semibold ${
                        order.status === 'Delivered' ? 'bg-emerald-100 text-emerald-700' :
                        order.status === 'Preparing' ? 'bg-amber-100 text-amber-700' :
                        order.status === 'Ready' ? 'bg-blue-100 text-blue-700' :
                        'bg-gray-100 text-gray-700'
                      }`}>
                        {order.status}
                      </span>
                      <span className="font-bold">${order.total}</span>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}

      {activeTab === 'maintenance' && activeReservation && (
        <div className="space-y-6">
          <div className="border border-primary/10 bg-white p-8 shadow-sm">
            <h2 className="text-lg font-semibold mb-4">Report Issue — Room #{activeReservation.roomNumber}</h2>
            <form onSubmit={submitMaintenance} className="space-y-4">
              <div>
                <label className="block text-sm uppercase tracking-[0.25em] text-primary/50">Room Number</label>
                {reservations.length > 0 ? (
                  <select
                    className="mt-2 w-full border border-primary/10 bg-white px-4 py-3 outline-none text-primary focus:border-accent"
                    value={maintRoom}
                    onChange={(e) => setMaintRoom(e.target.value)}
                    required
                  >
                    {reservations.map((reservation) => (
                      <option key={reservation.id} value={reservation.roomNumber}>
                        {reservation.roomNumber}
                      </option>
                    ))}
                  </select>
                ) : (
                  <input className="mt-2 w-full border border-primary/10 bg-gray-50 px-4 py-3 outline-none text-primary/60"
                    value={maintRoom} readOnly />
                )}
              </div>
              <label className="block text-sm uppercase tracking-[0.25em] text-primary/50">
                Issue Title
                <input className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent"
                  value={maintTitle} onChange={e => setMaintTitle(e.target.value)} placeholder="AC not working" required />
              </label>
              <label className="block text-sm uppercase tracking-[0.25em] text-primary/50">
                Description
                <textarea className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent" rows={3}
                  value={maintDesc} onChange={e => setMaintDesc(e.target.value)} placeholder="Describe the issue..." />
              </label>
              <button type="submit" className="btn btn-primary w-full">Submit Request</button>
              {maintStatus && <p className="text-center text-sm text-emerald-700">{maintStatus}</p>}
            </form>
          </div>

          <div className="border border-primary/10 bg-white p-8 shadow-sm">
            <h2 className="text-lg font-semibold mb-6">Maintenance Requests</h2>
            {loadingM ? <p className="text-primary/50">Loading...</p> : maintenance.length === 0 ? (
              <p className="text-primary/50">No requests.</p>
            ) : (
              <div className="space-y-3">
                {maintenance.map(item => (
                  <div key={item.id} className="flex justify-between items-center border border-primary/10 p-4">
                    <div>
                      <p className="font-semibold">{item.title}</p>
                      <p className="text-sm text-primary/50">Room {item.roomNumber}</p>
                    </div>
                    <div className="flex items-center gap-3">
                      <span className={`px-3 py-1 rounded-full text-xs font-semibold ${
                        item.priority === 'Critical' ? 'bg-red-100 text-red-700' :
                        item.priority === 'High' ? 'bg-orange-100 text-orange-700' :
                        'bg-blue-100 text-blue-700'
                      }`}>
                        {item.priority}
                      </span>
                      <span className="text-sm">{item.status}</span>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}
    </section>
  );
}
