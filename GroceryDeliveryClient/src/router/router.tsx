import { RootLayout } from '@/layout/root-layout';
import { IndexPage } from '@/pages/index/index-page';
import { ShoppingBasketPage } from '@/pages/shopping-basket/shopping-basket-page';
import { createBrowserRouter } from 'react-router-dom';

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
				path: '/cart',
				Component: ShoppingBasketPage
			}
		]
	}
]);