import { RootLayout } from '@/layout/root-layout';
import { IndexPage } from '@/pages/index/index-page';
import { createBrowserRouter } from 'react-router-dom';
import AdminDashboard from '@/pages/admin/dashboard/page';
import OrdersPage from '@/pages/admin/orders/page';
import OrderDetailsPage from '@/pages/admin/orders/[orderId]/page';
import UsersPage from '@/pages/admin/users/page';

export const router = createBrowserRouter([
	{
		path: "/",
		Component: RootLayout,
		children: [
			{
				index: true,
				path: "/",
				Component: IndexPage
			},
			{
				path: "/admin",
				children: [
					{
						index: true,
						Component: AdminDashboard
					},
					{
						path: "/admin/orders",
						Component: OrdersPage,
						children: [
							{
								path: "/admin/orders/:orderId",
								Component: OrderDetailsPage
							}
						]
					},
					{
						path: "/admin/users",
						Component: UsersPage
					}
				]
			}
		]
	}
]);