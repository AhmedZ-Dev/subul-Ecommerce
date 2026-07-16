'use client';

import { useSession, signOut } from 'next-auth/react';

export function useAuth() {
  const { data: session, status } = useSession();

  return {
    currentUser: session?.user ?? null,
    isLoading: status === 'loading',
    logout: () => signOut({ callbackUrl: '/login' }),
  };
}
