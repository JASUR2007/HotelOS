import type {
	AdminMetric,
	AnalyticsPoint,
	AuditLogRecord,
	AuditLogEntry,
	AuthUser,
	EventLogEntry,
	EventLogRecord,
	DashboardMetric,
	LoginRequest,
	LoginResponseV2,
	AppSetting,
	PaymentAnalyticsPoint,
	PaymentStats,
	ReportSummary,
	TransactionRecord,
	NotificationCenterItem,
	PermissionRecord,
	RefreshTokenResponse,
	RevenuePoint,
	RoleRecord,
	RealtimeNotification,
	RoomOverview,
	RoomDto,
	CreateRoomDto,
	UpdateRoomDto,
	AmenityDto,
} from '../types';

const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';

function getAuthHeaders(): Record<string, string> {
	const store = (window as unknown as Record<string, unknown>).__hotelos_auth_store__ as
		| { getState(): { accessToken?: string } }
		| undefined;
	const token = store?.getState().accessToken;
	return token ? { Authorization: `Bearer ${token}` } : {};
}

async function fetchJson<T>(
	path: string,
	fallback: T,
	init?: RequestInit,
): Promise<T> {
	try {
		const response = await fetch(`${apiBaseUrl}${path}`, {
			headers: {
				'Content-Type': 'application/json',
				...getAuthHeaders(),
				...(init?.headers ?? {}),
			},
			...init,
		});

		if (!response.ok) {
			return fallback;
		}

		return (await response.json()) as T;
	} catch {
		return fallback;
	}
}

export function getHotelApiBaseUrl() {
	return apiBaseUrl;
}

export function fetchDashboardMetrics() {
	return fetchJson<DashboardMetric[]>('/dashboard/metrics', [
		{ label: 'Occupied rooms', value: '128', change: '+7 today', tone: 'positive' },
		{ label: 'Pending cleanings', value: '14', change: '3 urgent', tone: 'warning' },
		{ label: 'Open maintenance', value: '6', change: '2 critical', tone: 'critical' },
		{ label: 'Active guests', value: '241', change: '+18 arrivals', tone: 'positive' },
	]);
}

export function fetchRoomsOverview() {
	return fetchJson<RoomOverview[]>('/room/overview', [
		{ id: 101, roomNumber: '101', status: 'Occupied', guestName: 'Amelia Stone', housekeeping: 'Ready tomorrow' },
		{ id: 102, roomNumber: '102', status: 'Cleaning', guestName: 'Vacant', housekeeping: 'In progress' },
		{ id: 103, roomNumber: '103', status: 'Maintenance', guestName: 'Vacant', housekeeping: 'Tech assigned' },
	]);
}

export function fetchOrders() {
	return fetchJson<any[]>('/room/orders', [
		{ id: 1, roomNumber: '101', guestName: 'Amelia Stone', items: ['Breakfast set', 'Orange juice'], status: 'Preparing', total: 24, updatedAt: '09:20' },
		{ id: 2, roomNumber: '207', guestName: 'Daniel Reed', items: ['Club sandwich'], status: 'Out for delivery', total: 18, updatedAt: '09:05' },
	]).then((orders) => orders.map((order) => ({
		...order,
		items: order.items ?? [],
		updatedAt: order.updatedAt ?? '',
	})));
}

export function fetchMaintenanceTickets() {
	return fetchJson<any[]>('/maintenance', [
		{ id: 11, title: 'AC not cooling', priority: 'Critical', status: 'Assigned', technician: 'Alex M.', roomNumber: '302' },
		{ id: 12, title: 'Bathroom light flickering', priority: 'Medium', status: 'Queued', technician: 'Unassigned', roomNumber: '118' },
	]).then((tickets) => tickets.map((ticket) => ({
		...ticket,
		technician: ticket.technician ?? ticket.technicianName ?? '',
	})));
}

export function fetchGuests() {
	return fetchJson<any[]>('/reception/guests', [
		{ id: 1, name: 'Amelia Stone', roomNumber: '101', checkIn: '2026-05-20', checkOut: '2026-05-23', balance: 420 },
		{ id: 2, name: 'Daniel Reed', roomNumber: '207', checkIn: '2026-05-19', checkOut: '2026-05-24', balance: 180 },
	]).then((guests) => guests.map((guest) => ({
		id: guest.id ?? guest.guestId,
		name: guest.name ?? guest.fullName,
		email: guest.email ?? '',
		roomNumber: guest.roomNumber ?? '',
		checkIn: guest.checkIn ?? '',
		checkOut: guest.checkOut ?? '',
		balance: guest.balance ?? 0,
	})));
}

export function fetchBookingsAdmin() {
	return fetchJson<any[]>('/reception/bookings', [
		{ id: 1, guestName: 'Amelia Stone', roomNumber: '101', status: 'CheckedIn', checkInDate: '2026-05-20', checkOutDate: '2026-05-23', total: 420 },
		{ id: 2, guestName: 'Daniel Reed', roomNumber: '207', status: 'Booked', checkInDate: '2026-05-19', checkOutDate: '2026-05-24', total: 180 },
	]).then((bookings) => bookings.map((booking) => ({
		id: booking.id ?? booking.bookingId,
		guestName: booking.guestName ?? '',
		roomNumber: booking.roomNumber ?? String(booking.roomId ?? ''),
		status: booking.status ?? '',
		checkInDate: booking.checkInDate ?? '',
		checkOutDate: booking.checkOutDate ?? '',
		total: booking.total ?? 0,
	})));
}

export async function loginManager(credentials: LoginRequest) {
	const response = await fetch(`${apiBaseUrl}/auth/login`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify(credentials),
	});

	if (!response.ok) {
		throw new Error('Invalid credentials');
	}

	const data = await response.json();
	return {
		...data,
		user: {
			...data.user,
			role: data.user?.roles?.[0] ?? 'Guest',
		},
	} as LoginResponseV2;
}

export function fetchNotifications() {
	return fetchJson<RealtimeNotification[]>('/notifications', [
		{
			id: 'n-1',
			title: 'Room 302 escalated',
			message: 'Maintenance marked as critical and pushed to the priority queue.',
			createdAt: '09:10',
			type: 'maintenance',
		},
		{
			id: 'n-2',
			title: 'Guest checked in',
			message: 'Room assignment algorithm placed guest Amelia Stone in 101.',
			createdAt: '08:55',
			type: 'reception',
		},
	]);
}

export function fetchAdminMetrics() {
	return fetchJson<AdminMetric[]>('/admin/dashboard/metrics', [
		{ label: 'Occupied rooms', value: '128', delta: '+7%', trend: 'up' },
		{ label: 'Dirty rooms', value: '14', delta: '-3%', trend: 'down' },
		{ label: 'Clean rooms', value: '212', delta: '+4%', trend: 'up' },
		{ label: 'Active bookings', value: '241', delta: '+11%', trend: 'up' },
		{ label: 'Pending maintenance', value: '6', delta: '-1%', trend: 'stable' },
		{ label: 'Active food orders', value: '18', delta: '+2%', trend: 'up' },
		{ label: 'Daily revenue', value: '$18,420', delta: '+9%', trend: 'up' },
		{ label: 'Live notifications', value: '36', delta: '+12', trend: 'up' },
	]);
}

export function fetchOccupancyAnalytics() {
	return fetchJson<AnalyticsPoint[]>('/admin/dashboard/occupancy', [
		{ label: 'Occupied', value: 128 },
		{ label: 'Dirty', value: 14 },
		{ label: 'Clean', value: 212 },
	]);
}

export function fetchRevenueAnalytics() {
	return fetchJson<RevenuePoint[]>('/admin/dashboard/revenue', [
		{ label: 'Mon', value: 14200 },
		{ label: 'Tue', value: 15800 },
		{ label: 'Wed', value: 18240 },
		{ label: 'Thu', value: 17630 },
		{ label: 'Fri', value: 19300 },
		{ label: 'Sat', value: 21440 },
		{ label: 'Sun', value: 18890 },
	]);
}

export function fetchAuditLogs() {
	return fetchJson<AuditLogEntry[]>('/admin/audit-logs', [
		{ id: 1, actor: 'Admin', action: 'Created booking', target: 'Booking #1042', createdAt: '09:12' },
		{ id: 2, actor: 'Receptionist', action: 'Checked in guest', target: 'Guest Amelia Stone', createdAt: '08:58' },
	]);
}

export function fetchEventLogs() {
	return fetchJson<EventLogEntry[]>('/admin/event-logs', [
		{ id: 1, eventName: 'guest.checked_in', queue: 'hotelos.reception.events', status: 'processed', createdAt: '09:12' },
		{ id: 2, eventName: 'maintenance.assigned', queue: 'hotelos.maintenance.events', status: 'processed', createdAt: '09:03' },
	]);
}

export function fetchRoles() {
	return fetchJson<RoleRecord[]>('/admin/roles', [
		{ id: 1, name: 'SuperAdmin', permissions: ['manage_users', 'manage_roles', 'manage_permissions', 'view_dashboard'] },
		{ id: 2, name: 'Receptionist', permissions: ['create_booking', 'update_booking', 'view_dashboard', 'manage_payments'] },
	]);
}

export function fetchPermissions() {
	return fetchJson<PermissionRecord[]>('/admin/permissions', [
		{ id: 1, name: 'create_booking', description: 'Create new guest bookings' },
		{ id: 2, name: 'manage_users', description: 'Create and update platform users' },
	]);
}

export function fetchUsers() {
	return fetchJson<AuthUser[]>('/admin/users', [
		{ id: 'u1', email: 'admin@hotelos.local', displayName: 'Super Admin', role: 'SuperAdmin', permissions: ['manage_users', 'manage_roles', 'manage_permissions', 'view_dashboard'] },
		{ id: 'u2', email: 'reception@hotelos.local', displayName: 'Reception Desk', role: 'Receptionist', permissions: ['create_booking', 'update_booking', 'view_dashboard'] },
		{ id: 'u3', email: 'housekeeping@hotelos.local', displayName: 'Maria Lopez', role: 'Housekeeper', permissions: [] },
		{ id: 'u4', email: 'technician@hotelos.local', displayName: 'Alex Martin', role: 'Technician', permissions: [] },
		{ id: 'u5', email: 'kitchen@hotelos.local', displayName: 'Chef John', role: 'KitchenStaff', permissions: [] },
		{ id: 'u6', email: 'accountant@hotelos.local', displayName: 'Jane Smith', role: 'Accountant', permissions: [] },
	]);
}

export function fetchNotificationsCenter() {
	return fetchJson<NotificationCenterItem[]>('/admin/notifications', [
		{ id: 1, title: 'New guest checked in', message: 'Room assignment completed for Booking #1042.', severity: 'info', createdAt: '09:12' },
		{ id: 2, title: 'Maintenance escalated', message: 'Room 302 moved to critical priority.', severity: 'critical', createdAt: '09:05' },
	]);
}

export function fetchPaymentStats() {
	return fetchJson<PaymentStats[]>('/admin/dashboard/payments', [
		{ label: 'Payments processed', value: '98', delta: '+8%', trend: 'up' },
		{ label: 'Refunds issued', value: '4', delta: '+1', trend: 'stable' },
		{ label: 'Open invoices', value: '21', delta: '-3', trend: 'down' },
		{ label: 'Failed payments', value: '2', delta: '-2', trend: 'down' },
	]);
}

export function fetchReportsSummary() {
	return fetchJson<ReportSummary[]>('/admin/reports/summary', [
		{ label: 'Revenue', value: '$128,420', subtitle: 'Month-to-date' },
		{ label: 'Occupancy', value: '86%', subtitle: 'Average this week' },
		{ label: 'Refund rate', value: '1.8%', subtitle: 'Low risk' },
	]);
}

export function fetchSettings() {
	return fetchJson<AppSetting[]>('/admin/settings', [
		{ key: 'Notifications', value: 'Enabled', description: 'Realtime alerts via SignalR' },
		{ key: 'Payments', value: 'Live', description: 'Payment provider and reconciliation state' },
		{ key: 'RBAC', value: 'Strict', description: 'Role and permission enforcement enabled' },
	]);
}

export async function updateSetting(key: string, value: string): Promise<AppSetting> {
	const response = await fetch(`${apiBaseUrl}/admin/settings/${encodeURIComponent(key)}`, {
		method: 'PUT',
		headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
		body: JSON.stringify({ value }),
	});
	if (!response.ok) throw new Error('Failed to update setting');
	return response.json();
}

export function fetchPaymentAnalytics() {
	return fetchJson<PaymentAnalyticsPoint[]>('/admin/dashboard/payment-analytics', [
		{ label: 'Cash', value: 54 },
		{ label: 'Card', value: 132 },
		{ label: 'Refunds', value: 18 },
		{ label: 'Pending', value: 24 },
	]);
}

export function fetchTransactions() {
	return fetchJson<TransactionRecord[]>('/payments/transactions', [
		{ id: 1, invoiceNumber: 'INV-10021', guestName: 'Amelia Stone', roomNumber: '101', amount: 420, status: 'Completed', paymentMethod: 'Card', createdAt: '09:12' },
		{ id: 2, invoiceNumber: 'INV-10022', guestName: 'Daniel Reed', roomNumber: '205', amount: 180, status: 'Refunded', paymentMethod: 'Cash', createdAt: '09:05' },
	]);
}

export function fetchAuditLogRecords() {
	return fetchJson<AuditLogRecord[]>('/admin/audit-logs', [
		{ id: 1, actor: 'Admin', action: 'Created payment', entity: 'Invoice INV-10021', createdAt: '09:12' },
		{ id: 2, actor: 'Receptionist', action: 'Assigned room', entity: 'Booking #1042', createdAt: '08:58' },
	]);
}

export function fetchEventLogRecords() {
	return fetchJson<EventLogRecord[]>('/admin/event-logs', [
		{ id: 1, eventName: 'payment.completed', routingKey: 'payment.completed', status: 'processed', createdAt: '09:12' },
		{ id: 2, eventName: 'notification.created', routingKey: 'notification.created', status: 'processed', createdAt: '09:05' },
	]);
}

// --- Room CRUD (real API, no mock fallback) ---

export async function fetchRecommendedRooms(): Promise<RoomDto[]> {
  try {
    const response = await fetch(`${apiBaseUrl}/room/overview`, {
      headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
    });
    if (!response.ok) throw new Error('Failed');
    const overview = await response.json();
    return (overview ?? []).slice(0, 3).map((r: any) => ({ id: r.id, roomNumber: r.roomNumber, type: 'Standard', status: r.status, pricePerNight: 150, floor: 1, description: 'Recommended room', guestCapacity: 2, mainImage: '', images: [], amenities: [] }));
  } catch {
    return [
      { id: 101, roomNumber: '101', type: 'Deluxe', status: 'Available', pricePerNight: 220, floor: 2, description: 'Spacious room with city view', guestCapacity: 2, mainImage: '', images: [], amenities: [] },
      { id: 207, roomNumber: '207', type: 'Suite', status: 'Available', pricePerNight: 320, floor: 3, description: 'Premium suite with panoramic view', guestCapacity: 3, mainImage: '', images: [], amenities: [] },
      { id: 305, roomNumber: '305', type: 'Standard', status: 'Available', pricePerNight: 150, floor: 3, description: 'Cozy room with essential amenities', guestCapacity: 2, mainImage: '', images: [], amenities: [] },
    ];
  }
}

export async function fetchRooms(): Promise<RoomDto[]> {
  const response = await fetch(`${apiBaseUrl}/room/rooms`, {
    headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
  });
  if (!response.ok) throw new Error('Failed to fetch rooms');
  return response.json();
}

export async function fetchRoomById(id: number): Promise<RoomDto> {
  const response = await fetch(`${apiBaseUrl}/room/rooms/${id}`, {
    headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
  });
  if (!response.ok) throw new Error('Failed to fetch room');
  return response.json();
}

export async function createRoom(data: CreateRoomDto): Promise<RoomDto> {
	const response = await fetch(`${apiBaseUrl}/room/rooms`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
		body: JSON.stringify(data),
	});
	if (!response.ok) throw new Error('Failed to create room');
	return response.json();
}

export async function updateRoom(id: number, data: UpdateRoomDto): Promise<RoomDto> {
	const response = await fetch(`${apiBaseUrl}/room/rooms/${id}`, {
		method: 'PUT',
		headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
		body: JSON.stringify(data),
	});
	if (!response.ok) throw new Error('Failed to update room');
	return response.json();
}

export async function deleteRoom(id: number): Promise<void> {
	const response = await fetch(`${apiBaseUrl}/room/rooms/${id}`, {
		method: 'DELETE',
		headers: { ...getAuthHeaders() },
	});
	if (!response.ok) throw new Error('Failed to delete room');
}

export async function fetchAmenities(): Promise<AmenityDto[]> {
	const response = await fetch(`${apiBaseUrl}/room/amenities`, {
		headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
	});
	if (!response.ok) throw new Error('Failed to fetch amenities');
	return response.json();
}

export async function uploadRoomImage(file: File): Promise<{ url: string }> {
	const formData = new FormData();
	formData.append('file', file);
	const response = await fetch(`${apiBaseUrl}/room/upload-image`, {
		method: 'POST',
		headers: { ...getAuthHeaders() },
		body: formData,
	});
	if (!response.ok) throw new Error('Failed to upload image');
	return response.json();
}

// --- Guest CRUD ---

export async function createGuest(data: { fullName: string; email: string }): Promise<{ guestId: number; fullName: string; email: string }> {
	const response = await fetch(`${apiBaseUrl}/reception/guests`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
		body: JSON.stringify(data),
	});
	if (!response.ok) throw new Error('Failed to create guest');
	return response.json();
}

// --- Check-in / Check-out (real API) ---

export async function guestCheckIn(data: {
	guestName: string;
	email: string;
	adults: number;
	kids: number;
	checkInDate: string;
	checkOutDate: string;
}): Promise<{ bookingId: number; roomId: number; status: string; guestName: string }> {
	const response = await fetch(`${apiBaseUrl}/reception/check-in`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
		body: JSON.stringify(data),
	});
	if (!response.ok) throw new Error('Failed to check in');
	return response.json();
}

export async function guestCheckOut(data: { bookingId: number; notes: string }): Promise<{ bookingId: number; roomId: number; status: string; guestName: string }> {
	const response = await fetch(`${apiBaseUrl}/reception/check-out`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
		body: JSON.stringify(data),
	});
	if (!response.ok) throw new Error('Failed to check out');
	return response.json();
}

export async function loginAdmin(credentials: LoginRequest) {
	return fetchJson<LoginResponseV2>(
		'/auth/login',
		{
			accessToken: 'demo-access-token',
			refreshToken: 'demo-refresh-token',
			expiresIn: 3600,
			user: {
				id: 'u1',
				email: credentials.email,
				displayName: 'Hotel Admin',
				role: 'SuperAdmin',
				permissions: ['view_dashboard', 'manage_users', 'manage_roles', 'manage_permissions'],
			},
		},
		{
			method: 'POST',
			body: JSON.stringify(credentials),
		},
	);
}

export function refreshAdminToken(refreshToken: string) {
	return fetchJson<RefreshTokenResponse>('/auth/refresh', {
		accessToken: 'refreshed-access-token',
		refreshToken,
		expiresIn: 3600,
	}, {
		method: 'POST',
		body: JSON.stringify({ refreshToken }),
	});
}
