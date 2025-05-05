import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import AdminLayout from '../../../../components/admin/layout/AdminLayout';
import { 
  ArrowLeft, 
  Package, 
  User, 
  CreditCard,
  AlertCircle
} from 'lucide-react';

// Temporary types until API integration
interface OrderItem {
  id: string;
  productName: string;
  quantity: number;
  price: number;
  total: number;
}

interface OrderDetails {
  id: string;
  customerName: string;
  customerEmail: string;
  date: string;
  status: 'pending' | 'processing' | 'completed' | 'cancelled';
  total: number;
  items: OrderItem[];
  shippingAddress: string;
  paymentMethod: string;
  notes: string;
}

export default function OrderDetailsPage() {
  const { orderId } = useParams();
  const navigate = useNavigate();
  const [isEditing, setIsEditing] = useState(false);
  const [orderNotes, setOrderNotes] = useState('');

  // Temporary mock data
  const order: OrderDetails = {
    id: orderId || 'ORD001',
    customerName: 'John Doe',
    customerEmail: 'john@example.com',
    date: '2024-03-15',
    status: 'pending',
    total: 150.00,
    items: [
      {
        id: '1',
        productName: 'Organic Bananas',
        quantity: 2,
        price: 1.99,
        total: 3.98
      },
      {
        id: '2',
        productName: 'Whole Milk',
        quantity: 1,
        price: 3.49,
        total: 3.49
      }
    ],
    shippingAddress: '123 Main St, City, Country',
    paymentMethod: 'Credit Card',
    notes: 'Customer requested early delivery'
  };

  const handleStatusChange = (newStatus: OrderDetails['status']) => {
    // Will be replaced with API call
    console.log('Status changed to:', newStatus);
  };

  const handleCancelOrder = () => {
    // Will be replaced with API call
    console.log('Order cancelled');
    void navigate('/admin/orders');
  };

  const handleSaveNotes = () => {
    // Will be replaced with API call
    console.log('Notes saved:', orderNotes);
    setIsEditing(false);
  };

  return (
    <AdminLayout>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-4">
            <button
              type="button"
              onClick={() => { void navigate('/admin/orders'); }}
              className="p-2 hover:bg-gray-100 rounded-full"
            >
              <ArrowLeft className="w-5 h-5" />
            </button>
            <h1 className="text-2xl font-semibold text-gray-900">
              Order #{order.id}
            </h1>
          </div>
          <div className="flex items-center space-x-4">
            <button
              type="button"
              onClick={() => { setIsEditing(!isEditing); }}
              className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
            >
              {isEditing ? 'Cancel Edit' : 'Edit Order'}
            </button>
            <button
              type="button"
              onClick={handleCancelOrder}
              className="px-4 py-2 text-sm font-medium text-white bg-red-600 rounded-md hover:bg-red-700"
            >
              Cancel Order
            </button>
          </div>
        </div>

        {/* Order Status */}
        <div className="bg-white p-6 rounded-lg shadow">
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-lg font-medium text-gray-900">Order Status</h2>
              <p className="mt-1 text-sm text-gray-500">
                Last updated: {order.date}
              </p>
            </div>
            <select
              value={order.status}
              onChange={(e) => { handleStatusChange(e.target.value as OrderDetails['status']); }}
              className="block w-48 px-3 py-2 text-sm border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="pending">Pending</option>
              <option value="processing">Processing</option>
              <option value="completed">Completed</option>
              <option value="cancelled">Cancelled</option>
            </select>
          </div>
        </div>

        {/* Order Details Grid */}
        <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
          {/* Customer Information */}
          <div className="bg-white p-6 rounded-lg shadow">
            <div className="flex items-center space-x-3 mb-4">
              <User className="w-5 h-5 text-gray-400" />
              <h2 className="text-lg font-medium text-gray-900">Customer Information</h2>
            </div>
            <div className="space-y-4">
              <div>
                <p className="text-sm font-medium text-gray-500">Name</p>
                <p className="mt-1">{order.customerName}</p>
              </div>
              <div>
                <p className="text-sm font-medium text-gray-500">Email</p>
                <p className="mt-1">{order.customerEmail}</p>
              </div>
              <div>
                <p className="text-sm font-medium text-gray-500">Shipping Address</p>
                <p className="mt-1">{order.shippingAddress}</p>
              </div>
            </div>
          </div>

          {/* Payment Information */}
          <div className="bg-white p-6 rounded-lg shadow">
            <div className="flex items-center space-x-3 mb-4">
              <CreditCard className="w-5 h-5 text-gray-400" />
              <h2 className="text-lg font-medium text-gray-900">Payment Information</h2>
            </div>
            <div className="space-y-4">
              <div>
                <p className="text-sm font-medium text-gray-500">Payment Method</p>
                <p className="mt-1">{order.paymentMethod}</p>
              </div>
              <div>
                <p className="text-sm font-medium text-gray-500">Total Amount</p>
                <p className="mt-1 text-lg font-semibold">${order.total.toFixed(2)}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Order Items */}
        <div className="bg-white rounded-lg shadow">
          <div className="p-6">
            <div className="flex items-center space-x-3 mb-4">
              <Package className="w-5 h-5 text-gray-400" />
              <h2 className="text-lg font-medium text-gray-900">Order Items</h2>
            </div>
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead>
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Product
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Quantity
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Price
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Total
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {order.items.map((item) => (
                    <tr key={item.id}>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                        {item.productName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {item.quantity}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        ${item.price.toFixed(2)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        ${item.total.toFixed(2)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>

        {/* Order Notes */}
        <div className="bg-white p-6 rounded-lg shadow">
          <div className="flex items-center space-x-3 mb-4">
            <AlertCircle className="w-5 h-5 text-gray-400" />
            <h2 className="text-lg font-medium text-gray-900">Order Notes</h2>
          </div>
          {isEditing ? (
            <div className="space-y-4">
              <textarea
                value={orderNotes}
                onChange={(e) => { setOrderNotes(e.target.value); }}
                rows={4}
                className="block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                placeholder="Add notes about this order..."
              />
              <div className="flex justify-end space-x-4">
                <button
                  type="button"
                  onClick={() => { setIsEditing(false); }}
                  className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
                >
                  Cancel
                </button>
                <button
                  type="button"
                  onClick={handleSaveNotes}
                  className="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-md hover:bg-blue-700"
                >
                  Save Notes
                </button>
              </div>
            </div>
          ) : (
            <div>
              <p className="text-sm text-gray-500">{order.notes || 'No notes available'}</p>
            </div>
          )}
        </div>
      </div>
    </AdminLayout>
  );
} 