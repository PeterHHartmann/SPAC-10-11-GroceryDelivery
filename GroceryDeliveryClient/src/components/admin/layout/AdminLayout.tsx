import { ReactNode } from 'react';
import AdminSidebar from './AdminSidebar.tsx';
import AdminHeader from './AdminHeader.tsx';

interface AdminLayoutProps {
  children: ReactNode;
}

export default function AdminLayout({ children }: AdminLayoutProps) {
  return (
    <div className="min-h-screen bg-gray-100">
      {/* Sidebar */}
      <AdminSidebar />
      
      {/* Main Content */}
      <div className="lg:pl-64">
        {/* Header */}
        <AdminHeader />
        
        {/* Page Content */}
        <main className="p-6">
          {children}
        </main>
      </div>
    </div>
  );
} 