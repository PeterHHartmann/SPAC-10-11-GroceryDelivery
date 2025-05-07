import { useProducts } from '@/api/queries/product-queries';
import { ProductCard } from '@/components/product-card';
import { RefreshCcw } from 'lucide-react';
import type { FC } from 'react';
import { useParams } from 'react-router-dom';

export const ShopProductsPage: FC = () => {
	const { categoryId } = useParams<{ categoryId: string; }>();
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
		<>
			{products.map((product, index) => (
				<ProductCard key={`productcard-${String(product.productId)}-card-${String(index)}`} product={product} />
			))}
		</>
	);
};