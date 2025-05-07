import { RootLayout } from '@/layout/root-layout';
import { ShoppingBasketPage } from '@/pages/shopping-basket/shopping-basket-page';
import { createBrowserRouter, redirect } from 'react-router-dom';
import AdminDashboard from '@/pages/admin/dashboard/page';
import { ShopProductsPage } from '@/pages/shop/products/products-page';
import { ShopPage } from '@/pages/shop/shop-page';

export const router = createBrowserRouter([
	{
		path: '/',
		Component: RootLayout,
		children: [
			{
				path: '/',
				index: true,
				loader: () => redirect("/products"),
			},
			{
				path: '/shop',
				Component: ShopPage,
				children: [
					{
						index: true,
						Component: ShopProductsPage
					},
					{
						path: '/shop/category/:categoryId',
						Component: ShopProductsPage
					}
				]
			},
			{
				path: '/basket',
				Component: ShoppingBasketPage
			},
			{
				path: "/admin",
				children: [
					{
						index: true,
						Component: AdminDashboard
					}
				]
			}
		]
	}
]);