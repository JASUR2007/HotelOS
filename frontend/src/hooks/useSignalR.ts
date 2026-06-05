import { HubConnectionBuilder, HubConnection, LogLevel, IHttpConnectionOptions } from '@microsoft/signalr';
import { useEffect, useRef, useState } from 'react';

function getAccessToken(): string {
  const store = (window as unknown as Record<string, unknown>).__hotelos_auth_store__ as
    | { getState(): { accessToken?: string } }
    | undefined;
  return store?.getState().accessToken ?? '';
}

function isValidToken(token: string): boolean {
  return token.length > 0 && token.startsWith('eyJ');
}

interface RealtimeNotification {
	id: string;
	title: string;
	message: string;
	createdAt: string;
	type: string;
}

export function useSignalR() {
	const [notifications, setNotifications] = useState<RealtimeNotification[]>([]);
	const [connected, setConnected] = useState(false);
	const [eventLog, setEventLog] = useState<{ type: string; data: unknown }[]>([]);
	const connectionRef = useRef<HubConnection | null>(null);

	useEffect(() => {
		const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';
		const hubUrl = `${apiBaseUrl.replace('/api', '/hubs')}/notifications`;
		const options: IHttpConnectionOptions = {
			accessTokenFactory: () => getAccessToken(),
		};

		const connection = new HubConnectionBuilder()
			.withUrl(hubUrl, options)
			.withAutomaticReconnect()
			.configureLogging(LogLevel.Information)
			.build();

		connectionRef.current = connection;

		connection.on('NotificationReceived', (notification: RealtimeNotification) => {
			setNotifications((prev) => [notification, ...prev].slice(0, 50));
			setEventLog((prev) => [{ type: notification.type, data: notification }, ...prev].slice(0, 100));
		});

		const events = [
			'BookingCreated', 'BookingCancelled', 'GuestCheckedIn', 'GuestCheckedOut',
			'RoomVacated', 'RoomCleaned', 'RoomOccupied', 'RoomStatusChanged',
			'OrderCreated', 'OrderPreparing', 'OrderDelivered', 'OrderStatusChanged',
			'MaintenanceCreated', 'MaintenanceAssigned', 'MaintenanceCompleted',
			'MaintenanceTicketUpdated', 'PaymentCompleted', 'PaymentRefunded',
		];

		for (const event of events) {
			connection.on(event, (data: unknown) => {
				console.log(`[SignalR] ${event}:`, data);
				setEventLog((prev) => [{ type: event, data }, ...prev].slice(0, 100));
			});
		}

		const accessToken = getAccessToken();
		if (!isValidToken(accessToken)) {
			console.info('[SignalR] Skipping connection — no valid access token');
			return;
		}

		connection.start()
			.then(() => setConnected(true))
			.catch((err) => console.warn('[SignalR] Connection failed:', err));

		return () => {
			connection.stop();
			connectionRef.current = null;
		};
	}, []);

	return { notifications, connected, eventLog };
}
