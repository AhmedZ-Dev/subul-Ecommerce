'use client';

import { useEffect } from 'react';
import { useSession } from 'next-auth/react';
import { setAxiosToken } from '@/lib/api-client';

export function TokenSync() {
  const { data: session } = useSession();

  useEffect(() => {
    setAxiosToken(session?.user?.accessToken ?? null);
  }, [session]);

  return null;
}
