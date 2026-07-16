import type { Metadata } from 'next';
import { LoginForm } from '@/features/auth';
import { messages } from '@/lib/messages.ar';

export const metadata: Metadata = {
  title: messages.auth.loginTitle,
};

export default function LoginPage() {
  return <LoginForm />;
}
