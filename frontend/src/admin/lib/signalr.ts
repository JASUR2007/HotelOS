import { HubConnectionBuilder, LogLevel, IHttpConnectionOptions } from '@microsoft/signalr';

function getAccessToken(): string {
  const store = (window as unknown as Record<string, unknown>).__hotelos_auth_store__ as
    | { getState(): { accessToken?: string } }
    | undefined;
  return store?.getState().accessToken ?? '';
}

export function hasValidAccessToken() {
  const token = getAccessToken();
  return token.length > 0 && token.startsWith('eyJ');
}

export function createHotelHubConnection() {
  const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';
  const hubUrl = `${apiBaseUrl.replace('/api', '/hubs')}/notifications`;
  const options: IHttpConnectionOptions = {
    accessTokenFactory: () => getAccessToken(),
  };
  return new HubConnectionBuilder()
    .withUrl(hubUrl, options)
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();
}
