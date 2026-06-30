import axios from 'axios';
import type { ApiResponse } from '@/types/api';

const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5101/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

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
