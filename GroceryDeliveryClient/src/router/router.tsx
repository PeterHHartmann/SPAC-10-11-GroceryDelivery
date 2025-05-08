import { RootLayout } from '@/layout/root-layout';
import { IndexPage } from '@/pages/index/index-page';
import { ShoppingBasketPage } from '@/pages/shopping-basket/shopping-basket-page';
import {CheckoutPage} from '@/pages/checkout/checkout-page';
import {OrderConfirmationPage} from '@/pages/order-confirmation/order-confirmation-page';
import { createBrowserRouter } from 'react-router-dom';
import AdminDashboard from '@/pages/admin/dashboard/page';

export const router = createBrowserRouter([
	{
		path: '/',
		Component: RootLayout,
		children: [
			{
				index: true,
				path: '/',
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
			},
			{
				path: 'basket/checkout',
				Component: CheckoutPage
			},
			{
				
				path: '/order-confirmation',
				Component: OrderConfirmationPage
			},
		]
	}
]);