import NextAuth from 'next-auth';
import Credentials from 'next-auth/providers/credentials';

const BACKEND_API =
  process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5101/api';

export const { handlers, signIn, signOut, auth } = NextAuth({
  providers: [
    Credentials({
      credentials: {
        email: { label: 'Email', type: 'email' },
        password: { label: 'Password', type: 'password' },
      },
      async authorize(credentials) {
        const res = await fetch(`${BACKEND_API}/auth/login`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            email: credentials?.email,
            password: credentials?.password,
          }),
        });

        if (!res.ok) return null;

        const json = await res.json();
        if (!json.success) return null;

        const { accessToken, user } = json.data;
        return { ...user, accessToken };
      },
    }),
  ],

  session: { strategy: 'jwt', maxAge: 8 * 60 * 60 },

  callbacks: {
    jwt({ token, user }) {
      if (user) {
        token.accessToken = (user as { accessToken: string }).accessToken;
        token.role = (user as { role: string }).role;
        token.id = (user as { id: number }).id;
      }
      return token;
    },
    session({ session, token }) {
      return {
        ...session,
        user: {
          ...session.user,
          id: token.id as number,
          role: token.role as string,
          accessToken: token.accessToken as string,
        },
      };
    },
  },

  pages: {
    signIn: '/login',
  },
});
