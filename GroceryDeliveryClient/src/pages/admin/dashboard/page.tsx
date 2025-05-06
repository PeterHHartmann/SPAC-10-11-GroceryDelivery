import AdminLayout from '../../../components/admin/layout/AdminLayout';

export default function AdminDashboard() {
  return (
    <AdminLayout>
      <div className="space-y-6">
        <h1 className="text-2xl font-semibold text-gray-900">Dashboard</h1>
        
        {/* Stats Grid */}
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
          {/* Total Orders */}
          <div className="p-6 bg-white rounded-lg shadow">
            <h3 className="text-sm font-medium text-gray-500">Total Orders</h3>
            <p className="mt-2 text-3xl font-semibold text-gray-900">0</p>
          </div>

          {/* Pending Orders */}
          <div className="p-6 bg-white rounded-lg shadow">
            <h3 className="text-sm font-medium text-gray-500">Pending Orders</h3>
            <p className="mt-2 text-3xl font-semibold text-gray-900">0</p>
          </div>

          {/* Total Products */}
          <div className="p-6 bg-white rounded-lg shadow">
            <h3 className="text-sm font-medium text-gray-500">Total Products</h3>
            <p className="mt-2 text-3xl font-semibold text-gray-900">0</p>
          </div>

          {/* Total Users */}
          <div className="p-6 bg-white rounded-lg shadow">
            <h3 className="text-sm font-medium text-gray-500">Total Users</h3>
            <p className="mt-2 text-3xl font-semibold text-gray-900">0</p>
          </div>
        </div>

        {/* Recent Orders */}
        <div className="bg-white rounded-lg shadow">
          <div className="p-6">
            <h2 className="text-lg font-medium text-gray-900">Recent Orders</h2>
            <div className="mt-6">
              <p className="text-sm text-gray-500">No recent orders</p>
            </div>
          </div>
        </div>
      </div>
    </AdminLayout>
  );
} 