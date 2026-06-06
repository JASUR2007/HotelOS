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

function normalizeApiBaseUrl(baseUrl: string) {
  return baseUrl.replace(/\/api\/?$/, '/hubs');
}

export function createHotelHubConnection() {
  const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';
  const hubUrl = `${normalizeApiBaseUrl(apiBaseUrl)}/notifications`;
  const options: IHttpConnectionOptions = {
    accessTokenFactory: () => getAccessToken(),
  };
  return new HubConnectionBuilder()
    .withUrl(hubUrl, options)
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();
}
