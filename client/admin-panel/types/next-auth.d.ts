import 'next-auth';
import 'next-auth/jwt';
import type { DefaultSession } from 'next-auth';

declare module 'next-auth' {
  interface Session {
    user: {
      id: number;
      role: string;
      accessToken: string;
    } & DefaultSession['user'];
  }

  interface User {
    id: number;
    name: string;
    email: string;
    role: string;
    accessToken: string;
  }
}

declare module 'next-auth/jwt' {
  interface JWT {
    id: number;
    role: string;
    accessToken: string;
  }
}
