import { RootLayout } from '@/layout/root-layout';
import { IndexPage } from '@/pages/index/index-page';
import { createBrowserRouter } from 'react-router-dom';
import AdminDashboard from '@/pages/admin/dashboard/page';
import OrdersPage from '@/pages/admin/orders/page';

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
						path: "orders",
						Component: OrdersPage
					}
				]
			}
		]
	}
]);