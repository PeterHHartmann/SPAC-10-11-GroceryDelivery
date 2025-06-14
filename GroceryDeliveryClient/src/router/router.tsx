import { RootLayout } from '@/layout/root-layout';
import { ShoppingBasketPage } from '@/pages/shopping-basket/shopping-basket-page';
import { createBrowserRouter, redirect } from 'react-router-dom';
import { ShopProductsPage } from '@/pages/shop/products/shop-products-page';
import { ShopPage } from '@/pages/shop/shop-page';
import { ProductDetailPage } from '@/pages/product-detail/product-detail-page';
import { CheckoutPage } from '@/pages/checkout/checkout-page';
import { OrderConfirmationPage } from '@/pages/order-confirmation/order-confirmation-page';
import { AdminRoutes } from '@/router/admin-routes';

export const router = createBrowserRouter([
	{
		path: '/',
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
				path: '/basket',
				Component: ShoppingBasketPage
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
	},
	...AdminRoutes
]);
