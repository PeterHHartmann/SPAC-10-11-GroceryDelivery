import AdminDashboardPage from '@/pages/admin/dashboard/admin-dashboard-page';
import AdminOrderDetailsPage from '@/pages/admin/orders/[orderId]/admin-orders-detail-page';
import AdminOrdersPage from '@/pages/admin/orders/admin-orders-page';
import AdminUsersPage from '@/pages/admin/users/admin-users-page';
import type { RouteObject } from 'react-router-dom';

export const AdminRoutes: RouteObject[] = [
	{
		path: "/admin",
		children: [
			{
				index: true,
				Component: AdminDashboardPage
			},
			{
				path: "/admin/orders",
				Component: AdminOrdersPage,
				children: [
					{
						path: "/admin/orders/:orderId",
						Component: AdminOrderDetailsPage
					}
				]
			},
			{
				path: "/admin/users",
				Component: AdminUsersPage
			}
		]
	},
];