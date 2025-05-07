import { RootLayout } from '@/layout/root-layout';
import { IndexPage } from '@/pages/index/index-page';
import { ShoppingBasketPage } from '@/pages/shopping-basket/shopping-basket-page';
import { createBrowserRouter } from 'react-router-dom';
import AdminDashboard from '@/pages/admin/dashboard/page';
import path from 'path';
import { Component } from 'lucide-react';

export const router = createBrowserRouter([
	{
		path: '/',
		Component: RootLayout,
		children: [
			{
				index: true,
				path: '/',
				Component: IndexPage,
			},
			{
				path: '/shop/:categoryId',
				Component: IndexPage
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