import { DashboardPage, getDashboardStats } from '@/features/dashboard';

export const metadata = {
  title: 'لوحة التحكم',
};

export default async function DashboardRoutePage() {
  const stats = await getDashboardStats().catch(() => null);
  return <DashboardPage stats={stats} />;
}
