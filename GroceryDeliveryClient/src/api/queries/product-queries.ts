import { useQuery } from '@tanstack/react-query';
import type { ApiError, PaginationParams, Product, ProductQueryParams } from '@/types';
import { ProductEndpoints } from '@/api/endpoints/product-endpoints';
import { ProductService } from '@/api/services/product-service';
import { convertToURLSearchParams } from '@/lib/utils';

export const useProducts = (params: ProductQueryParams & PaginationParams) => {
	return useQuery<Product[], ApiError>({
		queryKey: [...ProductEndpoints.getAll.key(convertToURLSearchParams(params))],
		queryFn: () => ProductService.getAll(params),
	});
};

export const useProduct = (id: number) => {
	return useQuery<Product, ApiError>({
		queryKey: [...ProductEndpoints.getById.key(id)],
		queryFn: () => ProductService.getById(id)
	});
};