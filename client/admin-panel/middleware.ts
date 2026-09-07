import { auth } from '@/auth';
import { NextResponse } from 'next/server';

export default auth((req) => {
  // Checked against a real claim, not the mere existence of req.auth.
  // GHSA-8fpg-xm3f-6cx3: on a configuration error Auth.js populates req.auth
  // with an error object, so `!!req.auth` is true for an unauthenticated
  // visitor and the whole admin panel fails open. A session that carries no
  // user id is not a session.
  //
  // The optional chaining is load-bearing even though Session["user"]["id"] is
  // typed as a required number: that type describes the healthy session, and
  // the failure mode this guards against is precisely the object that does not
  // match its own type.
  const isLoggedIn = req.auth?.user?.id != null;
  const isOnLogin = req.nextUrl.pathname === '/login';

  if (isOnLogin && isLoggedIn) {
    return NextResponse.redirect(new URL('/dashboard', req.url));
  }

  if (!isLoggedIn && !isOnLogin) {
    const loginUrl = new URL('/login', req.url);
    loginUrl.searchParams.set('from', req.nextUrl.pathname);
    return NextResponse.redirect(loginUrl);
  }

  return NextResponse.next();
});

export const config = {
  matcher: [
    '/((?!_next/static|_next/image|favicon.ico|api/auth|.*\\.(?:svg|png|jpg|jpeg|gif|webp|ico)$).*)',
  ],
};
