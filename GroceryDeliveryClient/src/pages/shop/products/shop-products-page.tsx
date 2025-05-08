import { useProducts } from '@/api/queries/product-queries';
import { ProductCard } from '@/components/product-card';
import { Button } from '@/components/ui/button';
import { Pagination, PaginationContent, PaginationItem, PaginationNext, PaginationPrevious } from '@/components/ui/pagination';
import { RefreshCcw } from 'lucide-react';
import { useMemo, type FC } from 'react';
import { useSearchParams } from 'react-router-dom';

const pageSize = 6;

export const ShopProductsPage: FC = () => {
	const [searchParams, setSearchParams] = useSearchParams();

	const currentCategoryId: number | undefined = useMemo(() => {
		const categoryId = searchParams.get('categoryId');
		if (categoryId) {
			return parseInt(categoryId, 10);
		}
		return undefined;
	}, [searchParams]);

	const currentPage: number | undefined = useMemo(() => {
		const page = searchParams.get('page');
		if (page) {
			return parseInt(page, 10);
		}
		return 1;
	}, [searchParams]);

	const { data: products, isLoading, error } = useProducts({
		page: currentPage,
		pageSize: pageSize,
		categoryId: currentCategoryId
	});

	const hasMorePages = useMemo(() => {
		if (products && products.length === pageSize) {
			return true;
		}
		return false;
	}, [products]);

	const handlePagePrevious = (): void => {
		if (currentPage > 1) {
			const params = searchParams;
			params.set('page', String(currentPage - 1));
			setSearchParams(params, {
				preventScrollReset: true
			});
			return;
		}
	};

	const handlePageNext = (): void => {
		if (hasMorePages) {
			const params = searchParams;
			params.set('page', String(currentPage + 1));
			setSearchParams(params, {
				preventScrollReset: true
			});
			return;
		}
	};

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
		<section className='grid grid-flow-row-dense w-full '>
			<Pagination>
				<PaginationContent className='py-2'>
					<PaginationItem>
						<Button
							variant={'ghost'}
							disabled={currentPage <= 1}
							size={'sm'}
							onClick={handlePagePrevious}
						>
							<PaginationPrevious />
						</Button>
					</PaginationItem>
					<PaginationItem>
						<span> Page <span className='text-primary'>{currentPage}</span>
						</span>
					</PaginationItem>
					<PaginationItem>
						<Button
							variant={'ghost'}
							disabled={!hasMorePages}
							size={'sm'}
							onClick={handlePageNext}
						>
							<PaginationNext />
						</Button>
					</PaginationItem>
				</PaginationContent>
			</Pagination>
			<div className='w-full h-full px-4 grid grid-cols-3 grid-flow-row gap-2'>
				{products.map((product, index) => (
					<ProductCard key={`productcard-${String(product.productId)}-card-${String(index)}`} product={product} />
				))}
			</div>
		</section>
	);
};