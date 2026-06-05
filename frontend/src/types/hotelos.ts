export interface DashboardMetric {
  label: string;
  value: string;
  change: string;
  tone: 'positive' | 'warning' | 'critical';
}

export interface RoomOverview {
  id: number;
  roomNumber: string;
  status: string;
  guestName: string;
  housekeeping: string;
  type?: string;
  pricePerNight?: number;
  floor?: number;
  guestCapacity?: number;
}

export interface RoomDto {
  id: number;
  roomNumber: string;
  type: string;
  status: string;
  pricePerNight: number;
  floor: number;
  description: string;
  guestCapacity: number;
  mainImage: string;
  images: string[];
  amenities: string[];
}

export interface CreateRoomDto {
  roomNumber: string;
  type: string;
  floor: number;
  pricePerNight: number;
  guestCapacity: number;
  description: string;
  mainImage: string;
  images: string[];
  amenityIds: string[];
  status?: string;
}

export interface UpdateRoomDto {
  roomNumber: string;
  type: string;
  status: string;
  pricePerNight: number;
  floor: number;
  description: string;
  guestCapacity: number;
  mainImage?: string;
  images?: string[];
  amenities?: string[];
}

export interface AmenityDto {
  id: number;
  name: string;
  iconUrl: string;
  description: string;
}

export interface OrderItem {
  id: number;
  roomNumber: string;
  guestName: string;
  items: string[];
  status: string;
  total: number;
  updatedAt: string;
}

export interface MaintenanceTicket {
  id: number;
  title: string;
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  status: string;
  technician: string;
  roomNumber: string;
}

export interface GuestRecord {
  id: number;
  name: string;
  roomNumber: string;
  checkIn: string;
  checkOut: string;
  balance: number;
}

export interface BookingRecord {
  id: number;
  guestName: string;
  roomNumber: string;
  status: string;
  checkInDate: string;
  checkOutDate: string;
  total: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  role: string;
  displayName: string;
}

export interface RealtimeNotification {
  id: string;
  title: string;
  message: string;
  createdAt: string;
  type: string;
}

export interface AdminMetric {
  label: string;
  value: string;
  delta: string;
  trend: 'up' | 'down' | 'stable';
}

export interface AnalyticsPoint {
  label: string;
  value: number;
}

export interface RevenuePoint {
  label: string;
  value: number;
}

export interface NotificationCenterItem {
  id: number;
  title: string;
  message: string;
  severity: 'info' | 'warning' | 'critical';
  createdAt: string;
}

export interface PaymentStats {
  label: string;
  value: string;
  delta: string;
  trend: 'up' | 'down' | 'stable';
}

export interface ReportSummary {
  label: string;
  value: string;
  subtitle: string;
}

export interface AppSetting {
  key: string;
  value: string;
  description: string;
}

export interface TransactionRecord {
  id: number;
  invoiceNumber: string;
  guestName: string;
  roomNumber: string;
  amount: number;
  status: string;
  paymentMethod: string;
  createdAt: string;
}

export interface PaymentAnalyticsPoint {
  label: string;
  value: number;
}

export interface AuditLogRecord {
  id: number;
  actor: string;
  action: string;
  entity: string;
  createdAt: string;
}

export interface EventLogRecord {
  id: number;
  eventName: string;
  routingKey: string;
  status: string;
  createdAt: string;
}

export interface AuditLogEntry {
  id: number;
  actor: string;
  action: string;
  target: string;
  createdAt: string;
}

export interface EventLogEntry {
  id: number;
  eventName: string;
  queue: string;
  status: string;
  createdAt: string;
}

export interface RoleRecord {
  id: number;
  name: UserRole;
  permissions: PermissionName[];
}

export interface PermissionRecord {
  id: number;
  name: PermissionName;
  description: string;
}

export type UserRole =
  | 'SuperAdmin'
  | 'Admin'
  | 'Receptionist'
  | 'Housekeeper'
  | 'Technician'
  | 'KitchenStaff'
  | 'Accountant'
  | 'Guest';

export type PermissionName =
  | 'create_booking'
  | 'update_booking'
  | 'delete_booking'
  | 'manage_rooms'
  | 'manage_users'
  | 'manage_roles'
  | 'manage_permissions'
  | 'create_orders'
  | 'update_orders'
  | 'resolve_maintenance'
  | 'assign_maintenance'
  | 'view_dashboard'
  | 'view_settings'
  | 'view_maintenances'
  | 'manage_payments'
  | 'view_reports';

export interface AuthUser {
  id: string;
  email: string;
  displayName: string;
  role: UserRole;
  permissions: PermissionName[];
}

export interface LoginResponseV2 {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: AuthUser;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}