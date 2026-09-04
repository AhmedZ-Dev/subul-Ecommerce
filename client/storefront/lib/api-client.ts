import axios from 'axios';

const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5101/api',
  headers: {
    'Content-Type': 'application/json',
  },
  // ASP.NET binds a List<T> from repeated keys (`brandIds=1&brandIds=2`).
  // Axios defaults to `brandIds[]=1`, which the model binder ignores outright —
  // the filter then silently does nothing instead of failing.
  paramsSerializer: { indexes: null },
});

export default apiClient;
