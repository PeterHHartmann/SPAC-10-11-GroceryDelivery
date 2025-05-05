import AdminLayout from '../../../components/admin/layout/AdminLayout';

// Temporary type and data - we'll move this to a shared location later
interface Order {
  id: string;
  customerName: string;
  date: string;
  status: 'pending' | 'processing' | 'completed' | 'cancelled';
  total: number;
}

// Temporary mock data - same as in orders page
const orders: Order[] = [
  {
    id: 'ORD001',
    customerName: 'John Doe',
    date: '2024-03-15',
    status: 'pending',
    total: 150.00
  },
  {
    id: 'ORD002',
    customerName: 'Jane Smith',
    date: '2024-03-14',
    status: 'completed',
    total: 75.50
  }
];

export default function AdminDashboard() {
  // Calculate statistics
  const totalOrders = orders.length;
  const pendingOrders = orders.filter(order => order.status === 'pending').length;
  const completedOrders = orders.filter(order => order.status === 'completed').length;

  return (
    <AdminLayout>
      <div className="space-y-6">
        <h1 className="text-2xl font-semibold text-gray-900">Dashboard</h1>
        
        {/* Stats Grid */}
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
          {/* Total Orders */}
          <div className="p-6 bg-white rounded-lg shadow">
            <h3 className="text-sm font-medium text-gray-500">Total Orders</h3>
            <p className="mt-2 text-3xl font-semibold text-gray-900">{totalOrders}</p>
          </div>

          {/* Pending Orders */}
          <div className="p-6 bg-white rounded-lg shadow">
            <h3 className="text-sm font-medium text-gray-500">Pending Orders</h3>
            <p className="mt-2 text-3xl font-semibold text-gray-900">{pendingOrders}</p>
          </div>

          {/* Completed Orders */}
          <div className="p-6 bg-white rounded-lg shadow">
            <h3 className="text-sm font-medium text-gray-500">Completed Orders</h3>
            <p className="mt-2 text-3xl font-semibold text-gray-900">{completedOrders}</p>
          </div>

          {/* Total Products */}
          <div className="p-6 bg-white rounded-lg shadow">
            <h3 className="text-sm font-medium text-gray-500">Total Products</h3>
            <p className="mt-2 text-3xl font-semibold text-gray-900">0</p>
          </div>
        </div>

        {/* Recent Orders */}
        <div className="bg-white rounded-lg shadow">
          <div className="p-6">
            <h2 className="text-lg font-medium text-gray-900">Recent Orders</h2>
            <div className="mt-6">
              {orders.length > 0 ? (
                <div className="space-y-4">
                  {orders.map(order => (
                    <div key={order.id} className="flex items-center justify-between border-b pb-4">
                      <div>
                        <p className="text-sm font-medium text-gray-900">{order.id}</p>
                        <p className="text-sm text-gray-500">{order.customerName}</p>
                      </div>
                      <div className="text-right">
                        <p className="text-sm font-medium text-gray-900">${order.total.toFixed(2)}</p>
                        <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full 
                          ${order.status === 'completed' ? 'bg-green-100 text-green-800' : 
                            order.status === 'pending' ? 'bg-yellow-100 text-yellow-800' :
                            order.status === 'cancelled' ? 'bg-red-100 text-red-800' :
                            'bg-blue-100 text-blue-800'}`}>
                          {order.status}
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <p className="text-sm text-gray-500">No recent orders</p>
              )}
            </div>
          </div>
        </div>
      </div>
    </AdminLayout>
  );
} 