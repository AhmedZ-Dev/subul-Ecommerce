import axios from 'axios';
import type { ApiResponse } from '@/types/api';

let _accessToken: string | null = null;

export function setAxiosToken(token: string | null) {
  _accessToken = token;
}

async function resolveAccessToken(): Promise<string | null> {
  if (_accessToken) return _accessToken;

  // Server Components / SSR: TokenSync never runs, so read JWT from the NextAuth session.
  if (typeof window === 'undefined') {
    try {
      const { auth } = await import('@/auth');
      const session = await auth();
      return session?.user?.accessToken ?? null;
    } catch {
      return null;
    }
  }

  return null;
}

const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5101/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use(async (config) => {
  const token = await resolveAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    return Promise.reject(error);
  },
);

export async function postFormData<T>(path: string, formData: FormData): Promise<T> {
  const { data } = await apiClient.post<ApiResponse<T>>(path, formData, {
    transformRequest: [(payload, headers) => {
      delete headers['Content-Type'];
      return payload;
    }],
  });
  if (!data.success) throw new Error(data.message ?? 'Request failed');
  return data.data!;
}

export default apiClient;
