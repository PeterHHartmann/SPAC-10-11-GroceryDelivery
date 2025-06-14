import { QueryClientProvider } from '@tanstack/react-query';
import { type FC } from 'react';
import { queryClient } from '@/api/queryClient';
import { RouterProvider } from 'react-router-dom';
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { router } from '@/router/router';
import { ShoppingBasketProvider } from '@/hooks/shopping-basket';

export const App: FC = () => {

	return (
		<ShoppingBasketProvider>
			<QueryClientProvider client={queryClient}>
				<RouterProvider router={router} />
				{import.meta.env.VITE_ENABLE_QUERY_DEVTOOLS === "true" && (
					<ReactQueryDevtools initialIsOpen={false} />
				)}
			</QueryClientProvider>
		</ShoppingBasketProvider>
	);
};
