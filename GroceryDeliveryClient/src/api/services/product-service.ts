import axiosClient from "@/api/axiosClient";
import { ProductEndpoints } from '@/api/endpoints/product-endpoints';
import { convertToURLSearchParams } from '@/lib/utils';
import { Product, type PaginationParams, type ProductQueryParams } from "@/types";
import type { AxiosError, AxiosResponse } from 'axios';

/**
 * Service for handling product-related API calls
 */
export const ProductService = {
	/**
	 * Fetch all products with optional sorting and ordering
	 * @param params - Sorting and ordering parameters
	 * @returns Promise with all product data
	 */
	async getAll(
		params?: ProductQueryParams & PaginationParams
	): Promise<Product[]> {

		const queryParams = params ? convertToURLSearchParams(params) : "";
		const response = await axiosClient.get<string, AxiosResponse<Product[], AxiosError>>(
			ProductEndpoints.getAll.url(queryParams)
		);
		const { data } = response;
		return data;
	},

	/**
	 * Fetch a single product by ID
	 * @param id - Product ID
	 * @returns Promise with product data
	 */
	async getById(id: number): Promise<Product> {
		const response = await axiosClient.get<string, AxiosResponse<Product, AxiosError>>(ProductEndpoints.getById.url(id));
		const { data } = response;
		return data;
	}

	// /**
	//  * Create a new product
	//  * @param product - Product data
	//  * @returns Promise with created product
	//  */
	// static async create(
	//     product: Omit<Product, "id" | "createdAt" | "updatedAt">
	// ): Promise<Product> {
	//     const response = await axiosClient.post(ProductEndpoints.create(), product);
	//     return response.data;
	// }

	// /**
	//  * Update an existing product
	//  * @param id - Product ID
	//  * @param product - Updated product data
	//  * @returns Promise with updated product
	//  */
	// static async update(id: string, product: Partial<Product>): Promise<Product> {
	//     const response = await axiosClient.put(
	//         ProductEndpoints.update(id),
	//         product
	//     );
	//     return response.data;
	// }

	// /**
	//  * Delete a product
	//  * @param id - Product ID
	//  * @returns Promise with deletion status
	//  */
	// static async delete(id: string): Promise<void> {
	//     await axiosClient.delete(ProductEndpoints.delete(id));
	// }

	// /**
	//  * Fetch products by category
	//  * @param category - Product category
	//  * @param params - Pagination parameters
	//  * @returns Promise with paginated product data
	//  */
	// static async getByCategory(
	//     category: string,
	//     params?: PaginationParams
	// ): Promise<PaginatedResponse<Product>> {
	//     const queryParams = params
	//         ? new URLSearchParams({
	//             page: params.page.toString(),
	//             limit: params.limit.toString(),
	//         }).toString()
	//         : "";

	//     const response = await axiosClient.get(
	//         `${ProductEndpoints.getByCategory(category)}${queryParams ? `?${queryParams}` : ""
	//         }`
	//     );
	//     return response.data;
	// }
};