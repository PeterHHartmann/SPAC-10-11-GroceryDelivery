import { useState, useEffect } from 'react';
import { useSearchParams, useNavigate, Outlet } from 'react-router-dom';
import AdminLayout from '../../../components/admin/layout/AdminLayout';
import { Search } from 'lucide-react';

// Temporary type until we connect to the backend
interface Order {
  id: string;
  customerName: string;
  date: string;
  status: 'pending' | 'processing' | 'completed' | 'cancelled';
  total: number;
}

export default function AdminOrdersPage() {
  const [searchParams] = useSearchParams();
  const [searchQuery, setSearchQuery] = useState('');
  const [dateRange, setDateRange] = useState({ start: '', end: '' });
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const navigate = useNavigate();

  // Temporary mock data
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

  // Get status from URL and update filter
  useEffect(() => {
    const status = searchParams.get('status');
    if (status) {
      setStatusFilter(status);
    }
  }, [searchParams]);

  // Filter orders based on status
  const filteredOrders = orders.filter(order => {
    if (statusFilter === 'all') return true;
    return order.status === statusFilter;
  });

  return (
    <>
      <AdminLayout>
        <div className="space-y-6">
          {/* Header */}
          <div className="flex justify-between items-center">
            <h1 className="text-2xl font-semibold text-gray-900">
              {statusFilter === 'all' ? 'All Orders' :
                statusFilter === 'pending' ? 'Pending Orders' :
                  statusFilter === 'completed' ? 'Completed Orders' : 'Orders'}
            </h1>
            <button
              type="button"
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
            >
              Export Orders
            </button>
          </div>

          {/* Filters */}
          <div className="bg-white p-4 rounded-lg shadow space-y-4">
            <div className="flex flex-col sm:flex-row gap-4">
              {/* Search */}
              <div className="flex-1">
                <div className="relative">
                  <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    <Search className="h-5 w-5 text-gray-400" />
                  </div>
                  <input
                    type="text"
                    className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    placeholder="Search orders..."
                    value={searchQuery}
                    onChange={(e) => { setSearchQuery(e.target.value); }}
                  />
                </div>
              </div>

              {/* Date Range */}
              <div className="flex gap-2">
                <input
                  type="date"
                  className="block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  value={dateRange.start}
                  onChange={(e) => { setDateRange(prev => ({ ...prev, start: e.target.value })); }}
                />
                <input
                  type="date"
                  className="block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  value={dateRange.end}
                  onChange={(e) => { setDateRange(prev => ({ ...prev, end: e.target.value })); }}
                />
              </div>

              {/* Status Filter */}
              <select
                className="block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                value={statusFilter}
                onChange={(e) => { setStatusFilter(e.target.value); }}
              >
                <option value="all">All Status</option>
                <option value="pending">Pending</option>
                <option value="processing">Processing</option>
                <option value="completed">Completed</option>
                <option value="cancelled">Cancelled</option>
              </select>
            </div>
          </div>

          {/* Orders Table */}
          <div className="bg-white rounded-lg shadow">
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Order ID
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Customer
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Date
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Status
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Total
                    </th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {filteredOrders.map((order) => (
                    <tr
                      key={order.id}
                      className="hover:bg-gray-50 cursor-pointer"
                      onClick={() => {
                        void navigate(`/admin/orders/${order.id}`);
                      }}
                    >
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-blue-600">
                        #{order.id}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {order.customerName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {order.date}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full 
                          ${order.status === 'completed' ? 'bg-green-100 text-green-800' :
                            order.status === 'pending' ? 'bg-yellow-100 text-yellow-800' :
                              order.status === 'cancelled' ? 'bg-red-100 text-red-800' :
                                'bg-blue-100 text-blue-800'}`}>
                          {order.status}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        ${order.total.toFixed(2)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                        <button
                          type="button"
                          className="text-blue-600 hover:text-blue-900 mr-4"
                          onClick={e => {
                            e.stopPropagation();
                            void navigate(`/admin/orders/${order.id}`);
                          }}
                        >
                          View
                        </button>
                        <button
                          type="button"
                          className="text-red-600 hover:text-red-900"
                        >
                          Cancel
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </AdminLayout>
      <Outlet />
    </>
  );
} 