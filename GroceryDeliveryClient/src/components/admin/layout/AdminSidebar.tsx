import { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { 
  LayoutDashboard, 
  ShoppingCart, 
  Package, 
  Users, 
  Settings,
  ChevronDown
} from 'lucide-react';

const navigation = [
  {
    title: 'Dashboard',
    path: '/admin',
    icon: LayoutDashboard
  },
  {
    title: 'Orders',
    path: '/admin/orders',
    icon: ShoppingCart,
    subItems: [
      { title: 'All Orders', path: '/admin/orders' },
      { title: 'Pending', path: '/admin/orders?status=pending' },
      { title: 'Completed', path: '/admin/orders?status=completed' }
    ]
  },
  {
    title: 'Products',
    path: '/admin/products',
    icon: Package
  },
  {
    title: 'Users',
    path: '/admin/users',
    icon: Users
  },
  {
    title: 'Settings',
    path: '/admin/settings',
    icon: Settings
  }
];

export default function AdminSidebar() {
  const [expandedItems, setExpandedItems] = useState<string[]>([]);
  const location = useLocation();

  const toggleSubItems = (title: string) => {
    setExpandedItems((prev) => {
      return prev.includes(title) 
        ? prev.filter(item => item !== title)
        : [...prev, title];
    });
  };

  return (
    <aside className="fixed inset-y-0 left-0 z-50 w-64 bg-white border-r border-gray-200 hidden lg:block">
      {/* Logo */}
      <div className="h-16 flex items-center justify-center border-b border-gray-200">
        <h1 className="text-xl font-bold text-gray-800">Admin Panel</h1>
      </div>

      {/* Navigation */}
      <nav className="p-4 space-y-1">
        {navigation.map((item) => (
          <div key={item.title}>
            <button
              type="button"
              onClick={() => {
                if (item.subItems) {
                  toggleSubItems(item.title);
                }
              }}
              className={`w-full flex items-center justify-between px-4 py-2 text-sm font-medium rounded-md ${
                location.pathname === item.path
                  ? 'bg-gray-100 text-gray-900'
                  : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
              }`}
            >
              <div className="flex items-center">
                <item.icon className="w-5 h-5 mr-3" />
                {item.title}
              </div>
              {item.subItems && (
                <ChevronDown
                  className={`w-4 h-4 transition-transform ${
                    expandedItems.includes(item.title) ? 'rotate-180' : ''
                  }`}
                />
              )}
            </button>

            {/* Sub Items */}
            {item.subItems && expandedItems.includes(item.title) && (
              <div className="ml-8 mt-1 space-y-1">
                {item.subItems.map((subItem) => (
                  <Link
                    key={subItem.path}
                    to={subItem.path}
                    className={`block px-4 py-2 text-sm rounded-md ${
                      location.pathname + location.search === subItem.path
                        ? 'bg-gray-100 text-gray-900'
                        : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                    }`}
                  >
                    {subItem.title}
                  </Link>
                ))}
              </div>
            )}
          </div>
        ))}
      </nav>
    </aside>
  );
} 