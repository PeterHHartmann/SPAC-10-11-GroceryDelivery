import { RootLayout } from '@/layout/root-layout';
import { ShoppingBasketPage } from '@/pages/shopping-basket/shopping-basket-page';
import { createBrowserRouter, redirect } from 'react-router-dom';
import AdminDashboard from '@/pages/admin/dashboard/page';
import { ShopProductsPage } from '@/pages/shop/products/shop-products-page';
import { ShopPage } from '@/pages/shop/shop-page';
import { ProductDetailPage } from '@/pages/product-detail/product-detail-page';
import OrdersPage from '@/pages/admin/orders/page';
import OrderDetailsPage from '@/pages/admin/orders/[orderId]/page';
import UsersPage from '@/pages/admin/users/page';

export const router = createBrowserRouter([
	{
		path: "/",
		Component: RootLayout,
		children: [
			{
				path: '/',
				index: true,
				loader: () => redirect("/shop"),
			},
			{
				path: '/shop',
				Component: ShopPage,
				children: [
					{
						index: true,
						Component: ShopProductsPage
					}
				]
			},
			{
				path: '/product/:productId',
				Component: ProductDetailPage,
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