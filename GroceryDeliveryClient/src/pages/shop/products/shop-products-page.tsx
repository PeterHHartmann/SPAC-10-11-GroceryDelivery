import { useProducts } from '@/api/queries/product-queries';
import { ProductCard } from '@/components/product-card';
import { RefreshCcw } from 'lucide-react';
import { useMemo, type FC } from 'react';
import { useSearchParams } from 'react-router-dom';

export const ShopProductsPage: FC = () => {
	const [searchParams] = useSearchParams();
	const categoryId: string | null = useMemo(() => {
		return searchParams.get('categoryId');
	}, [searchParams]);
	const { data: products, isLoading, error } = useProducts({ page: 1, pageSize: 10, categoryId: categoryId ? parseInt(categoryId, 10) : undefined });

	if (isLoading) {
		return (
			<div>
				<RefreshCcw size={48} className='animate-spin' />
			</div>
		);
	}

	if (error || !products) {
		return (
			<div>
				<p>Couldn't find any products</p>
			</div>
		);
	}

	return (
		<section className='flex w-full'>
			<div className='w-full h-full p-4 grid grid-cols-3 grid-flow-row gap-2'>
				{products.map((product, index) => (
					<ProductCard key={`productcard-${String(product.productId)}-card-${String(index)}`} product={product} />
				))}
			</div>
		</section>
	);
};